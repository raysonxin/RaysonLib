using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AtiSafe.MediaLib.Display;

namespace AtiSafeMediaToolkitTest
{
    /// <summary>
    /// RtspTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RtspTestWindow : Window
    {
        public RtspTestWindow()
        {
            InitializeComponent();
            this.Loaded += RtspTestWindow_Loaded;
            this.Closing += RtspTestWindow_Closing;
        }

        void RtspTestWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //  CloseVideo();
        }

        void RtspTestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.d3dSource = new D3DImageSource();

                if (this.d3dSource.SetupSurface(1920, 1080, SupportFormat.YV12))
                {
                    this.imageD3D.Source = this.d3dSource.ImageSource;
                }
                callBack = new RtspWrapper.VideoStreamComeDelegate(OnVideoStreamComing);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

        }

        D3DImageSource d3dSource = null;

        // public delegate void VideoStreamComeDelegate(byte[] buffer, int len, int width, int height);

        private void OnVideoStreamComing(IntPtr pointer, int len, int width, int height)
        {
            try
            {
                //byte[] bytes = new byte[len];
                //Marshal.Copy(pointer, bytes, 0, bytes.Length);

                //IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
                //System.Diagnostics.Debug.WriteLine("开始渲染。。。");
                //RtspWrapper.OutputDebugString("开始渲染");
                System.Threading.Thread.Sleep(1);
                this.d3dSource.Render(pointer);
                //RtspWrapper.OutputDebugString("渲染完成");
                //System.Diagnostics.Debug.WriteLine("渲染完成。。。");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        IntPtr hwnd = IntPtr.Zero;
        RtspWrapper.VideoStreamComeDelegate callBack = null;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = "rtsp://192.168.1.198:554/live1.sdp";
                hwnd = RtspWrapper.CreateRtspClientSession(url, callBack);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }


        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            CloseVideo();
        }

        private void CloseVideo()
        {
            if (hwnd != IntPtr.Zero)
            {
                RtspWrapper.DestroyRtspClientSession(hwnd);
                hwnd = IntPtr.Zero;
                callBack = null;
            }
        }
    }
}
