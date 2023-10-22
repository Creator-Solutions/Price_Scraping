using Microsoft.EntityFrameworkCore;

namespace Price_Scraping.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductLink { get; set; }

        public string ProductName { get; set; }
        public DateTime DateSearched { get; set; }

        public string ProductPrice { get; set; }
    }
}
