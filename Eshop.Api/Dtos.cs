using System.ComponentModel.DataAnnotations;

namespace Eshop.Api.Dtos;

public record GetProductsDtoV1(
    int PageNumber = 1,
    int PageSize = 5,
    string? Filter = null
);

public record ProductDtoV1(
    int Id,
    string Name,
    string Genre,
    decimal UnitPrice,
    int UnitInStock,
    DateTime ReleaseDate,
    string ImageUri
);

public record GetProductsDtoV2(
    int PageNumber = 1,
    int PageSize = 5,
    string? Filter = null
);

public record ProductDtoV2(
    int Id,
    string Name,
    string Genre,
    decimal UnitPrice,
    int UnitInStock,
    decimal RetailPrice,
    DateTime ReleaseDate,
    string ImageUri
);

public record CreateProductDto(
    [Required][StringLength(50)] string Name,
    [Required][StringLength(20)] string Genre,
    [Range(1, 100)] decimal UnitPrice,
    [Range(1, 100)] int UnitInStock,
    DateTime ReleaseDate,
    [Url][StringLength(100)] string ImageUri
);

public record UpdateProductDto(
    [Required][StringLength(50)] string Name,
    [Required][StringLength(20)] string Genre,
    [Range(1, 100)] decimal UnitPrice,
    [Range(1, 100)] int UnitInStock,
    DateTime ReleaseDate,
    [Url][StringLength(100)] string ImageUri
);
