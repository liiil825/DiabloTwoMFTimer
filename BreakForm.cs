using System;
using System.Windows.Forms;
using WinFormsDemo.Resources;

namespace WinFormsDemo
{
    public partial class BreakForm : Form
    {
        // 休息类型枚举
        public enum BreakType
        {
            ShortBreak,
            LongBreak
        }
        
        // 公共属性
        public int RemainingMilliseconds { get; set; } // 修改为毫秒级
        public BreakType CurrentBreakType { get; private set; }
        
        // 事件
        public event EventHandler? BreakSkipped;
        
        private Timer? breakTimer;
        
        public BreakForm(int breakDurationMinutes, BreakType breakType)
        {
            InitializeComponent();
            
            // 设置窗口属性 - 全屏显示
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None; // 无边框
            this.TopMost = true;
            
            // 初始化休息设置
            RemainingMilliseconds = breakDurationMinutes * 60 * 1000; // 转换为毫秒
            CurrentBreakType = breakType;
            
            // 初始化计时器 - 使用100ms间隔更稳定
            breakTimer = new Timer();
            breakTimer.Interval = 100; // 100毫秒
            breakTimer.Tick += BreakTimer_Tick;
            breakTimer.Start();
            
            // 添加窗口大小变化事件，确保按钮始终在正确位置
            this.SizeChanged += BreakForm_SizeChanged;
            
            // 更新界面
            UpdateUI();
        }
        
        private void InitializeComponent()
        {
            this.lblBreakMessage = new System.Windows.Forms.Label();
            this.lblBreakTime = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSkipBreak = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBreakMessage
            // 
            this.lblBreakMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBreakMessage.Font = new System.Drawing.Font("Microsoft YaHei", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblBreakMessage.ForeColor = System.Drawing.Color.Green;
            this.lblBreakMessage.Location = new System.Drawing.Point(0, 0);
            this.lblBreakMessage.Name = "lblBreakMessage";
            this.lblBreakMessage.Size = new System.Drawing.Size(800, 150);
            this.lblBreakMessage.TabIndex = 0;
            this.lblBreakMessage.Text = "休息时间";
            this.lblBreakMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBreakTime
            // 
            this.lblBreakTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBreakTime.Font = new System.Drawing.Font("Microsoft YaHei", 96F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblBreakTime.ForeColor = System.Drawing.Color.Green;
            this.lblBreakTime.Location = new System.Drawing.Point(0, 150);
            this.lblBreakTime.Name = "lblBreakTime";
            this.lblBreakTime.Size = new System.Drawing.Size(800, 450);
            this.lblBreakTime.TabIndex = 1;
            this.lblBreakTime.Text = "05:00.000";
            this.lblBreakTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnClose.Size = new System.Drawing.Size(120, 50);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSkipBreak
            // 
            this.btnSkipBreak.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSkipBreak.Size = new System.Drawing.Size(120, 50);
            this.btnSkipBreak.TabIndex = 3;
            this.btnSkipBreak.Text = "跳过休息";
            this.btnSkipBreak.UseVisualStyleBackColor = true;
            this.btnSkipBreak.Click += new System.EventHandler(this.btnSkipBreak_Click);
            // 
            // BreakForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.btnSkipBreak);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblBreakTime);
            this.Controls.Add(this.lblBreakMessage);
            this.Name = "BreakForm";
            this.Text = "休息时间";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BreakForm_FormClosing);
            this.ResumeLayout(false);
        }
        
        public void UpdateUI()
        {
            // 更新标题
            this.Text = LanguageManager.GetString("BreakTime") ?? "休息时间";
            
            // 更新休息消息
            if (CurrentBreakType == BreakType.ShortBreak)
            {
                lblBreakMessage!.Text = LanguageManager.GetString("ShortBreakMessage") ?? "短休息时间";
            }
            else
            {
                lblBreakMessage!.Text = LanguageManager.GetString("LongBreakMessage") ?? "长休息时间";
            }
            
            // 更新按钮文本
            btnClose!.Text = LanguageManager.GetString("Close") ?? "关闭";
            btnSkipBreak!.Text = LanguageManager.GetString("SkipBreak") ?? "跳过休息";
            
            // 更新倒计时显示
            UpdateBreakTimeDisplay();
        }
        
        private void UpdateBreakTimeDisplay()
        {
            // 计算分、秒和毫秒（只显示几百毫秒）
            int totalSeconds = RemainingMilliseconds / 1000;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            // 只保留百位的毫秒值
            int hundredsOfMilliseconds = (RemainingMilliseconds % 1000) / 100;
            
            // 格式化为 mm:ss:h 其中h表示几百毫秒
            string timeText = string.Format("{0:00}:{1:00}:{2}", minutes, seconds, hundredsOfMilliseconds);
            
            // 确保标签不为空再更新
            if (lblBreakTime != null && !string.IsNullOrEmpty(timeText))
            {
                lblBreakTime.Text = timeText;
            }
        }
        
        private void BreakTimer_Tick(object? sender, EventArgs e)
        {
            if (RemainingMilliseconds > 0)
            {
                RemainingMilliseconds -= 100; // 减去100毫秒，更稳定地更新
                UpdateBreakTimeDisplay();
            }
            else
            {
                // 休息时间结束，关闭窗口
                breakTimer?.Stop();
                this.Close();
            }
        }
        
        private void BreakForm_SizeChanged(object? sender, EventArgs e)
        {
            // 确保按钮始终在右下角，距离右边和下面各80px，按钮间距离40px
            const int marginRight = 80;
            const int marginBottom = 80;
            const int buttonSpacing = 40;
            
            // 检查按钮控件是否为null，避免空引用异常
            if (btnClose != null && btnSkipBreak != null)
            {
                int buttonWidth = btnClose.Width;
                int buttonHeight = btnClose.Height;
                
                // 设置关闭按钮位置
                btnClose.Left = this.ClientSize.Width - marginRight - buttonWidth;
                btnClose.Top = this.ClientSize.Height - marginBottom - buttonHeight;
                
                // 设置跳过休息按钮位置
                btnSkipBreak.Left = btnClose.Left - buttonWidth - buttonSpacing;
                btnSkipBreak.Top = btnClose.Top;
            }
        }
        
        private void btnClose_Click(object? sender, EventArgs e)
        {
            // 暂停计时器
            breakTimer?.Stop();
            // 只关闭窗口，不跳过休息
            this.Close();
        }
        
        private void btnSkipBreak_Click(object? sender, EventArgs e)
        {
            // 暂停计时器
            breakTimer?.Stop();
            // 触发跳过休息事件
            BreakSkipped?.Invoke(this, EventArgs.Empty);
            // 关闭窗口
            this.Close();
        }
        
        private void BreakForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // 确保计时器停止
            breakTimer?.Stop();
        }
        
        private Label? lblBreakMessage;
        private Label? lblBreakTime;
        private Button? btnClose;
        private Button? btnSkipBreak;
    }
}