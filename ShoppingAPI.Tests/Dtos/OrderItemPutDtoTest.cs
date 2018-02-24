using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using NUnit.Framework;
using ShoppingAPI.Core.Dtos;

namespace ShoppingAPI.Tests.Dtos
{
    [TestFixture]
    public class OrderItemPutDtoTest
    {
        [Test]
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

        [Test]
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
