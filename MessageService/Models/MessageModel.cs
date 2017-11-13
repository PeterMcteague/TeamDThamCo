using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService.Models
{
    public class MessageItem
    {
        public long id { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
        public string sender { get; set; }
        public string recipient { get; set; }
    }
}
