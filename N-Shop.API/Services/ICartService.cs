using Microsoft.AspNetCore.Mvc;
using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public interface ICartService:IService<Cart>
{
    Task<Cart> AddToCart(int productId,string userId,CancellationToken cancellationToken = default);
    Task<IEnumerable<Cart>> GetCartItemsAsync(string userId);
    Task<bool> RemoveRangeAsync(List<Cart> items,CancellationToken cancellationToken = default);
}