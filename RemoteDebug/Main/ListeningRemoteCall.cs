using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KemengSoft.UTILS.RemoteDebug
{
    public class ListeningRemoteCall : BackgroundService
    {
        private const int ThreadDelay = 1500;
        private readonly HttpClient httpClient;
        private readonly ListeningRemoteDebugSetting setting;

        public ListeningRemoteCall(ListeningRemoteDebugSetting setting)
        {
            httpClient = new HttpClient();
            this.setting = setting;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrWhiteSpace(setting.RemoteUrl))
            {
                return;
            }
            string endpoint = setting.RemoteUrl.EndsWith("/") ? "debug/remote" : $"/debug/remote";
            string fullRemoteUrl = setting.RemoteUrl + endpoint + (!string.IsNullOrWhiteSpace(setting.SecretKey) ? $"?secretKey={setting.SecretKey}" : "");
            //先测试一下是否可以请求成功
            var testResult = await httpClient.GetAsync(fullRemoteUrl);
            if (!testResult.IsSuccessStatusCode)
            {
                return;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                var getRes = await httpClient.GetAsync(fullRemoteUrl);
                HttpContent content = getRes.Content;
                string contentString = await content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(contentString))
                {
                    ResponseContent responseContent = JsonConvert.DeserializeObject<ResponseContent>(contentString);
                    if (responseContent.Code == 200)
                    {
                        try
                        {
                            foreach (var request in responseContent.Requests)
                            {
                                string url = setting.LocalUrl + "/" + request.Path.TrimStart('/');
                                using (HttpContent requestContent = new StringContent(request.Body, Encoding.UTF8, request.ContentType))
                                {
                                    if (!string.IsNullOrWhiteSpace(request.QueryString))
                                    {
                                        url += "?" + request.QueryString.TrimStart('?');
                                    }
                                    else if (request.Query != null && request.Query.Any())
                                    {
                                        url += "?" + string.Join('&', request.Query.Select(e => $"{e.Name}={e.Value}"));
                                    }
                                    HttpResponseMessage httpResponse = new HttpResponseMessage();
                                    if ("post".Equals(request.Method, StringComparison.OrdinalIgnoreCase))
                                    {
                                        httpResponse = await httpClient.PostAsync(url, requestContent);
                                    }
                                    else if ("get".Equals(request.Method, StringComparison.OrdinalIgnoreCase))
                                    {
                                        httpResponse = await httpClient.GetAsync(url);
                                    }
                                    else if ("delete".Equals(request.Method, StringComparison.OrdinalIgnoreCase))
                                    {
                                        httpResponse = await httpClient.DeleteAsync(url);
                                    }
                                    HttpContent callContent = httpResponse.Content;
                                    string callString = await callContent.ReadAsStringAsync();
                                    Trace.WriteLine("==========调试返回结果Start=======");
                                    Trace.WriteLine(callString);
                                    Trace.WriteLine("==========调试返回结果End=========");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (responseContent.Code == 403)
                    {
                        Trace.WriteLine(responseContent.Message);
                    }
                }
                await Task.Delay(ThreadDelay, stoppingToken);
            }
        }
    }
}
