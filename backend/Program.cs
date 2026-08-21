using backend.Data;
using backend.Data.Configurations;
using backend.Middlewares;
using backend.Models.Entities;
using backend.Security;
using backend.Services.Implementations;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Get JWT section from appsettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication configuration
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
        options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ClockSkew = TimeSpan.Zero
            };
    });

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Register services for dependency injection
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();

var app = builder.Build();

// Seed the database with initial data
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

// Create a correlation ID for each request
app.UseMiddleware<CorrelationIdMiddleware>();

// Use custom exception handling middleware and log exceptions
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Use custom request logging middleware to log incoming requests
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Https redirection middleware
app.UseHttpsRedirection();

// CORS middleware
app.UseCors("AllowAll");

// Authenticate using JWT
app.UseAuthentication();

// Authorization middleware
app.UseAuthorization();

// Map controllers to endpoints
app.MapControllers();

app.Run();
