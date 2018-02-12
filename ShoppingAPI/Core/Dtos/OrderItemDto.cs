using System.ComponentModel.DataAnnotations;

namespace ShoppingAPI.Core.Dtos
{
    public class OrderItemDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public ProductDto Product { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}