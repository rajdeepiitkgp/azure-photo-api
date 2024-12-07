using Serilog;

namespace Azure.Photo.Api.Extensions;

public static class LoggerExtensions
{
    public static void SetupSerilog()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
    }
}
