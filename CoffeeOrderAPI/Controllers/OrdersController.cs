using Microsoft.AspNetCore.Mvc;
using CoffeeOrderAPI.Models;
using CoffeeOrderAPI.Models.DTOs;
using CoffeeOrderAPI.Data;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


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
        public async Task<IActionResult> CreateOrder([FromBody] CoffeeOrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                PayloadJson = JsonConvert.SerializeObject(orderDto)
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
