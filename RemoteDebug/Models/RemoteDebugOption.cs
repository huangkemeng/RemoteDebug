using System;
using System.Collections.Generic;

namespace KemengSoft.UTILS.RemoteDebug
{
    public class RemoteDebugOption
    {
        /// <summary>
        /// 当前是否已打开远程调试
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// 信任的IP
        /// </summary>
        public List<TrustedConnection> TrustedConnections { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdate { get; set; }
    }
}
