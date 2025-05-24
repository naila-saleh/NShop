using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
public class ProductsController (IProductService productService): ControllerBase
{
    private readonly IProductService _productService=productService;
    [HttpGet("")]
    [AllowAnonymous]
    public IActionResult GetAll([FromQuery] string? query,[FromQuery] int page=1,[FromQuery] int limit=10)
    {
        var products = _productService.GetAsync(query,page,limit);
        return Ok(products.Adapt<IEnumerable<ProductResponse>>());
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetById([FromRoute]int id)
    {
        var product = _productService.GetOneAsync(x=>x.Id == id);
        return product == null? NotFound(): Ok(product.Adapt<ProductResponse>());
    }

    [HttpPost("")]
    public IActionResult Create([FromForm] ProductRequest product)
    {
        var productInDb = _productService.AddAsync(product);
        if(productInDb == null)return BadRequest();
        return CreatedAtAction(nameof(GetById), new { id = productInDb.Id }, productInDb.Adapt<ProductResponse>());
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromRoute] int id, [FromForm] ProductUpdateRequest product)
    {
        var productInDb = _productService.EditAsync(id, product);
        return !productInDb? NotFound(): NoContent();
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var product = _productService.RemoveAsync(id);
        if (!product) return NotFound();
        return NoContent();
    }
}