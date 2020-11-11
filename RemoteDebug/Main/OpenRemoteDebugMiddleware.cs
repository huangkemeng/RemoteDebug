using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemengSoft.UTILS.RemoteDebug
{
    public class OpenRemoteDebugMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey;
        private readonly List<string> _ignoreEndpoints;
        public OpenRemoteDebugMiddleware(RequestDelegate next, string secretKey, List<string> ignoreEndpoints)
        {
            _next = next;
            _secretKey = secretKey;
            _ignoreEndpoints = ignoreEndpoints;
        }

        public async Task Invoke(HttpContext context)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            string yyyyMMdd = DateTime.Now.ToString("yyyyMMdd");
            string remoteDebugDir = Path.Combine(AppContext.BaseDirectory, "RemoteDebug");
            string configFilePath = Path.Combine(remoteDebugDir, "debugConfig.json");
            string debugListPath = Path.Combine(remoteDebugDir, $"debugList_{yyyyMMdd}.json");
            if (!Directory.Exists(remoteDebugDir))
            {
                Directory.CreateDirectory(remoteDebugDir);
            }
            if (context.Request.Path.Value.ToLower() == "/debug/remote")
            {
                if (!string.IsNullOrWhiteSpace(_secretKey) && !_secretKey.Equals(context.Request.Query["secretKey"]))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseContent
                    {
                        Code = 403,
                        Message = "Please provide correct secret key!",
                    }, Formatting.Indented, serializerSettings));
                }
                else
                {
                    TrustedConnection currentConnection = new TrustedConnection
                    {
                        Id = context.Connection.Id,
                        Ip = context.Connection.RemoteIpAddress.ToString(),
                        TrustTime = DateTime.Now
                    };
                    if (!File.Exists(configFilePath))
                    {
                        var fileStream = File.Create(configFilePath);
                        fileStream.Dispose();
                        string body = JsonConvert.SerializeObject(new RemoteDebugOption
                        {
                            IsOpen = true,
                            TrustedConnections = new List<TrustedConnection> { currentConnection },
                            LastUpdate = DateTime.Now
                        }, Formatting.Indented, serializerSettings);
                        await File.WriteAllTextAsync(configFilePath, body);
                    }
                    else
                    {
                        string optionString = await File.ReadAllTextAsync(configFilePath);
                        RemoteDebugOption debugOption = new RemoteDebugOption
                        {
                            IsOpen = true,
                            TrustedConnections = new List<TrustedConnection> { currentConnection },
                            LastUpdate = DateTime.Now
                        };
                        if (!string.IsNullOrWhiteSpace(optionString))
                        {
                            debugOption = JsonConvert.DeserializeObject<RemoteDebugOption>(optionString, serializerSettings);
                        }
                        var firstTrustedConnection =
                            debugOption.TrustedConnections.FirstOrDefault(e => e.Ip ==
                            context.Connection.RemoteIpAddress.ToString());
                        if (firstTrustedConnection == null)
                        {
                            debugOption.LastUpdate = DateTime.Now;
                            debugOption.TrustedConnections.Add(currentConnection);
                        }
                        else
                        {
                            if (DateTime.Now - firstTrustedConnection.TrustTime > TimeSpan.FromMinutes(5))
                            {
                                debugOption.LastUpdate = DateTime.Now;
                                firstTrustedConnection.TrustTime = DateTime.Now;
                            }
                            currentConnection = firstTrustedConnection;
                        }
                        debugOption.IsOpen = true;
                        string newBody = JsonConvert.SerializeObject(debugOption, Formatting.Indented, serializerSettings);
                        await File.WriteAllTextAsync(configFilePath, newBody);
                    }
                    if (!File.Exists(debugListPath))
                    {
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseContent
                        {
                            Code = 204,
                            Message = "No any access info!"
                        }, Formatting.Indented, serializerSettings));
                    }
                    else
                    {
                        string listString = await File.ReadAllTextAsync(debugListPath);
                        if (!string.IsNullOrWhiteSpace(listString))
                        {
                            List<RemoteDebugRecord> debugRecords = JsonConvert.DeserializeObject<List<RemoteDebugRecord>>(listString, serializerSettings);
                            var debugUsable = debugRecords.Where(e => e.AccessTime > currentConnection.TrustTime && !e.HasSend);
                            if (!debugUsable.Any())
                            {
                                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseContent
                                {
                                    Code = 204,
                                    Message = "No any access info!"
                                }, Formatting.Indented, serializerSettings));
                            }
                            else
                            {
                                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseContent
                                {
                                    Code = 200,
                                    Message = "Get debug access successfully!",
                                    Requests = debugUsable.ToList()
                                }, Formatting.Indented, serializerSettings));
                                foreach (var able in debugUsable)
                                {
                                    able.HasSend = true;
                                }
                                await File.WriteAllTextAsync(debugListPath, JsonConvert.SerializeObject(debugRecords, Formatting.Indented, serializerSettings));
                            }
                        }
                        else
                        {
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseContent
                            {
                                Code = 204,
                                Message = "No any access info!"
                            }, Formatting.Indented, serializerSettings));
                        }
                    }
                }
            }
            else
            {
                if (_ignoreEndpoints == null || !_ignoreEndpoints.Contains(context.Request.Path.Value.ToString()))
                {
                    if (File.Exists(configFilePath))
                    {
                        if (!File.Exists(debugListPath))
                        {
                            var debugFile = File.Create(debugListPath);
                            debugFile.Dispose();
                        }
                        string optionString = await File.ReadAllTextAsync(configFilePath);
                        if (!string.IsNullOrWhiteSpace(optionString))
                        {
                            RemoteDebugOption debugOption = JsonConvert.DeserializeObject<RemoteDebugOption>(optionString, serializerSettings);
                            DateTime now = DateTime.Now;
                            if (debugOption != null)
                            {
                                if (debugOption.IsOpen
                                && debugOption.TrustedConnections.Any(e => now - e.TrustTime <= TimeSpan.FromMinutes(5)))
                                {
                                    string listString = await File.ReadAllTextAsync(debugListPath);
                                    List<RemoteDebugRecord> records = new List<RemoteDebugRecord>();
                                    if (!string.IsNullOrWhiteSpace(listString))
                                    {
                                        records = JsonConvert.DeserializeObject<List<RemoteDebugRecord>>(listString);
                                    }
                                    var newRecord = new RemoteDebugRecord
                                    {
                                        AccessTime = DateTime.Now,
                                        ContentLength = context.Request.ContentLength,
                                        ContentType = context.Request.ContentType,
                                        Headers = context.Request.Headers.Select(e => new NameValue<string>(e.Key, e.Value)),
                                        Cookies = context.Request.Cookies.Select(e => new NameValue<string>(e.Key, e.Value)),
                                        Host = context.Request.Host.ToString(),
                                        IsHttps = context.Request.IsHttps,
                                        Method = context.Request.Method,
                                        Path = context.Request.Path,
                                        PathBase = context.Request.PathBase,
                                        Protocol = context.Request.Protocol,
                                        Query = context.Request.Query.Select(e => new NameValue<string>(e.Key, e.Value)),
                                        QueryString = context.Request.QueryString.ToString(),
                                        Scheme = context.Request.Scheme,
                                        HasSend = false
                                    };
                                    context.Request.EnableBuffering();
                                    newRecord.Body = await (new StreamReader(context.Request.Body).ReadToEndAsync());
                                    context.Request.Body.Position = 0;
                                    if (context.Request.HasFormContentType)
                                    {
                                        newRecord.Form = context.Request.Form.Select(e => new NameValue<string>(e.Key, e.Value));
                                    }
                                    records.Add(newRecord);
                                    string newRecordString = JsonConvert.SerializeObject(records, Formatting.Indented, serializerSettings);
                                    await File.WriteAllTextAsync(debugListPath, newRecordString);
                                }
                                else
                                {
                                    if (debugOption.IsOpen)
                                    {
                                        debugOption.IsOpen = false;
                                        var newString = JsonConvert.SerializeObject(debugOption, Formatting.Indented, serializerSettings);
                                        await File.WriteAllTextAsync(configFilePath, newString);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            if (!context.Response.HasStarted)
            {
                await _next?.Invoke(context);
            }
        }

    }
}
