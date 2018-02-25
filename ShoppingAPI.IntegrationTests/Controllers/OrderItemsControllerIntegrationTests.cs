using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Results;
using FluentAssertions;
using NUnit.Framework;
using ShoppingAPI.Controllers;
using ShoppingAPI.Core.Dtos;
using ShoppingAPI.Core.Models;
using ShoppingAPI.IntegrationTests.Extensions;

namespace ShoppingAPI.IntegrationTests.Controllers
{
    [TestFixture]
    public class OrderItemsControllerIntegrationTests : ControllerIntegrationTestsBase
    {
        private OrderItemsController _controller;

        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            _controller = new OrderItemsController(_unitOfWork);
            _controller.MockCurrentUser();
        }

        [Test, Isolated]
        public void Get_WhenCalled_ShouldReturnOrderItem()
        {
            var orderItem = new OrderItem
            {
                Product = _productA,
                Quantity = 1,
                ShoppingBasket = _currentUserShoppingBasket
            };
            _unitOfWork.OrderItems.Add(orderItem);
            _unitOfWork.SaveChanges();

            var result = _controller.Get(orderItem.Id);
            var resultContent = ((OkNegotiatedContentResult<OrderItemGetDto>)result).Content;

            result.Should().BeOfType<OkNegotiatedContentResult<OrderItemGetDto>>();

            _unitOfWork.Reload(orderItem);
            resultContent.Id.Should().Be(orderItem.Id);
            resultContent.Quantity.Should().Be(orderItem.Quantity);
            resultContent.Product.Id.Should().Be(orderItem.ProductId);
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

            var sameProductOrderItemOriginalQuantity = 5;
            var sameProductOrderItem = new OrderItem
            {
                Product = _productA,
                ShoppingBasket = _currentUserShoppingBasket,
                Quantity = sameProductOrderItemOriginalQuantity
            };
            _unitOfWork.OrderItems.Add(sameProductOrderItem);
            _unitOfWork.SaveChanges();

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
            _unitOfWork.Reload(sameProductOrderItem);

            //newOrderItemInDb and existingOrderItem should be the same item.
            newOrderItemInDb.Id.Should().Be(sameProductOrderItem.Id);
            //new order item quantity add to existing order item quantity
            sameProductOrderItem.Quantity.Should().Be(orderItemPostDto.Quantity + sameProductOrderItemOriginalQuantity);

            result.Should().BeOfType<CreatedNegotiatedContentResult<OrderItemGetDto>>();
            resultContent.Id.Should().Be(sameProductOrderItem.Id);
            resultContent.Quantity.Should().Be(sameProductOrderItem.Quantity);
            resultContent.Product.Id.Should().Be(sameProductOrderItem.ProductId);
            ((CreatedNegotiatedContentResult<OrderItemGetDto>)result).Location.Should()
                .Be($@"{requestUri}/{sameProductOrderItem.Id}");

            _unitOfWork.Reload(_productA);
            _productA.StockQuantity.Should().Be(0);
        }

        [Test, Isolated]
        public void Put_WhenCalled_ShouldUpdateQuantityOfOrderItemUpdate_UpdateStockQuantityOfProduct_ReturnOk()
        {
            var orderItemOriginalQuantity = 5;
            var productAOriginalStockQuantity = _productA.StockQuantity;
            var orderItem = new OrderItem
            {
                Product = _productA,
                ShoppingBasket = _currentUserShoppingBasket,
                Quantity = orderItemOriginalQuantity
            };
            _unitOfWork.OrderItems.Add(orderItem);
            _unitOfWork.SaveChanges();

            var orderItemPutDto = new OrderItemPutDto
            {
                Quantity = _productA.StockQuantity
            };
            var result = _controller.Put(orderItem.Id, orderItemPutDto);
            result.Should().BeOfType<OkResult>();

            _unitOfWork.Reload(orderItem);
            orderItem.Quantity.Should().Be(orderItemPutDto.Quantity);

            _unitOfWork.Reload(_productA);
            _productA.StockQuantity.Should()
                .Be(productAOriginalStockQuantity + orderItemOriginalQuantity - orderItemPutDto.Quantity);
        }

        [Test, Isolated]
        public void Deleted_WhenCalled_ShouldDeleteOrderItem_UpdateStockQuantityOfProduct_ReturnOk()
        {
            var orderItemOriginalQuantity = 5;
            var productAOriginalStockQuantity = _productA.StockQuantity;
            var orderItem = new OrderItem
            {
                Product = _productA,
                ShoppingBasket = _currentUserShoppingBasket,
                Quantity = orderItemOriginalQuantity
            };
            _unitOfWork.OrderItems.Add(orderItem);
            _unitOfWork.SaveChanges();


            var result = _controller.Delete(orderItem.Id);
            result.Should().BeOfType<OkResult>();

            _unitOfWork.OrderItems.Find(orderItem.Id).Should().Be(null);

            _unitOfWork.Reload(_productA);
            _productA.StockQuantity.Should().Be(orderItemOriginalQuantity + productAOriginalStockQuantity);
        }

    }
}