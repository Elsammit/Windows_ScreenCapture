using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinScreenRec.ControlWindow
{
    class ControlViewModel : BindingBase
    {

        CaptureAreaWindow m_CaptureAreaWindow = new CaptureAreaWindow();
        bool IsCaputureAreaView = false;
        bool IsRecoding = false;

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
            IsRecoding = !IsRecoding;
            if (IsRecoding)
            {
                Console.WriteLine("Recoding start");
            }
            else
            {
                Console.WriteLine("Recording stop");
            }
        }

        private void CloseWindowFunc()
        {
            m_CaptureAreaWindow.Close();
        }

        private bool IsCmdTrue()
        {
            return true;
        }

    }
}
