
using BlackFriday.Api.Data;
using BlackFriday.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlackFriday.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantId}/orders")]
public class OrdersController : ControllerBase
{
    private readonly BlackFridayDbContext _db;

    public OrdersController(BlackFridayDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        string tenantId,
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ReservationId <= 0)
            return BadRequest("ReservationId must be greater than zero.");

        var order = new Order
        {
            ReservationId = request.ReservationId,
            TenantId = tenantId,
            Status = "Pending",
            RetryCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(cancellationToken);

        return Accepted(new
        {
            order.Id,
            order.TenantId,
            order.ReservationId,
            order.Status
        });
    }

    public record CreateOrderRequest(int ReservationId);
}
