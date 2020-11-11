using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace KemengSoft.UTILS.RemoteDebug
{
    public static class RemoteDebugExtension
    {
        /// <summary>
        /// 启用远程调试
        /// </summary>
        /// <param name="app"></param>
        /// <param name="debugSetting"></param>
        /// <returns></returns>
        public static IApplicationBuilder OpenRemoteDebug(this IApplicationBuilder app, Action<OpenRemoteDebugSetting> debugSetting)
        {
            OpenRemoteDebugSetting setting = new OpenRemoteDebugSetting();
            debugSetting?.Invoke(setting);
            return app.UseMiddleware<OpenRemoteDebugMiddleware>(setting.SecretKey, setting.IgnoredEndpoint);
        }

        /// <summary>
        /// 监听远程调试
        /// </summary>
        /// <param name="services"></param>
        /// <param name="listeningSetting"></param>
        /// <returns></returns>
        public static IServiceCollection ListeningRemoteDebug(this IServiceCollection services, Action<ListeningRemoteDebugSetting> listeningSetting)
        {
            ListeningRemoteDebugSetting setting = new ListeningRemoteDebugSetting();
            listeningSetting?.Invoke(setting);
            return services.AddTransient<IHostedService>(e => new ListeningRemoteCall(setting));
        }
    }
}
