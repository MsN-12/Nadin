using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Nadin.Application.DTOs;
using Newtonsoft.Json;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Nadin.Core.Entities;

namespace Integration.Test
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private HttpClient _client;
        private string _jwtToken;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();

            _jwtToken = GenerateJwtToken("test@example.com");
        }

        private string GenerateJwtToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("SuperSecureSecretKey"); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Test]
        public async Task Create_ShouldAddProduct()
        {
            var newProduct = new Product()
            {
                Name = "New Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test@example.com",
                IsAvailable = true
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            var content = new StringContent(JsonConvert.SerializeObject(newProduct), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/products", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductDto>(responseString);

            Assert.NotNull(product);
            Assert.AreEqual(newProduct.Name, product.Name);
        }

        [Test]
        public async Task Update_ShouldUpdateProduct()
        {
            var newProduct = new Product()
            {
                Name = "New Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test@example.com",
                IsAvailable = true
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            var createContent = new StringContent(JsonConvert.SerializeObject(newProduct), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/products", createContent);
            createResponse.EnsureSuccessStatusCode();

            var createdProduct = JsonConvert.DeserializeObject<ProductDto>(await createResponse.Content.ReadAsStringAsync());

            var updateProduct = new UpdateProductDto
            {
                Name = "Updated Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test@example.com",
                IsAvailable = true
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updateProduct), Encoding.UTF8, "application/json");
            var updateResponse = await _client.PutAsync($"/api/products/{createdProduct.Id}", updateContent);
            updateResponse.EnsureSuccessStatusCode();

            var responseString = await updateResponse.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductDto>(responseString);

            Assert.NotNull(product);
            Assert.AreEqual(updateProduct.Name, product.Name);
        }

        [Test]
        public async Task Delete_ShouldRemoveProduct()
        {
            var newProduct = new Product()
            {
                Name = "New Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test@example.com",
                IsAvailable = true
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            var createContent = new StringContent(JsonConvert.SerializeObject(newProduct), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/products", createContent);
            createResponse.EnsureSuccessStatusCode();

            var createdProduct = JsonConvert.DeserializeObject<ProductDto>(await createResponse.Content.ReadAsStringAsync());

            var deleteResponse = await _client.DeleteAsync($"/api/products/{createdProduct.Id}");
            deleteResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Test]
        public async Task UnauthorizedUpdate_ShouldReturnForbidden()
        {
            var newProduct = new Product()
            {
                Name = "New Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "unauthorized@example.com",
                IsAvailable = true
            };

            var unauthorizedToken = GenerateJwtToken("differentuser@example.com");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", unauthorizedToken);
            var createContent = new StringContent(JsonConvert.SerializeObject(newProduct), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/products", createContent);
            createResponse.EnsureSuccessStatusCode();

            var createdProduct = JsonConvert.DeserializeObject<ProductDto>(await createResponse.Content.ReadAsStringAsync());

            var updateProduct = new UpdateProductDto
            {
                Name = "Updated Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "unauthorized@example.com",
                IsAvailable = true
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updateProduct), Encoding.UTF8, "application/json");
            var updateResponse = await _client.PutAsync($"/api/products/{createdProduct.Id}", updateContent);

            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, updateResponse.StatusCode);
        }

        [Test]
        public async Task UnauthorizedDelete_ShouldReturnForbidden()
        {
            var newProduct = new Product()
            {
                Name = "New Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "unauthorized@example.com",
                IsAvailable = true
            };

            var unauthorizedToken = GenerateJwtToken("differentuser@example.com");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", unauthorizedToken);
            var createContent = new StringContent(JsonConvert.SerializeObject(newProduct), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/products", createContent);
            createResponse.EnsureSuccessStatusCode();

            var createdProduct = JsonConvert.DeserializeObject<ProductDto>(await createResponse.Content.ReadAsStringAsync());

            var deleteResponse = await _client.DeleteAsync($"/api/products/{createdProduct.Id}");

            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, deleteResponse.StatusCode);
        }
    }
}
