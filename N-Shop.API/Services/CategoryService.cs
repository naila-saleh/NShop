using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using N_Shop.API.Data;
using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public class CategoryService: Service<Category>,ICategoryService
{
    ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context):base(context)
    {
        _context = context;
    }
    
    public async Task<bool> EditAsync(int id, Category category, CancellationToken cancellationToken)
    {
        Category? categoryInDb = _context.Categories.Find(id);
        if (categoryInDb == null) return false;
        categoryInDb.Name = category.Name;
        categoryInDb.Description = category.Description;
        _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateToggleAsync(int id, CancellationToken cancellationToken = default)
    {
        Category? categoryInDb = _context.Categories.Find(id);
        if (categoryInDb == null) return false;
        categoryInDb.Status = !categoryInDb.Status;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}