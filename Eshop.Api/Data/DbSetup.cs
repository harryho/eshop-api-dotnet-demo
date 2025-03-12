using Eshop.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Api.Data;

public static class DbSetup
{
    public static async Task InitializeDbAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EshopContext>();
        await dbContext.Database.MigrateAsync();

        var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                                    .CreateLogger("DB Initializer");
        logger.LogInformation(9, "DB is ready!");        
    }

    public static IServiceCollection AddRepositories(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connString = configuration.GetConnectionString("EshopContext");
        services.AddSqlServer<EshopContext>(connString)
                .AddScoped<IProductsRepository, ProductsRepository>();

        return services;
    }
}