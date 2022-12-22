using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Runtime.InteropServices;

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

        VideoWriter writer = null;  // Video writer for recording
        string RecordFilePath = ""; // Record file path.
        RECT m_recordData = new RECT(); // Record area.          
        Bitmap bmp = null;

        /// <summary>
        /// Initialize video writer.
        /// </summary>
        /// <param name="rect">Cutting rectangle</param>
        public void InitVideoWriter(RECT rect)
        {
            int width = m_recordData.right - m_recordData.left;
            int height = m_recordData.bottom - m_recordData.top;
            writer = new VideoWriter(RecordFilePath, FourCC.WMV1, 10,
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
            InitVideoWriter(rect);
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
            //Mat mat = new Mat();

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

                if (isStartRec == 1 && writer.IsOpened())
                {
                    // You can't record images without it !!
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2RGB);
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.RGB2BGR);
                    writer.Write(mat);
                }
                else if(isStartRec == 2)
                {
                    if (writer != null && writer.IsOpened())
                    {
                        isStartRec = 0;
                        writer.Release();
                    }
                }
                //Cv2.ImShow("test", mat);
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
