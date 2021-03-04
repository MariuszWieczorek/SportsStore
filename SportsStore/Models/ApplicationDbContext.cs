using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;


// klasa bazowa DBContext zapewnia dostęp do funkcjonalności Entity Framework Core
// właściwość Product zapewnia dostęp do obiektów typu product
namespace SportsStore.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
            options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}