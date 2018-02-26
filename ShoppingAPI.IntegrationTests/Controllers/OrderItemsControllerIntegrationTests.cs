using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Results;
using FluentAssertions;
using NUnit.Framework;
using ShoppingAPI.Controllers;
using ShoppingAPI.Core.Dtos;
using ShoppingAPI.IntegrationTests.Extensions;

namespace ShoppingAPI.IntegrationTests.Controllers
{
    [TestFixture]
    public class OrderItemsControllerIntegrationTests : ControllerIntegrationTestsBase
    {
        private OrderItemsController _controller;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _controller = new OrderItemsController(_unitOfWork);
            _controller.MockCurrentUser();
        }

        [Test, Isolated]
        public void Get_WhenCalled_ShouldReturnOrderItem()
        {
            var orderItemInDb = CreateAnOrderItemInDb(1, _productA, _currentUserShoppingBasket);

            var result = _controller.Get(orderItemInDb.Id);
            var resultContent = ((OkNegotiatedContentResult<OrderItemGetDto>)result).Content;

            result.Should().BeOfType<OkNegotiatedContentResult<OrderItemGetDto>>();

            _unitOfWork.Reload(orderItemInDb);
            resultContent.Id.Should().Be(orderItemInDb.Id);
            resultContent.Quantity.Should().Be(orderItemInDb.Quantity);
            resultContent.Product.Id.Should().Be(orderItemInDb.ProductId);
        }

        [Test, Isolated]
        public void Post_WhenCalled_ShouldAddNewOrderItem_UpdateStockQuantityOfProduct_ReturnNewlyAddedOrderItem()
        {
            const string requestUri = "http://localhost:9862/api/OrderItems";
            _controller.Request =
                new HttpRequestMessage { RequestUri = new Uri(requestUri) };

            var orderItemPostDto = new OrderItemPostDto
            {
                ProductId = _productA.Id,
                Quantity = _productA.StockQuantity
            };

            var result = _controller.Post(orderItemPostDto);
            var resultContent = ((CreatedNegotiatedContentResult<OrderItemGetDto>)result).Content;

            _unitOfWork.Reload(_currentUserShoppingBasket);
            var newOrderItemInDb =
                _currentUserShoppingBasket.OrderItems.Single(o => o.ProductId == orderItemPostDto.ProductId);
            newOrderItemInDb.Quantity.Should().Be(orderItemPostDto.Quantity);

            result.Should().BeOfType<CreatedNegotiatedContentResult<OrderItemGetDto>>();
            resultContent.Id.Should().Be(newOrderItemInDb.Id);
            resultContent.Quantity.Should().Be(newOrderItemInDb.Quantity);
            resultContent.Product.Id.Should().Be(newOrderItemInDb.ProductId);
            ((CreatedNegotiatedContentResult<OrderItemGetDto>)result).Location.Should()
                .Be($@"{requestUri}/{newOrderItemInDb.Id}");

            _unitOfWork.Reload(_productA);
            _productA.StockQuantity.Should().Be(0);
        }

        [Test, Isolated]
        public void Post_WhenCalledButSameProductItemExistInDb_ShouldUpdateTheExistingOrderItem_UpdateStockQuantityOfProduct_ReturnTheExistingOrderItem()
        {
            const string requestUri = "http://localhost:9862/api/OrderItems";
            _controller.Request =
                new HttpRequestMessage { RequestUri = new Uri(requestUri) };

            var sameProductOrderItemInDbOriginalQuantity = 5;
            var sameProductOrderItemInDb = CreateAnOrderItemInDb(sameProductOrderItemInDbOriginalQuantity, _productA,
                _currentUserShoppingBasket);

            var orderItemPostDto = new OrderItemPostDto
            {
                ProductId = _productA.Id,
                Quantity = _productA.StockQuantity
            };
            var result = _controller.Post(orderItemPostDto);
            var resultContent = ((CreatedNegotiatedContentResult<OrderItemGetDto>)result).Content;

            _unitOfWork.Reload(_currentUserShoppingBasket);
            var newOrderItemInDb =
                _currentUserShoppingBasket.OrderItems.Single(o => o.ProductId == orderItemPostDto.ProductId);
            _unitOfWork.Reload(sameProductOrderItemInDb);

            //newOrderItemInDb and sameProductOrderItemInDb should be the same item.
            newOrderItemInDb.Id.Should().Be(sameProductOrderItemInDb.Id);
            //orderItemPostDto quantity should add to sameProductOrderItemInDb quantity
            sameProductOrderItemInDb.Quantity.Should().Be(orderItemPostDto.Quantity + sameProductOrderItemInDbOriginalQuantity);

            result.Should().BeOfType<CreatedNegotiatedContentResult<OrderItemGetDto>>();
            resultContent.Id.Should().Be(sameProductOrderItemInDb.Id);
            resultContent.Quantity.Should().Be(sameProductOrderItemInDb.Quantity);
            resultContent.Product.Id.Should().Be(sameProductOrderItemInDb.ProductId);
            ((CreatedNegotiatedContentResult<OrderItemGetDto>)result).Location.Should()
                .Be($@"{requestUri}/{sameProductOrderItemInDb.Id}");

            _unitOfWork.Reload(_productA);
            _productA.StockQuantity.Should().Be(0);
        }

        [Test, Isolated]
        public void Put_WhenCalled_ShouldUpdateQuantityOfOrderItemUpdate_UpdateStockQuantityOfProduct_ReturnOk()
        {
            var orderItemInDbOriginalQuantity = 5;
            var productAOriginalStockQuantity = _productA.StockQuantity;
            var orderItemInDb = CreateAnOrderItemInDb(orderItemInDbOriginalQuantity, _productA, _currentUserShoppingBasket);

            var orderItemPutDto = new OrderItemPutDto
            {
                Quantity = _productA.StockQuantity
            };
            var result = _controller.Put(orderItemInDb.Id, orderItemPutDto);
            result.Should().BeOfType<OkResult>();

            _unitOfWork.Reload(orderItemInDb);
            orderItemInDb.Quantity.Should().Be(orderItemPutDto.Quantity);

            _unitOfWork.Reload(_productA);
            _productA.StockQuantity.Should()
                .Be(productAOriginalStockQuantity + orderItemInDbOriginalQuantity - orderItemPutDto.Quantity);
        }

        [Test, Isolated]
        public void Deleted_WhenCalled_ShouldDeleteOrderItem_UpdateStockQuantityOfProduct_ReturnOk()
        {
            var orderItemInDbOriginalQuantity = 5;
            var productAOriginalStockQuantity = _productA.StockQuantity;
            var orderItemInDb = CreateAnOrderItemInDb(orderItemInDbOriginalQuantity, _productA, _currentUserShoppingBasket);

            var result = _controller.Delete(orderItemInDb.Id);
            result.Should().BeOfType<OkResult>();

            _unitOfWork.OrderItems.Find(orderItemInDb.Id).Should().Be(null);

            _unitOfWork.Reload(_productA);
            _productA.StockQuantity.Should().Be(orderItemInDbOriginalQuantity + productAOriginalStockQuantity);
        }

    }
}