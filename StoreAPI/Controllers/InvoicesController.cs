using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Models.Entities;

namespace StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly StoreDbContext _context;

        public InvoicesController(StoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices([FromQuery] int? orderId, [FromQuery] bool? isPaid)
        {
            var invoices = await _context.Invoice
                .Where(i => (!orderId.HasValue || i.OrderId == orderId.Value) &&
                            (!isPaid.HasValue || i.IsPaid == isPaid.Value))
                .ToListAsync();
            
            return Ok(invoices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var invoice = await _context.Invoice
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound();
            
            return Ok(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] Invoice invoice)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (invoice.Total == 0)
                invoice.Total = invoice.Subtotal + invoice.Tax;

            invoice.CreatedAt = DateTime.UtcNow;

            _context.Invoice.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(invoice);
        }
    }
}
