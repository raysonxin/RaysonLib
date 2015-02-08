//*******************************************************************
// 文件名（File Name）：		AmfFileReader
//
// 作者（Author）:			辛祥
//
// 日期（Create Time）:		2015/2/3 11:19:54
//
// 修改记录（Revision History）:
// 	    R1:
// 			修改作者：
//			修改日期：
//			修改理由：
//*******************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AtiSafe.MediaLib.MediaFile
{
    /// <summary>
    /// AMF媒体文件读取类
    /// </summary>
    public class AmfFileReader : IDisposable
    {
        #region Properties & Fields
        /// <summary>
        /// amf媒体文件路径
        /// </summary>
        private string _filePath = string.Empty;

        /// <summary>
        /// 文件读取工具
        /// </summary>
        private FileStream _reader = null;

        /// <summary>
        /// amf文件头大小
        /// </summary>
        private const int _amfHeaderSize = 76;

        /// <summary>
        /// 缓冲区
        /// </summary>
        private byte[] _buffer = null;


        private AmfHeadInfo _headerInfo;
        /// <summary>
        /// 文件头信息
        /// </summary>
        public AmfHeadInfo HeaderInfo
        {
            get { return _headerInfo; }
            set { _headerInfo = value; }
        }

        private List<FrameIndexInfo> _frames;
        /// <summary>
        /// amf文件帧信息
        /// </summary>
        public List<FrameIndexInfo> Frames
        {
            get { return _frames; }
            set { _frames = value; }
        }

        private UInt32 _videoLength;
        /// <summary>
        /// 视频长度
        /// </summary>
        public UInt32 VideoLength
        {
            get { return _videoLength; }
            set { _videoLength = value; }
        }

        private BitmapImage _thumbImage;
        /// <summary>
        /// 视频缩略图
        /// </summary>
        public BitmapImage ThumbImage
        {
            get { return _thumbImage; }
            set { _thumbImage = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_file">amf文件路径</param>
        public AmfFileReader(string _file)
        {
            /*参数检查*/
            if (string.IsNullOrEmpty(_file))
                throw new ArgumentNullException("文件路径不能为空！");
            if (!File.Exists(_file))
                throw new ArgumentException("指定的文件不存在！");

            /*成员变量初始化*/
            this._filePath = _file;
            this._frames = new List<FrameIndexInfo>();
        }

        /// <summary>
        /// 分析amf文件
        /// </summary>
        public void AnalyzeAmfFile()
        {
            try
            {
                //初始化文件读取对象
                _reader = new FileStream(_filePath, FileMode.Open, FileAccess.Read);

                //读取并分析文件头
                _buffer = new byte[_amfHeaderSize];
                int res = _reader.Read(_buffer, 0, _amfHeaderSize);
                if (res != _amfHeaderSize)
                    throw new FileFormatException("amf文件格式不正确");
                this._headerInfo = AmfMediaDecoder.AnalyzeFileHeader(_buffer);

                //读取并分析文件帧信息
                UInt32 tagOffset = _headerInfo.DataSize + 76 + 8;
                UInt32 tagLength = _headerInfo.FileSize - tagOffset;
                _buffer = new byte[tagLength];
                _reader.Position = tagOffset;
                res = _reader.Read(_buffer, 0, (int)tagLength);
                if (res != (int)tagLength)
                    throw new FileFormatException("amf文件格式不正确");
                this._frames = AmfMediaDecoder.AnalyzeFileFrame(_buffer, ref _videoLength);
            }
            catch (Exception ex)
            {
                throw new Exception("解析amf文件异常", ex);
            }
        }

        /// <summary>
        /// 抓取视频帧作为缩略图
        /// </summary>
        public void CaptureSingleFrame()
        {
            try
            {
                if (_frames == null || _frames.Count == 0)
                    throw new Exception("当前没有可以提取的视频帧，可能还未对视频进行分析");

                //随机选取一帧图像
                int total = _frames.Count;
                Random rand = new Random();
                int index = rand.Next(0, total);
                var jpg = ReadFrame(index);

                //生成ImageSource对象
                _thumbImage = new BitmapImage();
                _thumbImage.BeginInit();
                _thumbImage.CacheOption = BitmapCacheOption.OnLoad;
                _thumbImage.StreamSource = new MemoryStream(jpg);
                _thumbImage.EndInit();
                _thumbImage.Freeze();
            }
            catch (Exception ex)
            {
                throw new Exception("获取视频帧缩略图异常", ex);
            }
        }

        /// <summary>
        /// 按照帧索引读取视频帧
        /// </summary>
        /// <param name="frameIndex">视频帧</param>
        /// <returns></returns>
        public byte[] ReadFrame(int frameIndex)
        {
            try
            {
                if (frameIndex < 0 || frameIndex >= _frames.Count)
                    throw new Exception("帧序号不正确");

                FrameIndexInfo frame = _frames[frameIndex];
                //根据图像位置信息进行字节流提取
                _reader.Position = frame.Offset;
                _buffer = new byte[frame.Length];
                int res = _reader.Read(_buffer, 0, (int)frame.Length);
                if (res != frame.Length)
                    throw new Exception("提取视频帧时发生异常");

                //生成图像
                var jpg = _buffer.SubArray(12, _buffer.Length - 12);
                return jpg;
            }
            catch (Exception ex)
            {
                throw new Exception("读取视频帧数据异常", ex);
            }
        }
        #endregion

        #region IDisposable 实现
        /// <summary>
        /// 释放申请的资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                _headerInfo = null;
                _frames.Clear();
                if (_reader != null)
                {
                    _reader.Close();
                    _reader = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        #endregion
    }
}
