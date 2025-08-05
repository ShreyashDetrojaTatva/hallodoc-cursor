using Serilog;
using HalloDoc.Entities.Data.Context;
using HalloDoc.Repositories.Repositories.PingRepository;
using HalloDoc.Services.Services.PingService;
using HalloDoc.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HalloDoc.Services.Services;
using HalloDoc.Repositories.Repositories;
using HalloDoc.Repositories.Repositories.AuthRepository;
using HalloDoc.Repositories.Repositories.RequestRepository;
using HalloDoc.Services.Services.RequestService;
using HalloDoc.Repositories.Repositories.DocumentRepository;
using HalloDoc.Repositories.Repositories.PhysicianRepository;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// Build configuration as described
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var Configuration = configBuilder.Build();
ConfigItems.Configuration = Configuration;

// Add Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(Configuration));

// Add CORS for Angular
var corsPolicy = "AllowAngular";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        policy  =>
        {
            policy.WithOrigins("http://localhost:4300")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(ConfigItems.ConnectionString));

// Register repositories and services
builder.Services.AddScoped<IPingRepository, PingRepository>();
builder.Services.AddScoped<IPingService, PingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IAdminRequestService, AdminRequestService>();
builder.Services.AddScoped<IPhysicianRepository, PhysicianRepository>();
builder.Services.AddScoped<IPhysicianDashboardService, PhysicianDashboardService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

// Register IHttpContextAccessor and WorkContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HalloDoc.Services.Helpers.IWorkContext, HalloDoc.Services.Helpers.WorkContext>();

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigItems.JwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
});

// Add controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (ConfigItems.IsDevelopmentMode)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicy);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
