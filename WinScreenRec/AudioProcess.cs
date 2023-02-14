using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinScreenRec.Reference;

namespace WinScreenRec
{
    class AudioProcess
    {
        private IWaveIn WaveIn;
        private WaveFileWriter Writer;

        public AudioProcess()
        {
        }
        
        public void AudioRecProcessStart() 
        {
            InitializeAudioRecParam();
            WaveIn.StartRecording(); 
        }

        public void AudioRecProcessStop() { WaveIn.StopRecording(); }

        private void InitializeAudioRecParam()
        {
            WaveIn = new WasapiLoopbackCapture();
            Writer = new WaveFileWriter(Define.TEMPAUDIOPATH, WaveIn.WaveFormat);
            WaveIn.DataAvailable += OnDataAvailable;
            WaveIn.RecordingStopped += OnRecordingStopped;
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (Writer != null)
            {
                Writer.Close();
                Writer = null;
            }
            if (WaveIn != null)
            {
                WaveIn.Dispose();
                WaveIn = null;
            }
            if (e.Exception != null)
            {
                throw e.Exception;
            }
        }


        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            Writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

    }
}
