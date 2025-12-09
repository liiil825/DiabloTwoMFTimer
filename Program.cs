using System;
using System.IO;
using System.Threading; // 必须引用：用于 Mutex
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI;
using Microsoft.Extensions.DependencyInjection;

namespace DiabloTwoMFTimer;

static class Program
{
    private static IServiceProvider? _serviceProvider;

    // 定义一个唯一的互斥体名称，通常建议包含 GUID 以避免冲突
    private const string AppMutexName = "Global\\DiabloTwoMFTimer_Unique_Mutex_ID";

    [STAThread]
    static void Main(string[] args)
    {
        // 1. 创建 Mutex
        using (var mutex = new Mutex(false, AppMutexName))
        {
            // 2. 尝试获取锁 (抽取了独立方法，逻辑一目了然)
            if (!TryAcquireMutex(mutex))
            {
                MessageBox.Show("程序已经在运行中！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. 获取成功，进入主逻辑保护区
            try
            {
                RunApplication(args); // 可选：把主启动逻辑也封装一下，Main更干净
            }
            finally
            {
                // 4. 确保释放锁
                mutex.ReleaseMutex();
            }
        }
    }

    /// <summary>
    /// 尝试获取互斥锁，处理了遗弃锁的情况
    /// </summary>
    private static bool TryAcquireMutex(Mutex mutex)
    {
        try
        {
            // 等待3秒，给旧进程退出的时间
            return mutex.WaitOne(3000, false);
        }
        catch (AbandonedMutexException)
        {
            MessageBox.Show("程序已经在运行中！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            // 捕获“被遗弃的锁”（上一个进程非正常退出），视为获取成功
            return true;
        }
    }

    /// <summary>
    /// 应用程序启动主逻辑
    /// </summary>
    private static void RunApplication(string[] args)
    {
        // 全局异常处理
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += Application_ThreadException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        // UI 设置
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 调试参数
        if (args.Length > 0 && args[0].Equals("--debug", StringComparison.CurrentCultureIgnoreCase))
        {
            Utils.LogManager.IsDebugEnabled = true;
        }

        try
        {
            // 依赖注入配置
            _serviceProvider = ServiceConfiguration.ConfigureServices();
            var appSettings = _serviceProvider.GetRequiredService<IAppSettings>();
            Utils.ScaleHelper.Initialize(appSettings);
            UI.Theme.AppTheme.InitializeFonts();

            // 启动主窗体
            var mainForm = _serviceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            HandleFatalException(ex);
        }
    }

    private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
        LogAndShowError("thread_error_log.txt", e.Exception.ToString(), "发生线程异常");
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        string exceptionContent = e.ExceptionObject?.ToString() ?? "未知异常（ExceptionObject 为 null）";
        LogAndShowError("domain_error_log.txt", exceptionContent, "发生未处理的异常");
    }

    private static void HandleFatalException(Exception ex)
    {
        LogAndShowError("startup_error_log.txt", ex.ToString(), "程序启动失败");
    }

    private static void LogAndShowError(string fileName, string content, string title)
    {
        try
        {
            string errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            File.WriteAllText(errorLogPath, content);
            MessageBox.Show(
                $"{title}。错误详情已保存到 {errorLogPath}\n\n{content}",
                "应用程序错误",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        catch
        {
            // 如果连日志都写不进去，就只弹窗
            MessageBox.Show($"{title}。\n\n{content}", "应用程序严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}