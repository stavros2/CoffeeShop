using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CoffeeOrderAPI.Data;
using CoffeeOrderAPI.Models;
using System.Linq;

namespace CoffeeOrderAPI.Services
{
    public class OrderProcessor : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<OrderProcessor> _logger;

        public OrderProcessor(IServiceProvider services, ILogger<OrderProcessor> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("☕ Order Processor started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var pendingOrders = await db.Orders
                        .Where(o => o.Status == "Pending")
                        .ToListAsync(stoppingToken);

                    foreach (var order in pendingOrders)
                    {
                        try
                        {
                            _logger.LogInformation($"🔄 Processing order {order.Id}");

                            order.Status = "InProgress";
                            await db.SaveChangesAsync(stoppingToken);

                            // Simulate coffee prep time
                            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

                            // Simulate a 25% chance of failure
                            if (Random.Shared.Next(0, 4) == 0)
                            {
                                throw new Exception("Simulated failure");
                            }

                            order.Status = "Ready";
                            order.CompletedAt = DateTime.UtcNow;
                            _logger.LogInformation($"✅ Order {order.Id} is Ready.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"❌ Order {order.Id} failed: {ex.Message}");
                            order.RetryCount++;

                            if (order.RetryCount >= 3)
                            {
                                order.Status = "Failed";
                                _logger.LogError($"🔥 Order {order.Id} permanently failed after 3 retries.");
                            }
                            else
                            {
                                order.Status = "Pending"; // Re-queue
                            }
                        }

                        await db.SaveChangesAsync(stoppingToken);
                    }
                }

                // Wait before polling again
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
