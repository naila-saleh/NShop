using N_Shop.API.Models;

namespace N_Shop.API.DTOs.Responses;

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public string Image { get; set; }
    public int Quantity { get; set; }
    public decimal? Rating { get; set; }
    public bool Status { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
}