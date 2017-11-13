using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MessageService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MessageService.Controllers
{
    [Route("api/message")]
    public class MessageController : Controller
    {
        private readonly MessageContext _context;

        public MessageController(MessageContext context)
        {
            _context = context;

            if(_context.MessageItems.Count() == 0)
            {
                _context.MessageItems.Add(new MessageItem { subject = "Message1" });
                _context.SaveChanges();
            }
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<MessageItem> GetAll()
        {
            return _context.MessageItems.ToList();
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public IActionResult GetById(long id)
        {
            var item = _context.MessageItems.FirstOrDefault(t => t.id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] MessageItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.MessageItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetMessage", new { id = item.id }, item);
        }

     
    }
}
