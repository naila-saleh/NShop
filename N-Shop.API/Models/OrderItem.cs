using Microsoft.EntityFrameworkCore;

namespace N_Shop.API.Models;

[PrimaryKey(nameof(OrderId),nameof(ProductId))]
public class OrderItem
{
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public string? Note { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}