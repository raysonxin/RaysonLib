/* ============================================================
*类名称：DecodingHelper
 *类描述：
 *创建人：raysonxin
 *创建时间：2014-08-14 14:40:11
 *修改人：
 *修改备注：
 *@version 1.0
 * ============================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace AtiSafe.MediaLib.MediaFile
{
    public static class AmfTools
    {
        /// <summary>
        /// 将字节数组转成字符串
        /// </summary>
        /// <param name="bts">字节数组</param>
        /// <returns></returns>
        public static String Byte2String(Byte[] bts)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in bts)
            {
                sb.Append(Convert.ToChar(item));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 扩展方法：读取指定数量的字节，并向前推进读取指针
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Byte[] ReadBytesExtend(this System.IO.MemoryStream ms,Int32 start, Int32 count)
        {
            Byte[] bts = new Byte[count];
            ms.Position = start;
            Int32 read = ms.Read(bts, 0, count);
            //ms.Position += read;
            return bts;
        }

        /// <summary>
        /// 扩展方法：数组截取
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Byte[] SubArray(this System.Array array, Int32 start, Int32 length)
        {
            Byte[] bts = new Byte[length];
            Buffer.BlockCopy(array, start, bts, 0, length);
            return bts;
        }

        /// <summary>
        /// 4字节数组转成整形
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <param name="startIndex">开始位置</param>
        /// <returns></returns>
        public static UInt32 ConvertToInt32(Byte[] b, Int32 startIndex)
        {
            try
            {
                UInt32 output = 0;
                Byte current;
                for (Int32 i = 0; i < 4; i++)
                {
                    current = b[i + startIndex];
                    output += (UInt32)(current & 0xFF) << (8 * (3 - i));
                }
                return output;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 把整数转换成分秒字符串
        /// </summary>
        /// <param name="timespan">时间间隔，毫秒数</param>
        /// <returns></returns>
        public static String Int32ToTimeString(Int32 timespan)
        {
            Int32 total = timespan / 1000;
            Int32 hh = total / 60 / 60;
            Int32 mm = total / 60;
            Int32 ss = total % 60;
            return String.Format("{0}:{1}:{2}",
                hh.ToString().PadLeft(2, '0'),
                mm.ToString().PadLeft(2, '0'),
                ss.ToString().PadLeft(2, '0'));
        }

        /// <summary>
        /// 获取播放器背景图
        /// </summary>
        /// <returns></returns>
        public static Byte[] GetPlayerBackgroundImage()
        {
            try
            {
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    Bitmap bmp = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"player-bg.jpg");
                //    var temp = (Bitmap)bmp.Clone();
                //    temp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                //    return ms.ToArray();
                //}
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
