﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ModulePlayer.Models
{
    public class UploadViewModel
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }
    }
}
