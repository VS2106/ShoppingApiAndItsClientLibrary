using System.Collections.Generic;

namespace ShoppingAPI.Core.Models
{
    public class ShoppingBasket
    {
        public string Id => ApplicationUserId;
        public string ApplicationUserId { get; set; }
        /// <summary>
        /// ApplicationUser and ShoppingBasket is one to one relationship
        /// ApplicationUser is the pricipal in the relationship.
        /// </summary>
        public virtual ApplicationUser ApplicationUser { get; set; }


        public ICollection<OrderItem> OrderItems { get; set; }

        public ShoppingBasket()
        {
            OrderItems = new HashSet<OrderItem>();
        }
    }
}