using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using N_Shop.API.Data;
using N_Shop.API.DTOs.Requests;
using N_Shop.API.DTOs.Responses;
using N_Shop.API.Models;

namespace N_Shop.API.Services;

public class ProductService(ApplicationDbContext context):IProductService
{
    private readonly ApplicationDbContext _context=context;
    public IEnumerable<Product> GetAsync(string? query, int page = 1, int limit = 10)
    {
        IQueryable<Product> products=_context.Products;
        if (query != null)
        {
            products = products.Where(x => x.Name.ToLower().Contains(query.ToLower())||x.Description.ToLower().Contains(query.ToLower()));
        }

        if (page <= 0)page = 1;
        if (limit <= 0)limit = 10;
        products = products.Skip((page - 1) * limit).Take(limit);
        return products.ToList();
    }

    public Product? GetOneAsync(Expression<Func<Product, bool>> expression)
    {
        return _context.Products.FirstOrDefault(expression);
    }

    public Product AddAsync(ProductRequest product)
    {
        var file = product.Image;
        var productInDb = product.Adapt<Product>();
        if (file != null && file.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);
            using (var stream = System.IO.File.Create(path))
            {
                file.CopyTo(stream);
            }
            productInDb.Image=fileName;
            _context.Products.Add(productInDb);
            _context.SaveChanges();
        }
        return productInDb;
    }

    public bool EditAsync(int id, ProductUpdateRequest productRequest)
    {
        var productInDb = _context.Products.AsNoTracking().FirstOrDefault(x => x.Id == id);
        if (productInDb == null) return false;
        var product = productRequest?.Adapt<Product>();
        var file = productRequest.Image;
        if (file != null && file.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);
            using (var stream = System.IO.File.Create(path))
            {
                file.CopyTo(stream);
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(),"Images", productInDb.Image);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            product.Image = fileName;
        }
        else
        {
            product.Image = productInDb.Image;
        }
        product.Id = id;
        _context.Products.Update(product);
        _context.SaveChanges();
        return true;
    }

    public bool RemoveAsync(int id)
    {
        var productInDb = _context.Products.Find(id);
        if (productInDb == null) return false;
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", productInDb.Image);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
        _context.Products.Remove(productInDb);
        _context.SaveChanges();
        return true;
    }
}