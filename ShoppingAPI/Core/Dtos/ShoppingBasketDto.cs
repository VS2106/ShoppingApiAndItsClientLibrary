using System.ComponentModel.DataAnnotations;

namespace ShoppingAPI.Core.Dtos
{
    public class ShoppingBasketDto
    {
        [Required]
        public string Id { get; set; }
        public OrderItemDtoGetDto[] OrderItems { get; set; }

        public ShoppingBasketDto()
        {
            OrderItems = new OrderItemDtoGetDto[0];
        }
    }
}