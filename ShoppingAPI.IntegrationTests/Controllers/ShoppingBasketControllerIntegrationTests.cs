using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using NUnit.Framework;
using ShoppingAPI.Controllers;
using ShoppingAPI.Core.Dtos;
using ShoppingAPI.IntegrationTests.Extensions;

namespace ShoppingAPI.IntegrationTests.Controllers
{
    [TestFixture]
    public class ShoppingBasketControllerIntegrationTests : ControllerIntegrationTestsBase
    {
        private ShoppingBasketController _controller;

        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            _controller = new ShoppingBasketController(_unitOfWork);
            _controller.MockCurrentUser();
        }

        [Test, Isolated]
        public void Get_WhenCalled_ShouldReturnOKWithCurrentUserShoppingBasket()
        {
            CreateAnOrderItemInDb(3, _productA, _currentUserShoppingBasket);

            var result = _controller.Get();
            var resultContent = ((OkNegotiatedContentResult<ShoppingBasketDto>)result).Content;
            result.Should().BeOfType<OkNegotiatedContentResult<ShoppingBasketDto>>();

            _unitOfWork.Reload(_currentUserShoppingBasket);
            resultContent.Id.Should().Be(_currentUserShoppingBasket.Id);
            resultContent
               .OrderItems.OrderBy(i => i.Id).Select(i => $"{i.Id}{i.Product.Name}{i.Quantity}")
               .SequenceEqual(
                   _currentUserShoppingBasket
                       .OrderItems.OrderBy(i => i.Id).Select(i => $"{i.Id}{i.Product.Name}{i.Quantity}"))
               .Should()
               .Be(true);
        }

        [Test, Isolated]
        public void ClearOut_WhenCalled_ShouldReturnOK_RemoveAllOrderItemOfCurrentUserShoppingBasket_UpdateStockQuantityOfProduct()
        {
            var orderItemInDbOriginalQuantity = 5;
            var productAOriginalStockQuantity = _productA.StockQuantity;
            CreateAnOrderItemInDb(orderItemInDbOriginalQuantity, _productA, _currentUserShoppingBasket);

            var result = _controller.ClearOut();
            result.Should().BeOfType<OkResult>();

            _unitOfWork.Reload(_currentUserShoppingBasket);
            _currentUserShoppingBasket.OrderItems.Count().Should().Be(0);

            _unitOfWork.Reload(_productA);
            _productA.StockQuantity.Should().Be(productAOriginalStockQuantity + orderItemInDbOriginalQuantity);
        }


    }
}
