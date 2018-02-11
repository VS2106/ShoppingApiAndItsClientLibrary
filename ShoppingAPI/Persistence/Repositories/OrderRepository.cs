using System.Data.Entity;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Core.Repositories;

namespace ShoppingAPI.Persistence.Repositories
{
    sealed class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(DbSet<Order> set)
            : base(set)
        {
        }
    }
}