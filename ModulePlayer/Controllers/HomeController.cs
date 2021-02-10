using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModulePlayer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Options;
using MimeKit;
using ModulePlayer.Azure;
using ModulePlayer.Config;
using ModulePlayer.DataAccess;

namespace ModulePlayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAzureStorageHelper _storageHelper;
        private readonly IOptions<AzureStorageConfig> _config;
        private readonly ApplicationDbContext _ctx;
        private readonly IFileDownloader _fileDownloader;
        public HomeController(ILogger<HomeController> logger, IAzureStorageHelper storageHelper, IOptions<AzureStorageConfig> config, ApplicationDbContext ctx, IFileDownloader fileDownloader)
        {
            _logger = logger;
            _storageHelper = storageHelper;
            _config = config;
            _ctx = ctx;
            _fileDownloader = fileDownloader;
        }

        public IActionResult Index()
        {
            return View();
        }

        [RequestSizeLimit(2147483648)]
        [HttpPost]
        public async Task<IActionResult> Index([FromForm]UploadViewModel model)
        {
            var storageUrl = await _storageHelper.UploadScormFile(model.File, _config.Value);

            var zip = await _storageHelper.GetScormFile(storageUrl, _config.Value);

            using (var archive = new ZipArchive(model.File.OpenReadStream()))
            {
                foreach (var entry in archive.Entries)
                {
                    await using var entryStream = entry.Open();
                    using var entryReader = new BinaryReader(entryStream);
                    await _storageHelper.UploadFile(entryStream, entry.FullName,
                        MimeTypes.GetMimeType(entry.FullName), _config.Value, storageUrl);
                }
            }

            string url;

            var fileBytes = await _fileDownloader.DownloadFileBytes(storageUrl + "/imsmanifest.xml");

            if (fileBytes == null)
            {
                var fileUrl = storageUrl.Split("-");
                var folder = fileUrl[^1].Split(".");
                fileBytes = await _fileDownloader.DownloadFileBytes(storageUrl + "/" + folder[0] + "/imsmanifest.xml");
                url = "playcourse/" + storageUrl + "/" + folder[0] + "/";
            }
            else
            {
                url = "playcourse/" + storageUrl + "/";
            }

            var manifest = new XmlDocument();
            var ms = new MemoryStream(fileBytes);
            manifest.Load(ms);
            var elemList = manifest.GetElementsByTagName("resource");

            var attrVal = elemList[0].Attributes["href"].Value;

            var courseFileUrl = url + attrVal;
            await _ctx.Modules.AddAsync(new Module() {Title = model.Title, Url = courseFileUrl});
            await _ctx.SaveChangesAsync();
            return RedirectToAction("Index", "Modules");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
