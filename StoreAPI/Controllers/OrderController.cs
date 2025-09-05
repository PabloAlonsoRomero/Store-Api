using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Models.DTOs;
using StoreAPI.Models.Entities;

namespace StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly StoreDbContext _context;
        public OrderController(StoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            var orders = await _context.Order
                .Include(o => o.SystemUser)
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
    }
}
