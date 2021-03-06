﻿using System.Collections.Generic;

namespace ReactionRoullete.Models
{
    public class YoutubeVideoDescription
    {

        public long ID { get; set; }
        public string Thumbnail { get; internal set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public ICollection<Reaction>  Reactions { get; set; }

        public YoutubeVideoDescription()
        {
            this.Reactions = new HashSet<Reaction>();
        }
    }
}