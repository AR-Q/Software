using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Middleware;
using CloudHosting.Infrastructure.Services;
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
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "CloudHosting API", 
        Version = "v1",
        Description = "API for managing cloud containers and payments"
    });
});

// services
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddScoped<IPaymentService, ZarinPalService>();
builder.Services.AddSingleton<IDockerService, DockerService>();
builder.Services.AddSingleton<ICloudPlanService, CloudPlanService>();
builder.Services.AddControllers();
var app = builder.Build();

// error handling
app.UseMiddleware<ErrorHandlingMiddleware>();
    
// swagger
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CloudHosting API v1");
});

// health checks endpoint
app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    VerifyUserAttribute.isDev = true;
}

app.Lifetime.ApplicationStarted.Register(() => {
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("CloudHosting application started successfully");
});

app.Run();