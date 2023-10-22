using Microsoft.EntityFrameworkCore;

namespace Price_Scraping.Models
{
    public class ProductContext: DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ProductContext(DbContextOptions options) : base(options) { }
    }
}
