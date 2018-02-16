using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Ploeh.Hyprlinkr;
using ShoppingAPI.Core;
using ShoppingAPI.Core.Dtos;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.Controllers
{
    [Authorize]
    public class OrderItemsController : ApiController
    {

        private readonly IUnitOfWork _unitOfWork;

        public OrderItemsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(int id)
        {
            var orderItem = _unitOfWork.OrderItems.Find(id);

            if (orderItem == null)
                return NotFound();

            if (ItemDoesNotBelongToCurrentUser(orderItem))
                return Unauthorized();

            return Ok(Mapper.Map<OrderItem, OrderItemDtoGetDto>(orderItem));
        }

        public IHttpActionResult Post(OrderItemDtoPostDto orderItemDtoPostDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var shoppingBasket = _unitOfWork.ShoppingBaskets.Find(User.Identity.GetUserId());

            //Data error. shoppingBasket can never be null, by EF mapping, User and Shopping Basket is one to one relationship
            if (shoppingBasket == null)
                return InternalServerError();

            var product = _unitOfWork.Products.Find(orderItemDtoPostDto.ProductId);
            if (product == null)
                return BadRequest($"The product, Id {orderItemDtoPostDto.ProductId} does not exist.");

            if (product.StockQuantity - orderItemDtoPostDto.Quantity < 0)
                return BadRequest($"You want to order {orderItemDtoPostDto.Quantity}, but the stock quantity of this product is {product.StockQuantity}");

            product.StockQuantity = product.StockQuantity - orderItemDtoPostDto.Quantity;

            var orderItem = shoppingBasket.OrderItems.FirstOrDefault(o => o.ProductId == orderItemDtoPostDto.ProductId);
            if (orderItem == null)
            {
                orderItem = new OrderItem()
                {
                    ProductId = orderItemDtoPostDto.ProductId,
                    ShoppingBasketId = shoppingBasket.Id,
                    Quantity = orderItemDtoPostDto.Quantity
                };
                _unitOfWork.OrderItems.Add(orderItem);
            }
            else //OrderItem with same product exist for current user's shopping basket. 
            {
                orderItem.Quantity = orderItem.Quantity + orderItemDtoPostDto.Quantity;
            }

            try
            {
                _unitOfWork.SaveChanges();
                return Created(Url.GetLink<OrderItemsController>(a => a.Get(orderItem.Id)),
                   Mapper.Map<OrderItem, OrderItemDtoGetDto>(orderItem));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //TODO later: hander concurrency exception
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Put(int id, OrderItemDtoPutDto orderItemDtoPutDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var orderItem = _unitOfWork.OrderItems.Find(id);

            if (orderItem == null)
                return NotFound();

            if (ItemDoesNotBelongToCurrentUser(orderItem))
                return Unauthorized();

            var product = _unitOfWork.Products.Find(orderItem.ProductId);
            var stockQuantity = product.StockQuantity + orderItem.Quantity;

            if (stockQuantity - orderItemDtoPutDto.Quantity < 0)
            {
                return BadRequest($"You want to change the quantity of this order to {orderItemDtoPutDto.Quantity}, but the stock quantity of this product is {stockQuantity}");
            }

            product.StockQuantity = stockQuantity - orderItemDtoPutDto.Quantity;
            orderItem.Quantity = orderItemDtoPutDto.Quantity;

            try
            {
                _unitOfWork.SaveChanges();
                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //TODO later: hander concurrency error
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Delete(int id)
        {
            var orderItem = _unitOfWork.OrderItems.Find(id);

            if (orderItem == null)
                return NotFound();

            if (ItemDoesNotBelongToCurrentUser(orderItem))
                return Unauthorized();

            _unitOfWork.OrderItems.Delete(orderItem);
            _unitOfWork.SaveChanges();

            return Ok();
        }

        private bool ItemDoesNotBelongToCurrentUser(OrderItem orderItem)
        {
            return orderItem.ShoppingBasket.ApplicationUserId != User.Identity.GetUserId();
        }
    }
}
