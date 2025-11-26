using Asp.Versioning;
using Eshop.Api.Auth;
using Eshop.Api.Dtos;
using Eshop.Api.Entities;
using Eshop.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductsRepository _repository;

    public ProductsController(IProductsRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Get all products (Version 1)
    /// </summary>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [Authorize(Policy = Policies.StaffReadAccess)]
    [ProducesResponseType(typeof(IEnumerable<ProductDtoV1>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDtoV1>>> GetAllProductsV1()
    {
        var products = await _repository.GetAllAsync();
        return Ok(products.Select(p => p.AsDtoV1()));
    }

    /// <summary>
    /// Get product by ID (Version 1)
    /// </summary>
    [HttpGet("{id}")]
    [MapToApiVersion("1.0")]
    [Authorize(Policy = Policies.AdminReadAccess)]
    [ProducesResponseType(typeof(ProductDtoV1), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDtoV1>> GetProductV1ById(int id)
    {
        Product? product = await _repository.GetAsync(id);
        if (product is null)
        {
            return NotFound();
        }
        return Ok(product.AsDtoV1());
    }

    /// <summary>
    /// Get all products (Version 2)
    /// </summary>
    [HttpGet]
    [MapToApiVersion("2.0")]
    [Authorize(Policy = Policies.StaffReadAccess)]
    [ProducesResponseType(typeof(IEnumerable<ProductDtoV2>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDtoV2>>> GetAllProductsV2()
    {
        var products = await _repository.GetAllAsync();
        return Ok(products.Select(p => p.AsDtoV2()));
    }

    /// <summary>
    /// Get product by ID (Version 2)
    /// </summary>
    [HttpGet("{id}")]
    [MapToApiVersion("2.0")]
    [Authorize(Policy = Policies.AdminReadAccess)]
    [ProducesResponseType(typeof(ProductDtoV2), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDtoV2>> GetProductV2ById(int id)
    {
        Product? product = await _repository.GetAsync(id);
        if (product is null)
        {
            return NotFound();
        }
        return Ok(product.AsDtoV2());
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    [MapToApiVersion("1.0")]
    [Authorize(Policy = Policies.AdminWriteAccess)]
    [ProducesResponseType(typeof(ProductDtoV1), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDtoV1>> CreateProduct(CreateProductDto productDto)
    {
        Product product = new()
        {
            Name = productDto.Name,
            Genre = productDto.Genre,
            UnitPrice = productDto.UnitPrice,
            UnitInStock = productDto.UnitInStock,
            ReleaseDate = productDto.ReleaseDate,
            ImageUri = productDto.ImageUri
        };

        await _repository.CreateAsync(product);
        
        return CreatedAtAction(
            nameof(GetProductV1ById),
            new { id = product.Id, version = "1.0" },
            product.AsDtoV1()
        );
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    [MapToApiVersion("1.0")]
    [Authorize(Policy = Policies.AdminWriteAccess)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto productDto)
    {
        var existingProduct = await _repository.GetAsync(id);
        if (existingProduct is null)
        {
            return NotFound();
        }

        existingProduct.Name = productDto.Name;
        existingProduct.UnitPrice = productDto.UnitPrice;
        existingProduct.UnitInStock = productDto.UnitInStock;

        await _repository.UpdateAsync(existingProduct);
        return NoContent();
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    [MapToApiVersion("1.0")]
    [Authorize(Policy = Policies.AdminWriteAccess)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var existingProduct = await _repository.GetAsync(id);
        if (existingProduct is not null)
        {
            await _repository.DeleteAsync(id);
        }

        return NoContent();
    }
}
