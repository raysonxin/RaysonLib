using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using AtiSafe.MediaLib.Display;
using AtiSafe.MediaLib.MediaFile;

namespace AtiSafe.MediaLib.PlayerControl
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class MediaFilePlayer : UserControl, IDisposable
    {
        #region 构造方法

        public MediaFilePlayer()
        {
            InitializeComponent();
            this.Loaded += MediaFilePlayer_Loaded;
            //this.Unloaded += MediaFilePlayer_Unloaded;
        }

        //public MediaFilePlayer(string amfFilePath)
        //{

        //}
        #endregion

        #region Dependency Property
        /// <summary>
        /// 是否显示工具栏
        /// </summary>
        public bool IsShowToolBar
        {
            get { return (bool)GetValue(IsShowToolBarProperty); }
            set { SetValue(IsShowToolBarProperty, value); }
        }

        /// <summary>
        /// 是否显示工具栏的以来属性
        /// </summary>
        public static readonly DependencyProperty IsShowToolBarProperty =
            DependencyProperty.Register("IsShowToolBar", typeof(bool), typeof(MediaFilePlayer), new PropertyMetadata(true, IsShowToolBarChanged));

        /// <summary>
        /// 工具栏显示控制
        /// </summary>
        private static void IsShowToolBarChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = obj as MediaFilePlayer;
            if (control == null)
                return;
            if (control.IsShowToolBar)
            {
                control.toolBar.Visibility = Visibility.Visible;
            }
            else
            {
                control.toolBar.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region 私有成员
        /// <summary>
        /// 图像显示数据源
        /// </summary>
        private D3DImageSource _d3dSource = null;

        /// <summary>
        /// 播放或暂停控制对象
        /// </summary>
        private ManualResetEvent _playSet = null;

        /// <summary>
        /// 视频文件读取对象
        /// </summary>
        private AmfFileReader _reader = null;

        /// <summary>
        /// 视频文件路径
        /// </summary>
        private string _filePath = string.Empty;

        /// <summary>
        /// 当前帧索引
        /// </summary>
        private int _frameIndex = 0;

        /// <summary>
        /// 视频帧数量
        /// </summary>
        private int _frameCount = 0;

        /// <summary>
        /// 解码对象
        /// </summary>
        private UIntPtr _decoder;

        /// <summary>
        /// 图像大小：高
        /// </summary>
        private int _videoHeight;

        /// <summary>
        /// 图像大小：宽
        /// </summary>
        private int _videoWidth;

        /// <summary>
        /// 图像大小：yuv size
        /// </summary>
        private int _yuvSize;

        /// <summary>
        /// 视频播放进度
        /// </summary>
        private int _videoProgress = 0;

        /// <summary>
        /// 视频时间长度
        /// </summary>
        private int _videoLength;

        /// <summary>
        /// 是否正在播放
        /// </summary>
        private bool _isPlay = false;

        /// <summary>
        /// 原始父级元素
        /// </summary>
        private DependencyObject _parent = null;

        /// <summary>
        /// 是否全屏状态
        /// </summary>
        private bool _isFullScreen = false;

        /// <summary>
        /// 全屏窗口容器
        /// </summary>
        private FullWindow _winContainer = null;
        #endregion

        #region 公有成员
        /// <summary>
        /// 视频文件路径
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            private set { _filePath = value; }
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 控件加载方法
        /// </summary>
        void MediaFilePlayer_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Loaded -= MediaFilePlayer_Loaded;
                _playSet = new ManualResetEvent(true);

                this._d3dSource = new D3DImageSource();
                if (this._d3dSource.SetupSurface(1920, 1080, SupportFormat.YV12))
                {
                    this.renderControl.Source = this._d3dSource.ImageSource;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 控件卸载方法
        /// </summary>
        void MediaFilePlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("卸载控件时发生异常", ex);
            }
        }

        /// <summary>
        /// 开始播放媒体文件
        /// </summary>
        private void StartPlay()
        {
            try
            {
                byte[] buffer = null;
                while (_frameIndex < _frameCount)
                {
                    _playSet.WaitOne();

                    //读取jpeg字节流
                    buffer = _reader.ReadFrame(_frameIndex);
                    GCHandle hObject = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    IntPtr pObject = hObject.AddrOfPinnedObject();

                    //jpeg->yuv
                    var pt = AmfDecoder.JpegDecode(_decoder, pObject, buffer.Length, ref _yuvSize, ref _videoWidth, ref _videoHeight);

                    //显示
                    this.Dispatcher.Invoke(() =>
                    {
                        this._d3dSource.Render(pt);
                        //this._videoProgress++;
                        this.sliderProgress.Value = this._frameIndex;
                        this.txtProgress.Text = AmfTools.Int32ToTimeString((int)this._reader.Frames[this._frameIndex].Progress) + "/"
                            + AmfTools.Int32ToTimeString((Int32)this._videoLength);
                    });

                    //播放间隔
                    System.Threading.Thread.Sleep((int)_reader.Frames[_frameIndex].Timestamp);

                    //内存释放
                    if (hObject.IsAllocated)
                        hObject.Free();

                    //准备下一帧
                    _frameIndex++;
                }

                //准备重播
                _isPlay = false;
                this.Dispatcher.Invoke(() =>
                {
                    _frameIndex = 0;
                    this.sliderProgress.Value = this._frameIndex;
                    ckbPlay.IsChecked = false;
                });
                _playSet.Reset();
            }
            catch (Exception ex)
            {
                throw new Exception("播放视频时异常", ex);
            }
        }

        /// <summary>
        /// 初始化播放控件
        /// </summary>
        /// <param name="_path">文件路径</param>
        private void InitializeMedia()
        {
            try
            {
                _reader = new AmfFileReader(_filePath);
                _reader.AnalyzeAmfFile();

                this._videoLength = (int)_reader.VideoLength;
                this.txtProgress.Text = AmfTools.Int32ToTimeString(this._videoProgress) + "/" + AmfTools.Int32ToTimeString((Int32)this._videoLength);
                this.sliderProgress.Maximum = this._reader.Frames.Count;

                _frameIndex = 0;
                _frameCount = _reader.Frames.Count;

                //创建解码对象
                _decoder = AmfDecoder.CreateJpegDecoder();
            }
            catch (Exception ex)
            {
                throw new Exception("初始化播放器及媒体文件时发生异常", ex);
            }
        }

        /// <summary>
        /// 播放按钮操作
        /// </summary>
        private void ckbPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //false->Paused,true->Playing
                bool flag = ((CheckBox)sender).IsChecked == true ? true : false;
                if (flag)
                {
                    this._playSet.Set();
                    if (!_isPlay)
                    {
                        PlayMedia();
                        _isPlay = true;
                    }
                }
                else
                {
                    this._playSet.Reset();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("播放或暂停时发生异常", ex);
            }
        }



        #endregion

        #region 公有方法
        /// <summary>
        /// 播放视频文件
        /// <param name="_path">媒体文件路径</param>
        /// </summary>
        public void PlayMedia()
        {
            try
            {
                //开始播放
                Task.Run(() =>
                {
                    StartPlay();
                });
            }
            catch (Exception ex)
            {
                throw new Exception("exceptions occured when prepare to play media file", ex);
            }
        }

        /// <summary>
        /// 设置播放文件信息
        /// </summary>
        /// <param name="_path">媒体文件路径</param>
        public void SetMediaSource(string _path)
        {
            try
            {
                if (string.IsNullOrEmpty(_path))
                    throw new ArgumentException("media file path can not be null");
                if (!File.Exists(_path))
                    throw new FileNotFoundException("media file is not found");

                this._filePath = _path;
                this.InitializeMedia();
            }
            catch (Exception ex)
            {
                throw new Exception("设置播放文件时发生异常", ex);
            }
        }
        #endregion

        #region IDisposable成员
        /// <summary>
        /// 资源释放标志
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    _reader.Dispose();
                    _d3dSource.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("释放本地视频控件时发生异常", ex);
            }
            finally
            {
                if (_decoder != UIntPtr.Zero)
                {
                    AmfDecoder.DestroyJpegDecoder(_decoder);
                    _decoder = UIntPtr.Zero;
                }
            }
        }
        #endregion

        private void sliderProgress_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _playSet.Reset();
                _frameIndex = (int)sliderProgress.Value;
                _playSet.Set();
            }
        }

        private void btnFull_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isFullScreen)
                {
                    var win = Window.GetWindow(this);
                    _parent = this.Parent;
                    (this.Parent as Panel).Children.Remove(this);
                    win.WindowState = WindowState.Minimized;

                    _winContainer = new FullWindow();
                    _winContainer.AddFullScreenChild(this);
                    _winContainer.GoFullscreen();
                    _winContainer.Show();
                }
                else
                {
                    if (_winContainer == null)
                        return;

                    _winContainer.ExitFullscreen();
                    _winContainer.Layout.Children.Remove(this);
                    _winContainer.Close();

                    (this._parent as Panel).Children.Add(this);
                    Window.GetWindow(this._parent).WindowState = WindowState.Normal;
                }
                _isFullScreen = !_isFullScreen;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
