using CoffeeOrderAPI.Data;
using CoffeeOrderAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeOrderAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] object payload)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                PayloadJson = payload.ToString()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _context.Orders.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            return order == null ? NotFound() : Ok(order);
        }
    }
}
