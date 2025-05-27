using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using N_Shop.API.DTOs.Requests;
using N_Shop.API.Models;
using N_Shop.API.Services;
using N_Shop.API.Utility;
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
    private readonly IEmailSender _emailSender;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOrderItemService _orderItemService;

    public CheckOutsController(ICartService cartService,IOrderService orderService,IEmailSender emailSender,UserManager<ApplicationUser> userManager,IOrderItemService orderItemService)
    {
        _cartService = cartService;
        _orderService = orderService;
        _emailSender = emailSender;
        _userManager = userManager;
        _orderItemService = orderItemService;
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
        if (request.PaymentMethod == PaymentMethodType.CashOnDelivery.ToString())
        {
            order.PaymentMethodType = PaymentMethodType.CashOnDelivery;
            await _orderService.AddAsync(order);
            return RedirectToAction(nameof(Success),new {orderId = order.Id});
            //return Ok(new { message = "Order placed successfully" });
        }else if (request.PaymentMethod == PaymentMethodType.Visa.ToString())
        {
            await _orderService.AddAsync(order);
            order.PaymentMethodType = PaymentMethodType.Visa;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/api/CheckOuts/Success/{order.Id}",
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
            await _orderService.CommitAsync();
            return Ok(new { session.Url });
        }
        return BadRequest(new { message = "Invalid payment method" });
    }
    
    [HttpGet("Success/{orderId}")]
    [AllowAnonymous]//when you send the user here you send him without his token
    public async Task<IActionResult> Success([FromRoute]int orderId)
    {
        var order = await _orderService.GetOneAsync(e=>e.Id == orderId);
        if (order == null) return NotFound();
        var appUser = await _userManager.FindByIdAsync(order.ApplicationUserId);
        if (appUser == null) return NotFound();
        var subject = "";
        var body="";
        //now we should delete the cart items and send the items to the OrderItems table
        var carts = await _cartService.GetAsync(e=>e.ApplicationUserId==appUser.Id,[e=>e.Product]);
        List<OrderItem> orderItems = [];
        foreach (var item in carts)
        {
            // instead of this: await _orderItemService.AddAsync
            // do this:
            orderItems.Add(new ()//To avoid sending unnecessary requests
            {
                OrderId = orderId,
                ProductId = item.ProductId,
                Price = item.Product.Price * item.Count
            });
            // we should reduce the quantity in the db
            item.Product.Quantity -= item.Count;
        }
        //we need AddRangeAsync to send all items at once
        await _orderItemService.AddRangeAsync(orderItems);
        //now delete the cart items
        await _cartService.RemoveRangeAsync(carts.ToList());//you can convert this to list or from the service convert to IEnumberable
        await _orderService.CommitAsync();
        
        if (order.PaymentMethodType == PaymentMethodType.CashOnDelivery)
        {
            subject = "Order Received - Cash Payment";
            body = $"<h1>Hello {appUser.UserName}</h1>"+$"<p>your order with id: {orderId} has been placed successfully. Thank you for shopping with us.</p>";
        }
        else
        {
            order.OrderStatus = OrderStatus.Confirmed;
            var service = new SessionService();
            var session = await service.GetAsync(order.SessionId);
            order.TransactionId = session.PaymentIntentId;
            await _orderService.CommitAsync();
            subject = "Order Payment Successful - Visa Payment";
            body = $"<h1>Hello {appUser.UserName} Your order has been successfully paid. Thank you for shopping with us.</h1>"+$"<p>Your order number is {order.Id}.</p>";
        }
        await _emailSender.SendEmailAsync(appUser.Email, subject,body);
        return Ok(new { message = "Done" });
    }
    
    [HttpGet("Cancel")]
    public async Task<IActionResult> Cancel()
    {
        return Ok(new { message = "Payment cancelled" });
    }
}