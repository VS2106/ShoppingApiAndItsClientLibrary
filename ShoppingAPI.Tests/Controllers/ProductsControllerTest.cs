using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShoppingAPI.Controllers;
using ShoppingAPI.Core.Dtos;

namespace ShoppingAPI.Tests.Controllers
{
    [TestClass]
    public class ProductsControllerTest : ControllerTestBase
    {
        private ProductsController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            base.TestInitialize();
            _controller = new ProductsController(_mockUnitOfWork.Object);
        }

        [TestMethod]
        public void Get_ShouldReturnOKWithAllProducts()
        {
            var result = _controller.Get();
            var resultContent = ((OkNegotiatedContentResult<List<ProductDto>>)result).Content;

            result.Should().BeOfType<OkNegotiatedContentResult<List<ProductDto>>>();
            resultContent.Select(o => o.Id).OrderBy(t => t)
                .SequenceEqual(_allProducts.Select(o => o.Id).OrderBy(t => t))
                .Should()
                .Be(true);
        }
    }
}