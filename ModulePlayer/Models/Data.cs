using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModulePlayer.Models
{
    public class Data
    {
        public Guid Id { get; set; }
        public Result Result { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("completion")]
        public string Completion { get; set; }
        [JsonPropertyName("duration")]
        public string Duration { get; set; }
        [JsonPropertyName("extensions")]
        public Extensions Extensions { get; set; }
    }

    public class Extensions
    {
        [JsonPropertyName("http://w3id.org/xapi/cmi5/result/extensions/progress")]
        public string Progress { get; set; }
    }
}
