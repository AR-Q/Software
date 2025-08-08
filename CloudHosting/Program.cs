using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Data;
using CloudHosting.Infrastructure.Middleware;
using CloudHosting.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication
var configuration = builder.Configuration;

// Docker health check
builder.Services.AddHealthChecks()
    .AddCheck("DockerEngine", () => {
        try {
            var dockerClient = new Docker.DotNet.DockerClientConfiguration(
                new Uri(configuration["Docker:ConnectionString"] ?? "npipe://./pipe/docker_engine"))
                .CreateClient();
            
            // connection successful
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
        } 
        catch (Exception ex) {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Docker Engine not available", ex);
        }
    });

//  Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CloudHosting API", Version = "v1" });
});

// services
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddScoped<IPaymentService, ZarinPalService>();
builder.Services.AddSingleton<IDockerService, DockerService>();
builder.Services.AddSingleton<ICloudPlanService, CloudPlanService>();
builder.Services.AddControllers();
builder.Services.AddDbContext<CloudHostingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IFileStorageService, SqlFileStorageService>();
var app = builder.Build();

// error handling
app.UseMiddleware<ErrorHandlingMiddleware>();
    
// swagger
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CloudHosting API v2");
});

// health checks endpoint
app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() => {
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("CloudHosting application started successfully");
});

app.Run();