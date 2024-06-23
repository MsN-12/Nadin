using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nadin.Core.Entities;

namespace Nadin.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Product>()
                .HasIndex(p => p.ProduceDate)
                .IsUnique();

            builder.Entity<Product>()
                .HasIndex(p => p.ManufactureEmail)
                .IsUnique();
        }
    }
}