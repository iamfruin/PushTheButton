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
            var soundPlayer = new SoundPlayer(filepath);
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