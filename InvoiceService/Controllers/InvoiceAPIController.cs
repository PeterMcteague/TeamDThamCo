using InvoiceService.Data;
using InvoiceService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceService.Controllers
{
    public class InvoiceAPIController : Controller
    {
        private readonly InvoiceContext _context;

        public InvoiceAPIController(InvoiceContext context)
        {
            _context = context;
        }


    }
}
