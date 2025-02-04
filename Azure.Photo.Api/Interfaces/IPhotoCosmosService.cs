using Azure.Photo.Api.Models;

namespace Azure.Photo.Api.Interfaces;

public interface IPhotoCosmosService
{
    Task InsertDataToCosmosDb(PhotoImageAnalysisResult metadata);
    Task<IEnumerable<PhotoMetadataResponse>> GetPhotosFromTags(string searchQuery);
}