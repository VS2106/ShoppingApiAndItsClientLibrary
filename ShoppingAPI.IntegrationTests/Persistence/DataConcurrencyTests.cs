using System.Data.Entity.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace ShoppingAPI.IntegrationTests.Persistence
{
    [TestFixture]
    public class DataConcurrencyTests : PersistenceTestBase
    {
        [Test, Isolated]
        public void ABLoadedProductA_ThenAChangedProductAQuantity_ThenBTryToChangeProductAQuantity_ShouldThrowDbUpdateConcurrencyException()
        {
            var unitOfWorkAProductA = _unitOfWorkA.Products.First(p => p.Name == "ProductA");
            var unitOfWorkBProductA = _unitOfWorkB.Products.First(p => p.Name == "ProductA");

            unitOfWorkAProductA.StockQuantity = unitOfWorkAProductA.StockQuantity + 1;
            _unitOfWorkA.SaveChanges();

            unitOfWorkBProductA.StockQuantity = unitOfWorkBProductA.StockQuantity + 2;
            _unitOfWorkB.Invoking(b => b.SaveChanges()).Should().Throw<DbUpdateConcurrencyException>();
        }

        [Test, Isolated]
        public void ABLoadedSameOrderItem_ThenAChangedItsQuantity_ThenBTryToChangeItsQuantity_ShouldThrowDbUpdateConcurrencyException()
        {
            var orderItemInDb = CreateAnOrderItemInDb();

            var unitOfWorkATheOrderItem = _unitOfWorkA.OrderItems.First(o => o.Id == orderItemInDb.Id);
            var unitOfWorkBTheOrderItem = _unitOfWorkB.OrderItems.First(o => o.Id == orderItemInDb.Id);

            unitOfWorkATheOrderItem.Quantity = unitOfWorkATheOrderItem.Quantity + 1;
            _unitOfWorkA.SaveChanges();

            unitOfWorkBTheOrderItem.Quantity = unitOfWorkBTheOrderItem.Quantity + 2;
            _unitOfWorkB.Invoking(b => b.SaveChanges()).Should().Throw<DbUpdateConcurrencyException>();
        }

        [Test, Isolated]
        public void ABLoadedSameOrderItem_ThenADeltedTheOrderItem_ThenBTryToChangeItsQuantity_ShouldThrowDbUpdateConcurrencyException()
        {
            var orderItemInDb = CreateAnOrderItemInDb();

            var unitOfWorkATheOrderItem = _unitOfWorkA.OrderItems.First(o => o.Id == orderItemInDb.Id);
            var unitOfWorkBTheOrderItem = _unitOfWorkB.OrderItems.First(o => o.Id == orderItemInDb.Id);

            _unitOfWorkA.OrderItems.Delete(unitOfWorkATheOrderItem);
            _unitOfWorkA.SaveChanges();

            unitOfWorkBTheOrderItem.Quantity = unitOfWorkBTheOrderItem.Quantity + 2;
            _unitOfWorkB.Invoking(b => b.SaveChanges()).Should().Throw<DbUpdateConcurrencyException>();
        }

        [Test, Isolated]
        public void ABLoadedSameOrderItem_ThenADeltedTheOrderItem_ThenBTryToDeleteIt_ShouldThrowDbUpdateConcurrencyException()
        {
            var orderItemInDb = CreateAnOrderItemInDb();

            var unitOfWorkATheOrderItem = _unitOfWorkA.OrderItems.First(o => o.Id == orderItemInDb.Id);
            var unitOfWorkBTheOrderItem = _unitOfWorkB.OrderItems.First(o => o.Id == orderItemInDb.Id);

            _unitOfWorkA.OrderItems.Delete(unitOfWorkATheOrderItem);
            _unitOfWorkA.SaveChanges();

            _unitOfWorkB.OrderItems.Delete(unitOfWorkBTheOrderItem);
            _unitOfWorkB.Invoking(b => b.SaveChanges()).Should().Throw<DbUpdateConcurrencyException>();
        }
    }
}
