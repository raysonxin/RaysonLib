//*******************************************************************
// 文件名（File Name）：		D3DImageSource
//
// 作者（Author）:			辛祥
//
// 日期（Create Time）:		2015/2/4 13:37:35
//
// 修改记录（Revision History）:
// 	    R1:
// 			修改作者：
//			修改日期：
//			修改理由：
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace AtiSafe.MediaLib.Display
{
    public class D3DImageSource : IDisposable
    {
        #region 属性
        /// <summary>
        /// 图像数据源
        /// </summary>
        public ImageSource ImageSource
        {
            get { return this._imageSource; }
        }
        #endregion

        #region 私有变量
        private D3DImage _imageSource;
        private Int32Rect _imageSourceRect;
        private IntPtr _hwnd;
        private Form _hiddenWindow;

        private object _renderLock;
        private bool _isVistaOrBetter;
        private int _adapterId;

        private Direct3D _direct3D;
        private DisplayMode _displayMode;
        private CreateFlags _createFlag;
        private SupportFormat _frameFormat;

        private Device _device;
        private Texture _texture;
        private Surface _textureSurface;
        private Surface _inputSurface;

        private int _yStride;
        private int _yHeight;
        private int _uvStride;
        private int _uvHeight;
        private int _ySize;
        private int _uvSize;
        #endregion

        #region 构造函数
        public D3DImageSource()
            : this(0)
        {

        }

        public D3DImageSource(int adapterId)
        {
            this._renderLock = new object();
            this._isVistaOrBetter = Utilities.IsVistaOrBetter();
            this._imageSource = new D3DImage();
            this._imageSource.IsFrontBufferAvailableChanged += ImageSource_IsFrontBufferAvailableChanged;

            this.InitializeD3D();

            this._hiddenWindow = new Form();
            this._hwnd = this._hiddenWindow.Handle;
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化Direct 3D
        /// </summary>
        /// <param name="adapterId">显示器编号</param>
        private void InitializeD3D(int adapterId = 0)
        {
            this._adapterId = adapterId;
            this._direct3D = this._isVistaOrBetter ? new Direct3DEx() : new Direct3D();
            this._displayMode = this._direct3D.GetAdapterDisplayMode(this._adapterId);
            Capabilities deviceCap = this._direct3D.GetDeviceCaps(this._adapterId, DeviceType.Hardware);
            this._createFlag = CreateFlags.Multithreaded;
            if ((int)deviceCap.VertexProcessingCaps != 0)
            {
                this._createFlag |= CreateFlags.HardwareVertexProcessing;
            }
            else
            {
                this._createFlag |= CreateFlags.SoftwareVertexProcessing;
            }
        }

        /// <summary>
        /// 使图像无效进而刷新图像
        /// </summary>
        private void InvalidateImage()
        {
            if (!this._imageSource.Dispatcher.CheckAccess())
            {
                this._imageSource.Dispatcher.Invoke((Action)(() => this.InvalidateImage()));
                return;
            }

            if (!this._imageSource.IsFrontBufferAvailable)
            {
                return;
            }

            try
            {
                this._imageSource.Lock();
                this._imageSource.AddDirtyRect(this._imageSourceRect);
                this._imageSource.Unlock();
            }
            catch (Exception)
            {
            }
        }

        private void ImageSource_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this._imageSource.IsFrontBufferAvailable && this._textureSurface != null)
            {
                this._imageSource.Lock();
                this._imageSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, this._textureSurface.ComPointer);
                this._imageSource.Unlock();
            }
        }

        private void CreateResource(Format format, int width, int height)
        {
            PresentParameters presentParams = this.GetPrensentParameters(width, height);
            this._device = this._isVistaOrBetter ?
                new DeviceEx((Direct3DEx)this._direct3D, this._adapterId, DeviceType.Hardware, this._hwnd, this._createFlag, presentParams) :
                new Device(this._direct3D, this._adapterId, DeviceType.Hardware, this._hwnd, this._createFlag, presentParams);

            //设置纹理
            this._texture = new Texture(this._device, width, height, 1, Usage.RenderTarget, this._displayMode.Format, Pool.Default);
            this._textureSurface = this._texture.GetSurfaceLevel(0);

            //设置离屏表面
            this._inputSurface = this._isVistaOrBetter ?
                Surface.CreateOffscreenPlainEx((DeviceEx)this._device, width, height, format, Pool.Default, Usage.None) :
                Surface.CreateOffscreenPlain(this._device, width, height, format, Pool.Default);

            this._device.ColorFill(this._inputSurface, Utilities.BlackColor);

            this.SetImageSourceBackBuffer();
        }

        /// <summary>
        /// 设置图像后备缓冲区
        /// </summary>
        private void SetImageSourceBackBuffer()
        {
            if (!this._imageSource.Dispatcher.CheckAccess())
            {
                this._imageSource.Dispatcher.Invoke((Action)(() => this.SetImageSourceBackBuffer()));
                return;
            }

            this._imageSource.Lock();
            this._imageSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, this._textureSurface.ComPointer);
            this._imageSource.Unlock();

            this._imageSourceRect = new Int32Rect(0, 0, this._imageSource.PixelWidth, this._imageSource.PixelHeight);
        }

        /// <summary>
        /// 创建显示参数
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private PresentParameters GetPrensentParameters(int width, int height)
        {
            PresentParameters presentParams = new PresentParameters();
            presentParams.PresentFlags = PresentFlags.Video | PresentFlags.OverlayYCbCr_BT709;
            presentParams.Windowed = true;
            presentParams.DeviceWindowHandle = this._hwnd;
            presentParams.BackBufferWidth = width == 0 ? 1 : width;
            presentParams.BackBufferHeight = height == 0 ? 1 : height;
            presentParams.SwapEffect = SwapEffect.Discard;
            presentParams.PresentationInterval = PresentInterval.Immediate;
            presentParams.BackBufferFormat = this._displayMode.Format;
            presentParams.BackBufferCount = 1;
            presentParams.EnableAutoDepthStencil = false;
            return presentParams;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        private void ReleaseResource()
        {
            this.SafeRelease(this._inputSurface);
            this.SafeRelease(this._texture);
            this.SafeRelease(this._textureSurface);
            this.SafeRelease(this._device);
        }

        /// <summary>
        /// 填充缓冲区
        /// </summary>
        /// <param name="bufferPtr"></param>
        private void FillBuffer(IntPtr bufferPtr)
        {
            if (this._inputSurface == null)
                return;

            DataRectangle rect = this._inputSurface.LockRectangle(LockFlags.None);
            IntPtr surfaceBufferPtr = rect.Data.DataPointer;

            switch (this._frameFormat)
            {
                case SupportFormat.YV12:
                    if (rect.Pitch == this._yStride)
                    {
                        Utilities.Memcpy(surfaceBufferPtr, bufferPtr, this._ySize + this._uvSize + this._uvSize);
                    }
                    else
                    {
                        IntPtr srcPtr = bufferPtr;
                        int yPitch = rect.Pitch;
                        for (int i = 0; i < this._yHeight; i++)
                        {
                            Utilities.Memcpy(surfaceBufferPtr, srcPtr, this._yStride);
                            surfaceBufferPtr += yPitch;
                            srcPtr += this._yStride;
                        }

                        int uvPitch = yPitch >> 1;
                        for (int i = 0; i < this._yHeight; i++)
                        {
                            Utilities.Memcpy(surfaceBufferPtr, srcPtr, this._uvStride);
                            surfaceBufferPtr += uvPitch;
                            srcPtr += this._uvStride;
                        }
                    }
                    break;
                case SupportFormat.NV12:
                    break;
                default:
                    break;
            }
            this._inputSurface.UnlockRectangle();
        }

        /// <summary>
        /// 拉伸图像
        /// </summary>
        private void StretchSurface()
        {
            this._device.ColorFill(this._textureSurface, Utilities.BlackColor);
            this._device.StretchRectangle(this._inputSurface, this._textureSurface, TextureFilter.Linear);
        }

        /// <summary>
        /// 创建场景
        /// </summary>
        private void CreateScene()
        {
            this._device.Clear(ClearFlags.Target, Utilities.BlackColor, 1.0f, 0);
            this._device.BeginScene();

            //this._device.SetTexture(0, this._texture);
            //this._device.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);

            this._device.EndScene();
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 创建显示相关的参数信息
        /// </summary>
        /// <param name="videoWidth"></param>
        /// <param name="videoHeight"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public bool SetupSurface(int videoWidth, int videoHeight, SupportFormat format)
        {
            Format d3dFormat = Utilities.ConvertToD3D(format);
            if (!this.CheckFormat(d3dFormat))
            {
                return false;
            }

            this._frameFormat = format;
            switch (format)
            {
                case SupportFormat.YV12:
                    this._yStride = videoWidth;
                    this._yHeight = videoHeight;
                    this._ySize = videoHeight * videoWidth;
                    this._uvStride = this._yStride >> 1;
                    this._uvHeight = this._yHeight >> 1;
                    this._uvSize = this._ySize >> 2;
                    break;
                case SupportFormat.NV12:
                    this._yStride = videoWidth;
                    this._yHeight = videoHeight;
                    this._ySize = videoHeight * videoWidth;
                    this._uvStride = this._yStride;
                    this._uvHeight = this._yHeight >> 1;
                    this._uvSize = this._ySize >> 1;
                    break;
                default:
                    break;
            }
            this.ReleaseResource();
            this.CreateResource(d3dFormat, videoWidth, videoHeight);
            return true;
        }

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="buffer"></param>
        public void Render(IntPtr buffer)
        {
            lock (this._renderLock)
            {
                this.FillBuffer(buffer);
                this.StretchSurface();
                //this.CreateScene();
            }
            this.InvalidateImage();
        }
        #endregion

        #region 辅助方法
        private bool CheckFormat(Format d3dFormat)
        {
            if (!this._direct3D.CheckDeviceFormat(this._adapterId, DeviceType.Hardware, this._displayMode.Format, Usage.None, ResourceType.Surface, d3dFormat))
            {
                return false;
            }
            return this._direct3D.CheckDeviceFormatConversion(this._adapterId, DeviceType.Hardware, d3dFormat, this._displayMode.Format);
        }
        #endregion

        #region Dispose成员
        private bool isDisposed = false;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                if (disposing)
                {
                    this.ReleaseResource();

                    this.SafeRelease(this._direct3D);
                    this.SafeRelease(this._hiddenWindow);
                }
            }
        }

        private void SafeRelease(IDisposable item)
        {
            try
            {
                if (item != null)
                {
                    item.Dispose();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
