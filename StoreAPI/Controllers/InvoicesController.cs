using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using StoreAPI.Models.DTOs;
using StoreAPI.Models.Entities;

namespace StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly StoreDbContext _context;
        private readonly IConfiguration _config;

        public InvoicesController(
            StoreDbContext context,
            IConfiguration config)
        {
            _context = context;
            _config = config;
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
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceCDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var invoice = new Invoice()
            {
                InvoiceNumber = dto.InvoiceNumber,
                IssueDate = dto.IssueDate,
                DueDate = dto.DueDate,
                Subtotal = dto.Subtotal,
                Tax = dto.Tax,
                Total = dto.Total == 0 ? dto.Subtotal + dto.Tax : dto.Total,
                Currency = dto.Currency,
                IsPaid = dto.IsPaid,
                PaymentDate = dto.PaymentDate,
                BillingName = dto.BillingName,
                BillingAddress = dto.BillingAddress,
                BillingEmail = dto.BillingEmail,
                TaxId = dto.TaxId,
                OrderId = dto.OrderId,
                CreatedAt = DateTime.Now
            };

            _context.Invoice.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(invoice);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult> CreateInvoiceBulk([FromBody] List<InvoiceCDTO> invoices)
        {
            if (invoices == null || invoices.Count == 0)
            {
                return BadRequest("No se recibieron facturas");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newInvoices = invoices.Select(dto => new Invoice
                {
                    InvoiceNumber = dto.InvoiceNumber,
                    IssueDate = dto.IssueDate,
                    DueDate = dto.DueDate,
                    Subtotal = dto.Subtotal,
                    Tax = dto.Tax,
                    Total = dto.Total == 0 ? dto.Subtotal + dto.Tax : dto.Total,
                    Currency = dto.Currency,
                    IsPaid = dto.IsPaid,
                    PaymentDate = dto.PaymentDate,
                    BillingName = dto.BillingName,
                    BillingAddress = dto.BillingAddress,
                    BillingEmail = dto.BillingEmail,
                    TaxId = dto.TaxId,
                    OrderId = dto.OrderId,
                    CreatedAt = DateTime.Now
                }).ToList();

                _context.Invoice.AddRange(newInvoices);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Facturas agregadas");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Problem(ex.Message);
            }
        }

        [HttpPost("ai-analyze")]
        public async Task<ActionResult> AnalyzeInvoices()
        {
            // Obtener API KEY
            var openAIKey = _config["OpenAIKey"];
            var client = new ChatClient(
                model: "gpt-5-mini",
                apiKey: openAIKey
            );
            
            // Obtener todas las facturas
            var invoices = await _context.Invoice.ToListAsync();
            
            // Serializar a JSON
            var jsonData = JsonSerializer.Serialize(invoices);
            
            // Generar el prompt especifico
            var prompt = Prompts.GenerateInvoicesPrompt(jsonData);
            
            // Llamar a la IA
            var result = await client.CompleteChatAsync([
                new UserChatMessage(prompt)
            ]);

            var response = result.Value.Content[0].Text;
            
            // Si la IA responde "error", devolver BadRequest
            if (response.Trim().ToLower() == "error")
            {
                return BadRequest("No se pudo analizar las facturas");
            }

            return Ok(response);
        }
    }
}
