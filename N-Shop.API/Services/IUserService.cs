using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public interface IUserService:IService<ApplicationUser>
{
    Task<bool> ChangeRole(string userId,string roleName);
    Task<bool?> LockUnlock(string userId);
}