using Eshop.Api.Entities;

namespace Eshop.Api.Repositories;

public interface IProductsRepository
{
    Task<global::System.Int32> CountAsync(global::System.String filter);
    Task CreateAsync(Product product);
    Task DeleteAsync(global::System.Int32 id);
    Task<IEnumerable<Product>> GetAllAsyncWithPagination(global::System.Int32 pageNumber, global::System.Int32 pageSize, global::System.String filter);

    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetAsync(global::System.Int32 id);
    Task UpdateAsync(Product updatedProduct);
}
