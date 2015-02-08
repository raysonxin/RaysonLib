/* ============================================================
*类名称：AmfHeaderInfo
 *类描述：
 *创建人：raysonxin
 *创建时间：2014-08-15 10:41:59
 *修改人：
 *修改备注：
 *@version 1.0
 * ============================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AtiSafe.MediaLib.MediaFile
{
    /// <summary>
    /// amf媒体文件头结构
    /// </summary>
    public class AmfHeadInfo
    {
        /// <summary>
        /// 文件格式标识
        /// </summary>
        public String Format { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public UInt32 FileSize { get; set; }

        /// <summary>
        /// 音频标识
        /// </summary>
        public Byte[] AudioFlag { get; set; }

        /// <summary>
        /// 音频头大小
        /// </summary>
        public UInt32 AudioHeadSize { get; set; }

        /// <summary>
        /// 编码类型
        /// </summary>       
        public Char4 AudioCodec;

        /// <summary>
        /// 比特率
        /// </summary>
        public UInt32 AudioBitRate { get; set; }

        /// <summary>
        /// 声道数
        /// </summary>
        public UInt32 ChannelCount { get; set; }

        /// <summary>
        /// 采样率
        /// </summary>
        public UInt32 SamplingRate { get; set; }

        /// <summary>
        /// 单帧采样时间
        /// </summary>
        public UInt32 SamplingTime { get; set; }

        /// <summary>
        /// 音频帧总数
        /// </summary>
        public UInt32 AudioFrameCount { get; set; }

        /// <summary>
        /// 视频头
        /// </summary>
        public Byte[] VideoFlag { get; set; }

        /// <summary>
        /// 视频头长度
        /// </summary>
        public UInt32 VideoHeadSize { get; set; }

        /// <summary>
        /// 视频编码类型
        /// </summary>
        public Char4 VideoCodec { get; set; }



        /// <summary>
        /// 宽度，待定
        /// </summary>
        public UInt32 Width { get; set; }

        /// <summary>
        /// 高度，待定
        /// </summary>
        public UInt32 Height { get; set; }

        /// <summary>
        /// 帧率
        /// </summary>
        public UInt32 VideoFrameRate { get; set; }

        /// <summary>
        /// 视频帧总数
        /// </summary>
        public UInt32 VideoFrameCount { get; set; }

        /// <summary>
        /// 数据块唯一标识
        /// </summary>
        public Byte[] DataBlockFlag { get; set; }

        /// <summary>
        /// 数据块大小
        /// </summary>
        public UInt32 DataSize { get; set; }
    }

    public struct Char4
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public String Value;
    };
}
