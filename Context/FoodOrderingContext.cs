using Microsoft.EntityFrameworkCore;
using FoodOrdering.Models;

namespace FoodOrdering.Context
{
    public class FoodOrderingContext : DbContext
    {
        public FoodOrderingContext(DbContextOptions<FoodOrderingContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<MenuItems> MenuItems { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Orders> Orders { get; set; }

        public DbSet<Categories> Categories { get; set; }

        public DbSet<Tables> Tables { get; set; }

        public DbSet<Payments> Payments { get; set; }
    
    }
}
