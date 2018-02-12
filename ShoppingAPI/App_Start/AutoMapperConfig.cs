using AutoMapper;
using ShoppingAPI.Core.Dtos;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.App_Start
{
    public class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize((config) =>
            {
                config.CreateMap<Product, ProductDto>().ReverseMap();
                config.CreateMap<Order, OrderDto>().ReverseMap();
                config.CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            });
        }
    }
}