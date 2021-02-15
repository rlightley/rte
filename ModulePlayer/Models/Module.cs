using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModulePlayer.Models
{
    public class Module
    {
        protected Module()
        {
            
        }

        public Module(string title, string url, string packagetype)
        {
            Id = Guid.NewGuid();
            Title = title;
            Url = url;
            Packagetype = packagetype;
            TrackingData = new TrackingData("0",false, "", "");
            CompletedTimes = 0;
            RestartedTimes = 0;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Packagetype { get; set; }
        public TrackingData TrackingData { get; set; }
        public int CompletedTimes { get; set; }
        public int RestartedTimes { get; set; }
    }
}
