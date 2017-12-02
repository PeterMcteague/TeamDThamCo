using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OrderService.Controllers;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
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
            // Setting API key here as it is normally done in startup
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "../../../../../OrderService/")
                .AddJsonFile("appsettings.json");
            var Configuration = builder.Build();
            var key = Configuration.GetSection("Keys").GetValue<string>("Stripe");

            StripeConfiguration.SetApiKey(key);
            var result = _controller.PostPayment(5.00m, "tok_visa") as ObjectResult;
            var resultValue = result.Value;
            
            Assert.Equal(200, result.StatusCode);
        }
    }
}
