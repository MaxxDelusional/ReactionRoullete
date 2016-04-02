using Newtonsoft.Json;
using ReactionRoullete.Models;
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
}
