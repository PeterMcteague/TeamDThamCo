using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CustomerDetail.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerDetail.Controllers
{
    [Route("api/customer")]
    public class CustomerController : Controller
    {
        private readonly CustomerContext _context;

        public CustomerController(CustomerContext context)
        {
            _context = context;

            if (_context.Customers.Count() == 0)
            {
                _context.Customers.Add(new Customer { Name = "Customer1" });
                _context.SaveChanges();
            }
        }
        
        [HttpGet]
        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult GetById(long id)
        {
            var item = _context.Customers.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Customer item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.Customers.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetCustomer", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Customer item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var cust = _context.Customers.FirstOrDefault(t => t.Id == id);
            if (cust == null)
            {
                return NotFound();
            }

            cust.CanBuy = item.CanBuy;
            cust.Name = item.Name;
            cust.Email = item.Email;

            _context.Customers.Update(cust);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var cust = _context.Customers.FirstOrDefault(t => t.Id == id);
            if (cust == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(cust);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}