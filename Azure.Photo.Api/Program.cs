using Azure.Photo.Api.Controllers;
using Azure.Photo.Api.Settings;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.Configure<AzureStorageOptions>(builder.Configuration.GetSection("AzureStorage"));

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

