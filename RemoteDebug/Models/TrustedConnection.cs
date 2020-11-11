using System;

namespace KemengSoft.UTILS.RemoteDebug
{
    public class TrustedConnection
    {
        /// <summary>
        /// 信任的IP
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 信任的Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 信任的时间
        /// </summary>
        public DateTime TrustTime { get; set; }
    }
}
