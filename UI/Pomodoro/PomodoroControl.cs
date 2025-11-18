using System;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Pomodoro
{
    public partial class PomodoroControl : UserControl
    {
        private readonly PomodoroTimerService _timerService;
        private BreakForm? _breakForm;

        // UI控件
        private Button? btnPomodoroReset;
        private Button? btnStartPomodoro;
        private Button? btnPomodoroSettings;
        private Label? lblPomodoroTime;
        private Label? lblPomodoroCount;

        public PomodoroControl(PomodoroTimerService timerService)
        {
            _timerService = timerService;
            InitializeComponent();

            // 初始化计时器服务
            _timerService.TimerStateChanged += TimerService_TimerStateChanged;
            _timerService.PomodoroCompleted += TimerService_PomodoroCompleted;
            _timerService.BreakStarted += TimerService_BreakStarted;
            _timerService.BreakSkipped += TimerService_BreakSkipped;
            _timerService.TimeUpdated += TimerService_TimeUpdated;

            // 从配置文件加载设置
            LoadSettings();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            btnPomodoroReset = new Button();
            btnStartPomodoro = new Button();
            lblPomodoroTime = new Label();
            lblPomodoroCount = new Label();
            btnPomodoroSettings = new Button();
            SuspendLayout();

            // btnPomodoroReset
            btnPomodoroReset.Location = new Point(85, 379);
            btnPomodoroReset.Margin = new Padding(6);
            btnPomodoroReset.Name = "btnPomodoroReset";
            btnPomodoroReset.Size = new Size(371, 75);
            btnPomodoroReset.TabIndex = 4;
            btnPomodoroReset.UseVisualStyleBackColor = true;
            btnPomodoroReset.Click += BtnPomodoroReset_Click;

            // btnStartPomodoro
            btnStartPomodoro.Location = new Point(85, 211);
            btnStartPomodoro.Margin = new Padding(6);
            btnStartPomodoro.Name = "btnStartPomodoro";
            btnStartPomodoro.Size = new Size(371, 75);
            btnStartPomodoro.TabIndex = 2;
            btnStartPomodoro.UseVisualStyleBackColor = true;
            btnStartPomodoro.Click += BtnStartPomodoro_Click;

            // lblPomodoroTime
            lblPomodoroTime.AutoSize = true;
            lblPomodoroTime.Font = new Font("微软雅黑", 16F);
            lblPomodoroTime.Location = new Point(160, 37);
            lblPomodoroTime.Margin = new Padding(6, 0, 6, 0);
            lblPomodoroTime.Name = "lblPomodoroTime";
            lblPomodoroTime.Size = new Size(203, 50);
            lblPomodoroTime.TabIndex = 0;
            lblPomodoroTime.Text = "25:00:00:0";

            // lblPomodoroCount
            lblPomodoroCount.AutoSize = true;
            lblPomodoroCount.Font = new Font("微软雅黑", 10F);
            lblPomodoroCount.Location = new Point(197, 112);
            lblPomodoroCount.Margin = new Padding(6, 0, 6, 0);
            lblPomodoroCount.Name = "lblPomodoroCount";
            lblPomodoroCount.Size = new Size(124, 31);
            lblPomodoroCount.TabIndex = 1;
            lblPomodoroCount.Text = "0个大番茄";

            // btnPomodoroSettings
            btnPomodoroSettings.Location = new Point(85, 295);
            btnPomodoroSettings.Margin = new Padding(6);
            btnPomodoroSettings.Name = "btnPomodoroSettings";
            btnPomodoroSettings.Size = new Size(371, 75);
            btnPomodoroSettings.TabIndex = 3;
            btnPomodoroSettings.UseVisualStyleBackColor = true;
            btnPomodoroSettings.Click += BtnPomodoroSettings_Click;

            // PomodoroControl
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblPomodoroCount);
            Controls.Add(btnPomodoroSettings);
            Controls.Add(btnPomodoroReset);
            Controls.Add(btnStartPomodoro);
            Controls.Add(lblPomodoroTime);
            Margin = new Padding(6);
            Name = "PomodoroControl";
            Size = new Size(549, 562);
            ResumeLayout(false);
            PerformLayout();
        }

        private void LoadSettings()
        {
            var settings = SettingsManager.LoadSettings();
            if (settings != null)
            {
                _timerService.Settings.WorkTimeMinutes = settings.WorkTimeMinutes;
                _timerService.Settings.WorkTimeSeconds = settings.WorkTimeSeconds;
                _timerService.Settings.ShortBreakMinutes = settings.ShortBreakMinutes;
                _timerService.Settings.ShortBreakSeconds = settings.ShortBreakSeconds;
                _timerService.Settings.LongBreakMinutes = settings.LongBreakMinutes;
                _timerService.Settings.LongBreakSeconds = settings.LongBreakSeconds;
            }
        }

        private void SaveSettings()
        {
            var settings = SettingsManager.LoadSettings();
            if (settings != null)
            {
                settings.WorkTimeMinutes = _timerService.Settings.WorkTimeMinutes;
                settings.WorkTimeSeconds = _timerService.Settings.WorkTimeSeconds;
                settings.ShortBreakMinutes = _timerService.Settings.ShortBreakMinutes;
                settings.ShortBreakSeconds = _timerService.Settings.ShortBreakSeconds;
                settings.LongBreakMinutes = _timerService.Settings.LongBreakMinutes;
                settings.LongBreakSeconds = _timerService.Settings.LongBreakSeconds;
                SettingsManager.SaveSettings(settings);
            }
        }

        private void TimerService_TimerStateChanged(object? sender, TimerStateChangedEventArgs e)
        {
            UpdateUI();
        }

        private void TimerService_PomodoroCompleted(object? sender, PomodoroCompletedEventArgs e)
        {
            // 可以在这里处理番茄完成后的逻辑
        }

        private void TimerService_BreakStarted(object? sender, BreakStartedEventArgs e)
        {
            ShowBreakForm(e.BreakType);
        }

        private void TimerService_BreakSkipped(object? sender, EventArgs e)
        {
            // 跳过休息的逻辑已经在服务层处理
        }

        private void TimerService_TimeUpdated(object? sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }

        private void UpdateUI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateUI));
                return;
            }

            btnStartPomodoro!.Text = _timerService.IsRunning ?
                LanguageManager.GetString("PausePomodoro") :
                LanguageManager.GetString("StartPomodoro");
            btnPomodoroReset!.Text = LanguageManager.GetString("ResetPomodoro");
            btnPomodoroSettings!.Text = LanguageManager.GetString("Settings") ?? "设置";

            UpdateTimeDisplay();
            UpdateCountDisplay();
        }

        private void UpdateTimeDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateTimeDisplay));
                return;
            }

            var timeLeft = _timerService.TimeLeft;
            string formattedTime = string.Format("{0:00}:{1:00}:{2}",
                timeLeft.Minutes,
                timeLeft.Seconds,
                timeLeft.Milliseconds / 100);

            lblPomodoroTime!.Text = formattedTime;

            // 根据状态设置颜色
            lblPomodoroTime.ForeColor = _timerService.CurrentState == TimerState.Work ?
                Color.Black : Color.Green;
        }

        private void UpdateCountDisplay()
        {
            int completed = _timerService.CompletedPomodoros;
            int bigPomodoros = completed / 4;
            int smallPomodoros = completed % 4;

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

        private void BtnStartPomodoro_Click(object? sender, EventArgs e)
        {
            if (_timerService.IsRunning)
            {
                _timerService.Pause();
            }
            else
            {
                _timerService.Start();
            }
        }

        private void BtnPomodoroReset_Click(object? sender, EventArgs e)
        {
            _timerService.Reset();
        }

        private void BtnPomodoroSettings_Click(object? sender, EventArgs e)
        {
            using var settingsForm = new PomodoroSettingsForm(
                _timerService.Settings.WorkTimeMinutes,
                _timerService.Settings.WorkTimeSeconds,
                _timerService.Settings.ShortBreakMinutes,
                _timerService.Settings.ShortBreakSeconds,
                _timerService.Settings.LongBreakMinutes,
                _timerService.Settings.LongBreakSeconds);

            if (settingsForm.ShowDialog(this.FindForm()) == DialogResult.OK)
            {
                _timerService.Settings.WorkTimeMinutes = settingsForm.WorkTimeMinutes;
                _timerService.Settings.WorkTimeSeconds = settingsForm.WorkTimeSeconds;
                _timerService.Settings.ShortBreakMinutes = settingsForm.ShortBreakMinutes;
                _timerService.Settings.ShortBreakSeconds = settingsForm.ShortBreakSeconds;
                _timerService.Settings.LongBreakMinutes = settingsForm.LongBreakMinutes;
                _timerService.Settings.LongBreakSeconds = settingsForm.LongBreakSeconds;

                SaveSettings();
                _timerService.Reset();
            }
        }

        private void ShowBreakForm(BreakType breakType)
        {
            if (_breakForm != null && !_breakForm.IsDisposed)
            {
                _breakForm.Close();
            }

            _breakForm = new BreakForm(_timerService, breakType);
            _breakForm.Show(this.FindForm());
        }

        public void RefreshUI()
        {
            UpdateUI();
        }
    }
}