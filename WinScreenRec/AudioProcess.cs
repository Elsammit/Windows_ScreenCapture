﻿using NAudio.Wave;
using System;
using WinScreenRec.Reference;
using System.Media;
using System.Diagnostics;

namespace WinScreenRec
{
    class AudioProcess
    {
        private IWaveIn WaveIn;
        private WaveFileWriter Writer;

        Stopwatch stopwatch = new Stopwatch();

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
            int bytesPerSample = WaveIn.WaveFormat.BitsPerSample / 8;
            int sampleRate = WaveIn.WaveFormat.SampleRate;
            int channels = WaveIn.WaveFormat.Channels;
            int silenceDuration = (sampleRate * channels * bytesPerSample) * 60 / 1000;

            stopwatch.Stop();
            if (e.BytesRecorded <= 0)
            {
                var buffer = new byte[silenceDuration];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = 0; // 無音データの値を0に設定する
                }
                Writer.Write(buffer, 0, buffer.Length);
            }
            else
            {

                Writer.Write(e.Buffer, 0, e.BytesRecorded);
            }
            stopwatch.Reset();
            stopwatch.Start();

        }

    }
}
