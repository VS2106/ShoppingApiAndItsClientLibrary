using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using NUnit.Framework;
using ShoppingAPI.Controllers;
using ShoppingAPI.Core.Dtos;

namespace ShoppingAPI.Tests.Controllers
{
    [TestFixture]
    public class ProductsControllerTests : ControllerTestsBase
    {
        private ProductsController _controller;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _controller = new ProductsController(_mockUnitOfWork.Object);
        }

        [Test]
        public void Get_ShouldReturnOKWithAllProducts()
        {
            var result = _controller.Get();
            var resultContent = ((OkNegotiatedContentResult<List<ProductDto>>)result).Content;

            result.Should().BeOfType<OkNegotiatedContentResult<List<ProductDto>>>();
            resultContent.OrderBy(i => i.Id).Select(i => $"{i.Id}{i.Name}{i.StockQuantity}")
                .SequenceEqual(
                    _allProducts.OrderBy(i => i.Id).Select(i => $"{i.Id}{i.Name}{i.StockQuantity}"))
                .Should()
                .Be(true);
        }
    }
}