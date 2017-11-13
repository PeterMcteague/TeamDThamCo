using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CustomerDetail.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerDetail.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly CustomerContext _context;

        public TodoController(CustomerContext context)
        {
            _context = context;

            if (_context.Customer.Count() == 0)
            {
                _context.Customer.Add(new Customer { Name = "Item1" });
                _context.SaveChanges();
            }
        }
    }
}