using ReactionRoullete.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactionRoullete.Services
{
    public class YoutubeService
    {
        public YoutubeService()
        {

        }

        

        public async Task<IEnumerable<YoutubeVideoDescription>> GetYoutubeVideoDescriptions()
        {
       
            List<YoutubeVideoDescription> videos = new List<YoutubeVideoDescription>();
            /*
       videos.Add(new YoutubeVideoDescription()
       {
           ID = 1,
           Title = "TAYLOR vs. TREADMILL",
           Url = "https://www.youtube.com/watch?v=fK_zwl-lnmc",
           Thumbnail = "https://i.ytimg.com/vi/fK_zwl-lnmc/hqdefault.jpg?custom=true&w=320&h=180&stc=true&jpg444=true&jpgq=90&sp=68&sigh=zSbzEwcYz5dsCaTa_xiXv4FVn-M"
       });


    videos.Add(new YoutubeVideoDescription()
       {
           ID = 2,
           Title = "Experience YouTube in #SnoopaVision ",
           Url = "https://www.youtube.com/watch?v=DPEJB-FCItk",
           Thumbnail = "https://i.ytimg.com/vi/DPEJB-FCItk/hqdefault.jpg?custom=true&w=320&h=180&stc=true&jpg444=true&jpgq=90&sp=68&sigh=OG3S7-JoKXsbG2anIyaQYhOdZjo"
    });

       videos.Add(new YoutubeVideoDescription()
       {
           ID = 3,
           Title = "Donald Trump's Huge Campaign Announcement",
           Url = "https://www.youtube.com/watch?v=VO-1ePYypdU",
           Thumbnail = "https://i.ytimg.com/vi/VO-1ePYypdU/hqdefault.jpg?custom=true&w=320&h=180&stc=true&jpg444=true&jpgq=90&sp=68&sigh=6IUfgGZPO_A2c9ytjcrknrF5lyI"
       });

       videos.Add(new YoutubeVideoDescription()
       {
           ID = 4,
           Title = "The Ultimate Pixar Easter Egg Unveiled",
           Url = "https://www.youtube.com/watch?v=ByqIiRUuoQ0",
           Thumbnail = "https://i.ytimg.com/vi/ByqIiRUuoQ0/hqdefault.jpg?custom=true&w=320&h=180&stc=true&jpg444=true&jpgq=90&sp=68&sigh=YvTlcOIunkYtqTS-t4iL51S4_o0"
       });


       videos.Add(new YoutubeVideoDescription()
       {
           ID = 5,
           Title = "The Late Show Wheel Of News IV (with Bernie Sanders)",
           Url = "https://www.youtube.com/watch?v=faXLfQABEiQ",
           Thumbnail = "https://i.ytimg.com/vi/faXLfQABEiQ/hqdefault.jpg?custom=true&w=320&h=180&stc=true&jpg444=true&jpgq=90&sp=68&sigh=MwG6eVeUCLdmuc03EnkK0TMf1Tg"
       });



*/

            return videos;
        }
    }
}
