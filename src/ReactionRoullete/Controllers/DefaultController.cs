using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ReactionRoullete.Services;
using ReactionRoullete.Models;
using Microsoft.Data.Entity;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.AspNet.Http;

namespace ReactionRoullete.Controllers
{
    public class DefaultController : Controller
    {
        private readonly YoutubeService youtubeService;
        private readonly ApplicationDbContext db;
        private readonly EmotionServiceClient emotionService;

        public IActionResult TestVideoUpload(IFormFile file)
        {
            return Content("Hello World");
        }
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
        public async Task<IActionResult> React(long youtubeVideoDescriptionID, string url)
        {
            var videoDescription = await db.YoutubeVideoDescriptions.FirstOrDefaultAsync(x => x.ID == youtubeVideoDescriptionID);

            //Send request off to api here, get the operation header from congitive services and pass it to the results page

            //Operation-Header
            //  Client side should use this URL to query video operation status/ result.
            //Example: https://api.projectoxford.ai/emotion/v1.0/operations/EF217D0C-9085-45D7-AAE0-2B36471B89B5 




            VideoEmotionRecognitionOperation recognizeResult = null;

            
                recognizeResult = await emotionService.RecognizeInVideoAsync(url);

            



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
