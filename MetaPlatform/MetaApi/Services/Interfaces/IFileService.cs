using MetaApi.Core.Domain.Clothing;
using MetaApi.Core.Domain.Hair;
using MetaApi.Models.VirtualFit;

namespace MetaApi.Services.Interfaces
{
    public interface IFileService
    {
        ClothingCollection GetClothingCollection(string host);

        HairCollection GetHairCollection(string host);

        Task<string> UploadFileAsync(IFormFile file, FileType fileType, string host);

        Task<string> UploadResultFileAsync(string imageUrl, string host, string humanImg);
    }
}
