using System;
using System.Collections.Generic;
using System.Drawing;
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

        // 运行统计数据
        private int runCount = 0;
        private List<TimeSpan> runHistory = new List<TimeSpan>();
        private TimeSpan fastestTime = TimeSpan.MaxValue;

        // 事件
        public event EventHandler? TimerStateChanged;

        // 公共属性
        public bool IsTimerRunning => isTimerRunning;
        public Data.CharacterProfile? CurrentProfile => currentProfile;

        public TimerControl()
        {
            InitializeComponent();
            InitializeTimer();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            // 状态指示按钮
            btnStatusIndicator = new Button();
            
            // 主要计时显示标签
            lblTimeDisplay = new Label();
            
            // 运行统计信息
            lblRunCount = new Label();
            lblFastestTime = new Label();
            lblAverageTime = new Label();
            
            // 历史记录区域
            lstRunHistory = new ListBox();
            
            SuspendLayout();
            // 
            // btnStatusIndicator - 状态指示按钮
            // 
            btnStatusIndicator.Enabled = false;
            btnStatusIndicator.FlatStyle = FlatStyle.Flat;
            btnStatusIndicator.Size = new Size(16, 16);
            btnStatusIndicator.Location = new Point(15, 45);
            btnStatusIndicator.Name = "btnStatusIndicator";
            btnStatusIndicator.TabIndex = 0;
            btnStatusIndicator.TabStop = false;
            btnStatusIndicator.BackColor = Color.Red;
            btnStatusIndicator.FlatAppearance.BorderSize = 0;
            
            // 
            // lblTimeDisplay - 计时显示
            // 
            lblTimeDisplay.AutoSize = true;
            lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 36F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblTimeDisplay.Location = new Point(40, 30);
            lblTimeDisplay.Name = "lblTimeDisplay";
            lblTimeDisplay.Size = new Size(306, 64);
            lblTimeDisplay.TabIndex = 1;
            lblTimeDisplay.Text = "00:00:00:0";
            
            // 
            // lblRunCount - 运行次数显示
            // 
            lblRunCount.AutoSize = true;
            lblRunCount.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblRunCount.Location = new Point(15, 100);
            lblRunCount.Name = "lblRunCount";
            lblRunCount.Size = new Size(150, 21);
            lblRunCount.TabIndex = 2;
            lblRunCount.Text = "--- Run count 0 (0) ---";
            
            // 
            // lblFastestTime - 最快时间显示
            // 
            lblFastestTime.AutoSize = true;
            lblFastestTime.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblFastestTime.Location = new Point(15, 125);
            lblFastestTime.Name = "lblFastestTime";
            lblFastestTime.Size = new Size(120, 19);
            lblFastestTime.TabIndex = 3;
            lblFastestTime.Text = "Fastest time: --:--:--.-";
            
            // 
            // lblAverageTime - 平均时间显示
            // 
            lblAverageTime.AutoSize = true;
            lblAverageTime.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblAverageTime.Location = new Point(15, 145);
            lblAverageTime.Name = "lblAverageTime";
            lblAverageTime.Size = new Size(125, 19);
            lblAverageTime.TabIndex = 4;
            lblAverageTime.Text = "Average time: --:--:--.-";
            
            // 
            // lstRunHistory - 历史记录列表
            // 
            lstRunHistory.FormattingEnabled = true;
            lstRunHistory.ItemHeight = 15;
            lstRunHistory.Location = new Point(15, 170);
            lstRunHistory.Name = "lstRunHistory";
            lstRunHistory.Size = new Size(290, 139);
            lstRunHistory.TabIndex = 5;
            
            // 
            // TimerControl - 主控件设置
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnStatusIndicator);
            Controls.Add(lblTimeDisplay);
            Controls.Add(lblRunCount);
            Controls.Add(lblFastestTime);
            Controls.Add(lblAverageTime);
            Controls.Add(lstRunHistory);
            Name = "TimerControl";
            Size = new Size(320, 320);
            ResumeLayout(false);
            PerformLayout();
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 100; // 100毫秒
            timer.Tick += Timer_Tick;
        }

        public void UpdateUI()
        {
            // 更新状态指示按钮颜色
            if (btnStatusIndicator != null)
            {
                btnStatusIndicator.BackColor = isTimerRunning ? Color.Green : Color.Red;
            }
            
            // 更新时间显示
            if (isTimerRunning && startTime != DateTime.MinValue)
            {
                TimeSpan elapsed;
                
                if (isPaused && pauseStartTime != DateTime.MinValue)
                {
                    // 暂停状态，计算到暂停开始时的时间
                    elapsed = pauseStartTime - startTime - pausedDuration;
                }
                else
                {
                    // 运行状态，计算实际经过时间（扣除暂停时间）
                    elapsed = DateTime.Now - startTime - pausedDuration;
                }
                
                // 显示100毫秒格式
                string formattedTime = string.Format("{0:00}:{1:00}:{2:00}:{3}", 
                    elapsed.Hours, elapsed.Minutes, elapsed.Seconds, 
                    (int)(elapsed.Milliseconds / 100));
                    
                if (lblTimeDisplay != null) 
                {
                    // 根据时间长度调整字体大小确保显示完整
                    if (elapsed.Hours > 9)
                    {
                        lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 30F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                    }
                    else
                    {
                        lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 36F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                    }
                    
                    // 暂停时显示不同的样式
                    if (isPaused)
                    {
                        lblTimeDisplay.Text = formattedTime;
                    }
                    else
                    {
                        lblTimeDisplay.Text = formattedTime;
                    }
                }
            }
            else
            {
                if (lblTimeDisplay != null) 
                {
                    lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 36F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                    lblTimeDisplay.Text = "00:00:00:0";
                }
            }
            
            // 更新统计信息
            if (lblRunCount != null)
            {
                lblRunCount.Text = $"--- Run count {runCount} ({runCount}) ---";
            }
            
            if (lblFastestTime != null)
            {
                if (runCount > 0 && fastestTime != TimeSpan.MaxValue)
                {
                    string fastestFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}", 
                        fastestTime.Hours, fastestTime.Minutes, fastestTime.Seconds, 
                        (int)(fastestTime.Milliseconds / 100));
                    lblFastestTime.Text = $"Fastest time: {fastestFormatted}";
                }
                else
                {
                    lblFastestTime.Text = "Fastest time: --:--:--.-";
                }
            }
            
            if (lblAverageTime != null)
            {
                if (runCount > 0)
                {
                    TimeSpan averageTime = TimeSpan.Zero;
                    foreach (var time in runHistory)
                    {
                        averageTime += time;
                    }
                    averageTime = new TimeSpan(averageTime.Ticks / runHistory.Count);
                    
                    string averageFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}", 
                        averageTime.Hours, averageTime.Minutes, averageTime.Seconds, 
                        (int)(averageTime.Milliseconds / 100));
                    lblAverageTime.Text = $"Average time: {averageFormatted}";
                }
                else
                {
                    lblAverageTime.Text = "Average time: --:--:--.-";
                }
            }
            
            // 更新历史记录列表
            if (lstRunHistory != null)
            {
                lstRunHistory.Items.Clear();
                for (int i = 0; i < runHistory.Count; i++)
                {
                    var time = runHistory[i];
                    string timeFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}", 
                        time.Hours, time.Minutes, time.Seconds, 
                        (int)(time.Milliseconds / 100));
                    lstRunHistory.Items.Add($"Run {i + 1}: {timeFormatted}");
                }
                
                // 确保最新记录在顶部
                if (lstRunHistory.Items.Count > 0)
                {
                    lstRunHistory.SelectedIndex = 0;
                }
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        // 提供给外部调用的开始/停止方法，用于快捷键触发
        public void ToggleTimer()
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
        
        // 提供给外部调用的暂停方法，用于快捷键触发
        public void TogglePause()
        {
            if (isTimerRunning)
            {
                if (isPaused)
                {
                    ResumeTimer();
                }
                else
                {
                    PauseTimer();
                }
            }
        }
        
        private void PauseTimer()
        {
            if (isTimerRunning && !isPaused)
            {
                isPaused = true;
                pauseStartTime = DateTime.Now;
                UpdateUI();
            }
        }
        
        private void ResumeTimer()
        {
            if (isTimerRunning && isPaused && pauseStartTime != DateTime.MinValue)
            {
                pausedDuration += DateTime.Now - pauseStartTime;
                isPaused = false;
                pauseStartTime = DateTime.MinValue;
                UpdateUI();
            }
        }
        
        // 提供给外部调用的重置方法
        public void ResetTimerExternally()
        {
            ResetTimer();
        }

        private void StartTimer()
        {
            isTimerRunning = true;
            isPaused = false;
            startTime = DateTime.Now;
            pausedDuration = TimeSpan.Zero;
            pauseStartTime = DateTime.MinValue;
            timer?.Start();
            
            UpdateUI();
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void StopTimer()
        {
            isTimerRunning = false;
            isPaused = false;
            timer?.Stop();
            
            // 记录本次运行时间
            if (startTime != DateTime.MinValue)
            {
                TimeSpan runTime = DateTime.Now - startTime - pausedDuration;
                runHistory.Insert(0, runTime); // 新记录插入到开头
                runCount++;
                
                // 更新最快时间
                if (runTime < fastestTime)
                {
                    fastestTime = runTime;
                }
            }
            
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
            pausedDuration = TimeSpan.Zero;
            pauseStartTime = DateTime.MinValue;
            UpdateUI();
        }

        public void SetCurrentProfile(Data.CharacterProfile? profile)
        {
            currentProfile = profile;
            UpdateUI();
        }

        // 控件字段定义
        private Button? btnStatusIndicator;
        private Label? lblTimeDisplay;
        private Label? lblRunCount;
        private Label? lblFastestTime;
        private Label? lblAverageTime;
        private ListBox? lstRunHistory;
        
        // 计时器状态字段
        private bool isPaused = false;
        private TimeSpan pausedDuration = TimeSpan.Zero;
        private DateTime pauseStartTime = DateTime.MinValue;
    }
}