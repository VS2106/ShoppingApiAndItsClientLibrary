using System.Data.Entity;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Core.Repositories;

namespace ShoppingAPI.Persistence.Repositories
{
    sealed class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(DbSet<Product> set)
            : base(set)
        {
        }
    }
}