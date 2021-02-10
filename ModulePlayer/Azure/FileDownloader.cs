using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ModulePlayer.Config;

namespace ModulePlayer.Azure
{
    public class FileDownloader : IFileDownloader
    {
        private CloudStorageAccount _storageAccount = null;
        private CloudBlobClient _client = null;
        private CloudBlobContainer _container = null;
        private readonly AzureStorageConfig _config;
        public FileDownloader(IOptions<AzureStorageConfig> config)
        {
            _config = config.Value;
        }

        public async Task<byte[]> DownloadFileBytes(string filePath)
        {
            Setup();

            var blob = _container.GetBlockBlobReference(filePath);
            if (!await blob.ExistsAsync())
            {
                return null;
            }

            await blob.FetchAttributesAsync();
            byte[] byteArray = new byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(byteArray, 0);
            return byteArray;
        }

        private void Setup()
        {
            _storageAccount ??= CloudStorageAccount.Parse(_config.ConnectionString);

            _client ??= _storageAccount.CreateCloudBlobClient();

            _container ??= _client.GetContainerReference(_config.Modules);
        }
    }
}
