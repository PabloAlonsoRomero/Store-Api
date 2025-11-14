using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using StoreAPI.Models.DTOs;
using StoreAPI.Models.Entities;

namespace StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly StoreDbContext _context;
        private readonly IConfiguration _config;
        public OrderController(
            StoreDbContext context, 
            IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            var orders = await _context.Order
                .Include(o => o.SystemUser)
                .Select(o => new
                {
                    Id = o.Id,
                    Total = o.Total,
                    CreatedAt = o.CreatedAt,
                    User = new UserDTO
                    {
                        Id = o.SystemUser.Id,
                        Email = o.SystemUser.Email,
                        FirstName = o.SystemUser.FirstName,
                        LastName = o.SystemUser.LastName,
                    }
                })
                .ToListAsync();
            return Ok(orders);
        }
        
        // ID, Total, UserId
        [HttpPost]
        public async Task<ActionResult> CreateOrder(
            [FromBody] OrderCDTO order
            )
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newOrder = new Order()
                {
                    SystemUserId = order.SystemUserId,
                    CreatedAt = DateTime.Now,
                    Total = order.Total
                };
                _context.Order.Add(newOrder);
                await _context.SaveChangesAsync();
                
                // Insertar en OrderProduct
                // OrderProduct OrderId, ProductId
                // orderProducts = [OrderProduct(1,1), OrderProduct(1,2), OrderProduct(1,3)]
                // Productos = [1, 2, 3, 4, 5, 6]

                var orderProducts = order.Products
                    .Select(x => new OrderProduct{ OrderId = newOrder.Id, ProductId = x})
                    .ToList();
                _context.OrderProduct.AddRange(orderProducts);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Problem();
            }
        }

        //batch
        [HttpPost("bulk")]
        public async Task<ActionResult> CreateOrderBulk(
            [FromBody] List<OrderCDTO> orders
            )
        {
            if (orders == null || orders.Count == 0)
            {
                return BadRequest("No se recibieron ordenes");
            }
            // SI yo voy a modificar varias tablas o si muevo muchos registros. DEBO de hacer una transaccion en SQL

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Convertir lista de OrderDTO en Lista de Ordenes ->> PORQUEE??? Porque mi DbContext
                // necesita la entidad de Order no de OrderCDTO
                // FORMA UTILIZANDO PROGRAMACION NORMAL
                /*
                var newOrders = new List<Order>();
                foreach (var orderDto in orders)
                {
                    var newOrder = new Order();
                    newOrder.SystemUserId = orderDto.SystemUserId;
                    newOrder.Total = orderDto.Total;
                    newOrder.CreatedAt = DateTime.Now;
                    newOrders.Add(newOrder); 
                }
                */
                
                // USANDO LINQ
                var newOrders = orders
                    .Select(o => new Order()
                        {
                            SystemUserId = o.SystemUserId,
                            CreatedAt = DateTime.Now,
                            Total = o.Total,
                            OrderProducts = o.Products
                                .Select(op => new OrderProduct(){ Amount = 1, ProductId = op})
                                .ToList()
                        }
                    )
                    .ToList();
                _context.Order.AddRange(newOrders);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Ordenes agregadas");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Problem(ex.Message);
            }
        }

        [HttpGet("ai-analyze")]
        public async Task<ActionResult> AnalyzeOrders()
        {
            // Obtener API KEY
            var openAIKey = _config["OpenAIKey"];
            var client = new ChatClient(
                model: "gpt-5-mini",
                apiKey: openAIKey
                );
            
            // PRIMERO SE OBTIENEN LOS DATOS
            // Todas las ordenes, Con sus productos, con sus tiendas
            var orders = await _context.Order
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Store)
                .ToListAsync();
            var summary = orders.Select(o => new
            {
                o.Id,
                o.Total,
                o.CreatedAt,
                Products = o.OrderProducts.Select(op => new
                {
                    op.Product.Name,
                    op.Product.Price,
                    op.Product.Store.Description
                })
            });
            var jsonData = JsonSerializer.Serialize(summary);
            // SE HACE EL PROMPT
            
            var prompt = Prompts.GenerateOrdersPrompt(jsonData);
            var result = await client.CompleteChatAsync([
                new UserChatMessage(prompt)
            ]);

            // LA IA ANALIZA LOS DATOS Y ME RESPONDE
            // SE DA UNA RESPUESTA CON LOS DATOS DE LA IA
            var response = result.Value.Content[0].Text;
            return Ok(response);
        }
    }
}
