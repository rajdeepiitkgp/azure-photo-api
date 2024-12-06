namespace Azure.Photo.Api.Settings;

public class AzureStorageOption
{
    public string AccountName { get; set; } = string.Empty;
    public string AccountKey { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}