using System;
using System.Threading.Tasks;

namespace DiabloTwoMFTimer.Interfaces;

public interface ICommandDispatcher
{
    /// <summary>
    /// 注册一个异步命令
    /// </summary>
    /// <param name="commandId">命令ID (例如 "Timer.Start")</param>
    /// <param name="action">要执行的异步操作</param>
    void Register(string commandId, Func<Task> action);

    /// <summary>
    /// 注册一个同步命令 (自动包装为 Task)
    /// </summary>
    /// <param name="commandId">命令ID</param>
    /// <param name="action">要执行的同步操作</param>
    void Register(string commandId, Action action);

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="commandId">命令ID</param>
    Task ExecuteAsync(string commandId);

    /// <summary>
    /// 检查命令是否存在
    /// </summary>
    bool HasCommand(string commandId);
}
