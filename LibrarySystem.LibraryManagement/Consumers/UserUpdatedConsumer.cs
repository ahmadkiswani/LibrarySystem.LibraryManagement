using LibrarySystem.Common.DTOs.Library.Users;
using LibrarySystem.Common.Messaging;
using LibrarySystem.Services.Interfaces;
using MassTransit;

public class UserUpdatedConsumer : IConsumer<UserUpdatedMessage>
{
    private readonly IUserService _userService;

    public UserUpdatedConsumer(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<UserUpdatedMessage> context)
    {
        var msg = context.Message;

        await _userService.ApplyUserUpdatedEvent(
            msg.UserId,
            new UserUpdateDto
            {
                UserName = msg.UserName,
                UserEmail = msg.Email,
                RoleName = msg.RoleName,
            });
    }
}
