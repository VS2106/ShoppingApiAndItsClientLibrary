using NUnit.Framework;
using ShoppingAPI.Core;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Persistence;

namespace ShoppingAPI.IntegrationTests.Controllers
{
    public class ControllerIntegrationTestsBase
    {
        protected IUnitOfWork _unitOfWork;
        protected Product _productA;
        protected ShoppingBasket _currentUserShoppingBasket;

        protected void SetUp()
        {
            _unitOfWork = new UnitOfWork(new ShoppingApiDbContext());
            _productA = _unitOfWork.Products.FirstOrDefault(p => p.Name == "ProductA");
            _currentUserShoppingBasket = _unitOfWork.ShoppingBaskets.FindByUserId(GlobalSetUp._currentUserId);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork.Dispose();
        }
    }
}
