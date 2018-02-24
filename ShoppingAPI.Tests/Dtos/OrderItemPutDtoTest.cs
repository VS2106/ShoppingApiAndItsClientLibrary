using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShoppingAPI.Core.Dtos;

namespace ShoppingAPI.Tests.Dtos
{
    [TestClass]
    public class OrderItemPutDtoTest
    {
        [TestMethod]
        public void SetQuantityOfOrderItemPutDtoSmallerThan1_ModelStateShouldBeNotValid()
        {
            var orderItemPutDto = new OrderItemPutDto()
            {
                Quantity = 0
            };

            var context = new ValidationContext(orderItemPutDto, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(orderItemPutDto, context, results, true);

            isModelStateValid.Should().Be(false);
        }

        [TestMethod]
        public void SetValidValueForQuantityOfOrderItemPutDto_ModelStateShouldBeValid()
        {
            var orderItemPutDto = new OrderItemPutDto()
            {
                Quantity = 5
            };

            var context = new ValidationContext(orderItemPutDto, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(orderItemPutDto, context, results, true);

            isModelStateValid.Should().Be(true);
        }
    }
}
