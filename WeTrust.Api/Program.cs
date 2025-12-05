using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeTrust.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Read DB connection from env var (Render/Supabase)
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    // If DATABASE_URL is a full connection string, set it directly
    builder.Configuration["ConnectionStrings:DefaultConnection"] = databaseUrl;
}

// Configure DbContext (Npgsql)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT setup
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

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure DB created (simple approach for first run)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try {
        db.Database.EnsureCreated();
    } catch { }
}

app.Run();

