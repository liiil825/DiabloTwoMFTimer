using System;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.Utils;

public static class ControlExtensions
{
    /// <summary>
    /// 线程安全的 UI 更新方法。
    /// 自动判断是否需要 Invoke，并处理了控件已销毁的情况。
    /// </summary>
    /// <param name="control">目标控件</param>
    /// <param name="action">要执行的 UI 操作</param>
    public static void SafeInvoke(this Control control, Action action)
    {
        // 1. 如果控件已经销毁或句柄未创建，直接返回，防止 ObjectDisposedException
        if (control.IsDisposed || !control.IsHandleCreated)
        {
            return;
        }

        // 2. 如果需要跨线程调用
        if (control.InvokeRequired)
        {
            try
            {
                control.Invoke(action);
            }
            catch (ObjectDisposedException)
            {
                // 再次捕获极其罕见的并发销毁情况
            }
        }
        else
        {
            // 3. 在 UI 线程直接执行
            action();
        }
    }
}