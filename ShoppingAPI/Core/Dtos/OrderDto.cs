using System.ComponentModel.DataAnnotations;

namespace ShoppingAPI.Core.Dtos
{
    public class OrderDto
    {
        [Required]
        public string IdentityUserId { get; set; }
        public OrderItemDto[] OrderItems { get; set; }

        public OrderDto()
        {
            OrderItems = new OrderItemDto[0];
        }
    }
}