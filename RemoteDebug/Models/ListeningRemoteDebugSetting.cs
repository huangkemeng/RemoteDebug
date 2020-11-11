using System;
using System.Collections.Generic;
using System.Text;

namespace KemengSoft.UTILS.RemoteDebug
{
    public class ListeningRemoteDebugSetting
    {
        /// <summary>
        /// 远程地址，连接时必须
        /// </summary>
        public string RemoteUrl { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 本地地址
        /// </summary>
        public string LocalUrl { get; set; }
    }
}
