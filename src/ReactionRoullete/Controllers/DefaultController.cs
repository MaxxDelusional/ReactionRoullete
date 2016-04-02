﻿using System;
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

using System.Diagnostics;


namespace ReactionRoullete.Controllers
{
    public class DefaultController : Controller
    {
        private readonly YoutubeService youtubeService;
        private readonly ApplicationDbContext db;
        private readonly EmotionServiceClient emotionService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly AzureStorageService _StorageService;
        private readonly FFMpegLocator _FFMpegLocator;


        public DefaultController(YoutubeService youtubeService, ApplicationDbContext db, EmotionServiceClient emotionService, IHostingEnvironment hostingEnvironment, AzureStorageService storageService, FFMpegLocator ffmpegLocator)
        {

            this.youtubeService = youtubeService;
            this.db = db;
            this.emotionService = emotionService;
            this.hostingEnvironment = hostingEnvironment;
            this._StorageService = storageService;
            this._FFMpegLocator = ffmpegLocator;
        }


        public async Task<IActionResult> Index()
        {
            var randomVideoDescription = await (from x in db.YoutubeVideoDescriptions
                                                orderby Guid.NewGuid()
                                                select x).FirstOrDefaultAsync();
            return RedirectToAction("React", "Default", new { youtubeVideoDescriptionID = randomVideoDescription.ID });
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
            if (null == recordedVideo && Request.Form.Files.Count > 0)
                recordedVideo = Request.Form.Files[0];



            var videoDescription = await db.YoutubeVideoDescriptions.FirstOrDefaultAsync(x => x.ID == youtubeVideoDescriptionID);

            string url = await PersistVideoFile(recordedVideo);

            VideoEmotionRecognitionOperation recognizeResult = null;

            try
            {
                recognizeResult = await emotionService.RecognizeInVideoAsync(url);
            }
            catch (Exception ex)
            {

            }




            recognizeResult = await emotionService.RecognizeInVideoAsync(url);

            Reaction reaction = new Reaction();
            reaction.DateCreated = DateTimeOffset.Now;
            reaction.YoutubeVideoDescriptionID = youtubeVideoDescriptionID;
            reaction.OperationUrl = recognizeResult.Url;
            db.Reactions.Add(reaction);
            await db.SaveChangesAsync();

            return RedirectToAction("Results", "Default", new { reactionID = reaction.ID });
        }

        private async Task<string> PersistVideoFile(IFormFile videoFile)
        {
            var videoguid = Guid.NewGuid();
            string relativewebmpath = $"uploads/videos/{videoguid.ToString()}.webm";
            string relativemp4path = $"uploads/videos/{videoguid.ToString()}.mp4";

            var absolutewebmpath = hostingEnvironment.MapPath(relativewebmpath);
            var absolutemp4path = hostingEnvironment.MapPath(relativemp4path);

            await videoFile.SaveAsAsync(hostingEnvironment.MapPath(relativewebmpath));


            string ffmpegexe = _FFMpegLocator.GetFFMpegPath();

            if (string.IsNullOrEmpty(ffmpegexe))
                throw new NotSupportedException("FFMpeg was not found and can not be used.");


            string ffmpegarguments = $"-i {absolutewebmpath} {absolutemp4path}";

            Process p = Process.Start(ffmpegexe, ffmpegarguments);

            p.WaitForExit();

            if (!System.IO.File.Exists(absolutemp4path))
                throw new System.IO.FileNotFoundException("MP4 file was not generated");

            System.IO.File.Delete(absolutewebmpath);

            var storageuri = await _StorageService.PutPreflightFileAsync(absolutemp4path);

            System.IO.File.Delete(absolutemp4path);

            return storageuri.ToString();

            //  return Url.Content(relativemp4path);


        }

        public async Task<IActionResult> Results(long youtubeVideoDescriptionID, long reactionID)
        {
            var videoDescription = await db.YoutubeVideoDescriptions.FirstOrDefaultAsync(x => x.ID == youtubeVideoDescriptionID);

            ViewBag.ReactionID = reactionID;
            // var operationResult = await emotionService.GetOperationResultAsync(operationUrl);


            return View(videoDescription);
        }


        public async Task<Reaction> GetReaction(long reactionID)
        {
            Reaction reaction = await db.Reactions.FirstOrDefaultAsync(x => x.ID == reactionID);

            var operationResult = await emotionService.GetOperationResultAsync(reaction.OperationUrl);

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



        public async Task<IActionResult> Test()
        {
            var result = await emotionService.RecognizeInVideoAsync("https://reactionroulette.blob.core.windows.net/preflight/b2300c12-6637-4d1c-9a75-de7e0be3004d.mp4");
            return View();
        }



        public IActionResult Error()
        {
            return View();
        }
    }
}
