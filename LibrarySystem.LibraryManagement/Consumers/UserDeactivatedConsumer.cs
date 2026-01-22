using LibrarySystem.Common.Messaging;
using LibrarySystem.Services.Interfaces;
using MassTransit;

public class UserDeactivatedConsumer : IConsumer<UserDeactivatedMessage>
{
    private readonly IUserService _userService;

    public UserDeactivatedConsumer(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<UserDeactivatedMessage> context)
    {
        var msg = context.Message;

        await _userService.ApplyUserDeactivatedEvent(msg.UserId);
    }
}
