using System.ComponentModel.DataAnnotations;

namespace ShoppingAPI.Core.Dtos
{
    public class OrderItemPutDto
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class OrderItemPostDto : OrderItemPutDto
    {
        public int ProductId { get; set; }

    }

    public class OrderItemGetDto : OrderItemPutDto
    {
        public int Id { get; set; }
        public ProductDto Product { get; set; }
    }
}