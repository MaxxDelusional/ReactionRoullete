using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ReactionRoullete.Services
{
    public class EmotionServiceClient
    {
        private EmotionServiceApiKeyProvider _KeyProvider;

        public EmotionServiceClient(EmotionServiceApiKeyProvider keyProvider)
        {
            _KeyProvider = keyProvider;
        }

        //public async Task<VideoEmotionRecognitionOperation> RecognizeAsync(string url)
        //{
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

        //        StringContent body = new StringContent("{ \"url\": \"" + url + "\" }", Encoding.UTF8, "application/json");

        //        var result = await httpClient.PostAsync("https://api.projectoxford.ai/emotion/v1.0/recognize", body);

        //        if (result.IsSuccessStatusCode)
        //        {
        //            success = true;
        //            string response = await result.Content.ReadAsStringAsync();


        //        }
        //        else
        //        {

        //        }
        //    }

        //    return null;

        //}





        public async Task<VideoEmotionRecognitionOperation> RecognizeInVideoAsync(string url)
        {

            string key = _KeyProvider.GetApiKey(null);
            int trycount = 0;
            bool success = false;
            bool retry = false;
            bool wasretry = true;

            while (!success && trycount < 10)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

                    StringContent body = new StringContent("{ \"url\": \"" + url + "\" }", Encoding.UTF8, "application/json");


                    var result = await httpClient.PostAsync("https://api.projectoxford.ai/emotion/v1.0/recognizeinvideo", body);

                    if (result.IsSuccessStatusCode)
                    {
                        string operationalUrl = result.Headers.GetValues("Operation-Location").FirstOrDefault();
                        return new VideoEmotionRecognitionOperation()
                        {
                            Url = operationalUrl,
                            ApiKey = key
                        };
                    }
                    else
                    {
                        if (result.StatusCode == (System.Net.HttpStatusCode)429)
                        {
                            IEnumerable<string> retryvalues = null;
                            if (result.Headers.TryGetValues("Retry-After", out retryvalues))
                            {
                                var waittime = retryvalues.Select(v =>
                                {
                                    int i = 0;
                                    if (int.TryParse(v, out i)) return i;
                                    else return 0;
                                }).Max();

                                if (!wasretry && waittime < 5)
                                {
                                    retry = true;
                                    waittime++;
                                    await Task.Delay(waittime * 1000);
                                }
                            }
                        }
                    }
                }

                trycount++;
               
                if (!retry)
                {
                    key = _KeyProvider.GetApiKey(key);
                    wasretry = false;
                }
                else
                {
                    retry = false;
                    wasretry = true;
                }
            }

            return null;
        }

        public async Task<VideoOperationResult> GetOperationResultAsync(string operationUrl, string apiKey)
        {

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                string json = await httpClient.GetStringAsync(operationUrl);

                return JsonConvert.DeserializeObject<VideoOperationResult>(json);


            }

            return null;
        }


    }

    public class VideoOperationResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }


        [JsonProperty("progress")]
        public double? Progress { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("lastActionDateTime")]
        public DateTime? LastActionDateTime { get; set; }

        [JsonProperty("message")]
        public DateTime? Message { get; set; }

        [JsonProperty("processingResult")]
        public string ProcessingResultJson { get; set; }

    }

    public class VideoEmotionRecognitionOperation
    {
        public string ApiKey { get; set; }
        public string Url { get; set; }
    }


    public class ProcessingResult
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("timescale")]
        public int Timescale { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("framerate")]
        public float Framerate { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
    

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("fragments")]
        public Fragment[] Fragments { get; set; }
    }

    public class Fragment
    {
        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("interval")]
        public int Interval { get; set; }

        [JsonProperty("events")]
        public Event[][] Events { get; set; }
    }

    public class Event
    {
        [JsonProperty("windowFaceDistribution")]
        public WindowFaceDistribution WindowFaceDistribution { get; set; }

        [JsonProperty("windowMeanScores")]
        public WindowMeanScores WindowMeanScores { get; set; }
    }

    public class WindowFaceDistribution
    {
        [JsonProperty("neutral")]
        public int Neutral { get; set; }
        [JsonProperty("happiness")]
        public int Happiness { get; set; }
        [JsonProperty("surprise")]
        public int Surprise { get; set; }
        [JsonProperty("sadness")]
        public int Sadness { get; set; }
        [JsonProperty("anger")]
        public int Anger { get; set; }
        [JsonProperty("disgust")]
        public int Disgust { get; set; }
        [JsonProperty("fear")]
        public int Fear { get; set; }
        [JsonProperty("contempt")]
        public int Contempt { get; set; }
    }

    public class WindowMeanScores
    {
        [JsonProperty("neutral")]
        public float Neutral { get; set; }
        [JsonProperty("happiness")]
        public float Happiness { get; set; }
        [JsonProperty("surprise")]
        public float Surprise { get; set; }
        [JsonProperty("sadness")]
        public float Sadness { get; set; }
        [JsonProperty("anger")]
        public float Anger { get; set; }
        [JsonProperty("disgust")]
        public float Disgust { get; set; }
        [JsonProperty("fear")]
        public float Fear { get; set; }
        [JsonProperty("contempt")]
        public float Contempt { get; set; }
    }



}
