//*******************************************************************
// 文件名（File Name）：		AmfInterop
//
// 作者（Author）:			辛祥
//
// 日期（Create Time）:		2015/2/4 19:42:32
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AtiSafe.MediaLib.Display
{
    /// <summary>
    /// amf解码器
    /// </summary>
    public class AmfDecoder
    {
        #region JPEG->YUV
        /// <summary>
        /// 创建解码器
        /// </summary>
        /// <returns></returns>
        [DllImport("libjpgw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr CreateJpegDecoder();

        /// <summary>
        /// 解码方法：JPEG->YUV
        /// </summary>
        /// <param name="inHandle"></param>
        /// <param name="inData"></param>
        /// <param name="inLen"></param>
        /// <param name="ioLen"></param>
        /// <param name="ioWidth"></param>
        /// <param name="ioHeight"></param>
        /// <returns></returns>
        [DllImport("libjpgw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr JpegDecode(UIntPtr inHandle, IntPtr inData, int inLen, ref int ioLen, ref int ioWidth, ref int ioHeight);

        /// <summary>
        /// 销毁解码器
        /// </summary>
        /// <param name="inHandle"></param>
        [DllImport("libjpgw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyJpegDecoder(UIntPtr inHandle);

        #endregion
    }
}
