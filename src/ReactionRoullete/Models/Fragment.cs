using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactionRoullete.Models
{
    public class Fragment
    {
        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("interval")]
        public int Interval { get; set; }

        [JsonProperty("events")]
        public Event[][] Events { get; set; }
    }
}
