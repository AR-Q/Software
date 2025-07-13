using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CloudHosting.Infrastructure.Persistence;
using CloudHosting.Infrastructure.Docker;
using CloudHosting.Infrastructure.Storage;
using CloudHosting.Application.Interfaces;
using CloudHosting.Application.Services;
using CloudHosting.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// container services 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Authentication & Authorization
builder.Services.AddAuthentication()
    .AddJwtBearer();
builder.Services.AddAuthorization();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<ICloudHostingRepository, CloudHostingRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IDockerService, DockerService>();
builder.Services.AddScoped<CloudHostingService>();
builder.Services.AddScoped<AuthService>();

// Infrastructure Services
builder.Services.AddSingleton<IDateTime, DateTimeService>();
builder.Services.AddScoped<ICurrentUser, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health Check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck");

app.Run();
>>>>>>> b5cc780 (update1)
