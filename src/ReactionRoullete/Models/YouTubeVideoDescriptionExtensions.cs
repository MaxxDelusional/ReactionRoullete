using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactionRoullete.Models
{
    public static class YouTubeVideoDescriptionExtensions
    {
        public static string GetVideoId(this YoutubeVideoDescription d)
        {
            if (d.Url.StartsWith("https://www.youtube.com/watch?v="))
                return d.Url.Substring("https://www.youtube.com/watch?v=".Length);

            return null;
        }
    }
}
