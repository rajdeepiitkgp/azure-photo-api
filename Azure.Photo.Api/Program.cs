using Azure.Photo.Api.Controllers;
using Azure.Photo.Api.Extensions;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.RegisterDependencies(builder.Configuration);
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddApplicationInsightsTelemetry();
builder.Logging.AddApplicationInsights((config) => config.ConnectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING"), (options) => { });
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();

await app.RunAsync();


