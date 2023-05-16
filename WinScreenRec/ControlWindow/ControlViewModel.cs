using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using WinScreenRec.Reference;

namespace WinScreenRec.ControlWindow
{
    class MovieExtensions 
    {
        public string MovieExtension { get; set; }
    }


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

            ChangeAudioStatus();

            //MovieExtensions = new ObservableCollection<MovieExtensions>
            //{
            //    new MovieExtensions { MovieExtension = "AAA" }
            //}

        }

        /// <summary>
        /// /Record timer context.
        /// </summary>
        private String _RecordContent;
        public String RecordContent {
            get { return _RecordContent; }
            set
            {
                _RecordContent = value;
                OnPropertyChanged(nameof(RecordContent));
            }
        }

        /// <summary>
        /// Recording creation area display switching process.
        /// </summary>
        private DelegateCommand _SelectPreviewArea = null;
        public DelegateCommand SelectPreviewArea
        {
            get
            {
                if (_SelectPreviewArea == null)
                {
                    _SelectPreviewArea = new DelegateCommand(ViewPreviewAreaFunc, IsRecording);
                }
                return _SelectPreviewArea;
            }
        }

        /// <summary>
        /// Execute capture the area.
        /// </summary>
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

        /// <summary>
        /// Change record available or not.
        /// </summary>
        private DelegateCommand _OnAudioAvailable = null;
        public DelegateCommand OnAudioAvailable
        {
            get
            {
                if (_OnAudioAvailable == null)
                {
                    _OnAudioAvailable = new DelegateCommand(ChangeAudioStatus, IsAudioChgEna);
                }
                return _OnAudioAvailable;
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

        public bool ChangeColor { get; set; } = false;

        private SolidColorBrush _AudioEnaColor = 
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(0,0,0));
        public SolidColorBrush AudioEnaColor
        {
            get
            {
                return _AudioEnaColor;
            }
            set
            {
                _AudioEnaColor = value;
                OnPropertyChanged(nameof(AudioEnaColor));
            }
        }

        private string _AudioEnable = Define.ISAUDIOONMESSAGE;
        public string AudioEnable 
        {
            get
            {
                return _AudioEnable;
            }
            set
            {
                _AudioEnable = value;
                OnPropertyChanged(nameof(AudioEnable));
            }
        }

        private ObservableCollection<MovieExtensions> _movieExtLists;
        public ObservableCollection<MovieExtensions> movieExtLists
        {
            get { return _movieExtLists; }
            set
            {
                _movieExtLists = value;
                OnPropertyChanged(nameof(movieExtLists));
            }
        }



        private void ChangeAudioStatus()
        {
            ChangeColor = !ChangeColor;
            m_ControlModel.SetIsAudioON(ChangeColor);
            if (ChangeColor)
            {
                AudioEnable = Define.ISAUDIOONMESSAGE;
                AudioEnaColor = 
                    new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xff, 0, 0));
            }
            else
            {
                AudioEnable = Define.ISAUDIOOFFMESSAGE;
                AudioEnaColor = 
                    new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
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

        /// <summary>
        /// Recording time acquisition (asynchronous).
        /// </summary>
        public void GetRecordTimerAsync()
        {
            while (IsWindowClose)
            {
                // Get record time.
                TimerValue = m_ControlModel.GetTimer();

                // The recording time is displayed every second, 
                // so there is no need for a quick loop, so "wait" is inserted.
                Thread.Sleep(250);

                // Stops recording when the maximum recording time has elapsed.
                if (m_ControlModel.GetTimeCounter() > Define.MAXRECORDTIME * 10)
                {
                    RecordCaptureFunc();
                }
            }
        }

        /// <summary>
        /// Execution when screen is closed
        /// </summary>
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

        /// <summary>
        /// Record Flag.
        /// </summary>
        /// <returns></returns>
        private bool IsRecording()
        {
            if(m_ControlModel.CheckIsRecord() != Define.ISRECSTART)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsAudioChgEna()
        {
            bool ret = true;
            if (m_ControlModel.IsUsingAudioEna)
            {
                ret = IsRecording();
            }
            else
            {
                AudioEnable = Define.ISAUDIODISABLEMESSAGE;
                AudioEnaColor =
                    new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x5f, 0x5f, 0x5f));
                ret = false;
            }
            return ret;
        }

    }
}
