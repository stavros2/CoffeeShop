// Program.cs
using CoffeeOrderAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<OrderProcessingService>();
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite("Data Source=orders.db"));

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


// Models/Order.cs
namespace CoffeeOrderAPI.Models;

public enum OrderStatus { Pending, InProgress, Completed, Failed }

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PayloadJson { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public int RetryCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}


// Data/OrderDbContext.cs
using CoffeeOrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeOrderAPI;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
}


// Controllers/OrdersController.cs
using CoffeeOrderAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeOrderAPI.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _context;

    public OrdersController(OrderDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] object payload)
    {
        var order = new Order { PayloadJson = payload.ToString() ?? "{}" };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders() => Ok(await _context.Orders.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        return order == null ? NotFound() : Ok(order);
    }
}


// Services/OrderProcessingService.cs
using CoffeeOrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeOrderAPI;

public class OrderProcessingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderProcessingService> _logger;

    public OrderProcessingService(IServiceScopeFactory scopeFactory, ILogger<OrderProcessingService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var pendingOrders = await db.Orders
                .Where(o => o.Status == OrderStatus.Pending)
                .ToListAsync(stoppingToken);

            foreach (var order in pendingOrders)
            {
                order.Status = OrderStatus.InProgress;
                await db.SaveChangesAsync(stoppingToken);

                bool success = false;

                for (int attempt = 0; attempt < 3; attempt++)
                {
                    try
                    {
                        await Task.Delay(2000, stoppingToken); // Simulate making coffee
                        order.Status = OrderStatus.Completed;
                        order.CompletedAt = DateTime.UtcNow;
                        success = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        order.RetryCount++;
                        _logger.LogWarning($"Attempt {attempt + 1} failed for order {order.Id}: {ex.Message}");
                    }
                }

                if (!success)
                {
                    order.Status = OrderStatus.Failed;
                    order.CompletedAt = DateTime.UtcNow;
                    _logger.LogError($"Order {order.Id} marked as Failed after 3 attempts");
                }

                await db.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(5000, stoppingToken); // Wait before next check
        }
    }
}
