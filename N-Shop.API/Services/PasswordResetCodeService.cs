using N_Shop.API.Data;
using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public class PasswordResetCodeService : Service<PasswordResetCode>,IPasswordResetCodeService
{
    private readonly ApplicationDbContext _context;
    public PasswordResetCodeService(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}