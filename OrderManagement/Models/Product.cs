using System.Collections.Generic;

namespace OrderManagement.Models
{
    public class Product
    {
        public int Id { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal Price { get; set; }
        public string Name { get; set; }
        
    }
}