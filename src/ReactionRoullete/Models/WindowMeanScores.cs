using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactionRoullete.Models
{
    public class WindowMeanScores
    {
        [JsonProperty("neutral")]
        public float Neutral { get; set; }
        [JsonProperty("happiness")]
        public float Happiness { get; set; }
        [JsonProperty("surprise")]
        public float Surprise { get; set; }
        [JsonProperty("sadness")]
        public float Sadness { get; set; }
        [JsonProperty("anger")]
        public float Anger { get; set; }
        [JsonProperty("disgust")]
        public float Disgust { get; set; }
        [JsonProperty("fear")]
        public float Fear { get; set; }
        [JsonProperty("contempt")]
        public float Contempt { get; set; }
    }
}
