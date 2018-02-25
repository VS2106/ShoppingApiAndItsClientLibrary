using NUnit.Framework;
using ShoppingAPI.Core;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Persistence;

namespace ShoppingAPI.IntegrationTests.Persistence
{
    public class PersistenceTestBase
    {
        protected IUnitOfWork _unitOfWorkA;
        protected IUnitOfWork _unitOfWorkB;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkA = new UnitOfWork(new ShoppingApiDbContext());
            _unitOfWorkB = new UnitOfWork(new ShoppingApiDbContext());
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWorkA.Dispose();
            _unitOfWorkB.Dispose();
        }

        protected OrderItem CreateAnOrderItemInDb()
        {
            var orderItem = new OrderItem()
            {
                Product = _unitOfWorkA.Products.First(),
                Quantity = 5,
                ShoppingBasket = _unitOfWorkA.ShoppingBaskets.First()
            };
            _unitOfWorkA.OrderItems.Add(orderItem);
            _unitOfWorkA.SaveChanges();
            return orderItem;
        }
    }
}
