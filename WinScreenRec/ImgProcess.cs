using System;
using System.Windows;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Runtime.InteropServices;
using WinScreenRec.Reference;

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
        System.Drawing.Point m_mousePos = new System.Drawing.Point();   // Mouse Position.
        Scalar CursorColor = new Scalar();
        Bitmap bmp = null;

        int RecordCnt = 0;

        /// <summary>
        /// Initialize video writer.
        /// </summary>
        /// <param name="rect">Cutting rectangle</param>
        public void InitVideoWriter()
        {
            int width = m_recordData.right - m_recordData.left;
            int height = m_recordData.bottom - m_recordData.top;
            writer = new VideoWriter(RecordFilePath, FourCC.MP4V, 10,
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
                MessageBox.Show("正しくエリアを指定ください", "エリア指定エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                ret = false;
            }
            bmpGraphics.Dispose();
            
            return ret;
        }

        public void CalcMousePositionFromRect(RECT rect, System.Drawing.Point mousePos)
        {
            if(rect.left <= mousePos.X && rect.right >= mousePos.X &&
                rect.top <= mousePos.Y && rect.bottom >= mousePos.Y)
            {
                m_mousePos.X = mousePos.X - rect.left;
                m_mousePos.Y = mousePos.Y - rect.top;
            }
            else
            {
                m_mousePos.X = -1;
                m_mousePos.Y = -1;
            }
        }

        public void SetMouseCursorColor(bool leftBtn, bool rightBtn)
        {
            if(leftBtn && !rightBtn)
            {
                CursorColor = RED;
            }else if (!leftBtn && rightBtn)
            {
                CursorColor = GREEN;
            }
            else
            {
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

            int capWidth = m_recordData.right - m_recordData.left;
            int capHeight = m_recordData.bottom - m_recordData.top;
         
            if (capHeight <= 0 || capWidth <= 0)
            {
                Console.WriteLine(" size Error");
                return false;
            }
            
            Rectangle rectBuf = new System.Drawing.Rectangle(rect.left, rect.top,
                        capWidth, capHeight);
            bmp = screenBmp.Clone(rectBuf, screenBmp.PixelFormat);
            using (Mat mat = BitmapConverter.ToMat(bmp))
            {
                Cv2.Resize(mat, mat, new OpenCvSharp.Size(capWidth, capHeight));
                Cv2.Circle(mat, m_mousePos.X, m_mousePos.Y, 3, CursorColor, 3, LineTypes.AntiAlias);

                if (isStartRec == Define.ISRECSTART && writer != null && writer.IsOpened())
                {
                    // You can't record images without it !!
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2RGB);
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.RGB2BGR);

                    writer.Write(mat);
                    RecordCnt++;
                }
                else if(isStartRec == Define.ISRECSTOP)
                {
                    if (writer != null && writer.IsOpened())
                    {
                        isStartRec = Define.ISRECSTANBY;
                        RecordCnt = 0;
                        writer.Release();
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
                Cv2.WaitKey(60);
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
