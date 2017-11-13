using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageService.Controllers;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Models
{
    public class MessageContext : DbContext
    {
        public MessageContext(DbContextOptions<MessageContext> options)
            : base(options)
        {
        }

        public DbSet<MessageItem> MessageItems { get; set; }

       
    }
}
