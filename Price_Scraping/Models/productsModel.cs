namespace Price_Scraping.Models
{
    public class ProductsModel
    {

        private string? productNumber;
        private string? productName;
        private string? dateSearched;
        private string? productPrice;

        public string? ProductNumber
        {
            get { return productNumber; }
            set { productNumber = value; }
        }

        public string? ProductName
        {
            get { return productName; }
            set { productName = value; }
        }

        public string? DateSearched
        {
            get { return dateSearched; }
            set { dateSearched = value; }
        }

        public string? ProductPrice
        {
            get { return productPrice; }
            set { productPrice = value; }
        }
    }
}
