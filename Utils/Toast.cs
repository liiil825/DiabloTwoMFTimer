using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI;
using DiabloTwoMFTimer.UI.Form;

namespace DiabloTwoMFTimer.Utils;

public static class Toast
{
    private static readonly List<ToastForm> _openToasts = [];

    // 定义一个委托，用于从 UI 线程执行操作
    private static Action<Action>? _uiInvoker;

    /// <summary>
    /// 注册 UI 线程调用器 (通常由 MainForm 在启动时调用)
    /// </summary>
    public static void RegisterUiInvoker(Action<Action> invoker)
    {
        _uiInvoker = invoker;
    }

    // 公开的方法保持不变，服务层不需要改代码
    public static void Show(string message, ToastType type = ToastType.Info)
    {
        // 如果注册了 UI 调用器，且需要跨线程，则通过调用器执行
        if (_uiInvoker != null)
        {
            // 注意：这里调用 InternalShow，而不是递归调用 Show，防止死循环
            _uiInvoker(() => InternalShow(message, type));
        }
        else
        {
            // 如果没注册（比如单元测试环境），尝试直接执行
            InternalShow(message, type);
        }
    }

    // 真正的 UI 逻辑 (私有，确保只在 UI 线程运行)
    private static void InternalShow(string message, ToastType type)
    {
        try
        {
            var toast = new ToastForm(message, type);

            var screen = Screen.PrimaryScreen;
            if (screen == null)
            {
                toast.Location = new Point(100, 100);
            }
            else
            {
                var workingArea = screen.WorkingArea;
                int x = workingArea.X + (workingArea.Width - toast.Width) / 2;
                int startY = workingArea.Y + (int)(workingArea.Height * 0.15);

                // 清理已销毁的引用
                _openToasts.RemoveAll(t => t.IsDisposed);

                int offset = _openToasts.Count * (toast.Height + 10);
                toast.Location = new Point(x, startY + offset);
            }

            toast.FormClosed += (s, e) =>
            {
                _openToasts.Remove(toast);
            };
            _openToasts.Add(toast);

            toast.Show();
        }
        catch (Exception ex)
        {
            // 兜底防护，防止 UI 创建失败导致崩溃
            LogManager.WriteErrorLog("Toast", "显示通知失败", ex);
        }
    }

    public static void Success(string msg) => Show(msg, ToastType.Success);

    public static void Error(string msg) => Show(msg, ToastType.Error);

    public static void Info(string msg) => Show(msg, ToastType.Info);

    public static void Warning(string msg) => Show(msg, ToastType.Warning);
}
