using System.Collections.Generic;
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
    public class ProductControllerIntegrationTests : ControllerIntegrationTestsBase
    {
        private ProductsController _controller;

        [SetUp]
        public void TestMethod1()
        {
            base.SetUp();
            _controller = new ProductsController(_unitOfWork);
            _controller.MockCurrentUser();
        }

        [Test, Isolated]
        public void Get_WhenCalled_ShouldReturnOKWithAllProducts()
        {
            var products = _unitOfWork.Products.GetAll().ToList();

            var result = _controller.Get();
            var resultContent = ((OkNegotiatedContentResult<List<ProductDto>>)result).Content;
            result.Should().BeOfType<OkNegotiatedContentResult<List<ProductDto>>>();

            resultContent.OrderBy(i => i.Id).Select(i => $"{i.Id}{i.Name}{i.StockQuantity}")
               .SequenceEqual(
                   products.OrderBy(i => i.Id).Select(i => $"{i.Id}{i.Name}{i.StockQuantity}"))
               .Should()
               .Be(true);
        }
    }
}
