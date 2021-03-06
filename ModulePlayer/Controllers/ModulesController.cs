﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using ModulePlayer.Azure;
using ModulePlayer.DataAccess;
using ModulePlayer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModulePlayer.Controllers
{
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IFileDownloader _fileDownloader;

        public ModulesController(ApplicationDbContext ctx, IFileDownloader fileDownloader)
        {
            _ctx = ctx;
            _fileDownloader = fileDownloader;
        }

        public async Task<IActionResult> Index()
        {
            var modules = await _ctx.Modules.ToListAsync();
            return View(modules);
        }

        [HttpGet("module/{id}")]
        public async Task<IActionResult> Module(Guid id)
        {
            var module = await _ctx.Modules.Where(m => m.Id == id).Include(m => m.TrackingData).FirstOrDefaultAsync();
            return View(module);
        }

        [HttpGet("module/playcourse/{*filePath:regex(\\S{{1,}})}")]
        public async Task<IActionResult> GetFileAsync(string filePath)
        {
            var fileBytes = await _fileDownloader.DownloadFileBytes(filePath);
            if (fileBytes == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return File(fileBytes, MimeTypes.GetMimeType(filePath));
        }
    }
}
