using System;

namespace ShoppingAPI.Core
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
    }
}