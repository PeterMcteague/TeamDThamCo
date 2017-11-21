using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    public class PaymentAPIController : Controller
    {
        [HttpPost("MakePaymentTest", Name = "Tests payment API")]
        public async Task<IActionResult> PostPaymentTest()
        {
            return Ok();
        }
    }
}
