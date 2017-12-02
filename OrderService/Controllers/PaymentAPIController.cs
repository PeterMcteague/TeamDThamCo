using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Microsoft.Extensions.Configuration;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    public class PaymentAPIController : Controller
    {
        [HttpPost("/MakePayment", Name = "Makes a payment from a stripeTokenId (Use Javascript stripe payment in frontend to get this)")]
        public IActionResult PostPayment(Decimal totalGbp, string stripeTokenId)
        {
            int pence = Convert.ToInt32(totalGbp * 100); //Converts 49.99 to 4999
            try
            {

                var chargeService = new StripeChargeService();

                var chargeOptions = new StripeChargeCreateOptions()
                {
                    Amount = pence,
                    Currency = "gbp",
                    Description = "Charge from TeamDThamCo e-store",
                    SourceTokenOrExistingSourceId = stripeTokenId
                };
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
                var configuration = builder.Build();
                var key = configuration.GetSection("APIKeys").GetValue<string>("StripeKey");
                StripeConfiguration.SetApiKey(key);

                StripeCharge charge = chargeService.Create(chargeOptions);

                return Ok(charge.StripeResponse.ResponseJson);
            }
            catch (StripeException ex)
            {
                Console.Write(ex.Message);
                return StatusCode(400, ex.Message);
            }
        }
    }
}
