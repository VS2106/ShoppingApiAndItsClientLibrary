using System.Data.Entity.Infrastructure;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
using ShoppingAPI.Core;
using ShoppingAPI.Core.Dtos;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.Controllers
{
    [Authorize]
    public class ShoppingBasketController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingBasketController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var shoppingBasket = _unitOfWork.ShoppingBaskets.Find(User.Identity.GetUserId());

            //Data error. shoppingBasket can never be null, by EF mapping, User and Shopping Basket is one to one relationship
            if (shoppingBasket == null)
                return InternalServerError();

            return Ok(Mapper.Map<ShoppingBasket, ShoppingBasketDto>(shoppingBasket));
        }

        [Route("api/shoppingbasket/clearout")]
        [HttpPut]
        public IHttpActionResult ClearOut()
        {
            var shoppingBasket = _unitOfWork.ShoppingBaskets.Find(User.Identity.GetUserId());

            if (shoppingBasket == null)
                return InternalServerError();

            foreach (var shoppingBasketOrderItem in shoppingBasket.OrderItems)
            {
                shoppingBasketOrderItem.Product.StockQuantity =
                    shoppingBasketOrderItem.Quantity + shoppingBasketOrderItem.Product.StockQuantity;
            }

            _unitOfWork.OrderItems.DeleteRange(shoppingBasket.OrderItems);

            try
            {
                _unitOfWork.SaveChanges();
                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //TODO later: handle concurrency exception
                return InternalServerError(ex);
            }
        }
    }
}