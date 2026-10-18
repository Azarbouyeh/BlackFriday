
using BlackFriday.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BlackFriday.Api.Data;

public class BlackFridayDbContext : DbContext
{
    public BlackFridayDbContext(
        DbContextOptions<BlackFridayDbContext> options)
        : base(options)
    {
    }

    public DbSet<SaleEvent> SaleEvents => Set<SaleEvent>();

    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<Order> Orders => Set<Order>();
}
