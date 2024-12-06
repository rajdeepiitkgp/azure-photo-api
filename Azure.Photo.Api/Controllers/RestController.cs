namespace Azure.Photo.Api.Controllers;

public static class RestController
{
    public static void MapControllers(this WebApplication app)
    {
        app.MapGroup("/api").MapPhotosApi();
    }

    private static void MapPhotosApi(this RouteGroupBuilder group)
    {
        group.MapPost("upload", UploadPhotoHandler);
        group.MapGet("photos", GetPhotosHandler);
    }

    private static async Task<IResult> UploadPhotoHandler(IFormFile? photo)
    {
        if (photo is null) return TypedResults.BadRequest("Photo is required");
        await Task.Delay(10);

        return TypedResults.Created($"{photo.FileName}");
    }
    private static async Task<IResult> GetPhotosHandler()
    {
        await Task.Delay(10);
        var results = new List<string> { "Hello" };

        return TypedResults.Ok(results);
    }
}
