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
        group.MapGet("photos/search", SearchPhotosByTagsHandler);
        group.MapGet("photosV2", GetPhotosFromCosmosHandler);
        group.MapGet("photosV2/search", GetPhotoDetail);
    }

    private static async Task<IResult> UploadPhotoHandler(IFormFile? photo, IPhotoBlobService photoBlobService, IPhotoVisionService photoVisionService, IPhotoCosmosService photoCosmosService)
    {
        if (photo is null || photo.Length == 0) return Results.BadRequest("Photo is required");
        try
        {
            var blobUrl = await photoBlobService.UploadPhotoToBlobContainer(photo);
            var visionResults = await photoVisionService.AnalyzeImage(blobUrl);
            await photoCosmosService.InsertDataToCosmosDb(visionResults);
            return Results.Ok(visionResults);
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

    private static async Task<IResult> SearchPhotosByTagsHandler(string? tags, IPhotoCosmosService photoCosmosService)
    {
        if (string.IsNullOrEmpty(tags)) return Results.BadRequest("tags is required");
        try
        {
            var photos = await photoCosmosService.GetPhotosFromTags(tags);
            return Results.Ok(photos);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError($"Error retriving photos by Tags: {ex.Message}");
        }
    }
    private static async Task<IResult> GetPhotosFromCosmosHandler(IPhotoCosmosService photoCosmosService)
    {
        try
        {
            var photos = await photoCosmosService.GetPhotoList();
            return Results.Ok(photos);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError($"Error retriving photos from database: {ex.Message}");
        }
    }

    private static async Task<IResult> GetPhotoDetail(string? id, IPhotoCosmosService photoCosmosService)
    {
        if (string.IsNullOrEmpty(id)) return Results.BadRequest("id is required");

        try
        {
            var photoDetail = await photoCosmosService.GetPhotoFromId(id);
            if (photoDetail is null) return Results.NotFound($"Photo with id: {id} not found");
            return Results.Ok(photoDetail);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError($"Error retriving photo by id: {ex.Message}");
        }
    }
}
