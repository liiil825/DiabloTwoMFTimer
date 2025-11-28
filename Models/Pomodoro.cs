namespace DiabloTwoMFTimer.Models;

// 枚举和事件参数类
public enum PomodoroTimerState
{
    Work,
    ShortBreak,
    LongBreak,
}

public enum PomodoroBreakType
{
    ShortBreak,
    LongBreak,
}

public class PomodoroTimeSettings
{
    public int WorkTimeMinutes { get; set; } = 25;
    public int WorkTimeSeconds { get; set; } = 0;
    public int ShortBreakMinutes { get; set; } = 5;
    public int ShortBreakSeconds { get; set; } = 0;
    public int LongBreakMinutes { get; set; } = 15;
    public int LongBreakSeconds { get; set; } = 0;
}