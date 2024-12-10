using Azure.Identity;
using Azure.Photo.Api.Interfaces;
using Azure.Photo.Api.Settings;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace Azure.Photo.Api.Services;

public class PhotoBlobService(IOptions<AzureStorageOption> storageOption, IOptions<UserIdentityConfig> userIdentityConfig) : IPhotoBlobService
{
    private readonly AzureStorageOption _storageOption = storageOption.Value;
    private readonly UserIdentityConfig _userIdentityConfig = userIdentityConfig.Value;

    private BlobContainerClient GetBlobContainerClient()
    {
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = _userIdentityConfig.ClientId });
        var blobServiceClient = new BlobServiceClient(new Uri($"https://{_storageOption.AccountName}.blob.core.windows.net"), credential);
        return blobServiceClient.GetBlobContainerClient(_storageOption.ContainerName);
    }

    public async Task<Uri> UploadPhotoToBlobContainer(IFormFile photo)
    {
        var containerClient = GetBlobContainerClient();
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobName = $"{Guid.NewGuid()}-{photo.FileName}";
        var blobClient = containerClient.GetBlobClient(blobName);
        using var stream = photo.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        return new Uri(EncodeUrl(blobClient.Uri.ToString()));
    }

    public async Task<IEnumerable<string>> GetPhotosUrl()
    {
        var containerClient = GetBlobContainerClient();
        var blobs = new List<string>();

        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            blobs.Add(EncodeUrl(blobClient.Uri.ToString()));
        }
        return blobs;
    }

    private static string EncodeUrl(string fullUrl)
    {
        var uri = new Uri(fullUrl);
        var baseUrl = fullUrl[..(fullUrl.LastIndexOf('/') + 1)];
        var fileName = uri.Segments[^1];
        var encodedFileName = Uri.EscapeDataString(fileName);
        var encodedUrl = $"{baseUrl}{encodedFileName}";
        return encodedUrl;
    }
}
