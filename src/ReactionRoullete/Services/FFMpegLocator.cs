using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReactionRoullete.Services
{
    public class FFMpegLocator
    {
        const string LocalLocation = @"C:\git\ReactionRoullete\src\ReactionRoullete\ffmpeg.exe";
        const string AzureLocation = @"D:\home\site\ffmpeg.exe";

        public string GetFFMpegPath()
        {
            if (File.Exists(AzureLocation))
                return AzureLocation;
            else if (File.Exists(LocalLocation))
                return LocalLocation;
            else
                return null;
        }
    }
}
