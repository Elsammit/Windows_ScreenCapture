using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinScreenRec
{
    class SynthesisVideoAndAudio
    {
        private string OutputVideoPath = "";
        private string InputVideoPath = "";
        private string AudioPath = "";


        void SetOutputVideoPath(string path)
        {
            OutputVideoPath = path;
        } 

        string GetOutputVideoPath()
        {
            return OutputVideoPath;
        }

        bool ExecSynthesis()
        {
            bool ret = true;

            using (var process = new Process())
            {
                process.StartInfo.FileName = "ffmpeg";
                process.StartInfo.Arguments = $@"-i {InputVideoPath} -i {AudioPath} -c:v copy -c:a aac -map 1:a:0 {OutputVideoPath}";

                ret = process.Start();

                process.WaitForExit();

                Console.WriteLine("Command finish!!");
            }


            return ret;
        }
    }
}
