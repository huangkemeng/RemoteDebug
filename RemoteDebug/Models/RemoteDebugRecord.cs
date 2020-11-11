using System;
using System.Collections.Generic;

namespace KemengSoft.UTILS.RemoteDebug
{
    public class RemoteDebugRecord
    {
        public string Body { get; set; }

        public IEnumerable<NameValue<string>> Headers { get; set; }

        public long? ContentLength
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public IEnumerable<NameValue<string>> Cookies
        {
            get;
            set;
        }

        public IEnumerable<NameValue<string>> Form
        {
            get;
            set;
        }

        public string Host
        {
            get;
            set;
        }

        public bool IsHttps
        {
            get;
            set;
        }

        public string Method
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }


        public string PathBase
        {
            get;
            set;
        }


        public string Protocol
        {
            get;
            set;
        }


        public IEnumerable<NameValue<string>> Query
        {
            get;
            set;
        }


        public string QueryString
        {
            get;
            set;
        }

        public string Scheme { get; set; }
        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime AccessTime { get; set; }
        /// <summary>
        /// 是否已发送
        /// </summary>
        public bool HasSend { get; set; }
    }
}
