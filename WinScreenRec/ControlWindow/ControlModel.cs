using System.Threading;
using WinScreenRec.Reference;

namespace WinScreenRec.ControlWindow
{
    class ControlModel
    {
        AudioProcess m_AudioProcess;

        public ControlModel()
        {
            m_AudioProcess = new AudioProcess();

            Thread thread;
            thread = new Thread(new ThreadStart(() =>{ 
                CapAreaViewModel.m_CapAreaModel.CaptureMovieAsync();
            }));
            thread.Start();
        }

        /// <summary>
        /// API to set file paths to variables.
        /// </summary>
        /// <param name="filePath"></param>
        public void SetFilePath(string filePath)
        {
            CapAreaViewModel.m_CapAreaModel.SetFilePath(filePath);
        }

        /// <summary>
        /// Get is record start/stop flag.
        /// </summary>
        /// <returns></returns>
        public int CheckIsRecord()
        {
            return CapAreaViewModel.m_CapAreaModel.isStartRec;
        }

        /// <summary>
        /// Set start to recording flag
        /// </summary>
        public void StartRecord()
        {
            CapAreaViewModel.m_CapAreaModel.isStartRec = Define.ISRECSTART;
            m_AudioProcess.AudioRecProcessStart();
        }

        /// <summary>
        /// Set stop to record flag.
        /// </summary>
        public void StopRecord()
        {
            CapAreaViewModel.m_CapAreaModel.isStartRec = Define.ISRECSTOP;
            m_AudioProcess.AudioRecProcessStop();
        }

        /// <summary>
        /// Record Timer get function.
        /// </summary>
        /// <returns></returns>
        public string GetTimer()
        {
            return CapAreaViewModel.m_CapAreaModel.RecordTimer;
        }

        /// <summary>
        /// Get Timer count function.
        /// </summary>
        /// <returns></returns>
        public int GetTimeCounter()
        {
            return CapAreaViewModel.m_CapAreaModel.RecordCounter;
        }

        /// <summary>
        /// Control window is close.
        /// </summary>
        public void CloseControlWindow()
        {
            CapAreaViewModel.m_CapAreaModel.isStartRec = Define.ISRECSTOP;
            CapAreaViewModel.m_CapAreaModel.isStartPrev = false;
        }
    }
}
