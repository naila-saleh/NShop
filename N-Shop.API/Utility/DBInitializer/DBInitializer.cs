using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using N_Shop.API.Data;
using N_Shop.API.Models;

namespace N_Shop.API.Utility.DBInitializer;

public class DBInitializer:IDBInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    public DBInitializer(ApplicationDbContext context,RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }
    public async Task Initialize()
    {
        try
        {
            if(_context.Database.GetPendingMigrations().Any()) 
                _context.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        if (!_roleManager.Roles.Any())
        {
            await _roleManager.CreateAsync(new(StaticData.SuperAdmin));
            await _roleManager.CreateAsync(new(StaticData.Admin));
            await _roleManager.CreateAsync(new(StaticData.Customer));
            await _roleManager.CreateAsync(new(StaticData.Company));
            await _userManager.CreateAsync(new()
            {
                FirstName = "super",
                LastName = "admin",
                UserName = "super_admin",
                Gender = ApplicationUserGender.Female,
                DateOfBirth = new DateTime(2004,05,19),
                Email = "nailasaleh2004@gmail.com"
            },"Naila@05");
            var user = await _userManager.FindByEmailAsync("nailasaleh2004@gmail.com");
            await _userManager.AddToRoleAsync(user,StaticData.SuperAdmin);
        }

    }
}