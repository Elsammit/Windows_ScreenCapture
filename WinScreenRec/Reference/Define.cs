
using OpenCvSharp;

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

        public const string ISRECORDINGCONTENT = "Stop";
        public const string ISRECSTANDBYCONTENT = "Record";

        public const string ISAUDIOONMESSAGE = "Audio Rec. ON";
        public const string ISAUDIOOFFMESSAGE = "Audio Rec. OFF";
        public const string ISAUDIODISABLEMESSAGE = "Audio Rec. disable";

        public const int MOUSEPOINTRADIUS = 3;
        public const int MOUSEPOINTTHICKNESS = 3;

        public const int VIDEOFRAMERATE = 10;
        public static System.Diagnostics.Stopwatch CapFrameRate = new System.Diagnostics.Stopwatch();

        public static string[] EXTENSIONLIST = new string[3]{ "動画種類", "MP4", "WMV" };
        public static int[] EXTENSIONFOURCC = new int[2] { FourCC.MP4V, FourCC.WMV3 }; 
    }
}
