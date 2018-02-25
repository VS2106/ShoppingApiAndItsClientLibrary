using System.Data.Entity.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.IntegrationTests.Persistence
{
    [TestFixture]
    public class UniqueIndexOnMultipleColumnsTests : PersistenceTestBase
    {
        [Test, Isolated]
        public void ABGoingToAddAnOrderItemWithSameProductToTheSameShoppingBasket_ThenAAddedIt_ThenBTryToAddIt_ShouldThrowDbUpdateException()
        {
            var productAId = _unitOfWorkA.Products.First(p => p.Name == "ProductA").Id;
            var currentUserShoppingBasketId = _unitOfWorkA.ShoppingBaskets.FindByUserId(GlobalSetUp._currentUserId).Id;

            var newOrderItemUnitOfWorkAWantToAdd = new OrderItem()
            {
                ProductId = productAId,
                ShoppingBasketId = currentUserShoppingBasketId,
                Quantity = 5
            };

            var newOrderItemUnitOfWorkBWantToAdd = new OrderItem()
            {
                ProductId = productAId,
                ShoppingBasketId = currentUserShoppingBasketId,
                Quantity = 10
            };


            _unitOfWorkA.OrderItems.Add(newOrderItemUnitOfWorkAWantToAdd);
            _unitOfWorkA.SaveChanges();

            _unitOfWorkB.OrderItems.Add(newOrderItemUnitOfWorkBWantToAdd);
            _unitOfWorkB.Invoking(b => b.SaveChanges()).Should().Throw<DbUpdateException>();
        }
    }
}
