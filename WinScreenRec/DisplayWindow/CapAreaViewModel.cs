using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using Microsoft.Win32;
using Reactive.Bindings;
using System.Windows.Input;

namespace WinScreenRec
{
    class CapAreaViewModel : BindingBase
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static CapAreaModel m_CapAreaModel;

        public CapAreaViewModel()
        {
            m_CapAreaModel = new CapAreaModel();
        }

        private double _RectWidth = 0.0;
        public double RectWidth
        {
            get
            {
                return _RectWidth;
            }
            set
            {
                _RectWidth = value;
                OnPropertyChanged(nameof(RectWidth));
            }
        }

        private double _RectHeight = 0.0;
        public double RectHeight
        {
            get
            {
                return _RectHeight;
            }
            set
            {
                _RectHeight = value;
                OnPropertyChanged(nameof(RectHeight));
            }
        }

        private ReactiveCommand<Object> _MouseLeftBtnDwn = null;
        public ReactiveCommand<Object> MouseLeftBtnDwn 
        {
            get
            {
                if (_MouseLeftBtnDwn == null)
                {
                    _MouseLeftBtnDwn = new ReactiveCommand<Object>().WithSubscribe(obj => {
                        if (m_CapAreaModel.isStartRec != 1)
                        {
                            var ele = (IInputElement)obj;
                            var pos = Mouse.GetPosition(ele);
                            RectangleMargin = pos.X.ToString() + "," + pos.Y.ToString();
                            m_CapAreaModel.RectangleMargin = RectangleMargin;

                            m_CapAreaModel.IsMouseDown = true;
                            m_CapAreaModel.RectTop = pos.Y;
                            m_CapAreaModel.RectLeft = pos.X;
                        }
                    });
                }
                return _MouseLeftBtnDwn;
            }
        }

        private DelegateCommand _MouseLeftBtnUp = null;
        public DelegateCommand MouseLeftBtnUp
        {
            get
            {
                if (_MouseLeftBtnUp == null)
                {
                    _MouseLeftBtnUp = new DelegateCommand(ButtonUpFunc, CanExecute);
                }
                return _MouseLeftBtnUp;
            }
        }

        private void ButtonUpFunc()
        {
            m_CapAreaModel.IsMouseDown = false;
        }

        private System.Windows.Media.ImageSource _ImageArea = null;
        public System.Windows.Media.ImageSource ImageArea {
            get => _ImageArea;
            set
            {
                _ImageArea = value;
                OnPropertyChanged(nameof(ImageArea));
            }
        }

        private String _RectangleMargin;
        public String RectangleMargin
        {
            get
            {
                return _RectangleMargin;
            }
            set
            {
                _RectangleMargin = value;
                OnPropertyChanged(nameof(RectangleMargin));
            }
        }

        public void SetRectInformation(double rectHeight, double rectWidth, string rectMargin)
        {
            RectHeight = rectHeight;
            RectWidth = rectWidth;
            RectangleMargin = rectMargin;
        }

        private ReactiveCommand<Object> _MouseMoveCommand = null;
        public ReactiveCommand<Object> MouseMoveCommand
        {
            get
            {
                if (_MouseMoveCommand == null)
                {
                    _MouseMoveCommand = new ReactiveCommand<Object>().WithSubscribe(obj => {
                        var ele = (IInputElement)obj;
                        var pos = Mouse.GetPosition(ele);
                        m_CapAreaModel.MakePosition(pos, SetRectInformation);
                    });
                }

                return _MouseMoveCommand;
            }
        }


        private bool CanExecute()
        {
            return true;
        }
    }
}
