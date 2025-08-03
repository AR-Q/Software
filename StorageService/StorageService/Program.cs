using StorageService.Application.Interfaces;
using StorageService.Infrastructure;
using StorageService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHttpClient("iam", client =>
{
    client.BaseAddress = new Uri("https://localhost:5501/"); // change to actual IAM service
});



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var storage = scope.ServiceProvider.GetRequiredService<ICloudStorageService>();
        /*await storage.EnsureBucketExistsAsync();*/ // or skip for user-specific buckets
    }
    catch (Exception ex)
    {
        Console.WriteLine("Startup error: " + ex.Message);
        // Optionally rethrow or log
    }
}




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
