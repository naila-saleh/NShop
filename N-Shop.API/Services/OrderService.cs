using N_Shop.API.Data;
using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public class OrderService : Service<Order>,IOrderService
{
    private readonly ApplicationDbContext _context;
    public OrderService(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}