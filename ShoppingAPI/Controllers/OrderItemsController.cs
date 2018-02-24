using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
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

            return Ok(Mapper.Map<OrderItem, OrderItemGetDto>(orderItem));
        }

        public IHttpActionResult Post(OrderItemPostDto orderItemDtoPostDto)
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

            var stockQuantityOfProductAfterOrdering = product.StockQuantity - orderItemDtoPostDto.Quantity;
            if (stockQuantityOfProductAfterOrdering < 0)
                return
                    BadRequest(
                        $"You want to order {orderItemDtoPostDto.Quantity}, but the stock quantity of this product is {product.StockQuantity}");
            product.StockQuantity = stockQuantityOfProductAfterOrdering;

            var orderItem = shoppingBasket.OrderItems.FirstOrDefault(o => o.ProductId == orderItemDtoPostDto.ProductId);
            if (orderItem == null)
            {
                orderItem = new OrderItem
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
                return Created(new Uri($@"{Request.RequestUri}/{orderItem.Id}"),
                    Mapper.Map<OrderItem, OrderItemGetDto>(orderItem));
            }
            catch (DbUpdateConcurrencyException ex)
            // Entities may have been modified or deleted since entities were loaded.(OrderItemMap.cs and ProductMap.cs applied concurrency token to quantity)
            {
                //TODO later: handle concurrency exception
                return InternalServerError(ex);
            }
            catch (DbUpdateException ex)
            // Same product order item have been added to the shopping basket since entities were loaded. (OrderItemMap.cs applied a multi column unique index)
            {
                //TODO later: handle multi column unique index exception
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Put(int id, OrderItemPutDto orderItemDtoPutDto)
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
            var stockQuantityOfProductAfterOrdering = stockQuantity - orderItemDtoPutDto.Quantity;
            if (stockQuantityOfProductAfterOrdering < 0)
            {
                return
                    BadRequest(
                        $"You want to change the quantity of this order to {orderItemDtoPutDto.Quantity}, but the stock quantity of this product is {stockQuantity}");
            }
            product.StockQuantity = stockQuantityOfProductAfterOrdering;
            orderItem.Quantity = orderItemDtoPutDto.Quantity;

            return GetResultAfterTryingToSaveChanges();
        }

        public IHttpActionResult Delete(int id)
        {
            var orderItem = _unitOfWork.OrderItems.Find(id);

            if (orderItem == null)
                return NotFound();

            if (ItemDoesNotBelongToCurrentUser(orderItem))
                return Unauthorized();

            orderItem.Product.StockQuantity = orderItem.Product.StockQuantity + orderItem.Quantity;
            _unitOfWork.OrderItems.Delete(orderItem);
            return GetResultAfterTryingToSaveChanges();
        }

        private IHttpActionResult GetResultAfterTryingToSaveChanges()
        {
            try
            {
                _unitOfWork.SaveChanges();
                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            // Entities may have been modified or deleted since entities were loaded.
            {
                //TODO later: handle concurrency error
                return InternalServerError(ex);
            }
        }

        private bool ItemDoesNotBelongToCurrentUser(OrderItem orderItem)
        {
            return orderItem.ShoppingBasket.ApplicationUserId != User.Identity.GetUserId();
        }
    }
}