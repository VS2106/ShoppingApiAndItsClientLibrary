using System;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingAPI.Controllers;
using ShoppingAPI.Core;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Core.Repositories;
using ShoppingAPI.Tests.Extensions;

namespace ShoppingAPI.Tests.Controllers
{
    [TestClass]
    public class OrderItemsControllerTest
    {
        private OrderItemsController _controller;
        private Mock<IOrderItemRepository> _mockOrderItemRepository;
        private string _applicationUserId = Guid.NewGuid().ToString("N");

        [TestInitialize]
        public void TestInitialize()
        {
            _mockOrderItemRepository = new Mock<IOrderItemRepository>();

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.SetupGet(u => u.OrderItems).Returns(_mockOrderItemRepository.Object);

            _controller = new OrderItemsController(mockUnitOfWork.Object);
            _controller.MockCurrentUser(_applicationUserId, "test1@api.com");
        }


        [TestMethod]
        public void Get_NoOrderItemWithGivenIdExist_ShouldReturnNotFound()
        {
            var result = _controller.Get(2);
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public void Get_OrderItemNotBelongToCurrentUser_ShouldReturnUnauthorized()
        {
            var orderItem = new OrderItem()
            {
                Id = 1,
                ShoppingBasket = new ShoppingBasket() { ApplicationUserId = _applicationUserId + "Q" }
            };

            _mockOrderItemRepository.Setup(i => i.Find(1)).Returns(orderItem);

            var result = _controller.Get(1);
            result.Should().BeOfType<UnauthorizedResult>();
        }


        //TODO later: wrote more test cases
    }
}
