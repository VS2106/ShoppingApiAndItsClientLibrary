using System.Collections.Generic;
using ShoppingAPI.Core.Models.Base;

namespace ShoppingAPI.Core.Models
{
    public class Order : BaseDomainModel
    {
        public string IdentityUserId { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }
    }
}