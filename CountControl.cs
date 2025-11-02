using System;
using System.Windows.Forms;
using WinFormsDemo.Resources;

namespace WinFormsDemo
{
    public partial class CountControl : UserControl
    {
        // 计数功能相关字段
        private int count = 0;
        private DateTime startTime;
        private bool isTimerRunning = false;
        private Timer? timer;

        // 事件
        public event EventHandler? TimerStateChanged;

        public CountControl()
        {
            InitializeComponent();
            InitializeTimer();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            this.btnCount = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.lblTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCount
            // 
            this.btnCount.Location = new System.Drawing.Point(46, 20);
            this.btnCount.Name = "btnCount";
            this.btnCount.Size = new System.Drawing.Size(200, 40);
            this.btnCount.TabIndex = 0;
            this.btnCount.UseVisualStyleBackColor = true;
            this.btnCount.Click += new System.EventHandler(this.btnCount_Click);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(46, 73);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(44, 15);
            this.lblCount.TabIndex = 1;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(46, 103);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(200, 40);
            this.btnStartStop.TabIndex = 2;
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(46, 158);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(59, 15);
            this.lblTime.TabIndex = 3;
            // 
            // CountControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCount);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.lblTime);
            this.Name = "CountControl";
            this.Size = new System.Drawing.Size(292, 212);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 100; // 0.1秒
            timer.Tick += Timer_Tick;
        }

        public void UpdateUI()
        {
            // 更新计数功能界面
            btnCount!.Text = LanguageManager.GetString("CountButton");
            btnStartStop!.Text = isTimerRunning ? 
                LanguageManager.GetString("StopButton") : 
                LanguageManager.GetString("StartButton");
            
            // 更新计数显示
            lblCount!.Text = LanguageManager.GetString("CountLabel", count);
            
            // 更新时间显示
            if (isTimerRunning && startTime != DateTime.MinValue)
            {
                TimeSpan elapsed = DateTime.Now - startTime;
                // 格式化为 时:分:秒:十分之一秒
                string formattedTime = string.Format("{0:00}:{1:00}:{2:00}:{3}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds / 100);
                lblTime!.Text = LanguageManager.GetString("TimeLabel", formattedTime);
            }
            else
            {
                lblTime!.Text = LanguageManager.GetString("TimeLabel", "00:00:00:0");
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        private void btnCount_Click(object? sender, EventArgs e)
        {
            count++;
            UpdateUI();
        }

        private void btnStartStop_Click(object? sender, EventArgs e)
        {
            if (isTimerRunning)
            {
                // 停止计时
                isTimerRunning = false;
                timer?.Stop();
                btnStartStop!.Text = LanguageManager.GetString("StartButton");
            }
            else
            {
                // 开始计时
                isTimerRunning = true;
                startTime = DateTime.Now;
                timer?.Start();
                btnStartStop!.Text = LanguageManager.GetString("StopButton");
            }
            
            // 触发事件
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsTimerRunning => isTimerRunning;
        public int Count => count;
        public DateTime StartTime => startTime;
// 控件字段
        private Button? btnCount;
        private Label? lblCount;
        private Button? btnStartStop;
        private Label? lblTime;
    }
}