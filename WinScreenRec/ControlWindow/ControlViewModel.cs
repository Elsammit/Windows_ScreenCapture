﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Threading;

namespace WinScreenRec.ControlWindow
{
    class ControlViewModel : BindingBase
    {

        CaptureAreaWindow m_CaptureAreaWindow = new CaptureAreaWindow();
        ControlModel m_ControlModel = new ControlModel();
        bool IsCaputureAreaView = false;
        bool IsWindowClose = true;

        public ControlViewModel()
        {
            Thread recordThread;
            recordThread = new Thread(new ThreadStart(() => {
                GetRecordTimerAsync();
            }));
            recordThread.Start();
        }

        private DelegateCommand _SelectPreviewArea = null;
        public DelegateCommand SelectPreviewArea 
        {
            get
            {
                if(_SelectPreviewArea == null)
                {
                    _SelectPreviewArea = new DelegateCommand(ViewPreviewAreaFunc, IsCmdTrue);
                }
                return _SelectPreviewArea;
            }
        }

        private DelegateCommand _RecordCapture = null;
        public DelegateCommand RecordCapture 
        {
            get
            {
                if (_RecordCapture == null)
                {
                    _RecordCapture = new DelegateCommand(RecordCaptureFunc, IsCmdTrue);
                }
                return _RecordCapture;
            }
        }

        private DelegateCommand _ClickCloseWindow = null;

        public DelegateCommand ClickCloseWindow
        {
            get
            {
                if (_ClickCloseWindow == null)
                {
                    _ClickCloseWindow = new DelegateCommand(CloseWindowFunc, IsCmdTrue);
                }
                return _ClickCloseWindow;
            }
        }

        private string _EnableRecordMark = "Hidden";
        public string EnableRecordMark {
            get
            {
                return _EnableRecordMark;
            }
            set
            {
                _EnableRecordMark = value;
                OnPropertyChanged(nameof(EnableRecordMark));
            }
        }

        private string _EnableRecordTime = "Hidden";
        public string EnableRecordTime {
            get
            {
                return _EnableRecordTime;
            }
            set
            {
                _EnableRecordTime = value;
                OnPropertyChanged(nameof(EnableRecordTime));
            }
        }

        private string _TimerValue = "00:00";
        public string TimerValue {
            get
            {
                return _TimerValue;
            }
            set
            {
                _TimerValue = value;
                OnPropertyChanged(nameof(TimerValue));
            }
        }

        private void ViewPreviewAreaFunc()
        {
            IsCaputureAreaView = !IsCaputureAreaView;
            if (IsCaputureAreaView)
            {

                m_CaptureAreaWindow.Show();
            }
            else
            {

                m_CaptureAreaWindow.Hide();
            }
        }

        private void RecordCaptureFunc()
        {
            if (m_ControlModel.CheckIsRecord() != 1)
            {
                var dialog = new SaveFileDialog();
                dialog.Title = "ファイルを保存";
                dialog.Filter = "動画ファイル|*.mp4";
                if (dialog.ShowDialog() == true)
                {
                    EnableRecordTime = "Visible";
                    EnableRecordMark = "Visible";
                    m_CaptureAreaWindow.Hide();
                    m_ControlModel.StartRecord();
                    m_ControlModel.SetFilePath(dialog.FileName);
                    Console.WriteLine("Recoding start");
                }
            }
            else
            {
                EnableRecordTime = "Hidden";
                EnableRecordMark = "Hidden";
                m_ControlModel.StopRecord();
                Console.WriteLine("Recording stop");
            }
        }

        public void GetRecordTimerAsync()
        {
            while (IsWindowClose)
            {
                TimerValue = m_ControlModel.GetTimer();
                Thread.Sleep(500);
            }
        }

        private void CloseWindowFunc()
        {
            IsWindowClose = false;
            m_ControlModel.CloseControlWindow();
            m_CaptureAreaWindow.Close();
        }

        private bool IsCmdTrue()
        {
            return true;
        }

    }
}
