
using BlackFriday.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(BlackFridayDbContext db)
    {
        // Deterministic and safe to run repeatedly.
        if (await db.SaleEvents.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var events = new List<SaleEvent>();

        for (var tenant = 1; tenant <= 2; tenant++)
        {
            var tenantId = $"tenant-{tenant}";

            for (var i = 1; i <= 3; i++)
            {
                var sku = $"SKU-{i:000}";
                var startsAt = i switch
                {
                    1 => now.AddHours(-1), // active
                    2 => now.AddDays(1),   // future
                    _ => now.AddDays(-3)   // expired
                };

                var endsAt = i switch
                {
                    1 => now.AddDays(1),
                    2 => now.AddDays(2),
                    _ => now.AddDays(-2)
                };

                events.Add(new SaleEvent
                {
                    TenantId = tenantId,
                    Sku = sku,
                    InitialStock = 10,
                    RemainingStock = 10,
                    DiscountPrice = 49.99m,
                    StartsAtUtc = startsAt,
                    EndsAtUtc = endsAt
                });
            }
        }

        db.SaleEvents.AddRange(events);
        await db.SaveChangesAsync();
    }
}
