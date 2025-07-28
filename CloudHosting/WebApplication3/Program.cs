using WebApplication3.Core;
using WebApplication3.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Register DockerService and controllers
builder.Services.AddSingleton<IDockerService, DockerService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// If Swashbuckle.AspNetCore is not installed, this will fail. Install via NuGet if needed.
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
