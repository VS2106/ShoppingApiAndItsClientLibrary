using System;
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

            if (ItemNotFoundForCurrentUser(orderItem))
                return NotFound();

            return Ok(Mapper.Map<OrderItem, OrderItemDtoGetDto>(orderItem));
        }

        public IHttpActionResult Post(OrderItemDtoPostDto orderItemDtoPostDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var shoppingBasket = _unitOfWork.ShoppingBaskets.Find(User.Identity.GetUserId());

                //Data error. shoppingBasket can never be null, by EF mapping, User and Shopping Basket is one to one relationship
                if (shoppingBasket == null)
                    return InternalServerError();

                var product = _unitOfWork.Products.Find(orderItemDtoPostDto.ProductId);
                if (product == null)
                    return BadRequest($"The product with Id {orderItemDtoPostDto.ProductId} is not exist.");

                if (product.StockQuantity - orderItemDtoPostDto.Quantity < 0)
                    return BadRequest($"You want to order {orderItemDtoPostDto.Quantity}, but the stock quantity of this product is {product.StockQuantity}");

                product.StockQuantity = product.StockQuantity - orderItemDtoPostDto.Quantity;

                var orderItem = shoppingBasket.OrderItems.FirstOrDefault(o => o.ProductId == orderItemDtoPostDto.ProductId);
                if (orderItem == null)
                {
                    orderItem = new OrderItem()
                    {
                        ProductId = orderItemDtoPostDto.ProductId,
                        Quantity = orderItemDtoPostDto.Quantity
                    };
                    _unitOfWork.OrderItems.Add(orderItem);
                }
                else //OrderItem with same product exist
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
                    //TODO later: hander concurrency 
                    return InternalServerError(ex);
                }

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        public IHttpActionResult Put(int id, OrderItemDtoPutDto orderItemDtoPutDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var orderItem = _unitOfWork.OrderItems.Find(id);

                if (ItemNotFoundForCurrentUser(orderItem))
                    return NotFound();

                var product = _unitOfWork.Products.Find(orderItem.ProductId);
                if (product.StockQuantity + orderItem.Quantity - orderItemDtoPutDto.Quantity < 0)
                {
                    return BadRequest($"You want to order {orderItemDtoPutDto.Quantity}, but the stock quantity of this product is {product.StockQuantity + orderItem.Quantity}");
                }

                product.StockQuantity = product.StockQuantity + orderItem.Quantity - orderItemDtoPutDto.Quantity;
                orderItem.Quantity = orderItemDtoPutDto.Quantity;

                try
                {
                    _unitOfWork.SaveChanges();
                    return Ok();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    //TODO later: hander concurrency 
                    return InternalServerError(ex);
                }

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        public IHttpActionResult Delete(int id)
        {
            var orderItem = _unitOfWork.OrderItems.Find(id);
            if (ItemNotFoundForCurrentUser(orderItem))
                return NotFound();

            _unitOfWork.OrderItems.Delete(orderItem);
            _unitOfWork.SaveChanges();

            return Ok();
        }

        private bool ItemNotFoundForCurrentUser(OrderItem orderItem)
        {
            return orderItem == null ||
                   orderItem.ShoppingBasket.ApplicationUserId != User.Identity.GetUserId();
        }
    }
}
