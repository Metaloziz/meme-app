namespace MemeApp.Api.Configuration;

public static class CorsConfiguration
{
    public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOriginsRaw = configuration["CORS_ORIGINS"]
            ?? Environment.GetEnvironmentVariable("CORS_ORIGINS");

        var corsOrigins = string.IsNullOrWhiteSpace(corsOriginsRaw)
            ? "http://localhost:5173"
            : corsOriginsRaw;

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.SetIsOriginAllowed(origin =>
                {
                    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    {
                        return false;
                    }

                    var allowedOrigins = corsOrigins
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    foreach (var allowed in allowedOrigins)
                    {
                        if (!Uri.TryCreate(allowed, UriKind.Absolute, out var allowedUri))
                        {
                            continue;
                        }

                        if (string.Equals(uri.Scheme, allowedUri.Scheme, StringComparison.OrdinalIgnoreCase)
                            && string.Equals(uri.Host, allowedUri.Host, StringComparison.OrdinalIgnoreCase)
                            && uri.Port == allowedUri.Port)
                        {
                            return true;
                        }
                    }

                    return false;
                })
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        return services;
    }
}
