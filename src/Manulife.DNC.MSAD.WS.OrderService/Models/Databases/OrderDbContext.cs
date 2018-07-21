using Microsoft.EntityFrameworkCore;

namespace Manulife.DNC.MSAD.WS.OrderService.Models
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}
