using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ModulePlayer.Config;

namespace ModulePlayer.Azure
{
    public class AzureStorageHelper : IAzureStorageHelper
    {
        private string GetContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            switch (extension)
            {
                case ".zip":
                    return "application/zip";
                case ".png":
                    return "image/png";
                default:
                    return "image/jpeg";
            }
        }

        public async Task<string> UploadScormFile(IFormFile scormFile, AzureStorageConfig _storageConfig)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageConfig.ConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(_storageConfig.Modules);

            var fileName = Guid.NewGuid() + "-" + scormFile.FileName;
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = GetContentType(scormFile.FileName);
            blockBlob.Properties.CacheControl = "max-age=31536000";
            HashSet<string> blocklist = new HashSet<string>();
            var file = scormFile;
            const int pageSizeInBytes = 10485760;
            long prevLastByte = 0;
            long bytesRemain = file.Length;

            byte[] bytes;

            using (MemoryStream ms = new MemoryStream())
            {
                var fileStream = file.OpenReadStream();
                await fileStream.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            do
            {
                long bytesToCopy = Math.Min(bytesRemain, pageSizeInBytes);
                byte[] bytesToSend = new byte[bytesToCopy];

                Array.Copy(bytes, prevLastByte, bytesToSend, 0, bytesToCopy);
                prevLastByte += bytesToCopy;
                bytesRemain -= bytesToCopy;

                string blockId = Guid.NewGuid().ToString();
                string base64BlockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId));

                await blockBlob.PutBlockAsync(
                    base64BlockId,
                    new MemoryStream(bytesToSend, true),
                    null
                    );

                blocklist.Add(base64BlockId);

            } while (bytesRemain > 0);

            await blockBlob.PutBlockListAsync(blocklist);

            return await Task.FromResult(fileName);
        }

        public async Task<Stream> GetScormFile(string fileName, AzureStorageConfig _storageConfig)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageConfig.ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            var containerClient = blobClient.GetContainerReference(_storageConfig.Modules);
            var cli = containerClient.GetBlockBlobReference(fileName);

            var stream = new MemoryStream();
            await cli.DownloadToStreamAsync(stream);

            return stream;
        }

        public async Task UploadFile(Stream stream, string fileName, string mimeType, AzureStorageConfig _storageConfig, string folderName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageConfig.ConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(Path.Combine(_storageConfig.Modules, folderName));

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            blockBlob.Properties.CacheControl = "max-age=31536000";
            blockBlob.Properties.ContentType = mimeType;
            await blockBlob.UploadFromStreamAsync(stream);
        }
    }
}
