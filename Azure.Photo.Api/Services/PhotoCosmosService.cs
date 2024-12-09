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
        var tagList = searchQuery.Split(',').Select(t => t.ToLower()).Distinct();
        var tagArray = string.Join(", ", tagList.Select(tag => $"\"{tag}\""));
        var count = tagList.Count();
        var sqlQuery = $@"
                        SELECT * FROM c 
                        WHERE ARRAY_LENGTH(
                            ARRAY(
                                SELECT VALUE t 
                                FROM t IN c.tags 
                                WHERE t.name IN ({tagArray})
                            )
                        ) = {count}";

        using var client = GetCosmosClient();
        var container = client.GetContainer(_azureCosmosDbConfig.DbName, _azureCosmosDbConfig.ContainerName);
        using var feed = container.GetItemQueryIterator<PhotoImageAnalysisResult>(queryText: sqlQuery);
        var result = new List<PhotoMetadataResponse>();
        while (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync();
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