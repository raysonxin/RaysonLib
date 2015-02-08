//*******************************************************************
// 文件名（File Name）：		Utilities
//
// 作者（Author）:			辛祥
//
// 日期（Create Time）:		2015/2/4 13:41:30
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
using SlimDX;
using SlimDX.Direct3D9;

namespace AtiSafe.MediaLib.Display
{
    /// <summary>
    /// 通用操作方法类
    /// </summary>
    public class Utilities
    {
        public static Format D3DFormatYV12 = D3DX.MakeFourCC((byte)'Y', (byte)'V', (byte)'1', (byte)'2');

        public static Format D3DFormatNV12 = D3DX.MakeFourCC((byte)'N', (byte)'V', (byte)'1', (byte)'2');

        /// <summary>
        /// 黑色Argb
        /// </summary>
        public static Color4 BlackColor
        {
            get
            {
                return new Color4(Utilities.GetArgb(0xff, 0, 0, 0));
            }
        }

        /// <summary>
        /// 从argb各个分量转换为argb数值
        /// </summary>
        /// <returns></returns>
        public static int GetArgb(byte a, byte r, byte g, byte b)
        {
            return a << 24 + r << 16 + g << 8 + b;
        }

        /// <summary>
        /// 当前系统版本是否高于Vista
        /// </summary>
        /// <returns></returns>
        public static bool IsVistaOrBetter()
        {
            return Environment.OSVersion.Version.Major >= 6;
        }

        /// <summary>
        /// 把本库支持的视频帧格式转换为SlimDX支持的帧类型
        /// </summary>
        /// <param name="format">视频帧格式</param>
        /// <returns></returns>
        public static Format ConvertToD3D(SupportFormat format)
        {
            switch (format)
            {
                case SupportFormat.YV12:
                    return D3DFormatYV12;
                case SupportFormat.NV12:
                    return D3DFormatNV12;
                default:
                    throw new ArgumentException("unknown pixel format");
            }
        }

        [DllImport("ntdll.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Memcpy(IntPtr dest, IntPtr source, int length);
    }
}
