using System;
using System.Collections.Generic;
using ShoppingAPI.Core;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Core.Repositories;
using ShoppingAPI.Persistence.Repositories;

namespace ShoppingAPI.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ShoppingApiDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public IProductRepository Products => GetOrCreateRepository<ProductRepository, Product>();
        public IOrderItemRepository OrderItems => GetOrCreateRepository<OrderItemRepository, OrderItem>();
        public IOrderRepository Orders => GetOrCreateRepository<OrderRepository, Order>();

        public UnitOfWork(ShoppingApiDbContext context)
        {
            this._context = context;
        }

        TRepo GetOrCreateRepository<TRepo, TEntity>()
            where TRepo : class
            where TEntity : class
        {
            var repoType = typeof(TRepo);

            if (!_repositories.ContainsKey(repoType))
            {
                var repo = Activator.CreateInstance(repoType, _context.Set<TEntity>());
                _repositories.Add(repoType, repo);
            }

            return _repositories[repoType] as TRepo;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}