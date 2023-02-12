using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinScreenRec
{
    class SynthesisVideoAndAudio
    {
        private string OutputVideoPath = "";

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

            return ret;
        }
    }
}
