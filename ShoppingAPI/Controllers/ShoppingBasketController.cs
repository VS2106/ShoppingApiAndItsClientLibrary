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

        [Route("shoppingbasket/clearout")]
        [HttpPut]
        public IHttpActionResult ClearOut()
        {
            var shoppingBasket = _unitOfWork.ShoppingBaskets.Find(User.Identity.GetUserId());

            //Data error
            if (shoppingBasket == null)
                return InternalServerError();

            shoppingBasket.OrderItems.Clear();
            _unitOfWork.SaveChanges();

            return Ok();
        }
    }
}
