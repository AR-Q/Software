using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Middleware;
using CloudHosting.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication
var configuration = builder.Configuration;
var jwtKey = configuration["Jwt:Key"] ?? 
    throw new InvalidOperationException("JWT key is not configured. Please check your configuration.");
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = configuration["Jwt:Issuer"] ?? "CloudHosting",
        ValidAudience = configuration["Jwt:Audience"] ?? "CloudHostingUsers"
    };
});

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
    
    // JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// services
builder.Services.AddSingleton<IDockerService, DockerService>();
builder.Services.AddSingleton<ICloudPlanService, CloudPlanService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddControllers();var app = builder.Build();

// error handling
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CloudHosting API v1");
    });
}

// health checks endpoint
app.MapHealthChecks("/health");

// authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() => {
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("CloudHosting application started successfully");
});

app.Run();