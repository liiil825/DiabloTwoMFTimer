using System;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Services;

public class WindowCMDService : IWindowCMDService
{
    private readonly IAppSettings _appSettings;
    private readonly IMessenger _messenger;
    private readonly IMainService _mainService;
    private readonly ICommandDispatcher _dispatcher;

    public WindowCMDService(
        IAppSettings appSettings,
        IMessenger messenger,
        IMainService mainService,
        ICommandDispatcher dispatcher
    )
    {
        _appSettings = appSettings;
        _messenger = messenger;
        _mainService = mainService;
        _dispatcher = dispatcher;
    }

    public void Initialize()
    {
        // --- 系统/导航 ---
        _dispatcher.Register(
            "App.Exit",
            () =>
            {
                _mainService.HandleApplicationClosing();
                Application.Exit();
            }
        );

        // 最小化到托盘
        _dispatcher.Register(
            "App.Minimize",
            () =>
            {
                _messenger.Publish(new MinimizeToTrayMessage());
            }
        );

        // 从托盘恢复 (这个命令通常通过全局 LeaderKey 触发)
        _dispatcher.Register(
            "App.Restore",
            () =>
            {
                _messenger.Publish(new RestoreFromTrayMessage());
            }
        );

        _dispatcher.Register("App.SetPosition", (arg) => SetWindowPosition(arg?.ToString() ?? string.Empty));

        _dispatcher.Register("App.SetPosition.TopLeft", () => SetWindowPositionTopLeft());

        _dispatcher.Register("App.SetPosition.TopRight", () => SetWindowPositionTopRight());

        _dispatcher.Register("App.SetPosition.BottomLeft", () => SetWindowPositionBottomLeft());

        _dispatcher.Register("App.SetPosition.BottomRight", () => SetWindowPositionBottomRight());

        _dispatcher.Register(
            "App.SetOpacity",
            (arg) =>
            {
                if (double.TryParse(arg?.ToString(), out double val))
                {
                    // 调用调整透明度的逻辑
                    if (val < 0.1 || val > 1.0)
                    {
                        Utils.Toast.Error(Utils.LanguageManager.GetString("OpacityValueInvalid"));
                        return;
                    }
                    _appSettings.Opacity = val;
                    _appSettings.Save();
                    _messenger.Publish(new OpacityChangedMessage());
                    Utils.Toast.Success(Utils.LanguageManager.GetString("OpacitySet", val));
                }
            }
        );
        _dispatcher.Register(
            "App.SetSize",
            (arg) =>
            {
                if (float.TryParse(arg?.ToString(), out float val) && val >= 1.0f && val <= 2.5f)
                {
                    var result = DiabloTwoMFTimer.UI.Components.ThemedMessageBox.Show(
                        Utils.LanguageManager.GetString("UiScaleRestartRequired"),
                        Utils.LanguageManager.GetString("RestartRequired"),
                        MessageBoxButtons.YesNo
                    ); // 使用 YesNo 按钮
                    _appSettings.UiScale = val;
                    _appSettings.Save();
                    if (result == DialogResult.Yes)
                    {
                        Application.Restart();
                        Application.Exit();
                    }
                }
                else
                {
                    Utils.Toast.Error(Utils.LanguageManager.GetString("UiScaleValueInvalid"));
                }
            }
        );
    }

    public void SetWindowPosition(string position)
    {
        if (Enum.TryParse(position, true, out Models.WindowPosition windowPosition))
        {
            _appSettings.WindowPosition = windowPosition.ToString();
            _appSettings.Save();
            _messenger.Publish(new WindowPositionChangedMessage());
            string message = Utils.LanguageManager.GetString($"SetWindowPosition.{windowPosition}");
            Utils.Toast.Success(message);
        }
        else
        {
            Utils.Toast.Error("请输入有效的位置");
        }
    }

    public void SetWindowPositionTopLeft()
    {
        SetWindowPositionInternal(Models.WindowPosition.TopLeft);
    }

    public void SetWindowPositionTopRight()
    {
        SetWindowPositionInternal(Models.WindowPosition.TopRight);
    }

    public void SetWindowPositionBottomLeft()
    {
        SetWindowPositionInternal(Models.WindowPosition.BottomLeft);
    }

    public void SetWindowPositionBottomRight()
    {
        SetWindowPositionInternal(Models.WindowPosition.BottomRight);
    }

    private void SetWindowPositionInternal(Models.WindowPosition position)
    {
        _appSettings.WindowPosition = position.ToString();
        _appSettings.Save();
        _messenger.Publish(new WindowPositionChangedMessage());
        string message = Utils.LanguageManager.GetString($"SetWindowPosition.{position}");
        Utils.Toast.Success(message);
    }
}
