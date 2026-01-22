using LibrarySystem.Common.DTOs.Library.Users;
using LibrarySystem.Common.Messaging;
using LibrarySystem.Services.Interfaces;
using MassTransit;

public class UserCreatedConsumer : IConsumer<UserCreatedMessage>
{
    private readonly IUserService _userService;

    public UserCreatedConsumer(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<UserCreatedMessage> context)
    {
        var msg = context.Message;

        await _userService.ApplyUserCreatedEvent(new UserCreateDto
        {
            ExternalUserId = msg.UserId,
            UserName = msg.UserName,
            UserEmail = msg.Email,
        });
    }
}
