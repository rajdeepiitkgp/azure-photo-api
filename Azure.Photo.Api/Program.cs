using Azure.Photo.Api.Controllers;
using Azure.Photo.Api.Extensions;
using Scalar.AspNetCore;
using Serilog;
using LoggerExtensions = Azure.Photo.Api.Extensions.LoggerExtensions;

LoggerExtensions.SetupSerilog();

try
{
    Log.Information("Application Starting Up");
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddOpenApi();
    builder.Services.RegisterDependencies(builder.Configuration);
    builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
    builder.Services.AddSerilog();

    var app = builder.Build();
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.UseHttpsRedirection();
    app.UseCors();
    app.UseSerilogRequestLogging();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The Application Failed To Start Correctly");
}
finally
{
    await Log.CloseAndFlushAsync();
}


