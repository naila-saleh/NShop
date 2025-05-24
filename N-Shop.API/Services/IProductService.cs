using System.Linq.Expressions;
using N_Shop.API.DTOs.Requests;
using N_Shop.API.Models;

namespace N_Shop.API.Services;

public interface IProductService
{
    IEnumerable<Product> GetAsync(string? query, int page = 1, int limit = 10);
    Product? GetOneAsync(Expression<Func<Product, bool>> expression);
    Product AddAsync(ProductRequest product);
    bool EditAsync(int id,ProductUpdateRequest product);
    bool RemoveAsync(int id);
}