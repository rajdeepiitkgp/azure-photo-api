namespace Azure.Photo.Api.Settings;

public class AzureStorageOption
{
    public const string AzureStorage = nameof(AzureStorage);
    public string AccountName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}
