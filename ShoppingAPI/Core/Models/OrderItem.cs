namespace ShoppingAPI.Core.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        public string ShoppingBasketId { get; set; }
        public virtual ShoppingBasket ShoppingBasket { get; set; }
    }
}