using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactionRoullete.Models
{
    public class WindowFaceDistribution
    {
        [JsonProperty("neutral")]
        public int Neutral { get; set; }
        [JsonProperty("happiness")]
        public int Happiness { get; set; }
        [JsonProperty("surprise")]
        public int Surprise { get; set; }
        [JsonProperty("sadness")]
        public int Sadness { get; set; }
        [JsonProperty("anger")]
        public int Anger { get; set; }
        [JsonProperty("disgust")]
        public int Disgust { get; set; }
        [JsonProperty("fear")]
        public int Fear { get; set; }
        [JsonProperty("contempt")]
        public int Contempt { get; set; }
    }
}
