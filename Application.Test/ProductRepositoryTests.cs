using System;
using System.Linq;
using System.Threading.Tasks;
using Nadin.Application.Interfaces;
using Nadin.Application.Services;
using Nadin.Domain.Entities;
using Nadin.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Application.Test
{
    [TestFixture]
    public class ProductRepositoryTests
    {
        private IProductRepository _productRepository;
        private AppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _productRepository = new ProductRepository(_context);
        }

        [Test]
        public async Task AddProduct_ShouldAddProduct()
        {
            var product = new Product
            {
                Name = "Test Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test@example.com",
                IsAvailable = true
            };

            await _productRepository.AddAsync(product);
            var retrievedProduct = await _context.Products.FindAsync(product.Id);

            Assert.NotNull(retrievedProduct);
            Assert.AreEqual(product.Name, retrievedProduct.Name);
        }

        [Test]
        public async Task UpdateProduct_ShouldUpdateProduct()
        {
            var product = new Product
            {
                Name = "Test Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test@example.com",
                IsAvailable = true
            };

            await _productRepository.AddAsync(product);
            product.Name = "Updated Product";

            await _productRepository.UpdateAsync(product);
            var retrievedProduct = await _context.Products.FindAsync(product.Id);

            Assert.NotNull(retrievedProduct);
            Assert.AreEqual(product.Name, retrievedProduct.Name);
        }

        [Test]
        public async Task DeleteProduct_ShouldRemoveProduct()
        {
            var product = new Product
            {
                Name = "Test Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test@example.com",
                IsAvailable = true
            };

            await _productRepository.AddAsync(product);
            await _productRepository.DeleteAsync(product);
            var retrievedProduct = await _context.Products.FindAsync(product.Id);

            Assert.Null(retrievedProduct);
        }

        [Test]
        public async Task GetProduct_ShouldReturnProduct()
        {
            var product = new Product
            {
                Name = "Test Product",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test@example.com",
                IsAvailable = true
            };

            await _productRepository.AddAsync(product);
            var retrievedProduct = await _productRepository.GetByIdAsync(product.Id);

            Assert.NotNull(retrievedProduct);
            Assert.AreEqual(product.Name, retrievedProduct.Name);
        }

        [Test]
        public async Task GetAllProducts_ShouldReturnAllProducts()
        {
            var product1 = new Product
            {
                Name = "Test Product 1",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test1@example.com",
                IsAvailable = true
            };

            var product2 = new Product
            {
                Name = "Test Product 2",
                ProduceDate = DateTime.UtcNow,
                ManufacturePhone = "1234567890",
                ManufactureEmail = "test2@example.com",
                IsAvailable = true
            };

            await _productRepository.AddAsync(product1);
            await _productRepository.AddAsync(product2);
            var products = await _productRepository.GetAllAsync();

            Assert.NotNull(products);
            Assert.AreEqual(2, products.Count());
        }
    }
}