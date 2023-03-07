using NAudio.Wave;
using System;
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

            //int silenceDurationInSeconds = 10; // 無音の長さを10秒に設定する例
            //int bytesPerSample = WaveIn.WaveFormat.BitsPerSample / 8;
            //int sampleRate = WaveIn.WaveFormat.SampleRate;
            //int channels = WaveIn.WaveFormat.Channels;

            //var buffer = new byte[sampleRate * channels * bytesPerSample * silenceDurationInSeconds];
            //for (int i = 0; i < buffer.Length; i++)
            //{
            //    buffer[i] = 0; // 無音データの値を0に設定する
            //}

            WaveIn.StartRecording();

            //Writer.Write(buffer, 0, buffer.Length);
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
                var silenceBuffer = new byte[Writer.WaveFormat.AverageBytesPerSecond];
                Writer.Write(silenceBuffer, 0, silenceBuffer.Length);

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
            int silenceDurationInSeconds = 0; // 無音の長さを10秒に設定する例
            int bytesPerSample = WaveIn.WaveFormat.BitsPerSample / 8;
            int sampleRate = WaveIn.WaveFormat.SampleRate;
            int channels = WaveIn.WaveFormat.Channels;
            int silenceDuration = (sampleRate * channels * bytesPerSample) / 10;

            Console.WriteLine("Buffer:{0}, BytesRecorded:{1}", e.Buffer, e.BytesRecorded);
            if(e.BytesRecorded <= 0)
            {
                var buffer = new byte[silenceDuration];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = 0; // 無音データの値を0に設定する
                }
                Writer.Write(buffer, 0, 10000);
            }
            else
            {

                Writer.Write(e.Buffer, 0, e.BytesRecorded);
            }


        }

    }
}
