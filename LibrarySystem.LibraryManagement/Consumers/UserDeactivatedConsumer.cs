using LibrarySystem.Common.Messaging;
using LibrarySystem.Services.Interfaces;
using MassTransit;

public class UserDeactivatedConsumer : IConsumer<UserDeactivatedMessage>
{
    private readonly IUserService _userService;
    private readonly IBorrowService _borrowService;

    public UserDeactivatedConsumer(IUserService userService, IBorrowService borrowService)
    {
        _userService = userService;
        _borrowService = borrowService;
    }

    public async Task Consume(ConsumeContext<UserDeactivatedMessage> context)
    {
        var msg = context.Message;

        await _userService.ApplyUserDeactivatedEvent(msg.UserId);
        await _borrowService.AutoReturnBorrowsForExternalUserAsync(msg.UserId);
    }
}
