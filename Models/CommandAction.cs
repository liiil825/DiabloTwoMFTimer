using System;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.Models;

public class CommandAction
{
    public Keys TriggerKey { get; set; } // 触发键，如 Keys.A
    public string Description { get; set; } // 描述，如 "启动计时"
    public Action ActionToRun { get; set; } // 具体要执行的方法

    public CommandAction(Keys key, string desc, Action action)
    {
        TriggerKey = key;
        Description = desc;
        ActionToRun = action;
    }
}
