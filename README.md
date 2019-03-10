# 领承科技物联网平台服务器

该服务器基于 .Net Framework 4.5 开发，以Windows服务的形式发布，为领承科技物联网运营平台集成的硬件设备提供高性能套接字(Socket)接入服务。

## 服务安装和卸载
### 编译
使用Visual Studio 2017打开源代码，点击 Ctrl+Shift+B，或者点击菜单栏中Build -> Build Solution。建议在编译前将Solution Configuration设置为Release。
进入bin\Release目录，即可看到**Socket.Server.exe**，该程序为可以安装的Windows服务程序。

### 安装
控制台执行Socket.Server.exe，参数为-i，即可安装。代码如下。

    D:\iot-server\Socket.Server.exe -i

### 运行
打开Windows服务，找到Socket Server服务项，右键点击选择启动。
> 建议在启动首选项中将启动方式设置为“自动”，服务安装时默认选择的是手动

### 停止
打开Windows服务，找到Socket Server服务项，右键点击选择停止。

### 卸载
窗口+R打开命令行执行程序，输入cmd，点击回车；在控制台中输入sc delete SocketServer即可卸载服务，代码如下.

    C:\Users\viso\sc delete SocketServer

> 请注意：卸载前建议将SocketServer先停止。

## 日志
记录在 log\runtime.txt 中