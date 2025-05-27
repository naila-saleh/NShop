using N_Shop.API.Models;

namespace N_Shop.API.DTOs.Requests;

public class ReviewRequest
{
    public string? Content { get; set; } //Comment
    public int Rate { get; set; }
    public ICollection<ReviewImageRequest>? ReviewImages { get; set; } = new List<ReviewImageRequest>();
}