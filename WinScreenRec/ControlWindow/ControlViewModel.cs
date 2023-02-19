using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows;
using WinScreenRec.Reference;

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

            RecordContent = "Record";
        }

        private String _RecordContent;
        public String RecordContent {
            get { return _RecordContent; }
            set
            {
                _RecordContent = value;
                OnPropertyChanged(nameof(RecordContent));
            }
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

        private string _EnableRecordMark = Define.ISWIDGETHIDDEN;
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

        private string _EnableRecordTime = Define.ISWIDGETHIDDEN;
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

        private bool _PrevieAreaEnable = true;
        public bool PrevieAreaEnable {
            get
            {
                return _PrevieAreaEnable;
            }
            set
            {
                _PrevieAreaEnable = value;
                OnPropertyChanged(nameof(PrevieAreaEnable));
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
            if (m_ControlModel.CheckIsRecord() != Define.ISRECSTART)
            {
                var dialog = new SaveFileDialog();
                dialog.Title = "Save File";
                dialog.Filter = "video file|*.mp4";
                if (dialog.ShowDialog() == true)
                {
                    PrevieAreaEnable = false;
                    EnableRecordTime = Define.ISWIDGETVISIBLE;
                    EnableRecordMark = Define.ISWIDGETVISIBLE;

                    IsCaputureAreaView = false;
                    m_CaptureAreaWindow.Hide();
                    m_ControlModel.StartRecord();
                    m_ControlModel.SetFilePath(dialog.FileName);
                    RecordContent = Define.ISRECORDINGCONTENT;

                    Console.WriteLine("Recoding start");
                }
            }
            else
            {
                PrevieAreaEnable = true;
                EnableRecordTime = Define.ISWIDGETHIDDEN;
                EnableRecordMark = Define.ISWIDGETHIDDEN;
                m_ControlModel.StopRecord();
                RecordContent = Define.ISRECSTANDBYCONTENT;
                
                Console.WriteLine("Recording stop");
                MessageBox.Show("Record Finish !!");
            }
        }

        public void GetRecordTimerAsync()
        {
            while (IsWindowClose)
            {
                TimerValue = m_ControlModel.GetTimer();
                Thread.Sleep(500);

                if (m_ControlModel.GetTimeCounter() > Define.MAXRECORDTIME * 10)
                {
                    RecordCaptureFunc();
                }
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
