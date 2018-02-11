using System.Data.Entity;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Core.Repositories;

namespace ShoppingAPI.Persistence.Repositories
{
    sealed class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(DbSet<OrderItem> set)
            : base(set)
        {
        }
    }
}