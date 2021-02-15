using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModulePlayer.Models
{
    public class DataString
    {
        public Guid Id { get; set; }
        public Result Result { get; set; }
        public Context Context { get; set; }
    }

    public class Result
    {
        [JsonProperty("completion")]
        public string Completion { get; set; }
        [JsonProperty("duration")]
        public string Duration { get; set; }
        [JsonProperty("extensions")]
        public Extensions Extensions { get; set; }
    }

    public class Extensions
    {
        [JsonProperty("http://w3id.org/xapi/cmi5/result/extensions/progress")]
        public string Progress { get; set; }
    }

    public class Context
    {
        [JsonProperty("registration")]
        public string Registration { get; set; }
    }
}
