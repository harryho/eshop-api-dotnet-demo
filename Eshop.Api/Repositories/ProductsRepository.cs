using Eshop.Api.Data;
using Eshop.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Api.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly EshopContext dbContext;
    private readonly ILogger<ProductsRepository> logger;

    public ProductsRepository(
        EshopContext dbContext,
        ILogger<ProductsRepository> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllAsyncWithPagination(int pageNumber, int pageSize, string? filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;

        return await FilterProducts(filter)
                    .OrderBy(product => product.Id)
                    .Skip(skipCount)
                    .Take(pageSize)
                    .AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await dbContext.Products
                    .AsNoTracking().ToListAsync();
    }

    public async Task<Product?> GetAsync(int id)
    {
        return await dbContext.Products.FindAsync(id);
    }

    public async Task CreateAsync(Product product)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Created product {Name}.", product.Name);
    }

    public async Task UpdateAsync(Product updatedProduct)
    {
        dbContext.Update(updatedProduct);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await dbContext.Products.Where(product => product.Id == id)
                             .ExecuteDeleteAsync();
    }

    public async Task<int> CountAsync(string? filter)
    {
        return await FilterProducts(filter).CountAsync();
    }

    private IQueryable<Product> FilterProducts(string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return dbContext.Products;
        }

        return dbContext.Products
                        .Where(product => product.Name.Contains(filter) || product.Genre.Contains(filter));
    }
}