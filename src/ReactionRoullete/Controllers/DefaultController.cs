using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ReactionRoullete.Services;
using ReactionRoullete.Models;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Http;

namespace ReactionRoullete.Controllers
{
    public class DefaultController : Controller
    {
        private readonly YoutubeService youtubeService;
        private readonly ApplicationDbContext db;
        private readonly EmotionServiceClient emotionService;


        public DefaultController(YoutubeService youtubeService, ApplicationDbContext db, EmotionServiceClient emotionService)
        {
            this.youtubeService = youtubeService;
            this.db = db;
            this.emotionService = emotionService;
   
        }
        public async Task< IActionResult> Index()
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
        public async Task<IActionResult> React(long youtubeVideoDescriptionID, IFormFile recordedVideo)
        {
            var videoDescription = await db.YoutubeVideoDescriptions.FirstOrDefaultAsync(x => x.ID == youtubeVideoDescriptionID);


            //Save recordedVideo to Azure

            //Get Url from Azure

            //Pass url to cognitive

            string url = "http://reactionroullete.azurewebsites.net/testData/WIN_20160402_00_37_01_Pro.mp4";

            VideoEmotionRecognitionOperation recognizeResult = null;


            try
            {
                recognizeResult = await emotionService.RecognizeInVideoAsync(url);
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Results", "Default", new { operationUrl = recognizeResult.Url });
        }

        public async Task<IActionResult> Results(long youtubeVideoDescriptionID, string operationUrl)
        {
            var videoDescription = await db.YoutubeVideoDescriptions.FirstOrDefaultAsync(x => x.ID == youtubeVideoDescriptionID);


            ViewBag.OperationUrl = operationUrl;

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
