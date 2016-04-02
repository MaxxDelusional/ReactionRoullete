using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactionRoullete.Models
{
    public class Event
    {
        [JsonProperty("windowFaceDistribution")]
        public WindowFaceDistribution WindowFaceDistribution { get; set; }

        [JsonProperty("windowMeanScores")]
        public WindowMeanScores WindowMeanScores { get; set; }
    }
}
