using System.Windows;

namespace WinScreenRec
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CaptureAreaWindow : Window
    {
        public CaptureAreaWindow()
        {

            InitializeComponent();

            this.DataContext = new CapAreaViewModel();
        }
    }
}