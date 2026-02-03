using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Common.Messaging;
using LibrarySystem.Common.Middleware;
using LibrarySystem.Common.Repositories;
using LibrarySystem.Domain.Abstractions;
using LibrarySystem.Domain.Data;
using LibrarySystem.Domain.Repositories;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Domain.Repositories.Repo;
using LibrarySystem.Entities.Models;
using LibrarySystem.Helper.Api;
using LibrarySystem.Services;
using LibrarySystem.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RabbitMQ.Client;
using System.Text;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

#region Controllers + Validation
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors?.Count > 0)
                .SelectMany(e => e.Value!.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(
                new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Database
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],

        ValidateAudience = true,
        ValidAudiences = new[]
     {
        jwtSection["Audience"], 
        "gateway",                   
        "https://localhost:7171"
    },

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

});
#endregion

#region Authorization Policies (?? ???????)
builder.Services.AddAuthorization(options =>
{
    // ===== BOOK =====
    options.AddPolicy("BookCreate",
        p => p.RequireClaim("permission", "book.create"));

    options.AddPolicy("BookUpdate",
        p => p.RequireClaim("permission", "book.update"));

    options.AddPolicy("BookDelete",
        p => p.RequireClaim("permission", "book.delete"));

    options.AddPolicy("BookView",
        p => p.RequireClaim("permission", "book.view"));

    // ===== BORROW =====
    options.AddPolicy("BorrowCreate",
        p => p.RequireClaim("permission", "borrow.create"));

    options.AddPolicy("BorrowReturn",
        p => p.RequireClaim("permission", "borrow.return"));

    options.AddPolicy("BorrowView",
        p => p.RequireClaim("permission", "borrow.view"));

    // ===== CATEGORY =====
    options.AddPolicy("CategoryManage",
        p => p.RequireClaim("permission", "category.manage"));

    // ===== USER =====
    options.AddPolicy("UserManage",
        p => p.RequireClaim("permission", "user.manage"));
});
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
#endregion

#region RabbitMQ + MassTransit
var rabbitSection = builder.Configuration.GetSection("RabbitMq");
if (!rabbitSection.Exists())
    throw new Exception("RabbitMq configuration section is missing");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedConsumer>();
    x.AddConsumer<UserUpdatedConsumer>();
    x.AddConsumer<UserDeactivatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitSection["Host"], "/", h =>
        {
            h.Username(rabbitSection["Username"]);
            h.Password(rabbitSection["Password"]);
        });

        cfg.ReceiveEndpoint(LibraryQueues.UserCreated, e =>
        {
            e.ConfigureConsumeTopology = false;
            e.Bind(LibraryExchanges.Users, s =>
            {
                s.ExchangeType = ExchangeType.Topic;
                s.RoutingKey = LibraryRoutingKeys.UserCreated;
            });
            e.ConfigureConsumer<UserCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint(LibraryQueues.UserUpdated, e =>
        {
            e.ConfigureConsumeTopology = false;
            e.Bind(LibraryExchanges.Users, s =>
            {
                s.ExchangeType = ExchangeType.Topic;
                s.RoutingKey = LibraryRoutingKeys.UserUpdated;
            });
            e.ConfigureConsumer<UserUpdatedConsumer>(context);
        });

        cfg.ReceiveEndpoint(LibraryQueues.UserDeactivated, e =>
        {
            e.ConfigureConsumeTopology = false;
            e.Bind(LibraryExchanges.Users, s =>
            {
                s.ExchangeType = ExchangeType.Topic;
                s.RoutingKey = LibraryRoutingKeys.UserDeactivated;
            });
            e.ConfigureConsumer<UserDeactivatedConsumer>(context);
        });
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

#region Services
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookCopyService, BookCopyService>();
builder.Services.AddScoped<IBorrowService, BorrowService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBorrowRepository, BorrowRepository>();
builder.Services.AddScoped<IBookCopyRepository, BookCopyRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
builder.Services.AddScoped<IAuditUserProvider, AuditUserProvider>();


// ===== Generic Repository registrations =====
builder.Services.AddScoped<IRepository<Author>, Repository<LibraryDbContext, Author>>();
builder.Services.AddScoped<IRepository<Book>, Repository<LibraryDbContext, Book>>();
builder.Services.AddScoped<IRepository<BookCopy>, Repository<LibraryDbContext, BookCopy>>();
builder.Services.AddScoped<IRepository<Category>, Repository<LibraryDbContext, Category>>();
builder.Services.AddScoped<IRepository<Publisher>, Repository<LibraryDbContext, Publisher>>();
builder.Services.AddScoped<IRepository<Borrow>, Repository<LibraryDbContext, Borrow>>();
builder.Services.AddScoped<IRepository<User>, Repository<LibraryDbContext, User>>();
#endregion



var app = builder.Build();

#region Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>("LibrarySystem.API");

app.MapControllers();
#endregion

app.Run();
