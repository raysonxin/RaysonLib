//*******************************************************************
// 文件名（File Name）：		RtspWrapper
//
// 作者（Author）:			辛祥
//
// 日期（Create Time）:		2015/2/1 15:38:15
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
using System.Runtime.InteropServices;

namespace AtiSafeMediaToolkitTest
{

    public class RtspWrapper
    {

        [UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void VideoStreamComeDelegate(IntPtr pointer, int len, int width, int height);


        [DllImport("librvd.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateRtspClientSession(string url, VideoStreamComeDelegate handler);


        [DllImport("librvd.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyRtspClientSession(IntPtr hwnd);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);
    }
}
