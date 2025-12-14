using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Utils; // 引用 LogManager

namespace DiabloTwoMFTimer.Services;

public class CommandDispatcher : ICommandDispatcher
{
    // 使用 OrdinalIgnoreCase，忽略大小写 ("Timer.Start" == "timer.start")
    private readonly Dictionary<string, Func<Task>> _commands = new(StringComparer.OrdinalIgnoreCase);

    public void Register(string commandId, Func<Task> action)
    {
        if (string.IsNullOrWhiteSpace(commandId) || action == null)
            return;

        // 如果重复注册，后者覆盖前者
        _commands[commandId] = action;
    }

    public void Register(string commandId, Action action)
    {
        if (action == null)
            return;

        // 将同步 Action 包装成 Task
        Register(
            commandId,
            () =>
            {
                action();
                return Task.CompletedTask;
            }
        );
    }

    public async Task ExecuteAsync(string commandId)
    {
        if (string.IsNullOrWhiteSpace(commandId))
            return;

        if (_commands.TryGetValue(commandId, out var action))
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                // 记录错误但不崩溃，确保用户体验
                LogManager.WriteErrorLog("CommandDispatcher", $"执行命令失败: {commandId}", ex);
                Utils.Toast.Error($"执行命令失败: {commandId}");
            }
        }
        else
        {
            LogManager.WriteDebugLog("CommandDispatcher", $"未找到命令: {commandId}");
            // 这里未来可以扩展：如果是调试模式，弹个 Toast 提示命令未找到
        }
    }

    public bool HasCommand(string commandId)
    {
        return !string.IsNullOrWhiteSpace(commandId) && _commands.ContainsKey(commandId);
    }
}
