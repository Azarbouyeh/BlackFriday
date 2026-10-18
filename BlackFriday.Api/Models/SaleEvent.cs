
namespace BlackFriday.Api.Models;

public class SaleEvent
{
    public int Id { get; set; }

    public string TenantId { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public int InitialStock { get; set; }

    public int RemainingStock { get; set; }

    public decimal DiscountPrice { get; set; }

    public DateTime StartsAtUtc { get; set; }

    public DateTime EndsAtUtc { get; set; }
}
