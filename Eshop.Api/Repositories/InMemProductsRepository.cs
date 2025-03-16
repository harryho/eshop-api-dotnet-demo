using Eshop.Api.Entities;

namespace Eshop.Api.Repositories;


public class InMemProductsRepository : IProductsRepository
{

    static List<Product> products = new()
    {
        new Product()
        {
            Id = 1,
            Name = "Anta Air Zoom BB NXT",
            Genre = "Basketball Shoes",
            UnitPrice = 47.99M,
            UnitInStock = 11,
            ReleaseDate = new DateTime(2020, 2, 1),
            ImageUri = "https://dummyimage.com/200x200/eee/000"
        },
        new Product()
        {
            Id = 2,
            Name = "XTEP AntaCourt Royale",
            Genre = "Tennis Shoes",
            UnitPrice = 33.85M,
            UnitInStock = 31,
            ReleaseDate = new DateTime(2021, 7, 30),
            ImageUri = "https://dummyimage.com/200x200/eee/000"
        },
        new Product()
        {
            Id = 3,
            Name = "Anta Waffle Racer Crater",
            Genre = "Running Shoes",
            UnitPrice = 29.00M,
            UnitInStock = 14,
            ReleaseDate = new DateTime(2022, 3, 27),
            ImageUri = "https://dummyimage.com/200x200/eee/000"
        }
    };


    public async Task<IEnumerable<Product>> GetAllAsyncWithPagination(int pageNumber, int pageSize, string? filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;

        return await Task.FromResult(FilterProducts(filter).Skip(skipCount).Take(pageSize));
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {


        return await Task.FromResult(products);
    }

    public async Task<Product?> GetAsync(int id)
    {
        return await Task.FromResult(products.Find(product => product.Id == id));
    }

    public async Task CreateAsync(Product product)
    {
        product.Id = products.Max(product => product.Id) + 1;
        products.Add(product);

        await Task.CompletedTask;
    }

    public async Task UpdateAsync(Product updatedProduct)
    {
        var index = products.FindIndex(product => product.Id == updatedProduct.Id);
        products[index] = updatedProduct;

        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var index = products.FindIndex(product => product.Id == id);
        products.RemoveAt(index);

        await Task.CompletedTask;
    }

    public async Task<int> CountAsync(string? filter)
    {
        return await Task.FromResult(FilterProducts(filter).Count());
    }

    private IEnumerable<Product> FilterProducts(string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return products;
        }

        return products.Where(product => product.Name.Contains(filter) || product.Genre.Contains(filter));
    }


}