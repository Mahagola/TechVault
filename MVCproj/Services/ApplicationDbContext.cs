using Microsoft.EntityFrameworkCore;
using MVCproj.Models;

namespace MVCproj.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { 
            
        }
        public DbSet<Product>Prodcts{ get; set; }
    }
}
