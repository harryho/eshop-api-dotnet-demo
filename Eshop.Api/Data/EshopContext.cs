using System.Reflection;
using Eshop.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Api.Data;

public class EshopContext : DbContext
{
    public EshopContext(DbContextOptions<EshopContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}