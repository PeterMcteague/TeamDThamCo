using Microsoft.AspNetCore.Mvc;
using OrderService.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrderService.Test
{
    public class PaymentAPIControllerTest
    {
        private PaymentAPIController _controller;

        public PaymentAPIControllerTest()
        {
            // Create test subject
            _controller = new PaymentAPIController();
        }

        [Fact]
        public void MakeValidPaymentRequest_ShouldReturnOK()
        {
            var result = _controller.PostPayment(500, "tok_visa") as ObjectResult;
            var resultValue = result.Value;

            Assert.Equal(200, result.StatusCode);
        }
    }
}
