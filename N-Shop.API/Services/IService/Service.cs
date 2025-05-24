using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using N_Shop.API.Data;

namespace N_Shop.API.Services.IService;

public class Service<T> : IService<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;
    public Service(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression=null,Expression<Func<T, object>>? []includes = null, bool isTracked = true)
    {
        IQueryable<T> entities = _dbSet;
        if (expression != null)
        {
            entities = entities.Where(expression);
        }
        if (includes != null)
        {
            foreach (var item in includes)
            {
                entities = entities.Include(item);
            }
        }

        if (!isTracked)
        {
            entities = entities.AsNoTracking();
        }
        return await entities.ToListAsync();
    }

    public async Task<T?> GetOneAsync(Expression<Func<T, bool>> expression,Expression<Func<T, object>>? []includes = null, bool isTracked = true)
    {
        var all = await GetAsync(expression,includes,isTracked);
        return all.FirstOrDefault();
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }
    
    public async Task<bool> RemoveAsync(int id,CancellationToken cancellationToken = default)
    {
        T? entityInDb = _dbSet.Find(id);
        if (entityInDb == null) return false;
        _dbSet.Remove(entityInDb);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}