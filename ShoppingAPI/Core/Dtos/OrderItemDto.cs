using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace ShoppingAPI.Core.Dtos
{
    public class OrderItemDtoPutDto
    {
        [Required]
        [IntegerValidator(MinValue = 1)]
        public int Quantity { get; set; }
    }

    public class OrderItemDtoPostDto : OrderItemDtoPutDto
    {
        [Required]
        public int ShoppingBasketId { get; set; }
        [Required]
        public int ProductId { get; set; }

    }

    public class OrderItemDtoGetDto : OrderItemDtoPutDto
    {
        public int Id { get; set; }
        public ProductDto Product { get; set; }
    }
}