using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AtiSafe.MediaLib.PlayerControl
{
    /// <summary>
    /// FullWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FullWindow : Window
    {
        public FullWindow()
        {
            InitializeComponent();

            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.Topmost = true;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        }

        public void AddFullScreenChild(UserControl uc)
        {
            this.Layout.Children.Add(uc);
        }
    }
}
