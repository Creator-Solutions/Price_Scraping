using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Price_Scraping.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Transactions;

namespace Price_Scraping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private ProductContext _productContext { get; }

        public HomeController(ILogger<HomeController> logger, ProductContext context)
        {
            _logger = logger;
            this._productContext = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Privacy Scraper";
            
            List<Product> products = (from product in this._productContext.Products.Take(10)
                                      select product).ToList();

            ViewData["TotalProducts"] = products.Count;
            var lastDateSearched = this._productContext.Products
                                 .Take(10)
                                 .GroupBy(product => product.ProductId) 
                                 .Select(group => group.First().DateSearched)
                                 .ToList();

            DateTime lastDate = DateTime.Now;
            string formattedDate = lastDate.ToString("dd MMM yyyy HH:mm");

            ViewData["DateSearched"] = formattedDate;

            return View(products);
        }

        public async Task<IActionResult> Scraper()
        {
            List<Product> products = new();

            await Task.Run(() =>
            {
                ChromeOptions options = new();
                options.AddArguments("headless");

                ChromeDriver driver = new(options);
                driver.Navigate().GoToUrl("https://www.takealot.com/computers/components-26415?sort=Relevance");

                List<IWebElement> titles = driver.FindElements(By.XPath("//div[@data-ref='product-card']//div[contains(@class, product-card-module_title-wrapper_1sj9D)]//h4[contains(@class, product-title)]")).ToList();
                List<IWebElement> prices = driver.FindElements(By.XPath("//div[@data-ref='product-card']//div[contains(@class, product-card-module_title-wrapper_1sj9D)]//ul//li[@data-ref='price']//span[contains(@class, price)]")).ToList();
                List<IWebElement> links = driver.FindElements(By.XPath("//div[@data-ref='product-card']//a[@target='_blank']")).ToList();

                List<string> titleContents = titles.Select(title => title.Text).ToList();
                List<string> productLinks = links.Select(title => title.GetAttribute("href")).ToList();
                List<string> priceContents = prices.Select(price => price.Text).ToList();

                var combinedList = titleContents.Zip(priceContents, (title, price) => new { Title = title, Price = price })
                        .Zip(productLinks, (combinedList, link) => new { combinedList.Title, Url = link, combinedList.Price });


                int count = 1;
                var db = _productContext;
                var transaction = db.Database.BeginTransaction();
                db.Database.ExecuteSqlAsync($"SET IDENTITY_INSERT Products ON");
                foreach (var product in combinedList)
                {
                    Product prod = new()
                    {
                        ProductId = count,
                        ProductName = product.Title,
                        ProductLink = product.Url,
                        DateSearched = DateTime.Now,
                        ProductPrice = product.Price
                    };

                    products.Add(prod);

                
                    db.AddAsync(prod);                   
                    db.SaveChangesAsync();                                       
                    count++;
                }

                transaction.CommitAsync();                
            });
            

            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}