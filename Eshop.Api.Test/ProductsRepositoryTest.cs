using Eshop.Api.Data;
using Eshop.Api.Entities;
using Eshop.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using System.Linq;
using Castle.Core.Resource;
using Microsoft.Extensions.Options;

namespace Eshop.Api.Test;

public class InMemoryDBContext : DbContext
{
    public InMemoryDBContext(DbContextOptions<InMemoryDBContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Customers { get; set; }
}


public class ProductsRepositoryTest
{



    [Fact]
    public async void GetAllAsync_With_0_Product_Retrieved()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ProductsRepository>>();
        var dbContext = CreateTestDbContext();
        var mockRepository = new ProductsRepository(dbContext, mockLogger.Object);

        // Act
        var result = await mockRepository.GetAllAsync();
        // Accept
        Assert.Empty(result);
    }

    [Fact]
    public async void GetAllAsync_With_3_Products_Retrieved()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ProductsRepository>>();
        var dbContext = CreateTestDbContext();
        PrepareTestData(dbContext);
        var mockRepository = new ProductsRepository(dbContext, mockLogger.Object);

        // Act
        var result = await mockRepository.GetAllAsync();
        // Accept
        Assert.NotEmpty(result);
        Assert.True(3 == result.Count());
    }

    [Fact]
    public async void GetAsyncById_With_Product_Retrieved()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ProductsRepository>>();
        var dbContext = CreateTestDbContext();
        PrepareTestData(dbContext);
        var mockRepository = new ProductsRepository(dbContext, mockLogger.Object);

        // Act
        var result = await mockRepository.GetAsync(1);
        // Accept
        Assert.True(result != null);
    }


    [Fact]
    public async void GetAsyncById_Without_Product_Retrieved()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ProductsRepository>>();
        var dbContext = CreateTestDbContext();
        PrepareTestData(dbContext);
        var mockRepository = new ProductsRepository(dbContext, mockLogger.Object);

        // Act
        var result = await mockRepository.GetAsync(4);
        // Accept
        Assert.True(result == null);
    }


    [Fact]
    public async void AddProduct_Success()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ProductsRepository>>();
        var dbContext = CreateTestDbContext();
        PrepareTestData(dbContext);
        var mockRepository = new ProductsRepository(dbContext, mockLogger.Object);
        var newProduct = await mockRepository.GetAsync(1);
        newProduct.Id = 4;
        // Act
        if (newProduct != null)
            await mockRepository.CreateAsync(newProduct);
        var result = await mockRepository.GetAllAsync();
        // Accept
        Assert.True(4 == result.Count());
    }

    private EshopContext CreateTestDbContext()
    {
        return new EshopContext(new DbContextOptionsBuilder<EshopContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options);
    }



    private void PrepareTestData(EshopContext context)
    {
        List<Product> products = new()
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

        context.AddRange(products);
        context.SaveChanges();
    }

}
