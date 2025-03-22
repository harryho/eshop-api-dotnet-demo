using Eshop.Api.Dtos;
using Eshop.Api.Endpoints;
using Eshop.Api.Entities;
using Eshop.Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Eshop.Api.Test
{
    public class ProductsEndpointsTest
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

            // Act
            var result = await ProductsEndpoints.GetAllProductsV1(mock.Object);

            //Assert
            Assert.IsType<Ok<IEnumerable<ProductDtoV1>>>(result);

            Assert.NotNull(result.Value);
            Assert.NotEmpty(result.Value);
            Assert.Collection(result.Value, product1 =>
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

            // Act
            var result = await ProductsEndpoints.GetProductV1ById(mock.Object, 1);

            //Assert
            Assert.IsType<Ok<ProductDtoV1>>(result.Result);
            Assert.NotNull(result.Result);
            var pv1 = ((Ok<ProductDtoV1>)result.Result).Value;
            Assert.Equal(1, pv1?.Id);
            Assert.Equal("Anta Air Zoom BB NXT", pv1?.Name);

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

            // Act
            var result = await ProductsEndpoints.GetAllProductsV2(mock.Object);

            //Assert
            Assert.IsType<Ok<IEnumerable<ProductDtoV2>>>(result);

            Assert.NotNull(result.Value);
            Assert.NotEmpty(result.Value);
            Assert.Collection(result.Value, product1 =>
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

            // Act
            var result = await ProductsEndpoints.GetProductV2ById(mock.Object, 1);

            //Assert
            Assert.IsType<Ok<ProductDtoV2>>(result.Result);

            Assert.NotNull(result.Result);

            var pv2 = ((Ok<ProductDtoV2>)result.Result).Value;
            Assert.Equal(1, pv2?.Id);
            Assert.Equal("Anta Air Zoom BB NXT", pv2?.Name);
            Assert.True(pv2?.RetailPrice > pv2?.UnitPrice);
        }
    }
}
