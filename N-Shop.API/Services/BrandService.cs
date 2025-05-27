using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using N_Shop.API.Data;
using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public class BrandService: Service<Brand>,IBrandService
{
    ApplicationDbContext _context;
    public BrandService(ApplicationDbContext context):base(context)
    {
        _context = context;
    }

    public async Task<bool> EditAsync(int id,Brand brand,CancellationToken cancellationToken = default)
    {
        Brand? brandInDb = await _context.Brands.FindAsync(id);
        if (brandInDb == null) return false;
        brand.Id=brandInDb.Id;
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}