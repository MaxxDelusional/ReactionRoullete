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

using System.Diagnostics;
using System.Text;

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

        const string ExpressionStringMarker = "NHFDUSAC";

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

            Reaction reaction = new Reaction();
            reaction.DateCreated = DateTimeOffset.Now;
            reaction.YoutubeVideoDescriptionID = youtubeVideoDescriptionID;
            reaction.OperationUrl = recognizeResult.Url;
            reaction.ApiKey = recognizeResult.ApiKey;
            db.Reactions.Add(reaction);
            await db.SaveChangesAsync();


            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Results", "Default", new { youtubeVideoDescriptionID = youtubeVideoDescriptionID, reactionID = reaction.ID })
            });
            //     return RedirectToAction("Results", "Default", new { youtubeVideoDescriptionID = youtubeVideoDescriptionID, reactionID = reaction.ID });
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



            Reaction averageReaction = new Reaction();
            int count = 0;
            var releventReactions = (from x in db.Reactions
                                     where x.YoutubeVideoDescriptionID == youtubeVideoDescriptionID
                                     && x.ID != reactionID
                                     select x);


            averageReaction.Anger = releventReactions.Where(x => x.Anger != null).Average(x => x.Anger);
            averageReaction.Contempt = releventReactions.Where(x => x.Contempt != null).Average(x => x.Contempt);
            averageReaction.Disgust = releventReactions.Where(x => x.Disgust != null).Average(x => x.Disgust);
            averageReaction.Fear = releventReactions.Where(x => x.Fear != null).Average(x => x.Fear);
            averageReaction.Happiness = releventReactions.Where(x => x.Happiness != null).Average(x => x.Happiness);
            averageReaction.Neutral = releventReactions.Where(x => x.Neutral != null).Average(x => x.Neutral);
            averageReaction.Sadness = releventReactions.Where(x => x.Sadness != null).Average(x => x.Sadness);
            averageReaction.Surprise = releventReactions.Where(x => x.Surprise != null).Average(x => x.Surprise);



            ViewBag.AverageReaction = averageReaction;




            return View(videoDescription);
        }


        public async Task<Reaction> GetReaction(long reactionID)
        {
            Reaction reaction = await db.Reactions.FirstOrDefaultAsync(x => x.ID == reactionID);

            if (string.IsNullOrEmpty(reaction.ApiKey))
                reaction.ApiKey = "a728c60e913a44aeb33b659cb91e057e";

            if (!reaction.DateProcessed.HasValue)
            {


                var operationResult = await emotionService.GetOperationResultAsync(reaction.OperationUrl, reaction.ApiKey);

                if (operationResult.Status == "Succeeded")
                {

                    ProcessingResult pr = JsonConvert.DeserializeObject<ProcessingResult>(operationResult.ProcessingResultJson);


                    int totalNumberOfEvents = 0;


                    WindowFaceDistribution distributionAggregator = new WindowFaceDistribution();
                    WindowMeanScores meanAggregator = new WindowMeanScores();

                    StringBuilder windowstringbuilder = new StringBuilder();



                    foreach (var fragment in pr.Fragments)
                    {
                        if (null == fragment)
                            continue;

                        if (null != fragment.Events)
                            foreach (var ev in fragment.Events)
                            {
                                if (null == ev)
                                    continue;

                                foreach (var subEvent in ev)
                                {
                                    if (null == subEvent)
                                        continue;

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

                                    float[] elems = new float[] {subEvent.WindowMeanScores.Neutral,
                                    subEvent.WindowMeanScores.Happiness,
                                    subEvent.WindowMeanScores.Fear,
                                    subEvent.WindowMeanScores.Disgust,
                                    subEvent.WindowMeanScores.Surprise,
                                    subEvent.WindowMeanScores.Sadness,
                                    subEvent.WindowMeanScores.Anger,
                                    subEvent.WindowMeanScores.Contempt,
                                };

                                    var max = elems.Max();

                                    int idx = 0;

                                    for (int i = 0; i < elems.Length; i++)
                                        if (elems[i] == max)
                                        {
                                            idx = i;
                                            break;
                                        }

                                    windowstringbuilder.Append(ExpressionStringMarker[idx]);

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

                    var window = windowstringbuilder.ToString();

                    reaction.SerializedTimeString = window;
                    reaction.DateProcessed = DateTimeOffset.Now;

                    await db.SaveChangesAsync();

                    //   Reaction reaction = new Reaction();
                }
                if (operationResult.Status == "Failed")
                {
                    //Concider deleting reaction here
                    return null;
                }
            }



            return reaction;
        }



        public async Task<Reaction> Test()
        {
            //var result = await emotionService.RecognizeInVideoAsync("https://reactionroulette.blob.core.windows.net/preflight/b2300c12-6637-4d1c-9a75-de7e0be3004d.mp4");
            return await GetReaction(48);

        }



        public IActionResult Error()
        {
            return View();
        }
    }
}
