using FastBuy.Products.Contracts.Dtos;
using FastBuy.Products.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastBuy.Products.Api.Controllers;
[ApiController]
[Route("products")]
[Authorize(Roles = AdminRole)]
public class ProductsController :ControllerBase
{
    private const string AdminRole = "Admin";
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger
       )
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var items = await _productService.GetAsync();

        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _productService.GetAsync(id);

        if (item is null)
            return NotFound();

        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Post(ProductRequestDto createProductResponseDto)
    {
        var guid = await _productService.CreateAsync(createProductResponseDto);

        return CreatedAtAction(nameof(GetById),new { id = guid },new { id = guid });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id,ProductRequestDto updateProductResponseDto)
    {
        await _productService.UpdateAsync(id,updateProductResponseDto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.RemoveAsync(id);

        return NoContent();
    }
}