
using BlackFriday.Api.Data;
using BlackFriday.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Api.Services;

public class OrderProcessingService : BackgroundService
{
    private const int MaxRetries = 3;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderProcessingService> _logger;

    public OrderProcessingService(
        IServiceScopeFactory scopeFactory,
        ILogger<OrderProcessingService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<BlackFridayDbContext>();

                var order = await db.Orders
                    .Where(x => x.Status == "Pending")
                    .OrderBy(x => x.CreatedAtUtc)
                    .FirstOrDefaultAsync(stoppingToken);

                if (order != null)
                {
                    await ProcessOrderAsync(db, order, stoppingToken);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while processing orders");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }

    private async Task ProcessOrderAsync(
        BlackFridayDbContext db,
        Order order,
        CancellationToken cancellationToken)
    {
        try
        {
            // Placeholder for actual order-processing logic.
            await Task.Delay(
                TimeSpan.FromMilliseconds(100),
                cancellationToken);

            order.Status = "Processed";
            order.LastError = null;

            await db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Order {OrderId} processed successfully",
                order.Id);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            order.RetryCount++;
            order.LastError = ex.Message;

            if (order.RetryCount >= MaxRetries)
            {
                order.Status = "DeadLetter";

                _logger.LogError(
                    ex,
                    "Order {OrderId} moved to DLQ after {RetryCount} attempts",
                    order.Id,
                    order.RetryCount);
            }
            else
            {
                order.Status = "Pending";

                _logger.LogWarning(
                    ex,
                    "Order {OrderId} failed. Retry {RetryCount} of {MaxRetries}",
                    order.Id,
                    order.RetryCount,
                    MaxRetries);
            }

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
