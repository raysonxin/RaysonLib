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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AtiSafe.MediaLib.PlayerControl;

namespace AtiSafeMediaToolkitTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Unloaded += MainWindow_Unloaded;
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.player.Dispose();
        }

        MediaFilePlayer player = null;

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            player= new MediaFilePlayer();
            player.SetMediaSource(path);
            player.IsShowToolBar = true;
            layout.Children.Add(player);
        }

        private string path = AppDomain.CurrentDomain.BaseDirectory + @"0001.amf";

        private void Test()
        {
            try
            {
                //AmfFileReader reader = new AmfFileReader(path);
                //reader.AnalyzeAmfFile();
                //reader.CaptureSingleFrame();
                //thumb.Source = reader.ThumbImage;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Test();
            player.PlayMedia();
        }
    }
}
