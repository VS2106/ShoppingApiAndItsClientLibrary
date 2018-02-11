using ShoppingAPI.Core.Models.Base;

namespace ShoppingAPI.Core.Models
{
    public class OrderItem : BaseDomainModel
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}