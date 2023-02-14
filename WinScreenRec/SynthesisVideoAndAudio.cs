using System;
using System.Diagnostics;
using WinScreenRec.Reference;

namespace WinScreenRec
{
    class SynthesisVideoAndAudio
    {
        private string OutputVideoPath = "";
        private string InputVideoPath = "";
        private string AudioPath = "";


        public void SetOutputVideoPath(string path)
        {
            OutputVideoPath = path;
        } 

        public string GetOutputVideoPath()
        {
            return OutputVideoPath;
        }

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
