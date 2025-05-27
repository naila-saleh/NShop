using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N_Shop.API.Data;
using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public class CartService:Service<Cart>,ICartService
{
    private readonly ApplicationDbContext _context;
    public CartService(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<Cart> AddToCart(int productId,string userId,CancellationToken cancellationToken = default)
    {
        var existingCartItems = await _context.Carts.FirstOrDefaultAsync(e=>e.ApplicationUserId==userId && e.ProductId==productId);
        if (existingCartItems != null)
        {
            existingCartItems.Count++;
        }
        else
        {
            existingCartItems = new Cart{ProductId=productId,ApplicationUserId=userId,Count=1};
            await _context.Carts.AddAsync(existingCartItems,cancellationToken);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return existingCartItems;
    }

    public async Task<IEnumerable<Cart>> GetCartItemsAsync(string userId)
    {
        return await GetAsync(e=>e.ApplicationUserId==userId,includes: [c=>c.Product]);
        //without includes the result is null + without e=>e.ApplicationUserId==userId any user have your token can access your cart
    }
    
    public async Task<bool> RemoveRangeAsync(List<Cart> items,CancellationToken cancellationToken = default)
    {
        _context.Carts.RemoveRange(items);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}