using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Eshop.Api.Auth;

public static class EshopAuthorization
{
    public static IServiceCollection AddEshopAuthorization(this IServiceCollection services)
    {
        services
            .AddAuthorization(options =>
        {
            options.AddPolicy(Policies.StaffReadAccess, builder => builder.RequireRole("Staff").RequireClaim("scope", "products:read"));
            options.AddPolicy(Policies.StaffWriteAccess, builder => builder.RequireRole("Staff").RequireClaim("scope", "products:write"));
            options.AddPolicy(Policies.AdminReadAccess, builder => builder.RequireRole("Admin").RequireClaim("scope", "products:read"));
            options.AddPolicy(Policies.AdminWriteAccess, builder => builder.RequireRole("Admin").RequireClaim("scope", "products:write"));

        });

        return services;
    }
}