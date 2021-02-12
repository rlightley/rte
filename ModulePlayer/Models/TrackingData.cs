using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModulePlayer.Models
{
    public class TrackingData
    {
        public TrackingData()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Progress { get; set; }
        public bool Complete { get; set; }
        public string Location { get; set; }
    }
}
