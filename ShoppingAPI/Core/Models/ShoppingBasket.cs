using System.Collections.Generic;
using ShoppingAPI.Core.Models.Base;

namespace ShoppingAPI.Core.Models
{
    public class ShoppingBasket : BaseDomainModel
    {
        public string IdentityUserId { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public ShoppingBasket()
        {
            OrderItems = new HashSet<OrderItem>();
        }
    }
}