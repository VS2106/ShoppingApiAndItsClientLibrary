using System.Collections.Generic;

namespace ShoppingAPI.Core.Models
{
    public class ShoppingBasket
    {
        /// <summary>
        /// ApplicationUser and ShoppingBasket is one to one relationship
        /// ApplicationUser is the pricipal in the relationship.
        /// </summary>
        public virtual ApplicationUser ApplicationUser { get; set; }
        /// <summary>
        /// ApplicationUserId is the foreign key to pricipal
        /// ApplicationUserId also used as primary key of ShoppingBasket 
        /// </summary>
        public string ApplicationUserId { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public ShoppingBasket()
        {
            OrderItems = new HashSet<OrderItem>();
        }
    }
}