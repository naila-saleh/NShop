using N_Shop.API.Data;
using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public class OrderItemService : Service<OrderItem>,IOrderItemService
{
    private readonly ApplicationDbContext _context;
    public OrderItemService(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<OrderItem>> AddRangeAsync(List<OrderItem> orderItems,CancellationToken cancellationToken = default)
    {
        await _context.AddRangeAsync(orderItems,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return orderItems;
    }
}