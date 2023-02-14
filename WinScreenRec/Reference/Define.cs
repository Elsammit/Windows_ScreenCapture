using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinScreenRec.Reference
{
    static class Define
    {
        public const int ISRECSTANBY = 0;
        public const int ISRECSTART = 1;
        public const int ISRECSTOP = 2;

        public const string ISWIDGETVISIBLE = "Visible";
        public const string ISWIDGETHIDDEN = "Hidden";

        public const int MAXRECORDTIME = 30 * 60;

        public const string TEMPVIDEOPATH = @"E:\tempMovies.mp4";
        public const string TEMPAUDIOPATH = @"E:\tempAudio.wav";
    }
}
