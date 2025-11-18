using System;
using System.Media;

namespace DTwoMFTimerHelper.Services
{
    public class PomodoroTimerService
    {
        // 事件定义
        public event EventHandler<TimerStateChangedEventArgs>? TimerStateChanged;
        public event EventHandler<PomodoroCompletedEventArgs>? PomodoroCompleted;
        public event EventHandler<BreakStartedEventArgs>? BreakStarted;
        public event EventHandler? BreakSkipped;
        public event EventHandler? TimeUpdated;

        // 番茄时钟相关字段
        private TimeSpan _timeLeft;
        private bool _isRunning = false;
        private int _completedPomodoros = 0;
        private TimerState _currentState = TimerState.Work;
        private TimerState _previousState = TimerState.Work; // 记录之前的状态
        private readonly System.Windows.Forms.Timer _timer;

        // 时间设置
        public TimeSettings Settings
        {
            get; set;
        }

        public PomodoroTimerService()
        {
            Settings = new TimeSettings();
            _timer = new System.Windows.Forms.Timer { Interval = 100 };
            _timer.Tick += Timer_Tick;
            InitializeTimer();
        }

        public void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _timer.Start();
                OnTimerStateChanged();
            }
        }

        public void Pause()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _timer.Stop();
                OnTimerStateChanged();
            }
        }

        public void Reset()
        {
            _isRunning = false;
            _timer.Stop();
            _completedPomodoros = 0;
            _currentState = TimerState.Work;
            _timeLeft = GetWorkTime();
            OnTimerStateChanged();
        }

        public void SkipBreak()
        {
            if (_currentState != TimerState.Work)
            {
                _previousState = _currentState;
                _currentState = TimerState.Work;
                _timeLeft = GetWorkTime();
                BreakSkipped?.Invoke(this, EventArgs.Empty);
                OnTimerStateChanged();
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_isRunning)
            {
                _timeLeft = _timeLeft.Subtract(TimeSpan.FromMilliseconds(100));

                if (_timeLeft <= TimeSpan.Zero)
                {
                    HandleTimerCompletion();
                }

                TimeUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleTimerCompletion()
        {
            _timeLeft = TimeSpan.Zero;
            SystemSounds.Beep.Play();

            _previousState = _currentState; // 保存之前的状态

            switch (_currentState)
            {
                case TimerState.Work:
                    _completedPomodoros++;
                    var breakType = (_completedPomodoros % 4 == 0) ? BreakType.LongBreak : BreakType.ShortBreak;

                    // 触发完成事件
                    PomodoroCompleted?.Invoke(this, new PomodoroCompletedEventArgs(_completedPomodoros));

                    // 触发休息开始事件
                    BreakStarted?.Invoke(this, new BreakStartedEventArgs(breakType));

                    // 自动开始休息计时
                    _currentState = (breakType == BreakType.ShortBreak) ? TimerState.ShortBreak : TimerState.LongBreak;
                    _timeLeft = GetBreakTime(breakType);
                    break;

                case TimerState.ShortBreak:
                case TimerState.LongBreak:
                    // 休息结束，自动开始下一个工作周期
                    _currentState = TimerState.Work;
                    _timeLeft = GetWorkTime();
                    SystemSounds.Beep.Play();
                    break;
            }

            OnTimerStateChanged();
        }

        private void InitializeTimer()
        {
            _currentState = TimerState.Work;
            _previousState = TimerState.Work;
            _timeLeft = GetWorkTime();
            _isRunning = false;
        }

        private TimeSpan GetWorkTime()
        {
            return new TimeSpan(0, Settings.WorkTimeMinutes, Settings.WorkTimeSeconds);
        }

        private TimeSpan GetBreakTime(BreakType breakType)
        {
            return breakType == BreakType.ShortBreak
                ? new TimeSpan(0, Settings.ShortBreakMinutes, Settings.ShortBreakSeconds)
                : new TimeSpan(0, Settings.LongBreakMinutes, Settings.LongBreakSeconds);
        }

        private void OnTimerStateChanged()
        {
            TimerStateChanged?.Invoke(this, new TimerStateChangedEventArgs(_currentState, _previousState, _isRunning, _timeLeft));
        }

        // 公共属性
        public bool IsRunning => _isRunning;
        public TimeSpan TimeLeft => _timeLeft;
        public int CompletedPomodoros => _completedPomodoros;
        public TimerState CurrentState => _currentState;
        public TimerState PreviousState => _previousState;
    }

    // 枚举和事件参数类
    public enum TimerState
    {
        Work,
        ShortBreak,
        LongBreak
    }

    public enum BreakType
    {
        ShortBreak,
        LongBreak
    }

    public class TimeSettings
    {
        public int WorkTimeMinutes { get; set; } = 25;
        public int WorkTimeSeconds { get; set; } = 0;
        public int ShortBreakMinutes { get; set; } = 5;
        public int ShortBreakSeconds { get; set; } = 0;
        public int LongBreakMinutes { get; set; } = 15;
        public int LongBreakSeconds { get; set; } = 0;
    }

    public class TimerStateChangedEventArgs : EventArgs
    {
        public TimerState State
        {
            get;
        }
        public TimerState PreviousState
        {
            get;
        }
        public bool IsRunning
        {
            get;
        }
        public TimeSpan TimeLeft
        {
            get;
        }

        public TimerStateChangedEventArgs(TimerState state, TimerState previousState, bool isRunning, TimeSpan timeLeft)
        {
            State = state;
            PreviousState = previousState;
            IsRunning = isRunning;
            TimeLeft = timeLeft;
        }
    }

    public class PomodoroCompletedEventArgs : EventArgs
    {
        public int CompletedPomodoros
        {
            get;
        }

        public PomodoroCompletedEventArgs(int completedPomodoros)
        {
            CompletedPomodoros = completedPomodoros;
        }
    }

    public class BreakStartedEventArgs : EventArgs
    {
        public BreakType BreakType
        {
            get;
        }

        public BreakStartedEventArgs(BreakType breakType)
        {
            BreakType = breakType;
        }
    }
}