using System;
using ShoppingAPI.Core.Repositories;

namespace ShoppingAPI.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IOrderItemRepository OrderItems { get; }
        IShoppingBasketRepository ShoppingBaskets { get; }
        //Reload entity from Db
        void Reload(object entity);
        int SaveChanges();
    }
}