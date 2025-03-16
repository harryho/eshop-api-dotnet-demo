using Eshop.Api.Auth;
using Eshop.Api.Dtos;
using Eshop.Api.Entities;
using Eshop.Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Eshop.Api.Endpoints;


public static class ProductsEndpoints
{
    const string GetProductV1EndpointName = "GetProductV1";

    public static RouteGroupBuilder MapProductsEndpoints(this IEndpointRouteBuilder routes)
    {

        var group = routes.NewVersionedApi()
                          .MapGroup("/products")
                          .HasApiVersion(1.0)
                          .HasApiVersion(2.0)
                          .WithParameterValidation();

        // Version 1
        group.MapGet("/", GetAllProductsV1)
        .RequireAuthorization(Policies.StaffReadAccess)
        .MapToApiVersion(1.0);


        group.MapGet("/{id}", GetProductV1ById)
       .WithName(GetProductV1EndpointName)
       .RequireAuthorization(Policies.AdminReadAccess)
       .MapToApiVersion(1.0); ;

        // Version 2
        group.MapGet("/", GetAllProductsV2)
        .RequireAuthorization(Policies.StaffReadAccess).MapToApiVersion(2.0);


        group.MapGet("/{id}", GetProductV2ById)
       .RequireAuthorization(Policies.AdminReadAccess)
       .MapToApiVersion(2.0);

        group.MapPost("/", async Task<CreatedAtRoute<ProductDtoV1>> (IProductsRepository repository, CreateProductDto productDto) =>
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

            await repository.CreateAsync(product);
            return TypedResults.CreatedAtRoute(product.AsDtoV1(), GetProductV1EndpointName, new { id = product.Id });

        })
        .RequireAuthorization(Policies.StaffWriteAccess)
        .MapToApiVersion(1.0);


        group.MapPut("/{id}", async Task<Results<NoContent, NotFound>> (IProductsRepository repository, int id, UpdateProductDto product) =>
        {
            var existingProduct = await repository.GetAsync(id);
            if (existingProduct is null)
            {
                return TypedResults.NotFound();
            }
            existingProduct.Name = product.Name;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.UnitInStock = product.UnitInStock;

            await repository.UpdateAsync(existingProduct);
            return TypedResults.NoContent();
        })
        .RequireAuthorization(Policies.StaffWriteAccess)
        .MapToApiVersion(1.0);

        group.MapDelete("/{id}", async (IProductsRepository repository, int id) =>
        {
            var existingProduct = await repository.GetAsync(id);
            if (existingProduct is not null)
            {
                await repository.DeleteAsync(id);
            }

            return TypedResults.NoContent();
        })
        .RequireAuthorization(Policies.AdminWriteAccess)
        .MapToApiVersion(1.0);


        return group;
    }

    public static async Task<Ok<IEnumerable<ProductDtoV1>>> GetAllProductsV1(IProductsRepository repository)
    {
        var products = await repository.GetAllAsync();
        return TypedResults.Ok(products.Select(static p => p.AsDtoV1()));
    }

    public static async Task<Results<Ok<ProductDtoV1>, NotFound>> GetProductV1ById(IProductsRepository repository, int id)
    {
        Product? product = await repository.GetAsync(id);
        return product is not null ? TypedResults.Ok(product.AsDtoV1()) : TypedResults.NotFound();
    }

    public static async Task<Ok<IEnumerable<ProductDtoV2>>> GetAllProductsV2(IProductsRepository repository)
    {
        var products = await repository.GetAllAsync();
        return TypedResults.Ok(products.Select(static p => p.AsDtoV2()));
    }

    public static async Task<Results<Ok<ProductDtoV2>, NotFound>> GetProductV2ById(IProductsRepository repository, int id)
    {
        Product? product = await repository.GetAsync(id);
        return product is not null ? TypedResults.Ok(product.AsDtoV2()) : TypedResults.NotFound();
    }


}