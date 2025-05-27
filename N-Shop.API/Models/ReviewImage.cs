namespace N_Shop.API.Models;

public class ReviewImage
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public string ReviewId { get; set; }
    public Review Review { get; set; }
}