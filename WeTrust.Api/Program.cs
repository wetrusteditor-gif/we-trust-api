using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;
using WeTrust.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// ---- DATABASE (parse DATABASE_URL and force SSL for Supabase) ----
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    // Supports both "postgres://user:pass@host:5432/db" and full conn strings
    try
    {
        var csb = new NpgsqlConnectionStringBuilder(databaseUrl)
        {
            SslMode = SslMode.Require,           // require SSL
            TrustServerCertificate = true        // accept the certificate (Supabase uses valid certs; this avoids strict issues)
        };
        builder.Configuration["ConnectionStrings:DefaultConnection"] = csb.ToString();
        Console.WriteLine("Using DATABASE_URL from env (with SSL enforced).");
    }
    catch (Exception ex)
    {
        Console.WriteLine("DATABASE_URL parse error: " + ex.Message);
    }
}
else
{
    Console.WriteLine("DATABASE_URL not found in env; ensure it's set in Render environment.");
}

var defaultConn = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("Connection string (start): " + (defaultConn?.Substring(0, Math.Min(80, defaultConn.Length)) ?? "<null>"));

// ---- DbContext ----
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---- JWT (unchanged) ----
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "PLEASE_SET_JWT_SECRET";
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

// ---- Simple health endpoint ----
app.MapGet("/health", () => Results.Ok(new { status = "ok", now = DateTime.UtcNow }));

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure DB created (safe for first run)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();
        Console.WriteLine("Database EnsureCreated executed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("DB ensure/create error: " + ex.Message);
    }
}

app.Run();
