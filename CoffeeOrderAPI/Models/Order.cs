using System;

namespace CoffeeOrderAPI.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string PayloadJson { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public int RetryCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}
