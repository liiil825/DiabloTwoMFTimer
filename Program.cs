using System;
using System.Windows.Forms;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.UI;

namespace DTwoMFTimerHelper
{
    public static class ServiceConfiguration
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 注册服务
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddSingleton<ITimerHistoryService, TimerHistoryService>();
            services.AddSingleton<ITimerService, TimerService>();

            // 注册 IMainServices 接口
            services.AddSingleton<IMainServices, MainServices>();

            // 注册UI组件
            services.AddTransient<MainForm>();
            services.AddTransient<UI.Profiles.ProfileManager>();
            services.AddTransient<UI.Timer.TimerControl>();
            services.AddTransient<UI.Timer.CharacterSceneControl>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }

    static class Program
    {
        private static IServiceProvider? _serviceProvider;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // 添加全局异常处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 只在提供--debug参数时才启用调试日志
            if (args.Length > 0 && args[0].Equals("--debug", StringComparison.CurrentCultureIgnoreCase))
            {
                Utils.LogManager.IsDebugEnabled = true;
            }

            try
            {
                // 配置依赖注入
                _serviceProvider = ServiceConfiguration.ConfigureServices();

                // 从DI容器解析MainForm
                var mainForm = _serviceProvider.GetRequiredService<MainForm>();

                // 修复：通过接口解析IMainServices
                var mainServices = _serviceProvider.GetRequiredService<IMainServices>();

                // 初始化主窗体引用和相关组件
                mainServices.InitializeMainForm(mainForm);

                // 初始化应用程序
                mainServices.InitializeApplication();

                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                string errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");
                File.WriteAllText(errorLogPath, ex.ToString());
                MessageBox.Show($"发生未处理的异常。错误详情已保存到 {errorLogPath}", "应用程序错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thread_error_log.txt");
            File.WriteAllText(errorLogPath, e.Exception.ToString());
            MessageBox.Show($"发生线程异常。错误详情已保存到 {errorLogPath}", "应用程序错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "domain_error_log.txt");
            File.WriteAllText(errorLogPath, e.ExceptionObject.ToString());
            // 主线程异常可能导致应用程序崩溃，所以这里只记录日志
        }
    }
}