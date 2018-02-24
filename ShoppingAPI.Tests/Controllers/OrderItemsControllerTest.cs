using System;
using System.Net.Http;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingAPI.App_Start;
using ShoppingAPI.Controllers;
using ShoppingAPI.Core;
using ShoppingAPI.Core.Dtos;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Core.Repositories;
using ShoppingAPI.Tests.Extensions;

namespace ShoppingAPI.Tests.Controllers
{
    [TestClass]
    public class OrderItemsControllerTest
    {
        private readonly string _applicationUserId = Guid.NewGuid().ToString("N");
        private OrderItemsController _controller;
        private ShoppingBasket _currentUserShoppingBasketThatHasOrderItem1;
        private Mock<IOrderItemRepository> _mockOrderItemRepository;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IShoppingBasketRepository> _mockShoppingBasketRepository;
        private OrderItem _orderItem1ThatHasProduct1;
        private OrderItem _orderItem2ThatDoesNotBelongToCurrentUser;
        private Product _product1;
        private Product _product2;


        [TestInitialize]
        public void TestInitialize()
        {
            _mockOrderItemRepository = new Mock<IOrderItemRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockShoppingBasketRepository = new Mock<IShoppingBasketRepository>();

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.SetupGet(u => u.OrderItems).Returns(_mockOrderItemRepository.Object);
            mockUnitOfWork.SetupGet(u => u.Products).Returns(_mockProductRepository.Object);
            mockUnitOfWork.SetupGet(u => u.ShoppingBaskets).Returns(_mockShoppingBasketRepository.Object);

            _controller = new OrderItemsController(mockUnitOfWork.Object);
            _controller.MockCurrentUser(_applicationUserId, "test1@api.com");


            AutoMapperConfig.Initialize();

            SetUpDomainData();
        }

        private void SetUpDomainData()
        {
            _product1 = new Product
            {
                StockQuantity = 10,
                Id = 1,
                Name = "Candy"
            };
            _mockProductRepository.Setup(p => p.Find(_product1.Id)).Returns(_product1);

            _product2 = new Product
            {
                StockQuantity = 5,
                Id = 2,
                Name = "Cola"
            };
            _mockProductRepository.Setup(p => p.Find(_product2.Id)).Returns(_product2);

            _currentUserShoppingBasketThatHasOrderItem1 = new ShoppingBasket
            {
                ApplicationUserId = _applicationUserId,
            };
            _orderItem1ThatHasProduct1 = new OrderItem
            {
                Id = 1,
                Quantity = 5,
                ProductId = _product1.Id,
                Product = _product1,
                ShoppingBasket = _currentUserShoppingBasketThatHasOrderItem1,
                ShoppingBasketId = _currentUserShoppingBasketThatHasOrderItem1.Id
            };
            _currentUserShoppingBasketThatHasOrderItem1.OrderItems.Add(_orderItem1ThatHasProduct1);
            _mockOrderItemRepository.Setup(i => i.Find(_orderItem1ThatHasProduct1.Id))
                .Returns(_orderItem1ThatHasProduct1);
            _mockShoppingBasketRepository.Setup(i => i.Find(_applicationUserId))
                .Returns(_currentUserShoppingBasketThatHasOrderItem1);

            _orderItem2ThatDoesNotBelongToCurrentUser = new OrderItem
            {
                Id = 2,
                Quantity = 5,
                ShoppingBasket = new ShoppingBasket { ApplicationUserId = _applicationUserId + "Q" }
            };
            _mockOrderItemRepository.Setup(i => i.Find(_orderItem2ThatDoesNotBelongToCurrentUser.Id))
                .Returns(_orderItem2ThatDoesNotBelongToCurrentUser);
        }


        [TestMethod]
        public void Get_NoOrderItemWithGivenIdExist_ShouldReturnNotFound()
        {
            var result = _controller.Get(489);
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public void Get_OrderItemNotBelongToCurrentUser_ShouldReturnUnauthorized()
        {
            var result = _controller.Get(_orderItem2ThatDoesNotBelongToCurrentUser.Id);
            result.Should().BeOfType<UnauthorizedResult>();
        }


        [TestMethod]
        public void Get_ValidRequest_ShouldReturnOKWithTheOrderItem()
        {
            var result = _controller.Get(_orderItem1ThatHasProduct1.Id);
            var okNegotiatedContentResultContent =
                ((OkNegotiatedContentResult<OrderItemGetDto>)result).Content;
            result.Should().BeOfType<OkNegotiatedContentResult<OrderItemGetDto>>();

            okNegotiatedContentResultContent.Id.Should().Be(_orderItem1ThatHasProduct1.Id);
            okNegotiatedContentResultContent.Product.Id.Should().Be(_orderItem1ThatHasProduct1.ProductId);
            okNegotiatedContentResultContent.Quantity.Should().Be(_orderItem1ThatHasProduct1.Quantity);
        }

        [TestMethod]
        public void Post_NotValidRequest_ShouldReturnBadRequest()
        {
            _controller.ModelState.AddModelError("Quantity", "test");
            var result = _controller.Post(new OrderItemPostDto());
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public void Post_NoShoppingBasketFoundForCurrentUser_ShouldReturnInternalServerError()
        {
            _mockShoppingBasketRepository.Setup(i => i.Find(_applicationUserId)).Returns((ShoppingBasket)null);
            var result = _controller.Post(new OrderItemPostDto { Quantity = 1, ProductId = _product1.Id });
            result.Should().BeOfType<InternalServerErrorResult>();
        }

        [TestMethod]
        public void Post_ProductNotFound_ShouldReturnBadRequestWithErrorMessage()
        {
            var productId = 123;
            _mockProductRepository.Setup(p => p.Find(productId)).Returns((Product)null);
            var result = _controller.Post(new OrderItemPostDto { Quantity = 1, ProductId = productId });

            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Contain(productId.ToString());
        }

        [TestMethod]
        public void Post_StockQuantityOfProductIsNotEnough_ShouldReturnBadRequestWithErrorMessage()
        {
            var quantityOfOrderItem = 800;
            var result =
                _controller.Post(new OrderItemPostDto { Quantity = quantityOfOrderItem, ProductId = _product1.Id });
            var resultMessage = ((BadRequestErrorMessageResult)result).Message;

            result.Should().BeOfType<BadRequestErrorMessageResult>();
            resultMessage.Should().Contain(_product1.StockQuantity.ToString());
            resultMessage.Should().Contain(quantityOfOrderItem.ToString());
        }


        [TestMethod]
        public void
            Post_SameProductOrderItemExist_ShouldUpdateQuantityOfExistingOrderItem_UpdateStockQuantityOfProduct_ReturnCorrectLocation()
        {
            var originalStockQuantityOfProduct1 = _product1.StockQuantity;
            var originalQuantityOfOrderItem1ThatHasProduct1 = _orderItem1ThatHasProduct1.Quantity;
            var orderItemPostDto = new OrderItemPostDto { Quantity = 5, ProductId = _product1.Id };
            const string requestUri = "http://localhost:9862/api/OrderItems";
            _controller.Request =
                new HttpRequestMessage { RequestUri = new Uri(requestUri) };

            var result = _controller.Post(orderItemPostDto);
            var createdNegotiatedContentResult = ((CreatedNegotiatedContentResult<OrderItemGetDto>)result);

            result.Should().BeOfType<CreatedNegotiatedContentResult<OrderItemGetDto>>();
            _product1.StockQuantity.Should().Be(originalStockQuantityOfProduct1 - orderItemPostDto.Quantity);
            _orderItem1ThatHasProduct1.Quantity.Should()
                .Be(originalQuantityOfOrderItem1ThatHasProduct1 + orderItemPostDto.Quantity);
            createdNegotiatedContentResult.Location.Should()
                .Be($@"{requestUri}/{_orderItem1ThatHasProduct1.Id}");
            createdNegotiatedContentResult.Content.Id.Should().Be(_orderItem1ThatHasProduct1.Id);
            createdNegotiatedContentResult.Content.Product.Id.Should().Be(_product1.Id);
            createdNegotiatedContentResult.Content.Quantity.Should()
                .Be(originalQuantityOfOrderItem1ThatHasProduct1 + orderItemPostDto.Quantity);
        }

        [TestMethod]
        public void Post_ValidRequest_ShouldCreateNewOrderItem_UpdateStockQuantityOfProduct_ReturnCreated()
        {
            var originalStockQuantityOfProduct2 = _product2.StockQuantity;
            var orderItemPostDto = new OrderItemPostDto { Quantity = 2, ProductId = _product2.Id };
            const string requestUri = "http://localhost:9862/api/OrderItems";
            _controller.Request =
                new HttpRequestMessage { RequestUri = new Uri(requestUri) };

            var result = _controller.Post(orderItemPostDto);
            var createdNegotiatedContentResult = (CreatedNegotiatedContentResult<OrderItemGetDto>)result;

            result.Should().BeOfType<CreatedNegotiatedContentResult<OrderItemGetDto>>();
            _product2.StockQuantity.Should().Be(originalStockQuantityOfProduct2 - orderItemPostDto.Quantity);
            createdNegotiatedContentResult.Content.Quantity.Should()
                .Be(orderItemPostDto.Quantity);
        }

        [TestMethod]
        public void Put_NotValidRequest_ShouldReturnBadRequest()
        {
            _controller.ModelState.AddModelError("Quantity", "test");
            var result = _controller.Put(1, new OrderItemPutDto());
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public void Put_NoOrderItemWithGivenIdExists_ShouldReturnNotFound()
        {
            var result = _controller.Put(456, new OrderItemPutDto());
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public void Put_OrderItemNotBelongToCurrentUser_ShouldReturnUnauthorized()
        {
            var result = _controller.Put(_orderItem2ThatDoesNotBelongToCurrentUser.Id, new OrderItemPutDto());
            result.Should().BeOfType<UnauthorizedResult>();
        }


        [TestMethod]
        public void Put_StockQuantityOfProductIsNotEnough_ShouldReturnBadRequestWithErrorMessage()
        {
            var quantityOfOrderItem = 800;
            var originalQuantityOfOrderItem1ThatHasProduct1 = _orderItem1ThatHasProduct1.Quantity;
            var result =
                _controller.Put(
                    _orderItem1ThatHasProduct1.Id,
                    new OrderItemPutDto { Quantity = quantityOfOrderItem });
            var resultMessage = ((BadRequestErrorMessageResult)result).Message;

            result.Should().BeOfType<BadRequestErrorMessageResult>();
            resultMessage.Should().Contain((_product1.StockQuantity + originalQuantityOfOrderItem1ThatHasProduct1).ToString());
            resultMessage.Should().Contain(quantityOfOrderItem.ToString());
        }

        [TestMethod]
        public void
           Put_ValidRequest_ShouldUpdateQuantityOfOrderItem_UpdateStockQuantityOfProduct_ReturnOk()
        {
            var originalStockQuantityOfProduct1 = _product1.StockQuantity;
            var originalQuantityOfOrderItem1ThatHasProduct1 = _orderItem1ThatHasProduct1.Quantity;
            var orderItemPutDto = new OrderItemPutDto { Quantity = 5 };

            var result = _controller.Put(_orderItem1ThatHasProduct1.Id, orderItemPutDto);

            result.Should().BeOfType<OkResult>();
            _product1.StockQuantity.Should().Be(originalStockQuantityOfProduct1 + originalQuantityOfOrderItem1ThatHasProduct1 - orderItemPutDto.Quantity);
            _orderItem1ThatHasProduct1.Quantity.Should()
                .Be(orderItemPutDto.Quantity);
        }

        [TestMethod]
        public void Delete_NoOrderItemWithGivenIdExists_ShouldReturnNotFound()
        {
            var result = _controller.Delete(456);
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public void Delete_OrderItemNotBelongToCurrentUser_ShouldReturnUnauthorized()
        {
            var result = _controller.Delete(_orderItem2ThatDoesNotBelongToCurrentUser.Id);
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [TestMethod]
        public void
         Delete_ValidRequest_ShouldUpdateStockQuantityOfProduct_ReturnOk()
        {
            var originalStockQuantityOfProduct1 = _product1.StockQuantity;
            var result = _controller.Delete(_orderItem1ThatHasProduct1.Id);

            result.Should().BeOfType<OkResult>();
            _product1.StockQuantity.Should().Be(originalStockQuantityOfProduct1 + _orderItem1ThatHasProduct1.Quantity);
        }
    }
}