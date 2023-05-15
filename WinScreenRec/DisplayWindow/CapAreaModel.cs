using System;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Linq;
using WinScreenRec.Reference;
using System.Windows.Forms;

namespace WinScreenRec
{
    class CapAreaModel
    {

        ImgProcess m_ImgProcess = new ImgProcess(); // Image processing object.

        // Recording possition.
        public struct Position
        {
            public int left;
            public int top;
            public int width;
            public int height;
        }
        Position position = new Position();
        ImgProcess.RECT m_RECT = new ImgProcess.RECT();

        public double RectLeft { set; get; }        // Recording area left.
        public double RectTop { set; get; }         // Recording area top.
        public double RectHeight { set; get; }      // Recording area height.
        public double RectWidth { set; get; }       // Recording area width.
        public string RectangleMargin { set; get; } // Display Rectangle area position.

        public int isStartRec { set; get; } = Define.ISRECSTANBY;   // Start record flag(stanby:0, starting:1, stopping:2).
        public bool isStartPrev { set; get; } = true;   // Start preview flag(start:true, not start:false).
        public bool IsMouseDown { set; get; } = false;  // Is mouse down ? (Yes:true, No:false).

        public string RecordTimer { set; get; } = "";   // Recording Time.
        public int RecordCounter { set; get; } = 0;     // Recording Counter (for Recording stop). 

        /// <summary>
        /// Audio enable / disable.
        /// We need ffmpeg. If ffmpeg is not installed, audio is disable.
        /// </summary>
        public bool IsUsingAudioEna
        {
            get
            {
                return m_ImgProcess.IsUsingAudioEna;
            }
        }

        /// <summary>
        /// Audio ON / OFF.
        /// Audio settings can be toggled manually.
        /// </summary>
        public bool IsAudioON {
            get
            {
                return m_ImgProcess.IsAudioOn;
            }
            set
            {
                m_ImgProcess.IsAudioOn = value;
            }
        }

        public delegate void SetRectInformation(double rectHeight, double rectWidth, string rectMargin);
        public CapAreaModel()
        {
        }

        /// <summary>
        /// Position get function.
        /// </summary>
        /// <returns></returns>
        public Position Getposition()
        {
            return position;
        }

        /// <summary>
        /// Set record file path.
        /// </summary>
        /// <param name="fileName">record file path</param>
        public void SetFilePath(string fileName)
        {
            m_ImgProcess.SetFilePath(fileName, m_RECT);
        }


        /// <summary>
        /// calculate the cutting positoin and area.
        /// </summary>
        /// <param name="pos">cutting position</param>
        /// <param name="setRectInformation">cutting area</param>
        public void MakePosition(System.Windows.Point pos, SetRectInformation setRectInformation)
        {
            double Xpos = RectLeft; // Horizontal axis of recording area.
            double Ypos = RectTop;  // Vertical axis of recording area.

            // If the mouse is being held down, change the area.
            if (IsMouseDown)
            {
                RectHeight = Math.Abs(pos.Y - RectTop);
                RectWidth = Math.Abs(pos.X - RectLeft);

                if ((RectTop > pos.Y) && (RectLeft > pos.X))
                {
                    RectangleMargin = pos.X.ToString() + "," + pos.Y.ToString();
                    Xpos = pos.X;
                    Ypos = pos.Y;
                }
                else if (RectTop > pos.Y)
                {
                    RectangleMargin = RectLeft.ToString() + "," + pos.Y.ToString();
                    Xpos = RectLeft;
                    Ypos = pos.Y;
                }
                else if (RectLeft > pos.X)
                {
                    RectangleMargin = pos.X.ToString() + "," + RectTop.ToString();
                    Xpos = pos.X;
                    Ypos = RectTop;
                }

                var window = System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().FirstOrDefault(w => w is CaptureAreaWindow);
                var rectCanvas = (Canvas)window.FindName("RectArea");

                position.width = (int)(RectWidth * (SystemParameters.PrimaryScreenWidth / rectCanvas.ActualWidth));
                position.height = (int)(RectHeight * (SystemParameters.PrimaryScreenHeight / rectCanvas.ActualHeight));
                position.top = (int)(Ypos * (SystemParameters.PrimaryScreenHeight / rectCanvas.ActualHeight));
                position.left = (int)(Xpos * (SystemParameters.PrimaryScreenWidth / rectCanvas.ActualWidth));

                setRectInformation(RectHeight, RectWidth, RectangleMargin);
            }
        }


        /// <summary>
        /// Movie Capture thread.
        /// </summary>
        public void CaptureMovieAsync()
        {
            bool ret = true;

            // Get the window area.
            var bitmap = new System.Drawing.Bitmap(
                (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            while (isStartPrev)
            {
                Define.CapFrameRate.Restart();
                ChangeCntToTimer(m_ImgProcess.GetRecordCount());    // Timer count is up.
                ret = CaputureScreen(ref bitmap);                   
                if (!ret) { isStartPrev = ret; }
            }
            Console.WriteLine("Finish capture thread.");
            bitmap.Dispose();
        }

        /// <summary>
        /// Caputure area function.
        /// </summary>
        /// <param name="bitmap">caputure image</param>
        /// <returns></returns>
        public bool CaputureScreen(ref Bitmap bitmap)
        {
            bool ret = true;

            Position position = Getposition();

            // If position is not select, caputuring window all area.
            // If an area is selected, only the selected area is captured.
            if (position.width <= 0 || position.height <= 0)
            {
                m_RECT.right = (int)SystemParameters.PrimaryScreenWidth;
                m_RECT.bottom = (int)SystemParameters.PrimaryScreenHeight;
                m_RECT.left = 0;
                m_RECT.top = 0;
            }
            else
            {
                m_RECT.right = position.width + position.left;
                m_RECT.bottom = position.height + position.top;
                m_RECT.left = position.left;
                m_RECT.top = position.top;
            }
            System.Drawing.Point cPoint = System.Windows.Forms.Cursor.Position;

            // Mouse cursor position superimposed on image.
            m_ImgProcess.SetMouseCursorColor(
                System.Windows.Forms.Control.MouseButtons.HasFlag(MouseButtons.Left),
                System.Windows.Forms.Control.MouseButtons.HasFlag(MouseButtons.Right)
            );
            m_ImgProcess.CalcMousePositionFromRect(m_RECT, cPoint);
            m_ImgProcess.GetCaptureImage(isStartRec, m_RECT, ref bitmap);


            return ret;
        }

        /// <summary>
        /// Change count to timer.
        /// </summary>
        /// <param name="timerCnt">timer count</param>
        void ChangeCntToTimer(int timerCnt)
        {
            int minutes = timerCnt / 10 / 60;
            int seconds = timerCnt / 10 % 60;

            RecordTimer = minutes.ToString("00") + ":" + seconds.ToString("00");
            RecordCounter = timerCnt;
        }
    }
}
