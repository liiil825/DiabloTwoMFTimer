# DiabloTwoMFTimer

这是一个简单的D2R MF工具，包含以下功能：
- MF 记录功能
- 番茄时钟功能

[![暗黑2刷图神器演示视频](https://i0.hdslb.com/bfs/archive/BV1fs2GB2EKU_1.jpg)](https://www.bilibili.com/video/BV1fs2GB2EKU/)

## 运行步骤

要运行此应用程序，您需要：

1. **安装 .NET SDK**
   - 确保已安装 .NET 6.0 或更高版本的SDK
   - 根据检测，您的系统目前尚未安装.NET SDK
   - 详细安装步骤：
     1. 访问 [微软官方网站](https://dotnet.microsoft.com/download)
     2. 下载适用于Windows的最新.NET SDK安装程序
     3. 运行安装程序并按照提示完成安装
     4. 安装完成后，打开新的命令提示符或PowerShell窗口
     5. 运行 `dotnet --version` 命令验证安装是否成功

2. **编译和运行应用程序**
   
   ### 使用命令行：
   
   打开命令提示符或PowerShell，导航到项目文件夹，然后执行以下命令：
   
   ```bash
   # 编译并运行应用程序
   dotnet run
   ```
   
   或者，您可以先构建然后运行：
   
   ```bash
   # 构建应用程序
   dotnet build
   
   # 运行应用程序
   dotnet run
   ```

## 应用程序功能
1. **MF 记录功能**：
   - 点击"开始计时"按钮开始计时
   - 标签会显示已经过的秒数
   - 再次点击按钮可以停止计时
2. **番茄时钟功能**：
   - 点击"开始番茄"按钮开始番茄时钟
   - 番茄时钟会倒计时25分钟
   - 倒计时结束后，会播放提示音
   - 点击"重置"按钮可以重置番茄时钟

## 许可证 (License)

本项目采用 **[CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/deed.zh-hans)** 协议进行许可。

您可以自由地：
* **共享** — 在任何媒介以任何形式复制、发行本作品。
* **演绎** — 修改、转换或以本作品为基础进行创作。

惟须遵守下列条件：
* **署名** — 您必须给出适当的署名，提供指向本许可协议的链接，同时标明是否（对原始作品）作了修改。
* **非商业性使用** — 您不得将本作品用于商业目的。

Commercial usage is **strictly prohibited**.