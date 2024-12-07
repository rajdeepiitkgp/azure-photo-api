using Azure.Photo.Api.Interfaces;

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

    private static async Task<IResult> UploadPhotoHandler(IFormFile? photo, IPhotoBlobService photoBlobService, IPhotoVisionService photoVisionService)
    {
        if (photo is null || photo.Length == 0) return Results.BadRequest("Photo is required");
        try
        {
            var blobUrl = await photoBlobService.UploadPhotoToBlobContainer(photo);
            var visionResults = await photoVisionService.AnalyzeImage(blobUrl);
            return Results.Ok(new { Message = "Photo uploaded successfully", BlobUrl = blobUrl.ToString(), VisionResults = visionResults });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError($"Error uploading photo: {ex.Message}");
        }
    }
    private static async Task<IResult> GetPhotosHandler(IPhotoBlobService photoBlobService)
    {
        try
        {
            var blobs = await photoBlobService.GetPhotosUrl();
            return Results.Ok(blobs);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError($"Error retriving photos: {ex.Message}");
        }
    }

}
