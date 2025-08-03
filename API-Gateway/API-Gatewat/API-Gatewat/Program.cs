using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

var app = builder.Build();

await app.UseOcelot();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/iam/swagger/v1/swagger.json", "IAM API");
    c.SwaggerEndpoint("/storage/swagger/v1/swagger.json", "Storage API");
});


app.Run();
