namespace Eshop.Api.Extensions;

public static class CorsExtensions
{
    private const string allowedOriginSetting = "AllowedOrigin";

    public static IServiceCollection AddEshopCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsBuilder =>
            {
                var allowedOrigin = configuration[allowedOriginSetting]
                                    ?? throw new InvalidOperationException("AllowedOrigin is not set");
                corsBuilder.WithOrigins(allowedOrigin)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .WithExposedHeaders("X-Pagination");
            });
        });
    }
}