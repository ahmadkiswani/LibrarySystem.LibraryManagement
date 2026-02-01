using LibrarySystem.BorrowScheduler.Workers;
using LibrarySystem.Domain.Data;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Services;
using LibrarySystem.Services.Interfaces;
using MassTransit;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Domain.Repositories.Repo;
using LibrarySystem.Domain.Repositories;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }
    ));
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});


builder.Services.AddScoped<IBorrowService, BorrowService>();
builder.Services.AddScoped<IBorrowRepository, BorrowRepository>();


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddHostedService<BorrowOverdueWorker>();

var host = builder.Build();
host.Run();
