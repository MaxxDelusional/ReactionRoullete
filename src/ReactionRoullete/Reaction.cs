using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactionRoullete.Models
{
    public class Reaction
    {
        [JsonProperty("id")]
        public long ID { get; set; }

        [JsonProperty("dateCreated")]
        public DateTimeOffset DateCreated { get; set; }

        [JsonProperty("youtubeVideoDescriptionID")]
        public long YoutubeVideoDescriptionID { get; set; }


        public YoutubeVideoDescription YoutubeVideoDescription { get; set; }


        [JsonProperty("dateProcessed")]
        public DateTimeOffset? DateProcessed { get; set; }


        [JsonProperty("operationUrl")]
        public string OperationUrl { get; set; }

        [NotMapped]
        [JsonProperty("processingComplete")]
        public bool ProcessingComplete { get { return this.DateProcessed.HasValue; } }


        [NotMapped]
        [JsonProperty("processingFailed")]
        public bool ProcessingFailed { get { return this.DateProcessed == null && this.DateCreated.AddMinutes(10) < DateTimeOffset.Now; } }

        [JsonProperty("neutral")]
        public double? Neutral { get; set; }
        [JsonProperty("happiness")]
        public double? Happiness { get; set; }
        [JsonProperty("surprise")]
        public double? Surprise { get; set; }
        [JsonProperty("sadness")]
        public double? Sadness { get; set; }
        [JsonProperty("anger")]
        public double? Anger { get; set; }
        [JsonProperty("disgust")]
        public double? Disgust { get; set; }
        [JsonProperty("fear")]
        public double? Fear { get; set; }
        [JsonProperty("contempt")]
        public double? Contempt { get; set; }
    }
}