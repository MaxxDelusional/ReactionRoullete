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
        private string key;

        public EmotionServiceClient(string key)
        {
            this.key = key;
        }

        public async Task<VideoEmotionRecognitionOperation> RecognizeAsync(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

                StringContent body = new StringContent("{ \"url\": \"" + url + "\" }", Encoding.UTF8, "application/json");

                var result = await httpClient.PostAsync("https://api.projectoxford.ai/emotion/v1.0/recognize", body);

                if (result.IsSuccessStatusCode)
                {


                    string response = await result.Content.ReadAsStringAsync();


                }
                else
                {

                }
            }

            return null;

        }





        public async Task<VideoEmotionRecognitionOperation> RecognizeInVideoAsync(string url)
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
                        Url = operationalUrl
                    };
                }
                else
                {

                }
            }

            return null;
        }

        public async Task<string> RecognitionInVideoOperationResult(string operationUrl)
        {

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

                string response = await httpClient.GetStringAsync(operationUrl);
                return response;
            }

            return null;
        }


    }
    public class VideoEmotionRecognitionOperation
    {

        public string Url { get; set; }
    }
}
