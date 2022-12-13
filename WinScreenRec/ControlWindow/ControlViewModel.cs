using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;

namespace WinScreenRec.ControlWindow
{
    class ControlViewModel : BindingBase
    {

        CaptureAreaWindow m_CaptureAreaWindow = new CaptureAreaWindow();
        ControlModel m_ControlModel = new ControlModel();
        bool IsCaputureAreaView = false;

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
            if (!m_ControlModel.CheckIsRecord())
            {
                var dialog = new SaveFileDialog();
                dialog.Title = "ファイルを保存";
                dialog.Filter = "動画ファイル|*.wmv";
                if (dialog.ShowDialog() == true)
                {
                    m_CaptureAreaWindow.Hide();
                    m_ControlModel.SetFilePath(dialog.FileName);
                    m_ControlModel.StartRecord();
                    Console.WriteLine("Recoding start");
                }
            }
            else
            {
                m_ControlModel.StopRecord();
                Console.WriteLine("Recording stop");
            }
        }

        private void CloseWindowFunc()
        {
            m_ControlModel.CloseControlWindow();
            m_CaptureAreaWindow.Close();
        }

        private bool IsCmdTrue()
        {
            return true;
        }

    }
}
