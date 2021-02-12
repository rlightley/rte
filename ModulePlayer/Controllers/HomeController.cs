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
        const string TINCAN = "xApi";
        const string SCORM = "SCORM 1.2";
        const string SCORM2004 = "SCORM 2004";
        const string CMI5 = "cmi5";


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
            var packagetypes = new List<string>()
            {
                SCORM,
                SCORM2004,
                TINCAN,
                CMI5
            };

            var model = new UploadViewModel()
            {
                Packagetypes = packagetypes
            };
            
            return View(model);
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
            var manifestName = "";
            var entryTag = "";
            
            if (model.Packagetype == SCORM || model.Packagetype == SCORM2004)
            {
                manifestName = "/imsmanifest.xml";
                entryTag = "resource";
            }

            if (model.Packagetype == TINCAN)
            {
                manifestName = "/tincan.xml";
                entryTag = "launch";
            }


            if (model.Packagetype == CMI5)
            {
                manifestName = "/cmi5.xml";
                entryTag = "url";
            }

            var fileBytes = await _fileDownloader.DownloadFileBytes(storageUrl + manifestName);

            if (fileBytes == null)
            {
                var fileUrl = storageUrl.Split("-");
                var folder = fileUrl[^1].Split(".");
                fileBytes = await _fileDownloader.DownloadFileBytes(storageUrl + "/" + folder[0] + manifestName);
                url = "playcourse/" + storageUrl + "/" + folder[0] + "/";
            }
            else
            {
                url = "playcourse/" + storageUrl + "/";
            }

            var manifest = new XmlDocument();
            var ms = new MemoryStream(fileBytes);
            manifest.Load(ms);
            var elemList = manifest.GetElementsByTagName(entryTag);

            var attrVal = "";
            if (model.Packagetype == SCORM || model.Packagetype == SCORM2004)
            {
                attrVal = elemList[0].Attributes["href"].Value;
            }
            else
            {
                attrVal = elemList[0].InnerXml;
            }

            var courseFileUrl = url + attrVal;
            await _ctx.Modules.AddAsync(new Module() {Title = model.Title, Url = courseFileUrl, Packagetype = model.Packagetype});
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
