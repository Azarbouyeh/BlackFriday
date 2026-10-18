
using BlackFriday.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantId}/sale-events")]
public class SaleEventsController : ControllerBase
{
    private readonly BlackFridayDbContext _db;

    public SaleEventsController(BlackFridayDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        string tenantId,
        CancellationToken cancellationToken)
    {
        var events = await _db.SaleEvents
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.StartsAtUtc)
            .Select(x => new
            {
                x.Id,
                x.Sku,
                x.InitialStock,
                x.RemainingStock,
                x.DiscountPrice,
                x.StartsAtUtc,
                x.EndsAtUtc
            })
            .ToListAsync(cancellationToken);

        return Ok(events);
    }
}
