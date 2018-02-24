using System.ComponentModel.DataAnnotations;

namespace ShoppingAPI.Core.Dtos
{
    public class ShoppingBasketDto
    {
        [Required]
        public string Id { get; set; }
        public OrderItemGetDto[] OrderItems { get; set; }

        public ShoppingBasketDto()
        {
            OrderItems = new OrderItemGetDto[0];
        }
    }
}