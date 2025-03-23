using Azure.Identity;
using Azure.Photo.Api.Interfaces;
using Azure.Photo.Api.Models;
using Azure.Photo.Api.Settings;
using Microsoft.Azure.Cosmos;
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
        var tagList = searchQuery.Split(',').Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim().ToLower()).Distinct();
        var tagArray = string.Join(", ", tagList.Select(tag => $"\"{tag}\""));
        var count = tagList.Count();
        if (count <= 0)
        {
            throw new ArgumentException("tags should be finite");
        }
        var sqlQuery = $@"
                        SELECT * FROM c 
                        WHERE ARRAY_LENGTH(
                            ARRAY(
                                SELECT VALUE t 
                                FROM t IN c.tags 
                                WHERE t.name IN ({tagArray})
                            )
                        ) = {count}";
        if (count == 1 && (tagList.First() == "%" || tagList.First() == "*"))
        {
            sqlQuery = "SELECT * FROM c";
        }
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
                    Caption = item.Caption,
                    Tags = item.Tags.Select(t => t.Name)
                });
            }
        }
        return result;
    }

    public async Task<IEnumerable<PhotoDetail>> GetPhotoList()
    {
        _logger.LogInformation("Fetching All Photos");
        var sqlQuery = "SELECT c.id,c.url FROM c ORDER BY c._ts";
        using var client = GetCosmosClient();
        var container = client.GetContainer(_azureCosmosDbConfig.DbName, _azureCosmosDbConfig.ContainerName);
        using var feed = container.GetItemQueryIterator<PhotoDetail>(queryText: sqlQuery);
        var result = new List<PhotoDetail>();
        while (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync();
            foreach (var item in response)
            {
                result.Add(new()
                {
                    Id = item.Id,
                    Url = item.Url
                });
            }
        }
        return result;
    }

    public async Task<PhotoMetadataResponse?> GetPhotoFromId(string id)
    {
        _logger.LogInformation("Fetching Photo for id: {id}", id);
        var sqlQuery = "SELECT * FROM c WHERE c.id = @id";
        var query = new QueryDefinition(sqlQuery).WithParameter("@id", id);
        using var client = GetCosmosClient();
        var container = client.GetContainer(_azureCosmosDbConfig.DbName, _azureCosmosDbConfig.ContainerName);
        using var feed = container.GetItemQueryIterator<PhotoImageAnalysisResult>(query, requestOptions: new() { PartitionKey = new(id) });
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
                    Caption = item.Caption,
                    Tags = item.Tags.Select(t => t.Name)
                });
            }
        }
        return result.FirstOrDefault();
    }
}