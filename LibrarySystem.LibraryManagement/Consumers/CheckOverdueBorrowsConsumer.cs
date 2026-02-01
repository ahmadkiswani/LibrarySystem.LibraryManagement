using LibrarySystem.Common.Events;
using LibrarySystem.Services.Interfaces;
using MassTransit;

public class CheckOverdueBorrowsConsumer
    : IConsumer<CheckOverdueBorrowsCommand>
{
    private readonly IBorrowService _borrowService;

    public CheckOverdueBorrowsConsumer(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    public async Task Consume(ConsumeContext<CheckOverdueBorrowsCommand> ctx)
    {
        await _borrowService.ProcessOverdueBorrowsAsync(ctx.Message.OccurredAt);
    }
}
