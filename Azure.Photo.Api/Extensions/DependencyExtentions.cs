using Azure.Photo.Api.Interfaces;
using Azure.Photo.Api.Services;
using Azure.Photo.Api.Settings;
using Microsoft.Extensions.Options;

namespace Azure.Photo.Api.Extensions;

public static class DependencyExtentions
{
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureStorageOption>(configuration.GetSection(AzureStorageOption.AzureStorage));
        services.AddSingleton<IValidateOptions<AzureStorageOption>, AzureStorageOptionValidation>();

        services.Configure<UserIdentityConfig>(configuration.GetSection(UserIdentityConfig.UserIdentity));
        services.AddSingleton<IValidateOptions<UserIdentityConfig>, UserIdentityConfigValidation>();

        services.Configure<ComputerVisionConfig>(configuration.GetSection(ComputerVisionConfig.ComputerVision));
        services.AddSingleton<IValidateOptions<ComputerVisionConfig>, ComputerVisionConfigValidation>();

        services.Configure<AzureCosmosDbConfig>(configuration.GetSection(AzureCosmosDbConfig.CosmosDb));
        services.AddSingleton<IValidateOptions<AzureCosmosDbConfig>, AzureCosmosDbConfigValidation>();

        services.AddSingleton<IPhotoBlobService, PhotoBlobService>();
        services.AddSingleton<IPhotoVisionService, PhotoVisionService>();
        services.AddSingleton<IPhotoCosmosService, PhotoCosmosService>();
    }
}
