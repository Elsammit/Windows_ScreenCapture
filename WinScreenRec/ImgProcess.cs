using System;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Runtime.InteropServices;
using WinScreenRec.Reference;
using System.IO;

namespace WinScreenRec
{
    class ImgProcess
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        Scalar BLUE = new Scalar(255, 0, 0);                            
        Scalar RED = new Scalar(0, 0, 255);
        Scalar GREEN = new Scalar(0, 255, 0);

        VideoWriter writer = null;                                      // Video writer for recording
        string RecordFilePath = "";                                     // Record file path.
        RECT m_recordData = new RECT();                                 // Record area.
        System.Drawing.Point m_mousePos = new System.Drawing.Point();   // Mouse position.
        Scalar CursorColor = new Scalar();                              // Mouse cursor color.
        Bitmap bmp = null;

        public bool IsAudioOn { set; get; } = false;                    // Audio Enable/Disable flag.

        // Object generation for combining video and audio.
        SynthesisVideoAndAudio m_SynthesisVideoAndAudio = new SynthesisVideoAndAudio();

        int RecordCnt = 0;  // Recoring counter.


        public bool IsUsingAudioEna
        {
            get
            {
                return m_SynthesisVideoAndAudio.UsingAudioEna;
            }
        }

        /// <summary>
        /// Initialize video writer.
        /// </summary>
        /// <param name="rect">Cutting rectangle</param>
        public void InitVideoWriter()
        {
            int width = m_recordData.right - m_recordData.left;
            int height = m_recordData.bottom - m_recordData.top;
            writer = new VideoWriter(Define.TEMPVIDEOPATH, FourCC.MP4V, 10,
                    new OpenCvSharp.Size(width, height));
        }

        /// <summary>
        /// Setting file path to member parameter.
        /// </summary>
        /// <param name="filePath">record file path</param>
        /// <param name="rect">cutting area</param>
        public void SetFilePath(string filePath, RECT rect)
        {
            RecordFilePath = filePath;
            InitVideoWriter();
        }

        public int GetRecordCount()
        {
            return RecordCnt;
        }

        /// <summary>
        /// Get Caputure Image and write to video file.
        /// </summary>
        /// <param name="isStartRec">Is starting record ?</param>
        /// <param name="rect">cutting area</param>
        /// <param name="screenBmp">capture image</param>
        /// <returns></returns>
        public bool GetCaptureImage(int isStartRec, RECT rect, ref System.Drawing.Bitmap screenBmp)
        {
            bool ret = true;

            var bmpGraphics = Graphics.FromImage(screenBmp);
            bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);

            if (!WriteVideo(isStartRec, ref screenBmp, rect))
            {
                ret = false;
            }
            bmpGraphics.Dispose();
            
            return ret;
        }

        /// <summary>
        /// Mouse cursor position calculation in the area.
        /// </summary>
        /// <param name="rect">Video Area</param>
        /// <param name="mousePos">Mouse position</param>
        public void CalcMousePositionFromRect(RECT rect, System.Drawing.Point mousePos)
        {
            // If the mouse is in the area.
            if (rect.left <= mousePos.X && rect.right >= mousePos.X &&
                rect.top <= mousePos.Y && rect.bottom >= mousePos.Y)
            {
                m_mousePos.X = mousePos.X - rect.left;
                m_mousePos.Y = mousePos.Y - rect.top;
            }
            else
            {   // If the mouse is not in the area.
                m_mousePos.X = -1;
                m_mousePos.Y = -1;
            }
        }

        /// <summary>
        /// Setting mouse cursor color.
        /// </summary>
        /// <param name="leftBtn">Click left mouse button</param>
        /// <param name="rightBtn">Click right mouse button</param>
        public void SetMouseCursorColor(bool leftBtn, bool rightBtn)
        {
            if(leftBtn && !rightBtn)
            {   // If click left button, mouse cursor change red.
                CursorColor = RED;
            }else if (!leftBtn && rightBtn)
            {  // If click left button, mouse cursor change green.
                CursorColor = GREEN;
            }
            else
            {  // If click both left and right button, mouse cursor change blue.
                CursorColor = BLUE;
            }
        }

        /// <summary>
        /// Video write to file.
        /// </summary>
        /// <param name="isStartRec">Is starting record ?</param>
        /// <param name="rect">cutting area</param>
        /// <param name="screenBmp">capture image</param>
        /// <returns></returns>
        private bool WriteVideo(int isStartRec, ref Bitmap screenBmp, RECT rect)
        {
            m_recordData = rect;

            // Calculate the capture area.
            int capWidth = m_recordData.right - m_recordData.left;
            int capHeight = m_recordData.bottom - m_recordData.top;

            // If the capture area is small.
            if (capHeight <= 0 || capWidth <= 0)
            {
                Console.WriteLine("size Error");
                return false;
            }

            // Create cutout area.
            Rectangle rectBuf = 
                new System.Drawing.Rectangle(rect.left, rect.top, capWidth, capHeight);
            bmp = screenBmp.Clone(rectBuf, screenBmp.PixelFormat);
            using (Mat mat = BitmapConverter.ToMat(bmp))
            {
                // Resize capture size.
                Cv2.Resize(mat, mat, new OpenCvSharp.Size(capWidth, capHeight));
                
                // Add mouse position.
                Cv2.Circle(mat, m_mousePos.X, m_mousePos.Y, 3, CursorColor, 3, LineTypes.AntiAlias);

                
                if (isStartRec == Define.ISRECSTART && writer != null && writer.IsOpened())
                {   // Writes to video file when in video recording state.

                    // You can't record images without it !!
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2RGB);
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.RGB2BGR);

                    writer.Write(mat);
                    RecordCnt++;
                }
                else if(isStartRec == Define.ISRECSTOP)
                {   // At the end of the video, 
                    // the system goes into standby mode after the Close process of video creation.
                    if (writer != null && writer.IsOpened())
                    {
                        isStartRec = Define.ISRECSTANBY;    // State to standby.
                        RecordCnt = 0;
                        writer.Release();
                        if (File.Exists(RecordFilePath))
                        {
                            File.Delete(RecordFilePath);
                        }

                        // When recording audio is enabled.
                        if (IsAudioOn)
                        {
                            m_SynthesisVideoAndAudio.SetOutputVideoPath(RecordFilePath);
                            m_SynthesisVideoAndAudio.ExecSynthesis();
                        }
                        else 
                        {
                            File.Copy(Define.TEMPVIDEOPATH, RecordFilePath);
                        }
                    }
                }
                else
                {
                    if(writer != null && !writer.IsOpened())
                    {
                        InitVideoWriter();
                    }
                }
                // Cv2.ImShow("test", mat);
                Cv2.WaitKey(48);
            }
            bmp.Dispose();

            return true;
        }

        private bool GetAppPosition(ref RECT rect)
        {
            bool flag = false;
            string appName = "notepad"; //ここに実行ファイル名(拡張子なし)を記入する.

            try
            {
                var mainWindowHandle = System.Diagnostics.Process.GetProcessesByName(appName)[0].MainWindowHandle;
                flag = GetWindowRect(mainWindowHandle, out rect);
            }
            catch
            {
                flag = false;
            }

            return flag;
        }
    }
}
