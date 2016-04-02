using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactionRoullete.Services
{
    public class EmotionServiceApiKeyProvider
    {
        static List<string> _ApiKeys;

        static EmotionServiceApiKeyProvider()
        {
            _ApiKeys = new List<string>();
            _ApiKeys.Add("a728c60e913a44aeb33b659cb91e057e");
            _ApiKeys.Add("9799c513119d49c0a905af880b501906");
            _ApiKeys.Add("b0c2c88edec84f4b885eb277bfa5d613");
            _ApiKeys.Add("445c2dcbe89d4c69b1f723bd47acb2c0");
        }

        public EmotionServiceApiKeyProvider()
        {

        }

        public string GetApiKey(string previousKey = null)
        {
            if (string.IsNullOrEmpty(previousKey))
                return _ApiKeys.First();
            else
                return _ApiKeys[(_ApiKeys.IndexOf(previousKey) + 1) % _ApiKeys.Count];
        }
    }
}
