using System.Linq.Expressions;
using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public interface IBrandService:IService<Brand>
{
    Task<bool> EditAsync(int id,Brand brand,CancellationToken cancellationToken = default);
}