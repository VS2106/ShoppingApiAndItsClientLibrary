using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using ShoppingAPI.Core;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Core.Repositories;

namespace ShoppingAPI.Tests.Controllers
{
    public class ControllerTestsBase
    {
        protected readonly string _applicationUserId = Guid.NewGuid().ToString("N");
        protected List<Product> _allProducts;
        protected ShoppingBasket _currentUserShoppingBasketThatHasOrderItem1;
        protected Mock<IOrderItemRepository> _mockOrderItemRepository;
        protected Mock<IProductRepository> _mockProductRepository;
        protected Mock<IShoppingBasketRepository> _mockShoppingBasketRepository;
        protected Mock<IUnitOfWork> _mockUnitOfWork;
        protected OrderItem _orderItem1ThatHasProduct1;
        protected OrderItem _orderItem2ThatDoesNotBelongToCurrentUser;
        protected Product _product1;
        protected Product _product2;


        protected void SetUp()
        {
            _mockOrderItemRepository = new Mock<IOrderItemRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockShoppingBasketRepository = new Mock<IShoppingBasketRepository>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.SetupGet(u => u.OrderItems).Returns(_mockOrderItemRepository.Object);
            _mockUnitOfWork.SetupGet(u => u.Products).Returns(_mockProductRepository.Object);
            _mockUnitOfWork.SetupGet(u => u.ShoppingBaskets).Returns(_mockShoppingBasketRepository.Object);

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

            _allProducts = new List<Product> { _product1, _product2 };
            _mockProductRepository.Setup(p => p.GetAll()).Returns(_allProducts.AsQueryable());

            _currentUserShoppingBasketThatHasOrderItem1 = new ShoppingBasket
            {
                ApplicationUserId = _applicationUserId
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
            _mockShoppingBasketRepository.Setup(i => i.FindByUserId(_applicationUserId))
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
    }
}