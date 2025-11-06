using System;
using System.Windows.Forms;
using DTwoMFTimerHelper.Data;

namespace DTwoMFTimerHelper
{
    public partial class TimerControl : UserControl
    {
        // 计时器相关字段
        private bool isTimerRunning = false;
        private DateTime startTime = DateTime.MinValue;
        private System.Windows.Forms.Timer? timer;
        private Data.CharacterProfile? currentProfile = null;

        // 事件
        public event EventHandler? TimerStateChanged;

        // 公共属性
        public bool IsTimerRunning => isTimerRunning;
        public Data.CharacterProfile? CurrentProfile => currentProfile;
        // Removed reference to non-existent currentRecord field

        public TimerControl()
        {
            InitializeComponent();
            InitializeTimer();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            // 主要计时显示标签
            lblTimeDisplay = new Label();
            
            // 控制按钮
            btnStartStop = new Button();
            btnReset = new Button();
            
            // 信息显示标签
            lblCurrentProfile = new Label();
            
            SuspendLayout();
            // 
            // lblTimeDisplay - 大型计时器显示
            // 
            lblTimeDisplay.AutoSize = true;
            lblTimeDisplay.Font = new System.Drawing.Font("Microsoft YaHei UI", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            lblTimeDisplay.Location = new System.Drawing.Point(40, 40);
            lblTimeDisplay.Name = "lblTimeDisplay";
            lblTimeDisplay.Size = new System.Drawing.Size(360, 86);
            lblTimeDisplay.TabIndex = 0;
            lblTimeDisplay.Text = "00:00:00";
            
            // 
            // btnStartStop - 启动/停止按钮
            // 
            btnStartStop.Location = new System.Drawing.Point(40, 200);
            btnStartStop.Name = "btnStartStop";
            btnStartStop.Size = new System.Drawing.Size(100, 50);
            btnStartStop.TabIndex = 1;
            btnStartStop.Text = "开始";
            btnStartStop.UseVisualStyleBackColor = true;
            btnStartStop.Click += btnStartStop_Click;
            
            // 
            // btnReset - 重置按钮
            // 
            btnReset.Location = new System.Drawing.Point(280, 200);
            btnReset.Name = "btnReset";
            btnReset.Size = new System.Drawing.Size(100, 50);
            btnReset.TabIndex = 2;
            btnReset.Text = "重置";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            
            // 
            // lblCurrentProfile - 当前角色显示
            // 
            lblCurrentProfile.AutoSize = true;
            lblCurrentProfile.Location = new System.Drawing.Point(40, 140);
            lblCurrentProfile.Name = "lblCurrentProfile";
            lblCurrentProfile.Size = new System.Drawing.Size(120, 20);
            lblCurrentProfile.TabIndex = 3;
            
            // 
            // TimerControl - 主控件设置
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblTimeDisplay);
            Controls.Add(btnStartStop);
            Controls.Add(btnReset);
            Controls.Add(lblCurrentProfile);
            Name = "TimerControl";
            Size = new System.Drawing.Size(420, 320);
            ResumeLayout(false);
            PerformLayout();
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 1000; // 1秒
            timer.Tick += Timer_Tick;
        }

        public void UpdateUI()
        {
            // 更新按钮文本
            if (btnStartStop != null) btnStartStop.Text = isTimerRunning ? "停止" : "开始";
            
            // 更新当前角色显示
            if (currentProfile != null)
            {
                if (lblCurrentProfile != null) lblCurrentProfile.Text = $"角色: {currentProfile.Name}";
            }
            else
            {
                if (lblCurrentProfile != null) lblCurrentProfile.Text = "角色: 未选择";
            }
            
            // 更新时间显示
            if (isTimerRunning && startTime != DateTime.MinValue)
            {
                TimeSpan elapsed = DateTime.Now - startTime;
                string formattedTime = string.Format("{0:00}:{1:00}:{2:00}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
                if (lblTimeDisplay != null) lblTimeDisplay.Text = formattedTime;
            }
            else
            {
                if (lblTimeDisplay != null) lblTimeDisplay.Text = "00:00:00";
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        private void btnStartStop_Click(object? sender, EventArgs e)
        {
            if (!isTimerRunning)
            {
                StartTimer();
            }
            else
            {
                StopTimer();
            }
        }

        private void btnReset_Click(object? sender, EventArgs e)
        {
            ResetTimer();
        }

        private void StartTimer()
        {
            startTime = DateTime.Now;
            isTimerRunning = true;
            timer?.Start();
            
            UpdateUI();
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void StopTimer()
        {
            isTimerRunning = false;
            timer?.Stop();
            UpdateUI();
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ResetTimer()
        {
            StopTimer();
            ResetTimerDisplay();
        }

        private void ResetTimerDisplay()
        {
            startTime = DateTime.MinValue;
            UpdateUI();
        }

        public void SetCurrentProfile(Data.CharacterProfile? profile)
        {
            currentProfile = profile;
            UpdateUI();
        }

        // 私有字段定义
        // 控件字段定义
        private Label? lblTimeDisplay;
        private Button? btnStartStop;
        private Button? btnReset;
        private Label? lblCurrentProfile;
    }
}