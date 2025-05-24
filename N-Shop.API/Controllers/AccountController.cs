using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using N_Shop.API.DTOs.Requests;
using N_Shop.API.Models;
using N_Shop.API.Utility;

namespace N_Shop.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,RoleManager<IdentityRole> roleManager)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._emailSender = emailSender;
        this._roleManager = roleManager;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        var applicationUser= registerRequest.Adapt<ApplicationUser>();
        var result = await _userManager.CreateAsync(applicationUser, registerRequest.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(applicationUser,StaticData.Customer);
            //cookies
            //await _signInManager.SignInAsync(applicationUser, false);
            //token
            var token =await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            var emailConfirmUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = applicationUser.Id, token }, Request.Scheme, Request.Host.Value);
            await _emailSender.SendEmailAsync(applicationUser.Email, "Confirm Email for N-Shop",
                $"<h1>Hello {applicationUser.UserName}, thank you for registering with us.</h1>"+
                $"<a href='{emailConfirmUrl}'>click here</a>");
            return NoContent();
        }
        return BadRequest(result.Errors);
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return Ok(new { message = "Email confirmed successfully" });
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var applicationUser = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (applicationUser != null)
        {
            var result = await _signInManager.PasswordSignInAsync(applicationUser, loginRequest.Password, loginRequest.RememberMe, false);
            List<Claim> claims=new();
            claims.Add(new (ClaimTypes.Name,applicationUser.UserName));
            claims.Add(new (ClaimTypes.NameIdentifier,applicationUser.Id));
            var userRoles = await _userManager.GetRolesAsync(applicationUser);
            if (userRoles.Count > 0)
            {
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            if (result.Succeeded)
            {
                //cookies
                //await _signInManager.SignInAsync(applicationUser, loginRequest.RememberMe);  
                
                //token
                SymmetricSecurityKey symmetricSecurityKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9itzSNVJRJHesEX6mevgGCjltu79tbCj"));
                SigningCredentials signingCredentials=new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256);
                var jwtToken= new JwtSecurityToken(
                    claims:claims,
                    expires:DateTime.Now.AddDays(30),
                    signingCredentials:signingCredentials
                    );
                string token=new JwtSecurityTokenHandler().WriteToken(jwtToken);
                return Ok(new{token});
            }
            else
            {
                if (result.IsLockedOut)
                {
                    return BadRequest(new { message = "Your account is locked out, please try again later" });
                }else if (result.IsNotAllowed)
                {
                    return BadRequest(new { message = "Confirm your email before logging in" });
                }
            }
        }

        return BadRequest(new { message = "Email or password is incorrect" });
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }

    [HttpPut("ChangePassword")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var applicationUser = await _userManager.GetUserAsync(User);
        if (applicationUser != null)
        {
            var result = await _userManager.ChangePasswordAsync(applicationUser, changePasswordRequest.OldPassword,
                changePasswordRequest.NewPassword);
            if (result.Succeeded) return NoContent();
            else return BadRequest(result.Errors);
        }
        return BadRequest(new { message = "Invalid data" });
    }
}