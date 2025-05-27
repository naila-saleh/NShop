namespace N_Shop.API.Models;

public class Review
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public string? Content { get; set; } //Comment
    public int Rate { get; set; }
    public DateTime ReviewDate { get; set; }
    public ICollection<ReviewImage>? ReviewImages { get; set; } = new List<ReviewImage>();
    //you can also add: Reply to Review (list)
}