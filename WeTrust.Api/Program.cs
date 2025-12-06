using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeTrust.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Bind web server to PORT (Railway) or 8080 locally
var httpPort = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{httpPort}");

// ---------- DATABASE: parse DATABASE_URL and prefer IPv4 (synchronous) ----------
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string finalConn = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(databaseUrl))
{
    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var host = uri.Host;
        var dbPort = uri.Port > 0 ? uri.Port : 5432;   // 👈 renamed
        var database = uri.AbsolutePath.TrimStart('/');

        string hostToUse = host;
        try
        {
            Console.WriteLine($"Resolving DNS for host: {host} (synchronous)");
            var addresses = System.Net.Dns.GetHostAddresses(host);
            var ipv4 = addresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            if (ipv4 != null)
            {
                hostToUse = ipv4.ToString();
                Console.WriteLine($"Using IPv4 address for host: {hostToUse}");
            }
            else
            {
                Console.WriteLine("No IPv4 address found; will use host name (may attempt IPv6).");
            }
        }
        catch (Exception dnsEx)
        {
            Console.WriteLine("DNS resolution failed, will use host name: " + dnsEx.Message);
            hostToUse = host;
        }

        finalConn =
            $"Host={hostToUse};Port={dbPort};Username={username};Password={password};Database={database};Ssl Mode=Require;Trust Server Certificate=true";
        builder.Configuration["ConnectionStrings:DefaultConnection"] = finalConn;
        Console.WriteLine("DATABASE_URL parsed and connection string set (ssl enforced).");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error parsing DATABASE_URL: " + ex.Message);
    }
}
else
{
    Console.WriteLine("No DATABASE_URL env var found; using configured DefaultConnection if present.");
}

Console.WriteLine("Connection string preview: " + (finalConn?.Substring(0, Math.Min(160, finalConn.Length)) ?? "<null>"));

// -------- DbContext ----------
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(60); // seconds
        });
});

// -------- JWT ----------
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? builder.Configuration["Jwt:Secret"] ?? "PLEASE_SET_JWT_SECRET";
var key = Encoding.UTF8.GetBytes(jwtSecret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---- Health endpoint ----
app.MapGet("/health", async (AppDbContext db) =>
{
    try
    {
        await db.Database.CanConnectAsync();
        return Results.Ok(new { status = "ok", db = "connected", now = DateTime.UtcNow });
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "db_error", detail: ex.Message);
    }
});

// ---- other middleware ----
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Optional: ensure DB created (safe for first runs)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();
        Console.WriteLine("Database EnsureCreated succeeded.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Database EnsureCreated failed: " + ex.Message);
    }
}

app.Run();
