using IAM.Application;
using IAM.Application.AuthenticationService;
using IAM.Application.AuthenticationService.Repositories;
using IAM.Application.Common.Code;
using IAM.Application.Common.Hash;
using IAM.Application.Common.Tokens;
using IAM.Infrastructure;
using IAM.Infrastructure.Code;
using IAM.Infrastructure.Hash;
using IAM.Infrastructure.Token;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication().AddInfrastructure();
//var c = ConnectionMultiplexer.Connect("127.0.0.1:2028");
//builder.Services.AddSingleton<IConnectionMultiplexer>(c);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Logging.AddConsole();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Replace with allowed origins
              .AllowAnyMethod()
              .AllowAnyHeader()
              ;
    });

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
