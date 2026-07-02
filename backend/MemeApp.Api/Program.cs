using System.Text;
using MemeApp.Api.Data;
using MemeApp.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddSingleton<JwtTokenService>();

var jwtSecret = builder.Configuration["JWT_SECRET"]
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? "local-dev-secret-change-in-production-32chars";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

builder.Services.AddAuthorization();

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

var seedImagesPath = Path.Combine(app.Environment.ContentRootPath, "SeedImages");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.InitializeAsync(db, seedImagesPath);
    await SeedData.EnsureSeedImagesAsync(db, seedImagesPath);
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

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
