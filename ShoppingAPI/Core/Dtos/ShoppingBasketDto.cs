using System.ComponentModel.DataAnnotations;

namespace ShoppingAPI.Core.Dtos
{
    public class ShoppingBasketDto
    {
        [Required]
        public string IdentityUserId { get; set; }
        public OrderItemDto[] OrderItems { get; set; }

        public ShoppingBasketDto()
        {
            OrderItems = new OrderItemDto[0];
        }
    }
}