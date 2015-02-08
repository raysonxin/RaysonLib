
/* ============================================================
*类名称：AmfMediaDecode
 *类描述：
 *创建人：raysonxin
 *创建时间：2014-07-02 11:58:45
 *修改人：
 *修改备注：
 *@version 1.0
 * ============================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtiSafe.MediaLib.MediaFile
{
    public class AmfMediaDecoder
    {
        /// <summary>
        /// 分析amf文件内容帧信息
        /// </summary>
        /// <param name="tags">帧索引</param>
        /// <returns></returns>
        public static List<FrameIndexInfo> AnalyzeFileFrame(Byte[] tags, ref UInt32 length)
        {
            try
            {
                Int32 count = tags.Length / 20;
                List<FrameIndexInfo> frames = new List<FrameIndexInfo>();
                for (Int32 i = 0; i < count; i++)
                {
                    var temp = new FrameIndexInfo
                         {
                             Id = i,
                             Offset = AmfTools.ConvertToInt32(tags, 12 + 20 * i),
                             Length = AmfTools.ConvertToInt32(tags, 16 + 20 * i) + 12,
                             Timestamp = AmfTools.ConvertToInt32(tags, 4 + 20 * i)
                         };
                    temp.Length += temp.Length % 2 == 1 ? (UInt32)1 : (UInt32)0;
                    frames.Add(temp);
                }

                //整理时间间隔
                for (int i = 0; i < frames.Count - 1; i++)
                {
                    frames[i + 1].Distance = frames[i + 1].Offset - frames[i].Offset - frames[i].Length;
                    if (frames[i + 1].Timestamp >= frames[i].Timestamp)
                    {
                        length += frames[i + 1].Timestamp - frames[i].Timestamp;
                        frames[i].Timestamp = frames[i + 1].Timestamp - frames[i].Timestamp;                      
                    }
                    else
                    {
                        length += 1000000 - frames[i].Timestamp + frames[i + 1].Timestamp;
                        UInt32 t = (1000000 - frames[i].Timestamp);
                        t += frames[i + 1].Timestamp;
                        frames[i].Timestamp = t;
                    }
                    frames[i].Timestamp = frames[i].Timestamp < 1 ? 33 : frames[i].Timestamp;
                    frames[i].Progress = length;
                }

                //处理尾帧
                frames[frames.Count - 1].Timestamp = 0;
                frames[frames.Count - 1].Progress = length;
                return frames;
            }
            catch (Exception ex)
            {
                throw new FormatException("网络文件格式不正确。" + ex.ToString());
            }
        }

        /// <summary>
        /// 分析amf文件头部
        /// </summary>
        /// <param name="header">文件头字符数组</param>
        /// <returns></returns>
        public static AmfHeadInfo AnalyzeFileHeader(Byte[] header)
        {
            try
            {
                if (header.Length != 76)
                    return null;

                System.IO.MemoryStream ms = new System.IO.MemoryStream(header);
                var temp = new AmfHeadInfo
                {
                    Format = AmfTools.Byte2String(ms.ReadBytesExtend(0, 4)),
                    FileSize = AmfTools.ConvertToInt32(header, 4) + 8,
                    AudioFlag = AmfTools.SubArray(header, 8, 4),
                    AudioHeadSize = AmfTools.ConvertToInt32(header, 12),
                    AudioCodec = new Char4 { Value = AmfTools.Byte2String(ms.ReadBytesExtend(16, 4)) },//AmfTools.SubArray(header,16,4)},
                    AudioBitRate = AmfTools.ConvertToInt32(header, 20),
                    ChannelCount = AmfTools.ConvertToInt32(header, 24),
                    SamplingRate = AmfTools.ConvertToInt32(header, 28),
                    SamplingTime = AmfTools.ConvertToInt32(header, 32),
                    AudioFrameCount = AmfTools.ConvertToInt32(header, 36),
                    VideoFlag = AmfTools.SubArray(header, 40, 4),
                    VideoHeadSize = AmfTools.ConvertToInt32(header, 44),
                    VideoCodec = new Char4 { Value = AmfTools.Byte2String(ms.ReadBytesExtend(48, 4)) },//AmfTools.SubArray(header, 48, 4),
                    Width = AmfTools.ConvertToInt32(header, 52),
                    Height = AmfTools.ConvertToInt32(header, 56),
                    VideoFrameRate = AmfTools.ConvertToInt32(header, 60),
                    VideoFrameCount = AmfTools.ConvertToInt32(header, 64),
                    DataBlockFlag = AmfTools.SubArray(header, 68, 4),
                    DataSize = AmfTools.ConvertToInt32(header, 72)
                };
                ms.Close();
                return temp;
            }
            catch 
            {
                throw;
            }
        }
    }
}
