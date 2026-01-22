using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using LibrarySystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.BorrowScheduler.Workers;

public class BorrowOverdueWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BorrowOverdueWorker> _logger;

    public BorrowOverdueWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<BorrowOverdueWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var borrowRepo =
                    scope.ServiceProvider
                        .GetRequiredService<IGenericRepository<Borrow>>();

                var borrowService =
                    scope.ServiceProvider
                        .GetRequiredService<IBorrowService>();

                var now = DateTime.UtcNow;

                var overdueCandidates = await borrowRepo
                    .GetQueryable()
                    .Where(b =>
                        b.Status == BorrowStatus.Borrowed &&
                        b.DueDate < now &&
                        b.ReturnDate == DateTime.Now)
                    .Select(b => b.Id)
                    .ToListAsync(stoppingToken);

                foreach (var borrowId in overdueCandidates)
                {
                    await borrowService.MarkOverdueAsync(borrowId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while processing overdue borrows");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
