using System.ComponentModel.DataAnnotations;

namespace ShoppingAPI.Core.Dtos
{
    public class ShoppingBasketDto
    {
        [Required]
        public string IdentityUserId { get; set; }
        public OrderItemDtoGetDto[] OrderItemsDto { get; set; }

        public ShoppingBasketDto()
        {
            OrderItemsDto = new OrderItemDtoGetDto[0];
        }
    }
}