using CoffeeOrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeOrderAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Order> Orders { get; set; }
    }
}
