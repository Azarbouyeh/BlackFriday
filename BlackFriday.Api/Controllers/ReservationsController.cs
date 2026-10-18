
using BlackFriday.Api.Data;
using BlackFriday.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantId}/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly BlackFridayDbContext _db;

    public ReservationsController(BlackFridayDbContext db)
    {
        _db = db;
    }

    public record ReserveRequest(int SaleEventId);

    [HttpPost]
    public async Task<IActionResult> Create(
        string tenantId,
        [FromBody] ReserveRequest request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        if (request.SaleEventId <= 0)
        {
            return BadRequest("SaleEventId must be greater than zero.");
        }

        await using var transaction =
            await _db.Database.BeginTransactionAsync(cancellationToken);

        var affected = await _db.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE SaleEvents
            SET RemainingStock = RemainingStock - 1
            WHERE Id = {request.SaleEventId}
              AND TenantId = {tenantId}
              AND RemainingStock > 0
              AND StartsAtUtc <= {now}
              AND EndsAtUtc > {now}",
            cancellationToken);

        if (affected == 0)
            return Conflict(
                "Sale unavailable, inactive, or out of stock.");

        var sale = await _db.SaleEvents
            .AsNoTracking()
            .SingleAsync(
                x => x.Id == request.SaleEventId
                  && x.TenantId == tenantId,
                cancellationToken);

        var reservation = new Reservation
        {
            SaleEventId = sale.Id,
            TenantId = tenantId,
            Sku = sale.Sku,
            CreatedAtUtc = now,
            Status = "Reserved"
        };

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Ok(new
        {
            reservation.Id,
            reservation.TenantId,
            reservation.Sku,
            reservation.Status
        });
    }
}
