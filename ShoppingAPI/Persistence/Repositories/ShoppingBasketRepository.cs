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
    }
}