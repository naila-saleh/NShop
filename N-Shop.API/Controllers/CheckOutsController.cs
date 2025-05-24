using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N_Shop.API.DTOs.Requests;
using N_Shop.API.Models;
using N_Shop.API.Services;
using Stripe.Checkout;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace N_Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckOutsController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public CheckOutsController(ICartService cartService,IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }
    
    [HttpGet("Pay")]
    public async Task<IActionResult> Pay([FromBody]PaymentRequest request)
    {
        var appUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var carts = await _cartService.GetAsync(e => e.ApplicationUserId == appUser, [e=>e.Product]);
        if (carts == null) return NotFound();
        Order order = new()
        {
            OrderStatus = OrderStatus.Pending,
            OrderDate = DateTime.Now,
            TotalPrice = carts.Sum(e=>e.Product.Price*e.Count),
            ApplicationUserId = appUser,
        };
        if (request.PaymentMethod == "CashOnDelivery")
        {
            order.PaymentMethodType = PaymentMethodType.CashOnDelivery;
            _orderService.AddAsync(order);
            return Ok(new { message = "Order placed successfully" });
        }else if (request.PaymentMethod == "Visa")
        {
            order.PaymentMethodType = PaymentMethodType.Visa;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/api/CheckOuts/Success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/api/CheckOuts/Cancel",
            };
            foreach(var item in carts)
            {
                options.LineItems.Add(
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "USD",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                                Description = item.Product.Description,
                            },
                            UnitAmount = (long)item.Product.Price*100,
                        },
                        Quantity = item.Count,
                    }
                );
            }
            var service = new SessionService();
            var session = service.Create(options);
            order.SessionId = session.Id;
            order.TotalPrice = carts.Sum(e=>e.Product.Price*e.Count);
            await _orderService.AddAsync(order);
            return Ok(new { session.Url });
        }
        return BadRequest(new { message = "Invalid payment method" });
    }
    
    [HttpGet("Success")]
    [AllowAnonymous]//when you send the user here you send him without his token
    public async Task<IActionResult> Success()
    {
        return Ok(new { message = "Payment successful" });
    }
    
    [HttpGet("Cancel")]
    public async Task<IActionResult> Cancel()
    {
        return Ok(new { message = "Payment cancelled" });
    }
}