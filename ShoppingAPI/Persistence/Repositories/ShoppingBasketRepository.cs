using System.Data.Entity;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Core.Repositories;

namespace ShoppingAPI.Persistence.Repositories
{
    sealed class ShoppingBasketRepository : BaseRepository<ShoppingBasket>, IShoppingBasketRepository
    {
        public ShoppingBasketRepository(DbSet<ShoppingBasket> set)
            : base(set)
        {
        }

        public ShoppingBasket FindByUserId(string id)
        {
            /* ApplicationUser and ShoppingBasket is one to one relationship
             * ApplicationUser is the pricipal in the relationship.
             * ShoppingBasket uses ApplicationUser's Id as primary key */
            return Find(id);
        }
    }
}