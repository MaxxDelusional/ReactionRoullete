using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ReactionRoullete.Services;
using ReactionRoullete.Models;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Hosting;
using System.Diagnostics;

namespace ReactionRoullete.Controllers
{
    public class DefaultController : Controller
    {
        private readonly YoutubeService youtubeService;
        private readonly ApplicationDbContext db;
        private readonly EmotionServiceClient emotionService;
        private readonly IHostingEnvironment hostingEnvironment;


        public DefaultController(YoutubeService youtubeService, ApplicationDbContext db, EmotionServiceClient emotionService, IHostingEnvironment hostingEnvironment)
        {
            this.youtubeService = youtubeService;
            this.db = db;
            this.emotionService = emotionService;
            this.hostingEnvironment = hostingEnvironment;

        }



        public IActionResult Index()
        {
            //var videos = from x in await youtubeService.GetYoutubeVideoDescriptions()
            //             select x;



            var videoDescriptions = from x in db.YoutubeVideoDescriptions
                                    select x;
            return View(videoDescriptions);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public async Task<IActionResult> React(long youtubeVideoDescriptionID)
        {
            var videoDescription = await db.YoutubeVideoDescriptions.FirstOrDefaultAsync(x => x.ID == youtubeVideoDescriptionID);

            return View(videoDescription);
        }


        [HttpPost]
        public async Task<IActionResult> React([FromQuery]long youtubeVideoDescriptionID, IFormFile recordedVideo)
        {
            if (null == recordedVideo && Request.Form.Files.Count > 0)
                recordedVideo = Request.Form.Files[0];



            var videoDescription = await db.YoutubeVideoDescriptions.FirstOrDefaultAsync(x => x.ID == youtubeVideoDescriptionID);

            string url = await PersistVideoFile(recordedVideo);

            VideoEmotionRecognitionOperation recognizeResult = null;

            recognizeResult = await emotionService.RecognizeInVideoAsync(url);

            return RedirectToAction("Results", "Default", new { operationUrl = recognizeResult.Url });
        }

        private async Task<string> PersistVideoFile(IFormFile videoFile)
        {
            var videoguid = Guid.NewGuid();
            string relativewebmpath = $"uploads/videos/{videoguid.ToString()}.webm";
            string relativemp4path = $"uploads/videos/{videoguid.ToString()}.mp4";

            var absolutewebmpath = hostingEnvironment.MapPath(relativewebmpath);
            var absolutemp4path = hostingEnvironment.MapPath(relativemp4path);

            await videoFile.SaveAsAsync(hostingEnvironment.MapPath(relativewebmpath));

            //Transcode here
            string ffmpegexe = @"C:\git\ReactionRoullete\src\ReactionRoullete\ffmpeg.exe";

            string ffmpegarguments = $"-i {absolutewebmpath} {absolutemp4path}";

            Process p = Process.Start(ffmpegexe, ffmpegarguments);

            p.WaitForExit();

            return Url.Content(relativemp4path);
            //  return "http://reactionroullete.azurewebsites.net/testData/WIN_20160402_00_37_01_Pro.mp4";

        }

        public async Task<IActionResult> Results(long youtubeVideoDescriptionID, string operationUrl)
        {
            var videoDescription = await db.YoutubeVideoDescriptions.FirstOrDefaultAsync(x => x.ID == youtubeVideoDescriptionID);
            ViewBag.OperationUrl = operationUrl;




            var stuff = await emotionService.RecognitionInVideoOperationResult(operationUrl);

            return View(videoDescription);
        }




        public IActionResult Test()
        {
            return View();
        }



        public IActionResult Error()
        {
            return View();
        }
    }
}
