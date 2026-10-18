
namespace BlackFriday.Api.Models;

public class Order
{
    public int Id { get; set; }

    public int ReservationId { get; set; }

    public string TenantId { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public int RetryCount { get; set; } = 0;

    public string? LastError { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
