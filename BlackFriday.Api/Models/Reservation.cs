
namespace BlackFriday.Api.Models;

public class Reservation
{
    public int Id { get; set; }

    public int SaleEventId { get; set; }

    public string TenantId { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public string Status { get; set; } = "Reserved";
}
