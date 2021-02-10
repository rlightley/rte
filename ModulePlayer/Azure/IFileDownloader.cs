using System.Threading.Tasks;

namespace ModulePlayer.Azure
{
    public interface IFileDownloader
    {
        Task<byte[]> DownloadFileBytes(string filePath);
    }
}
