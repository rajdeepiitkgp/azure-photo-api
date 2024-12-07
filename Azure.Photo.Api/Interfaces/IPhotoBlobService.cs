namespace Azure.Photo.Api.Interfaces;

public interface IPhotoBlobService
{
    Task<Uri> UploadPhotoToBlobContainer(IFormFile photo);
    Task<IEnumerable<string>> GetPhotosUrl();
}
