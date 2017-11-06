using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Messaging.Models
{
    public class MessagingContext : DbContext
    {
        public MessagingContext(DbContextOptions<MessagingContext> options)
            : base(options)
        { }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        public class Conversation
        {
            public int ConId { get; set; }
            public string Topic { get; set; }
            public string Url { get; set; }

            public List<Message> Messages { get; set; }
        }

        public class Message
        {
            public int MessageId { get; set; }
            public string UserId { get; }
            public string Content { get; set; }
            public string Topic { get; set; }
            public Conversation Conversation { get; set; }
        }
    }
}
