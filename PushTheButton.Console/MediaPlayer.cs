using System;
using System.IO;
using System.Media;

namespace PushTheButton.Console
{
    public static class MediaPlayer
    {
        public static void Play(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return;
            }

            var result = File.ReadAllBytes(filepath);
            var ms = new MemoryStream(result);
            var soundPlayer = new SoundPlayer(ms);
            try
            {
               soundPlayer.PlaySync();
            }
            catch //swallow
            {    
            }
        }
    }
}