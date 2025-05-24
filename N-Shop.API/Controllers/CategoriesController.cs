using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N_Shop.API.Data;
using N_Shop.API.DTOs.Requests;
using N_Shop.API.DTOs.Responses;
using N_Shop.API.Models;
using N_Shop.API.Services;
using N_Shop.API.Utility;

namespace N_Shop.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = $"{StaticData.SuperAdmin},{StaticData.Admin},{StaticData.Company}")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService categoryService=categoryService;
    
    [HttpGet("")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var categories = await categoryService.GetAsync();
        return Ok(categories.Adapt<IEnumerable<CategoryResponse>>());
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById([FromRoute]int id)
    {
        var category = await categoryService.GetOneAsync(e=>e.Id == id);
        return category == null ? NotFound() : Ok(category.Adapt<CategoryResponse>());
    }

    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] CategoryRequest category,CancellationToken cancellationToken)
    {
        var categoryInDb = await categoryService.AddAsync(category.Adapt<Category>(), cancellationToken);
        //return Created($"{Request.Scheme}://{Request.Host}/api/Categories/{category.Id}",category);
        return CreatedAtAction(nameof(GetById), new { categoryInDb.Id }, categoryInDb.Adapt<CategoryResponse>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute]int id,[FromBody] CategoryRequest category,CancellationToken cancellationToken)
    {
        var categoryToUpdate = await categoryService.EditAsync(id,category.Adapt<Category>(),cancellationToken);
        if (categoryToUpdate == null) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id,CancellationToken cancellationToken)
    {
        var categoryToDelete = await categoryService.RemoveAsync(id,cancellationToken);
        if (categoryToDelete == null) return NotFound();
        return NoContent();
    }
}