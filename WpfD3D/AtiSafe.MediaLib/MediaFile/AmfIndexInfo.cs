/* ============================================================
*类名称：AmfIndexInfo
 *类描述：
 *创建人：raysonxin
 *创建时间：2014-08-15 10:44:01
 *修改人：
 *修改备注：
 *@version 1.0
 * ============================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtiSafe.MediaLib.MediaFile
{
    /// <summary>
    /// amf文件帧信息
    /// </summary>
    public class FrameIndexInfo
    {
        /// <summary>
        /// 帧编号
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// 帧偏移量
        /// </summary>
        public UInt32 Offset { get; set; }

        /// <summary>
        /// 帧长度
        /// </summary>
        public UInt32 Length { get; set; }

        /// <summary>
        /// 是否下载完成
        /// </summary>
        public Boolean IsSuccess { get; set; }

        /// <summary>
        /// 下载尝试次数
        /// </summary>
        public Int32 TryCount { get; set; }

        /// <summary>
        /// 与前一帧间距
        /// </summary>
        public UInt32 Distance { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public UInt32 Timestamp { get; set; }

        /// <summary>
        /// 到此帧时播放进度
        /// </summary>
        public UInt32 Progress { get; set; }

        public override string ToString()
        {
            return String.Format("Id={0},Offset={1},Length={2},Distance={3},Timestamp={4},Progress={5}", this.Id, this.Offset, this.Length, this.Distance, this.Timestamp,this.Progress);
        }
    }
}
