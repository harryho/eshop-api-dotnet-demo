using Eshop.Api.Controllers;
using Eshop.Api.Dtos;
using Eshop.Api.Entities;
using Eshop.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Eshop.Api.Test
{
    public class ProductsControllerTest
    {
        [Fact]
        public async Task GetAllProductsV1_With_2_V1_Product_Retrieved()
        {
            // Arrange
            var mock = new Mock<IProductsRepository>();

            mock.Setup(m => m.GetAllAsync())
                .ReturnsAsync(new List<Product> {
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
        },});

            var controller = new ProductsController(mock.Object);

            // Act
            var result = await controller.GetAllProductsV1();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductDtoV1>>(okResult.Value);

            Assert.NotNull(products);
            Assert.NotEmpty(products);
            Assert.Collection(products, product1 =>
            {
                Assert.Equal(1, product1.Id);
                Assert.Equal("Anta Air Zoom BB NXT", product1.Name);

            }, product2 =>
            {
                Assert.Equal(2, product2.Id);
                Assert.Equal("XTEP AntaCourt Royale", product2.Name);

            });
        }

        [Fact]
        public async Task GetProductV1ById_With_1_ProductV1_Retrieved()
        {
            // Arrange
            var mock = new Mock<IProductsRepository>();

            mock.Setup((m) => m.GetAsync(It.IsAny<int>()))
                    .ReturnsAsync(new Product
                    {
                        Id = 1,
                        Name = "Anta Air Zoom BB NXT",
                        Genre = "Basketball Shoes",
                        UnitPrice = 47.99M,
                        UnitInStock = 11,
                        ReleaseDate = new DateTime(2020, 2, 1),
                        ImageUri = "https://dummyimage.com/200x200/eee/000"
                    });

            var controller = new ProductsController(mock.Object);

            // Act
            var result = await controller.GetProductV1ById(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pv1 = Assert.IsType<ProductDtoV1>(okResult.Value);
            Assert.Equal(1, pv1.Id);
            Assert.Equal("Anta Air Zoom BB NXT", pv1.Name);

        }

        [Fact]
        public async Task Test_GetAllProductsV2()
        {
            // Arrange
            var mock = new Mock<IProductsRepository>();

            mock.Setup(m => m.GetAllAsync())
                .ReturnsAsync(new List<Product> {
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
                    });

            var controller = new ProductsController(mock.Object);

            // Act
            var result = await controller.GetAllProductsV2();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductDtoV2>>(okResult.Value);

            Assert.NotNull(products);
            Assert.NotEmpty(products);
            Assert.Collection(products, product1 =>
            {
                Assert.Equal(1, product1.Id);
                Assert.Equal("Anta Air Zoom BB NXT", product1.Name);
                Assert.True(product1.RetailPrice > product1.UnitPrice);

            }, product2 =>
            {
                Assert.Equal(2, product2.Id);
                Assert.Equal("XTEP AntaCourt Royale", product2.Name);
                Assert.True(product2.RetailPrice > product2.UnitPrice);

            });
        }


        [Fact]
        public async Task GetProductV2ById_With_1_ProductV2_Retrieved()
        {
            // Arrange
            var mock = new Mock<IProductsRepository>();

            mock.Setup(m => m.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Product()
                {
                    Id = 1,
                    Name = "Anta Air Zoom BB NXT",
                    Genre = "Basketball Shoes",
                    UnitPrice = 47.99M,
                    UnitInStock = 11,
                    ReleaseDate = new DateTime(2020, 2, 1),
                    ImageUri = "https://dummyimage.com/200x200/eee/000"
                });

            var controller = new ProductsController(mock.Object);

            // Act
            var result = await controller.GetProductV2ById(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pv2 = Assert.IsType<ProductDtoV2>(okResult.Value);
            Assert.Equal(1, pv2.Id);
            Assert.Equal("Anta Air Zoom BB NXT", pv2.Name);
            Assert.True(pv2.RetailPrice > pv2.UnitPrice);
        }
    }
}
