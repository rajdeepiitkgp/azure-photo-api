namespace Azure.Photo.Api.Settings;

public class AzureCosmosDbConfig
{
    public const string CosmosDb = nameof(CosmosDb);
    public string AccountName { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;

}