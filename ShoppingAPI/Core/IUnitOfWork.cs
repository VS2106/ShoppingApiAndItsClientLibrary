using System;
using ShoppingAPI.Core.Repositories;

namespace ShoppingAPI.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IOrderItemRepository OrderItems { get; }
        IOrderRepository Orders { get; }
        int SaveChanges();
    }
}