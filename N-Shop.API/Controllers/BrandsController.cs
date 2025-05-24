using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N_Shop.API.DTOs.Requests;
using N_Shop.API.DTOs.Responses;
using N_Shop.API.Models;
using N_Shop.API.Services;
using N_Shop.API.Utility;

namespace N_Shop.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = $"{StaticData.SuperAdmin},{StaticData.Admin},{StaticData.Company}")]
public class BrandsController(IBrandService brandService) : ControllerBase
{
    private readonly IBrandService _brandService=brandService;

    [HttpGet("")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var brands = await _brandService.GetAsync();
        return Ok(brands.Adapt<IEnumerable<BrandResponse>>());
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var brand= await _brandService.GetOneAsync(b=>b.Id==id);
        return brand == null ? NotFound() : Ok(brand.Adapt<BrandResponse>());
    }

    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] BrandRequest brand,CancellationToken cancellationToken = default)
    {
        var brandToCreate = await _brandService.AddAsync(brand.Adapt<Brand>(),cancellationToken);
        return CreatedAtAction(nameof(GetById), new { brandToCreate.Id }, brandToCreate.Adapt<BrandResponse>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BrandRequest brand,CancellationToken cancellationToken = default)
    {
        var brandToUpdate = await _brandService.EditAsync(id, brand.Adapt<Brand>(),cancellationToken);
        return (!brandToUpdate)? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id,CancellationToken cancellationToken = default)
    {
        var brandToDelete = await _brandService.RemoveAsync(id,cancellationToken);
        if(!brandToDelete) return NotFound();
        return NoContent();
    }
}