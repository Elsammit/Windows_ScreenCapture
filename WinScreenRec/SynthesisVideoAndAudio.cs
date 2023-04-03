using System;
using System.Diagnostics;
using WinScreenRec.Reference;

namespace WinScreenRec
{
    class SynthesisVideoAndAudio
    {
        private string OutputVideoPath = "";    // Output video path.
        private string InputVideoPath = "";     // Input video path.
        private string AudioPath = "";          // Input audio path.

        // Audio enable flag.
        public bool UsingAudioEna { get; set; } = true; 

        public SynthesisVideoAndAudio()
        {
            // Check audio synthesis enable /disable.
            using (var process = new Process())
            {
                process.StartInfo.FileName = "ffmpeg";
                process.StartInfo.Arguments = "-version";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;

                bool ret = true;

                // Execute command.
                try
                {
                    ret = process.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ffmpeg check Exeption:{0}",e);
                    UsingAudioEna = false;
                    return;
                }

                Console.WriteLine("process ret:{0}", ret);
                
                UsingAudioEna = ret;
            }
        }

        /// <summary>
        /// Setting output video path.
        /// </summary>
        /// <param name="path">setted video path.</param>
        public void SetOutputVideoPath(string path)
        {
            OutputVideoPath = path;
        } 

        /// <summary>
        /// Get output video path.
        /// </summary>
        /// <returns></returns>
        public string GetOutputVideoPath()
        {
            return OutputVideoPath;
        }

        /// <summary>
        /// Execute synthesis the audio.
        /// </summary>
        /// <returns></returns>
        public bool ExecSynthesis()
        {
            bool ret = true;

            using (var process = new Process())
            {
                InputVideoPath = Define.TEMPVIDEOPATH;
                AudioPath = Define.TEMPAUDIOPATH;

                process.StartInfo.FileName = "ffmpeg";
                process.StartInfo.Arguments = $@"-i {InputVideoPath} -i {AudioPath} -c:v copy -c:a aac -map 0:v:0 -map 1:a:0 {OutputVideoPath}";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;

                ret = process.Start();

                process.WaitForExit();

                Console.WriteLine("Command finish!!");
            }

            return ret;
        }
    }
}
