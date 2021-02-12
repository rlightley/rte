using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModulePlayer.Models
{
    public class Module
    {
        public Module()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Packagetype { get; set; }
    }
}
