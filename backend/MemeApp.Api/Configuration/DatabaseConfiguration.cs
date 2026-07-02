using MemeApp.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace MemeApp.Api.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = ResolveConnectionString(configuration);
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        return services;
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        var seedImagesPath = Path.Combine(app.Environment.ContentRootPath, "SeedImages");

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await SeedData.InitializeAsync(db, seedImagesPath);
        await SeedData.EnsureSeedImagesAsync(db, seedImagesPath);
    }

    private static string ResolveConnectionString(IConfiguration configuration)
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            return ParseDatabaseUrl(databaseUrl);
        }

        return configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' or DATABASE_URL is required.");
    }

    private static string ParseDatabaseUrl(string databaseUrl)
    {
        var normalizedUrl = databaseUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            ? "postgresql://" + databaseUrl["postgres://".Length..]
            : databaseUrl;

        var uri = new Uri(normalizedUrl);
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
        var database = uri.AbsolutePath.TrimStart('/');
        var dbPort = uri.Port > 0 ? uri.Port : 5432;

        return $"Host={uri.Host};Port={dbPort};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
}
