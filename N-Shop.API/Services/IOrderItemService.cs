using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public interface IOrderItemService : IService<OrderItem>
{
    Task<List<OrderItem>> AddRangeAsync(List<OrderItem> orderItems,CancellationToken cancellationToken = default);
}