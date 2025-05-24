using System.Linq.Expressions;
using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public interface ICategoryService:IService<Category>
{
    Task<bool> EditAsync(int id,Category category,CancellationToken cancellationToken = default);
    Task<bool> UpdateToggleAsync(int id,CancellationToken cancellationToken = default);
}