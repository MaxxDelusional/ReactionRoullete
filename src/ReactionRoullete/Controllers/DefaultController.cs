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
using Newtonsoft.Json;

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
        public IActionResult TestVideoUpload(IFormFile file)
        {
            return Content("Hello World");
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

            try
            {
                recognizeResult = await emotionService.RecognizeInVideoAsync(url);
            }
            catch (Exception ex)
            {

            }

            Reaction reaction = new Reaction();


            reaction.DateCreated = DateTimeOffset.Now;
            reaction.YoutubeVideoDescriptionID = youtubeVideoDescriptionID;

            db.Reactions.Add(reaction);
            await db.SaveChangesAsync();

            return RedirectToAction("Results", "Default", new { operationUrl = recognizeResult.Url, reactionID = reaction.ID });
        }

        private async Task<string> PersistVideoFile(IFormFile videoFile)
        {

            string relativePath = "uploads/videos/" + Guid.NewGuid().ToString() + ".mp4";
            await videoFile.SaveAsAsync(hostingEnvironment.MapPath(relativePath));


         //   return Url.Content(relativePath);

            
            return "http://reactionroullete.azurewebsites.net/testData/WIN_20160402_00_37_01_Pro.mp4";

        }

        public async Task<IActionResult> Results(long youtubeVideoDescriptionID, long reactionID,  string operationUrl)
        {
            var videoDescription = await db.YoutubeVideoDescriptions.FirstOrDefaultAsync(x => x.ID == youtubeVideoDescriptionID);
            ViewBag.OperationUrl = operationUrl;
            ViewBag.ReactionID = reactionID;
           // var operationResult = await emotionService.GetOperationResultAsync(operationUrl);


            return View(videoDescription);
        }


        public async Task<Reaction> GetVideoOperationResult(string operationUrl, long reactionID)
        {
            Reaction reaction = await db.Reactions.FirstOrDefaultAsync(x => x.ID == reactionID);



            var operationResult = await emotionService.GetOperationResultAsync(operationUrl);

            if (operationResult.Status == "Succeeded")
            {

                ProcessingResult pr = JsonConvert.DeserializeObject<ProcessingResult>(operationResult.ProcessingResultJson);


                int totalNumberOfEvents = 0;

                WindowFaceDistribution distributionAggregator = new WindowFaceDistribution();
                WindowMeanScores meanAggregator = new WindowMeanScores();

                foreach (var fragment in pr.Fragments)
                {

                    foreach (var ev in fragment.Events)
                    {
                        foreach (var subEvent in ev)
                        {

                            distributionAggregator.Anger += subEvent.WindowFaceDistribution.Anger;
                            distributionAggregator.Contempt += subEvent.WindowFaceDistribution.Contempt;
                            distributionAggregator.Disgust += subEvent.WindowFaceDistribution.Disgust;
                            distributionAggregator.Fear += subEvent.WindowFaceDistribution.Fear;
                            distributionAggregator.Happiness += subEvent.WindowFaceDistribution.Happiness;
                            distributionAggregator.Sadness += subEvent.WindowFaceDistribution.Sadness;
                            distributionAggregator.Surprise += subEvent.WindowFaceDistribution.Surprise;


                            meanAggregator.Anger += subEvent.WindowMeanScores.Anger;
                            meanAggregator.Contempt += subEvent.WindowMeanScores.Contempt;
                            meanAggregator.Disgust += subEvent.WindowMeanScores.Disgust;
                            meanAggregator.Fear += subEvent.WindowMeanScores.Fear;
                            meanAggregator.Happiness += subEvent.WindowMeanScores.Happiness;
                            meanAggregator.Sadness += subEvent.WindowMeanScores.Sadness;
                            meanAggregator.Surprise += subEvent.WindowMeanScores.Surprise;


                            totalNumberOfEvents++;
                        }
                    }
                }


                distributionAggregator.Anger = distributionAggregator.Anger / totalNumberOfEvents;
                distributionAggregator.Contempt += distributionAggregator.Contempt / totalNumberOfEvents;
                distributionAggregator.Disgust += distributionAggregator.Disgust / totalNumberOfEvents;
                distributionAggregator.Fear += distributionAggregator.Fear / totalNumberOfEvents;
                distributionAggregator.Happiness += distributionAggregator.Happiness / totalNumberOfEvents;
                distributionAggregator.Sadness += distributionAggregator.Sadness / totalNumberOfEvents;
                distributionAggregator.Surprise += distributionAggregator.Surprise / totalNumberOfEvents;

                meanAggregator.Anger = meanAggregator.Anger / totalNumberOfEvents;
                meanAggregator.Contempt += meanAggregator.Contempt / totalNumberOfEvents;
                meanAggregator.Disgust += meanAggregator.Disgust / totalNumberOfEvents;
                meanAggregator.Fear += meanAggregator.Fear / totalNumberOfEvents;
                meanAggregator.Happiness += meanAggregator.Happiness / totalNumberOfEvents;
                meanAggregator.Sadness += meanAggregator.Sadness / totalNumberOfEvents;
                meanAggregator.Surprise += meanAggregator.Surprise / totalNumberOfEvents;



                reaction.Anger = distributionAggregator.Anger;
                reaction.Contempt = distributionAggregator.Contempt;
                reaction.Disgust = distributionAggregator.Disgust;
                reaction.Fear = distributionAggregator.Fear;
                reaction.Happiness = distributionAggregator.Happiness;
                reaction.Sadness = distributionAggregator.Sadness;
                reaction.Surprise = distributionAggregator.Surprise;


                reaction.DateProcessed = DateTimeOffset.Now;
                await db.SaveChangesAsync();


                //   Reaction reaction = new Reaction();


            }
            if (operationResult.Status == "Failed")
            {
                //Concider deleting reaction here
                return null;
            }
            return reaction;
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
