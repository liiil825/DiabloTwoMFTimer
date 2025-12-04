using System;
using System.IO;
using System.Threading; // 必须引用：用于 Mutex
using System.Windows.Forms;
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
        // 尝试创建一个命名的 Mutex
        // createdNew: 如果为 true，表示当前是第一个实例；如果为 false，表示 Mutex 已存在（程序已在运行）
        using (var mutex = new Mutex(true, AppMutexName, out bool createdNew))
        {
            if (!createdNew)
            {
                // 如果不是新创建的，说明程序已经在运行
                MessageBox.Show("程序已经在运行中！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // 直接退出
            }

            // --- 以下是原有的启动逻辑 ---

            // 1. 全局异常处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 2. 基础 UI 设置
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 3. 调试模式检查
            if (args.Length > 0 && args[0].Equals("--debug", StringComparison.CurrentCultureIgnoreCase))
            {
                Utils.LogManager.IsDebugEnabled = true;
            }

            try
            {
                // 4. 配置依赖注入
                _serviceProvider = ServiceConfiguration.ConfigureServices();

                // 5. 启动应用程序
                var mainForm = _serviceProvider.GetRequiredService<MainForm>();

                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                HandleFatalException(ex);
            }
        } // 退出 using 块时，Mutex 会被自动释放
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