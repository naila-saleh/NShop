using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N_Shop.API.DTOs.Requests;
using N_Shop.API.Models;
using N_Shop.API.Services;

namespace N_Shop.API.Controllers;

[ApiController]
[Route("api/Products/{productId}/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;
    private readonly IReviewService _reviewService;

    public ReviewsController(IOrderItemService orderItemService,IReviewService reviewService)
    {
        _orderItemService = orderItemService;
        _reviewService = reviewService;
    }
    [HttpPost("Create")]
    public async Task<IActionResult> Create(int productId,[FromForm]ReviewRequest request)
    {
        var appUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var hasOrder = (await _orderItemService.GetAsync(e=>e.ProductId == productId && e.Order.ApplicationUserId==appUser)).Any();
        if (hasOrder)
        {
            var hasReview = (await _reviewService.GetAsync(e=>e.ProductId == productId && e.ApplicationUserId == appUser)).Any();
            if(hasReview)return BadRequest(new {message = "You can only review the product once"});
            var review = request.Adapt<Review>();
            review.ApplicationUserId = appUser;
            review.ProductId = productId;
            review.ReviewDate = DateTime.Now;
            await _reviewService.AddAsync(review);
            return Ok(new {message = "Review added successfully"});
        }
        return BadRequest(new {message = "You can only review the product you already bought"});
    }
    
    [HttpPatch("Edit")]
    public async Task<IActionResult> Edit(int productId, [FromForm] ReviewRequest request, CancellationToken cancellationToken = default)
    {
        var appUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var existingReview = (await _reviewService.GetAsync(e => e.ProductId == productId && e.ApplicationUserId == appUser)).FirstOrDefault();
        if (existingReview == null) return BadRequest(new { message = "You can't edit reviews you don't own" });
        // Update fields from request
        existingReview.Rate = request.Rate;
        existingReview.Content = request.Content;
        existingReview.ReviewDate = DateTime.Now;
        await _reviewService.EditAsync(existingReview.Id, existingReview, cancellationToken);
        await _reviewService.CommitAsync(cancellationToken);
        return Ok(new { message = "Review updated successfully" });
    }
}