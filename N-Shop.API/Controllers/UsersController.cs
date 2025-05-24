using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using N_Shop.API.DTOs.Responses;
using N_Shop.API.Models;
using N_Shop.API.Services;
using N_Shop.API.Utility;

namespace N_Shop.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = $"{StaticData.SuperAdmin}")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAsync();
        return Ok(users.Adapt<IEnumerable<UserResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute]string id)
    {
        var user = await _userService.GetOneAsync(x=>x.Id == id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> ChangeRole([FromRoute] string userId, [FromQuery] string roleName)
    {
        var result = await _userService.ChangeRole(userId,roleName);
        return result ? Ok(result) : BadRequest();
    }

    [HttpPatch("LockUnlock/{userId}")]
    public async Task<IActionResult> LockUnlock([FromRoute] string userId)
    {
        var result = await _userService.LockUnlock(userId);
        return result is not null? Ok(result) : BadRequest();
    }
}