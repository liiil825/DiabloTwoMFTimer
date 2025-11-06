using System;
using System.Windows.Forms;
using System.Media;
using DTwoMFTimerHelper.Resources;
using DTwoMFTimerHelper.Settings;

namespace DTwoMFTimerHelper
{
    public partial class PomodoroControl : UserControl
    {
        // 番茄时钟相关字段
        private TimeSpan pomodoroTimeLeft;
        private bool isPomodoroRunning = false;
        private int workTimeMinutes = 25; // 番茄时钟时长（分钟）
        private int workTimeSeconds = 0; // 番茄时钟时长（秒）
        private int shortBreakMinutes = 5; // 短休息时长（分钟）
        private int shortBreakSeconds = 0; // 短休息时长（秒）
        private int longBreakMinutes = 15; // 长休息时长（分钟）
        private int longBreakSeconds = 0; // 长休息时长（秒）
        private int completedPomodoros = 0; // 已完成的番茄数
        private Timer? timer;
        private BreakForm? breakForm; // 休息窗口引用
        
        // 状态枚举
        private enum TimerState
        {
            Work,      // 工作状态
            ShortBreak,// 短休息状态
            LongBreak  // 长休息状态
        }
        
        private TimerState currentState = TimerState.Work;

        // 事件
        public event EventHandler? TimerStateChanged;
        public event EventHandler? PomodoroCompleted;

        public PomodoroControl()
        {
            InitializeComponent();
            InitializeTimer();
            
            // 从配置文件加载设置
            LoadSettings();
            
            InitializePomodoro();
            UpdateUI();
        }
        
        private void LoadSettings()
        {
            var settings = SettingsManager.LoadSettings();
            if (settings != null)
            {
                workTimeMinutes = settings.WorkTimeMinutes;
                workTimeSeconds = settings.WorkTimeSeconds;
                shortBreakMinutes = settings.ShortBreakMinutes;
                shortBreakSeconds = settings.ShortBreakSeconds;
                longBreakMinutes = settings.LongBreakMinutes;
                longBreakSeconds = settings.LongBreakSeconds;
            }
        }
        
        private void SaveSettings()
        {
            var settings = SettingsManager.LoadSettings();
            if (settings != null)
            {
                settings.WorkTimeMinutes = workTimeMinutes;
                settings.WorkTimeSeconds = workTimeSeconds;
                settings.ShortBreakMinutes = shortBreakMinutes;
                settings.ShortBreakSeconds = shortBreakSeconds;
                settings.LongBreakMinutes = longBreakMinutes;
                settings.LongBreakSeconds = longBreakSeconds;
                SettingsManager.SaveSettings(settings);
            }
        }

        private void InitializeComponent()
        {
            this.btnPomodoroReset = new System.Windows.Forms.Button();
            this.btnStartPomodoro = new System.Windows.Forms.Button();
            this.lblPomodoroTime = new System.Windows.Forms.Label();
            this.lblPomodoroCount = new System.Windows.Forms.Label();
            this.btnPomodoroSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPomodoroReset
            // 
            this.btnPomodoroReset.Location = new System.Drawing.Point(46, 203);
            this.btnPomodoroReset.Name = "btnPomodoroReset";
            this.btnPomodoroReset.Size = new System.Drawing.Size(200, 40);
            this.btnPomodoroReset.TabIndex = 4;
            this.btnPomodoroReset.UseVisualStyleBackColor = true;
            this.btnPomodoroReset.Click += new System.EventHandler(this.btnPomodoroReset_Click);
            // 
            // btnStartPomodoro
            // 
            this.btnStartPomodoro.Location = new System.Drawing.Point(46, 113);
            this.btnStartPomodoro.Name = "btnStartPomodoro";
            this.btnStartPomodoro.Size = new System.Drawing.Size(200, 40);
            this.btnStartPomodoro.TabIndex = 2;
            this.btnStartPomodoro.UseVisualStyleBackColor = true;
            this.btnStartPomodoro.Click += new System.EventHandler(this.btnStartPomodoro_Click);
            // 
            // btnPomodoroSettings
            // 
            this.btnPomodoroSettings.Location = new System.Drawing.Point(46, 158);
            this.btnPomodoroSettings.Name = "btnPomodoroSettings";
            this.btnPomodoroSettings.Size = new System.Drawing.Size(200, 40);
            this.btnPomodoroSettings.TabIndex = 3;
            this.btnPomodoroSettings.UseVisualStyleBackColor = true;
            this.btnPomodoroSettings.Click += new System.EventHandler(this.btnPomodoroSettings_Click);
            // 
            // lblPomodoroTime
            // 
            this.lblPomodoroTime.AutoSize = true;
            this.lblPomodoroTime.Font = new System.Drawing.Font("Microsoft YaHei", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPomodoroTime.Location = new System.Drawing.Point(86, 20);
            this.lblPomodoroTime.Name = "lblPomodoroTime";
            this.lblPomodoroTime.Size = new System.Drawing.Size(122, 30);
            this.lblPomodoroTime.TabIndex = 0;
            this.lblPomodoroTime.Text = "25:00:00:0";
            // 
            // lblPomodoroCount
            // 
            this.lblPomodoroCount.AutoSize = true;
            this.lblPomodoroCount.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPomodoroCount.Location = new System.Drawing.Point(106, 60);
            this.lblPomodoroCount.Name = "lblPomodoroCount";
            this.lblPomodoroCount.Size = new System.Drawing.Size(82, 20);
            this.lblPomodoroCount.TabIndex = 1;
            this.lblPomodoroCount.Text = "0个大番茄";
            // 
            // PomodoroControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblPomodoroCount);
            this.Controls.Add(this.btnPomodoroSettings);
            this.Controls.Add(this.btnPomodoroReset);
            this.Controls.Add(this.btnStartPomodoro);
            this.Controls.Add(this.lblPomodoroTime);
            this.Name = "PomodoroControl";
            this.Size = new System.Drawing.Size(292, 280);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 100; // 0.1秒
            timer.Tick += Timer_Tick;
        }

        private void InitializePomodoro()
        {
            // 初始化番茄时钟时间
            currentState = TimerState.Work;
            pomodoroTimeLeft = new TimeSpan(0, workTimeMinutes, workTimeSeconds);
            isPomodoroRunning = false;
            UpdatePomodoroDisplay();
        }

        public void UpdateUI()
        {
            // 更新番茄时钟界面
            btnStartPomodoro!.Text = isPomodoroRunning ? 
                LanguageManager.GetString("PausePomodoro") : 
                LanguageManager.GetString("StartPomodoro");
            btnPomodoroReset!.Text = LanguageManager.GetString("ResetPomodoro");
            btnPomodoroSettings!.Text = LanguageManager.GetString("Settings") ?? "设置";
            
            // 更新番茄时钟显示
            UpdatePomodoroDisplay();
            
            // 如果有休息窗口，也更新其UI
            if (breakForm != null && !breakForm.IsDisposed)
            {
                breakForm.Invoke((MethodInvoker)delegate
                {
                    breakForm.UpdateUI();
                });
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (isPomodoroRunning)
            {
                pomodoroTimeLeft = pomodoroTimeLeft.Subtract(TimeSpan.FromMilliseconds(100));
                
                if (pomodoroTimeLeft <= TimeSpan.Zero)
                {
                    // 时间结束，切换状态
                            pomodoroTimeLeft = TimeSpan.Zero;
                            
                            // 工作时间结束时不停止计时器，继续进行休息时间计时
                            // 休息时间结束时才停止计时器
                            if (currentState != TimerState.Work)
                            {
                                isPomodoroRunning = false;
                                timer?.Stop();
                            }
                    
                    // 播放提示音
                    SystemSounds.Beep.Play();
                    
                    // 根据当前状态处理
                    switch (currentState)
                    {
                        case TimerState.Work:
                            // 工作时间结束，增加完成的番茄数
                            completedPomodoros++;
                            
                            // 检查是否需要长休息
                            if (completedPomodoros % 4 == 0)
                            {
                                // 长休息
                                ShowBreakForm(longBreakMinutes, BreakForm.BreakType.LongBreak);
                            }
                            else
                            {
                                // 短休息
                                ShowBreakForm(shortBreakMinutes, BreakForm.BreakType.ShortBreak);
                            }
                            break;
                            
                        case TimerState.ShortBreak:
                            case TimerState.LongBreak:
                                // 休息时间结束，返回工作状态
                                currentState = TimerState.Work;
                                pomodoroTimeLeft = new TimeSpan(0, workTimeMinutes, workTimeSeconds);
                            
                            // 播放提示音
                            SystemSounds.Beep.Play();
                            break;
                    }
                    
                    // 触发完成事件
                    PomodoroCompleted?.Invoke(this, EventArgs.Empty);
                    
                    // 更新显示
                    UpdatePomodoroDisplay();
                }
                
                UpdatePomodoroDisplay();
            }
        }

        private void UpdatePomodoroDisplay()
        {
            // 格式化为 分:秒:十分之一秒（只显示一位）
            string formattedTime = string.Format("{0:00}:{1:00}:{2}", 
                pomodoroTimeLeft.Minutes, 
                pomodoroTimeLeft.Seconds, 
                pomodoroTimeLeft.Milliseconds / 100);
            
            // 根据状态设置时间显示颜色
            switch (currentState)
            {
                case TimerState.Work:
                    lblPomodoroTime!.ForeColor = System.Drawing.Color.Black;
                    break;
                case TimerState.ShortBreak:
                case TimerState.LongBreak:
                    lblPomodoroTime!.ForeColor = System.Drawing.Color.Green;
                    break;
            }
            
            lblPomodoroTime!.Text = formattedTime;
            
            // 更新番茄计数显示
            int bigPomodoros = completedPomodoros / 4;
            int smallPomodoros = completedPomodoros % 4;
            
            string countText;
            if (smallPomodoros == 0)
            {
                countText = $"{bigPomodoros}个大番茄";
            }
            else
            {
                countText = $"{bigPomodoros}个大番茄，{smallPomodoros}个小番茄";
            }
            
            lblPomodoroCount!.Text = countText;
        }

        private void btnStartPomodoro_Click(object? sender, EventArgs e)
        {
            if (!isPomodoroRunning)
            {
                // 开始或继续番茄时钟
                isPomodoroRunning = true;
                timer?.Start();
                btnStartPomodoro!.Text = LanguageManager.GetString("PausePomodoro");
                
                // 触发事件
                TimerStateChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // 暂停番茄时钟
                isPomodoroRunning = false;
                timer?.Stop();
                btnStartPomodoro!.Text = LanguageManager.GetString("StartPomodoro");
                
                // 触发事件
                TimerStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void btnPomodoroReset_Click(object? sender, EventArgs e)
        {
            // 重置番茄时钟
            InitializePomodoro();
            UpdateUI();
            
            // 触发事件
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }
        
        private void btnPomodoroSettings_Click(object? sender, EventArgs e)
        {
            // 打开设置窗口
            using (var settingsForm = new PomodoroSettingsForm(workTimeMinutes, workTimeSeconds, shortBreakMinutes, shortBreakSeconds, longBreakMinutes, longBreakSeconds))
            {
                var result = settingsForm.ShowDialog(this.FindForm());
                
                if (result == DialogResult.OK)
                {
                    // 更新时间设置
                    workTimeMinutes = settingsForm.WorkTimeMinutes;
                    workTimeSeconds = settingsForm.WorkTimeSeconds;
                    shortBreakMinutes = settingsForm.ShortBreakMinutes;
                    shortBreakSeconds = settingsForm.ShortBreakSeconds;
                    longBreakMinutes = settingsForm.LongBreakMinutes;
                    longBreakSeconds = settingsForm.LongBreakSeconds;
                    
                    // 保存设置到配置文件
                    SaveSettings();
                    
                    // 如果当前不是运行状态，更新显示的时间
                    if (!isPomodoroRunning && currentState == TimerState.Work)
                    {
                        pomodoroTimeLeft = new TimeSpan(0, workTimeMinutes, workTimeSeconds);
                        UpdatePomodoroDisplay();
                    }
                }
            }
        }
        
        private void ShowBreakForm(int breakDurationMinutes, BreakForm.BreakType breakType)
        {
            // 如果已经有休息窗口打开，先关闭
            if (breakForm != null && !breakForm.IsDisposed)
            {
                breakForm.Close();
            }
            
            // 创建新的休息窗口
            breakForm = new BreakForm(breakDurationMinutes, breakType);
            breakForm.BreakSkipped += BreakForm_BreakSkipped;
            
            // 显示休息窗口（非模态）
            breakForm.Show(this.FindForm());
            
            // 设置相应的休息状态
            currentState = (breakType == BreakForm.BreakType.ShortBreak) ? TimerState.ShortBreak : TimerState.LongBreak;
            
            // 根据休息类型设置时间
            if (breakType == BreakForm.BreakType.ShortBreak)
            {
                pomodoroTimeLeft = new TimeSpan(0, shortBreakMinutes, shortBreakSeconds);
            }
            else
            {
                pomodoroTimeLeft = new TimeSpan(0, longBreakMinutes, longBreakSeconds);
            }
        }
        
        private void BreakForm_BreakSkipped(object? sender, EventArgs e)
        {
            // 跳过休息，直接进入下一个工作状态
            currentState = TimerState.Work;
            pomodoroTimeLeft = new TimeSpan(0, workTimeMinutes, workTimeSeconds);
            
            // 播放提示音
            SystemSounds.Beep.Play();
        }

        public bool IsPomodoroRunning => isPomodoroRunning;
        public TimeSpan PomodoroTimeLeft => pomodoroTimeLeft;

        private Button? btnPomodoroReset;
        private Button? btnStartPomodoro;
        private Button? btnPomodoroSettings;
        private Label? lblPomodoroTime;
        private Label? lblPomodoroCount;
    }
}