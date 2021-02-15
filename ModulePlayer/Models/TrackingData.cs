using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModulePlayer.Models
{
    public class TrackingData
    {
        protected TrackingData()
        {
            
        }

        public TrackingData(string progress, bool complete, string location, string suspendData)
        {
            Id = Guid.NewGuid();
            Progress = progress;
            Complete = complete;
            Location = location;
            SuspendData = suspendData;
        }
        
        public Guid Id { get; set; }
        public string Progress { get; set; }
        public bool Complete { get; set; }
        public string Location { get; set; }
        public string SuspendData { get; set; }
    }
}
