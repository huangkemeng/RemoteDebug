# 本项目最主要的是两个方法：
- IServiceCollection的拓展方法ListeningRemoteDebug
- IApplicationBuilder的拓展方法OpenRemoteDebug
# 用法：
 ## 在远程(部署)环境中使用,该环境扮演着参数转发者的角色
 -  在Startup.cs的Configure方法中调用OpenRemoteDebug打开远程调试，需要的参数如下
 ```cs
    app.OpenRemoteDebug(setting =>
            {
                setting.SecretKey = "1234";
                setting.IgnoredEndpoint = new System.Collections.Generic.List<string>
                {
                      "/index.html",
                      "/swagger/v1/swagger.json",
                      "/favicon.ico"
                };
            });
```
### 参数说明
- SecretKey是指定连接者需要提供的密钥，这是一个安全措施，避免谁都能连接
- IgnoredEndpoint 是指定需要过滤的路由，并不是所有请求进来之后都需要转发的，这里可以做一个过滤的配置

## 在本地(开发)环境中使用,该环境扮演者参数接收者的角色
- 在Startup.cs的ConfigureServices方法中调用ListeningRemoteDebug监听远程环境接收到的请求，需要的参数如下：
```cs
     services.ListeningRemoteDebug(listeningSetting =>
            {
                listeningSetting.LocalUrl = "http://localhost:5001";
                listeningSetting.RemoteUrl = "http://www.baidu.com";
                listeningSetting.SecretKey = "1234";
            });
```
### 参数说明
- LocalUrl本地环境的地址
- RemoteUrl远程环境的地址
- SecretKey远程环境需要提供的密钥
