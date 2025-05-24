using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using N_Shop.API.DTOs.Responses;
using N_Shop.API.Models;
using N_Shop.API.Services;

namespace N_Shop.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly UserManager<ApplicationUser> _userManager;
    public CartsController(ICartService cartService,UserManager<ApplicationUser> userManager)
    {
        _cartService = cartService;
        _userManager = userManager;
    }

    [HttpPost("{ProductId}")]
    public async Task<IActionResult> AddToCart([FromRoute]int ProductId,CancellationToken cancellationToken)
    {
        var appUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _cartService.AddToCart(ProductId,appUser,cancellationToken);
        return Ok(result.Adapt<CartResponse>());
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetCartItemsAsync()
    {
        var appUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var cartItems = await _cartService.GetCartItemsAsync(appUser);
        var cartResponse = cartItems.Select(e => e.Product).Adapt<IEnumerable<CartResponse>>();//without select product is null
        var totalPrice = cartItems.Sum(e=>e.Product.Price*e.Count);
        return Ok(new{cartResponse,totalPrice});
    }
}