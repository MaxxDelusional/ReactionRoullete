using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactionRoullete.Models
{
    public class VideoOperationResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }


        [JsonProperty("progress")]
        public double? Progress { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("lastActionDateTime")]
        public DateTime? LastActionDateTime { get; set; }

        [JsonProperty("message")]
        public DateTime? Message { get; set; }

        [JsonProperty("processingResult")]
        public string ProcessingResultJson { get; set; }

    }
}
