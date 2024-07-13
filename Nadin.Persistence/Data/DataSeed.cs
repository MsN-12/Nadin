using System;
using Nadin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Nadin.Persistence.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Products.Any())
            {
                return;   
            }

            var products = new[]
            {
                new Product { Name = "Product 1", ProduceDate = DateTime.UtcNow, ManufacturePhone = "1234567890", ManufactureEmail = "manufacturer1@example.com", IsAvailable = true },
                new Product { Name = "Product 2", ProduceDate = DateTime.UtcNow, ManufacturePhone = "0987654321", ManufactureEmail = "manufacturer2@example.com", IsAvailable = false },
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}