using Azure.Identity;
using Azure.Photo.Api.Interfaces;
using Azure.Photo.Api.Models;
using Azure.Photo.Api.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;

namespace Azure.Photo.Api.Services;

public class PhotoCosmosService(IOptions<UserIdentityConfig> userIdentityConfig, IOptions<AzureCosmosDbConfig> azureCosmosDbConfig, ILogger<PhotoCosmosService> logger) : IPhotoCosmosService
{
    private readonly UserIdentityConfig _userIdentityConfig = userIdentityConfig.Value;
    private readonly AzureCosmosDbConfig _azureCosmosDbConfig = azureCosmosDbConfig.Value;
    private readonly ILogger<PhotoCosmosService> _logger = logger;

    private CosmosClient GetCosmosClient()
    {
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = _userIdentityConfig.ClientId });
        var client = new CosmosClient($"https://{_azureCosmosDbConfig.AccountName}.documents.azure.com:443/", credential);
        return client;
    }
    public async Task InsertDataToCosmosDb(PhotoImageAnalysisResult metadata)
    {
        using var client = GetCosmosClient();
        var container = client.GetContainer(_azureCosmosDbConfig.DbName, _azureCosmosDbConfig.ContainerName);
        await container.CreateItemAsync(metadata, new PartitionKey(metadata.Id));
    }
    public async Task<IEnumerable<PhotoMetadataResponse>> GetPhotosFromTags(string searchQuery)
    {
        _logger.LogInformation("Fetching Photos for tags: {tags}", searchQuery);
        using var client = GetCosmosClient();
        var container = client.GetContainer(_azureCosmosDbConfig.DbName, _azureCosmosDbConfig.ContainerName);
        var tagList = searchQuery.Split(',');
        var queryable = container.GetItemLinqQueryable<PhotoImageAnalysisResult>();
        var matches = queryable.Where(photo => tagList.All(t => photo.Tags.Any(tag => tag.Name == t)));
        using var linqFeed = matches.ToFeedIterator();
        var result = new List<PhotoMetadataResponse>();
        while (linqFeed.HasMoreResults)
        {
            var response = await linqFeed.ReadNextAsync();
            foreach (var item in response)
            {
                result.Add(new()
                {
                    Id = item.Id,
                    Url = item.Url,
                    Tags = item.Tags.Select(t => t.Name)
                });
            }
        }
        return result;
    }
}