using LibrarySystem.Common.Messaging;
using LibrarySystem.Services.Interfaces;
using MassTransit;

public class UserReactivatedConsumer : IConsumer<UserReactivatedMessage>
{
    private readonly IUserService _userService;

    public UserReactivatedConsumer(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<UserReactivatedMessage> context)
    {
        var msg = context.Message;
        await _userService.ApplyUserReactivatedEvent(msg.UserId);
    }
}

