using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using NUnit.Framework;
using ShoppingAPI.Controllers;
using ShoppingAPI.Core.Dtos;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Tests.Extensions;

namespace ShoppingAPI.Tests.Controllers
{
    [TestFixture]
    public class ShoppingBasketControllerTests : ControllerTestsBase
    {
        private ShoppingBasketController _controller;

        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            _controller = new ShoppingBasketController(_mockUnitOfWork.Object);
            _controller.MockCurrentUser(_applicationUserId, "test1@api.com");
        }

        [Test]
        public void Get_NoShoppingBasketFoundForCurrentUser_ShouldReturnInternalServerError()
        {
            _mockShoppingBasketRepository.Setup(i => i.Find(_applicationUserId)).Returns((ShoppingBasket)null);
            var result = _controller.Get();

            result.Should().BeOfType<InternalServerErrorResult>();
        }

        [Test]
        public void Get_ValidRequest_ShouldReturnOkWithCurrentUserShoppingBasket()
        {
            var result = _controller.Get();
            var resultContent = ((OkNegotiatedContentResult<ShoppingBasketDto>)result).Content;

            result.Should().BeOfType<OkNegotiatedContentResult<ShoppingBasketDto>>();
            resultContent.Id.Should().Be(_applicationUserId);
            resultContent.OrderItems.Select(o => o.Id).OrderBy(t => t)
                .SequenceEqual(_currentUserShoppingBasketThatHasOrderItem1.OrderItems.Select(o => o.Id).OrderBy(t => t))
                .Should()
                .Be(true);
        }

        [Test]
        public void ClearOut_NoShoppingBasketFoundForCurrentUser_ShouldReturnInternalServerError()
        {
            _mockShoppingBasketRepository.Setup(i => i.Find(_applicationUserId)).Returns((ShoppingBasket)null);
            var result = _controller.ClearOut();

            result.Should().BeOfType<InternalServerErrorResult>();
        }


        [Test]
        public void ClearOut_ValidRequest_ShouldReturnOk_UpdateStockQuantityOfProduct()
        {
            var orderItemThatNeedToBeClearOut = _currentUserShoppingBasketThatHasOrderItem1
                .OrderItems.First(o => o.ProductId == 1);
            var originalStockQuantityOfProduct = orderItemThatNeedToBeClearOut.Product.StockQuantity;
            var originalQuantityOfOrderItem = orderItemThatNeedToBeClearOut.Quantity;

            var result = _controller.ClearOut();

            result.Should().BeOfType<OkResult>();
            _product1.StockQuantity.Should()
                .Be(originalStockQuantityOfProduct + originalQuantityOfOrderItem);
        }
    }
}