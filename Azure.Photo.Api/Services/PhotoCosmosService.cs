using Azure.Identity;
using Azure.Photo.Api.Interfaces;
using Azure.Photo.Api.Models;
using Azure.Photo.Api.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Azure.Photo.Api.Services;

public class PhotoCosmosService(IOptions<UserIdentityConfig> userIdentityConfig, IOptions<AzureCosmosDbConfig> azureCosmosDbConfig) : IPhotoCosmosService
{
    private readonly UserIdentityConfig _userIdentityConfig = userIdentityConfig.Value;
    private readonly AzureCosmosDbConfig _azureCosmosDbConfig = azureCosmosDbConfig.Value;

    public async Task InsertDataToCosmosDb(PhotoImageAnalysisResult metadata)
    {
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = _userIdentityConfig.ClientId });
        using var client = new CosmosClient($"https://{_azureCosmosDbConfig.AccountName}.documents.azure.com:443/", credential);
        var container = client.GetContainer(_azureCosmosDbConfig.DbName, _azureCosmosDbConfig.ContainerName);
        await container.CreateItemAsync(metadata, new PartitionKey(metadata.Id));
    }
}