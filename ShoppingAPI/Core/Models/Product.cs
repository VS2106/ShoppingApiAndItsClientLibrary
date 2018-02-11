using ShoppingAPI.Core.Models.Base;

namespace ShoppingAPI.Core.Models
{
    public class Product : BaseDomainModel
    {
        public string Name { get; set; }
        public int StockQuantity { get; set; }
    }
}