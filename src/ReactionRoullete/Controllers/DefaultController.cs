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
using Microsoft.AspNet.Hosting;

namespace ReactionRoullete.Controllers
{
    public class DefaultController : Controller
    {
        private readonly YoutubeService youtubeService;
        private readonly ApplicationDbContext db;
        private readonly EmotionServiceClient emotionService;
        private readonly IHostingEnvironment hostingEnvironment;

<<<<<<< HEAD
        public DefaultController(YoutubeService youtubeService, ApplicationDbContext db, EmotionServiceClient emotionService, IHostingEnvironment hostingEnvironment)
=======
        public IActionResult TestVideoUpload(IFormFile file)
        {
            return Content("Hello World");
        }
        public DefaultController(YoutubeService youtubeService, ApplicationDbContext db, EmotionServiceClient emotionService)
>>>>>>> 91d3cdbf952f60cf66936d888abeb40a38b95950
        {
            this.youtubeService = youtubeService;
            this.db = db;
            this.emotionService = emotionService;
            this.hostingEnvironment = hostingEnvironment;
   
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

           string url = await  PersistVideoFile(recordedVideo);
  
            VideoEmotionRecognitionOperation recognizeResult = null;

<<<<<<< HEAD
            try
            {
=======
            
>>>>>>> 91d3cdbf952f60cf66936d888abeb40a38b95950
                recognizeResult = await emotionService.RecognizeInVideoAsync(url);

            return RedirectToAction("Results", "Default", new { operationUrl = recognizeResult.Url });
        }

        private async Task<string> PersistVideoFile(IFormFile videoFile)
        {

            string relativePath = "uploads/videos/" + Guid.NewGuid().ToString() + ".mp4";
            await videoFile.SaveAsAsync(hostingEnvironment.MapPath(relativePath));


            return Url.Content(relativePath);
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
