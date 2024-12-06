using Azure.Photo.Api.Controllers;
using Azure.Photo.Api.Settings;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.Configure<AzureStorageOption>(builder.Configuration.GetSection("AzureStorage"));
builder.Services.AddSingleton<IValidateOptions<AzureStorageOption>, AzureStorageOptionValidation>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();

app.Run();

