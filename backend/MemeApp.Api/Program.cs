using MemeApp.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

var connectionString = ResolveConnectionString(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();

var corsOriginsRaw = builder.Configuration["CORS_ORIGINS"]
    ?? Environment.GetEnvironmentVariable("CORS_ORIGINS");

var corsOrigins = string.IsNullOrWhiteSpace(corsOriginsRaw)
    ? "http://localhost:5173"
    : corsOriginsRaw;

builder.Services.AddCors(options =>
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.InitializeAsync(db);
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();

static string ResolveConnectionString(IConfiguration configuration)
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        return ParseDatabaseUrl(databaseUrl);
    }

    return configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' or DATABASE_URL is required.");
}

static string ParseDatabaseUrl(string databaseUrl)
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
