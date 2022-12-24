using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinScreenRec.ControlWindow
{
    class ControlModel
    {

        public ControlModel()
        {
            Thread thread;
            thread = new Thread(new ThreadStart(() =>{ 
                CapAreaViewModel.m_CapAreaModel.CaptureMovieAsync();
            }));
            thread.Start();
        }

        public void SetFilePath(string filePath)
        {
            CapAreaViewModel.m_CapAreaModel.SetFilePath(filePath);
        }

        public int CheckIsRecord()
        {
            return CapAreaViewModel.m_CapAreaModel.isStartRec;
        }

        public void StartRecord()
        {
            CapAreaViewModel.m_CapAreaModel.isStartRec = 1;
        }

        public void StopRecord()
        {
            CapAreaViewModel.m_CapAreaModel.isStartRec = 2;
        }

        public string GetTimer()
        {
            return CapAreaViewModel.m_CapAreaModel.RecordTimer;
        }

        public void CloseControlWindow()
        {
            CapAreaViewModel.m_CapAreaModel.isStartRec = 2;
            CapAreaViewModel.m_CapAreaModel.isStartPrev = false;
        }
    }
}
