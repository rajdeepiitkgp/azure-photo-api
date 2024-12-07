using Azure.Identity;
using Azure.Photo.Api.Settings;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace Azure.Photo.Api.Controllers;

public static class RestController
{
    public static void MapControllers(this WebApplication app)
    {
        app.MapGroup("/api").MapPhotosApi();
    }

    private static void MapPhotosApi(this RouteGroupBuilder group)
    {
        group.MapPost("upload", UploadPhotoHandler).DisableAntiforgery();
        group.MapGet("photos", GetPhotosHandler);
    }

    private static async Task<IResult> UploadPhotoHandler(IFormFile? photo, IOptions<AzureStorageOption> storageOption)
    {
        if (photo is null || photo.Length == 0) return Results.BadRequest("Photo is required");
        try
        {
            var containerClient = GetBlobContainerClient(storageOption.Value);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobName = $"{Guid.NewGuid()}-{photo.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);
            using var stream = photo.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return Results.Ok(new { Message = "Photo uploaded successfully", BlobUrl = blobClient.Uri });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError($"Error uploading photo: {ex.Message}");
        }
    }
    private static async Task<IResult> GetPhotosHandler(IOptions<AzureStorageOption> storageOption)
    {
        try
        {
            var containerClient = GetBlobContainerClient(storageOption.Value);
            var blobs = new List<string>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                blobs.Add(blobClient.Uri.ToString());
            }
            return Results.Ok(blobs);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError($"Error retriving photos: {ex.Message}");
        }
    }

    private static BlobContainerClient GetBlobContainerClient(AzureStorageOption storageOption)
    {
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = storageOption.ClientId });
        var blobServiceClient = new BlobServiceClient(new Uri($"https://{storageOption.AccountName}.blob.core.windows.net"), credential);
        return blobServiceClient.GetBlobContainerClient(storageOption.ContainerName);
    }
}
