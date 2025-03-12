using Eshop.Api.Dtos;

namespace Eshop.Api.Entities;

public static class EntityAsDtos
{
    public static ProductDtoV1 AsDtoV1(this Product product)
    {
        return new ProductDtoV1(
            product.Id,
            product.Name,
            product.Genre,
            product.UnitPrice,
            product.UnitInStock,
            product.ReleaseDate,
            product.ImageUri
        );
    }

    public static ProductDtoV2 AsDtoV2(this Product product)
    {
        return new ProductDtoV2(
            product.Id,
            product.Name,
            product.Genre,
            product.UnitPrice - (product.UnitPrice * .3m),
            product.UnitInStock,
            product.UnitPrice,
            product.ReleaseDate,
            product.ImageUri
        );
    }    
}