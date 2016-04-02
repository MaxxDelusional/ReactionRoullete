using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactionRoullete.Models
{
    public class ProcessingResult
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("timescale")]
        public int Timescale { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("framerate")]
        public float Framerate { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }


        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("fragments")]
        public Fragment[] Fragments { get; set; }
    }
}
