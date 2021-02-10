using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ModulePlayer.Config;

namespace ModulePlayer.Azure
{
    public interface IAzureStorageHelper
    {
        Task<string> UploadScormFile(IFormFile scormFile, AzureStorageConfig storageConfig);
        Task<Stream> GetScormFile(string fileName, AzureStorageConfig storageConfig);
        Task UploadFile(Stream stream, string fileName, string mimeType, AzureStorageConfig storageConfig, string folderName);
    }
}
