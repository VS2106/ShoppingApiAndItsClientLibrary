using ShoppingAPI.Core.Models;

namespace ShoppingAPI.Core.Repositories
{
    public interface IShoppingBasketRepository : IBaseRepository<ShoppingBasket>
    {
        ShoppingBasket FindByUserId(string id);
    }
}