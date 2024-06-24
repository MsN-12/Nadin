using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Nadin.Application.DTOs;
using Nadin.Application.Interfaces;
using Nadin.Core.Entities;
using Nadin.WebAPI;
using NUnit.Framework;

namespace Integration.Test
{
    public class ProductsControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/Products");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<ProductDto>>(responseString);
            Assert.IsNotNull(products);
        }

        [Test]
        public async Task GetById_ReturnsNotFound_ForInvalidId()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/Products/9999");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task Create_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            var createProductDto = new TestCreateProductDto()
            {
                Name = "Test Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test@example.com",
                IsAvailable = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Products", createProductDto);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public async Task Create_ReturnsCreatedResult_WithValidProduct()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Mock dependencies
                    var productRepositoryMock = new Mock<IProductRepository>();
                    productRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
                    productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Product());

                    var userManagerMock = new Mock<UserManager<IdentityUser>>(MockBehavior.Default,
                        new Mock<IUserStore<IdentityUser>>().Object,
                        null, null, null, null, null, null, null);

                    var mapperMock = new Mock<IMapper>();

                    services.AddSingleton(productRepositoryMock.Object);
                    services.AddSingleton(userManagerMock.Object);
                    services.AddSingleton(mapperMock.Object);
                });
            }).CreateClient();

            var createProductDto = new TestCreateProductDto
            {
                Name = "Test Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test@example.com",
                IsAvailable = true
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Products", createProductDto);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseString = await response.Content.ReadAsStringAsync();
            var productDto = JsonSerializer.Deserialize<ProductDto>(responseString);
            Assert.IsNotNull(productDto);
        }

        // Similar tests for Update and Delete can be written following the same pattern.
    }
}
