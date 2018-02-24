using AutoMapper;
using ShoppingAPI.Core.Dtos;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.App_Start
{
    public class AutoMapperConfig
    {
        private static bool _isInitialized;
        public static void Initialize()
        {
            if (!_isInitialized)
            {
                Mapper.Initialize((config) =>
                {
                    config.CreateMap<Product, ProductDto>().ReverseMap();
                    config.CreateMap<ShoppingBasket, ShoppingBasketDto>().ReverseMap();
                    config.CreateMap<OrderItem, OrderItemGetDto>().ReverseMap();
                });
                _isInitialized = true;
            }
        }
    }
}