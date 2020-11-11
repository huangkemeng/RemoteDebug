using System;
using System.Collections.Generic;
using System.Text;

namespace KemengSoft.UTILS.RemoteDebug
{
    public class OpenRemoteDebugSetting
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 忽略的终结点
        /// </summary>
        public List<string> IgnoredEndpoint { get; set; }

    }
}
