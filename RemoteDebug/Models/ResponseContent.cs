using System.Collections.Generic;

namespace KemengSoft.UTILS.RemoteDebug
{
    public class ResponseContent
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 请求
        /// </summary>
        public List<RemoteDebugRecord> Requests { get; set; }
    }
}
