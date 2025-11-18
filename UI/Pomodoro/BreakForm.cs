using System;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Pomodoro
{
    public partial class BreakForm : Form
    {
        private readonly PomodoroTimerService _timerService;
        private readonly BreakType _breakType;
        private bool _isAutoClosed = false;

        // UI控件
        private Label? lblBreakMessage;
        private Label? lblBreakTime;
        private Button? btnClose;
        private Button? btnSkipBreak;

        public BreakForm(PomodoroTimerService timerService, BreakType breakType)
        {
            _timerService = timerService;
            _breakType = breakType;

            InitializeComponent();
            SetupForm();
            UpdateUI();

            // 订阅时间更新事件
            _timerService.TimeUpdated += TimerService_TimeUpdated;
            _timerService.TimerStateChanged += TimerService_TimerStateChanged;
        }

        private void InitializeComponent()
        {
            lblBreakMessage = new Label();
            lblBreakTime = new Label();
            btnClose = new Button();
            btnSkipBreak = new Button();
            SuspendLayout();

            // lblBreakMessage
            lblBreakMessage.Dock = DockStyle.Top;
            lblBreakMessage.Font = new Font("微软雅黑", 48F);
            lblBreakMessage.ForeColor = Color.Green;
            lblBreakMessage.Location = new Point(0, 0);
            lblBreakMessage.Margin = new Padding(6, 0, 6, 0);
            lblBreakMessage.Name = "lblBreakMessage";
            lblBreakMessage.Size = new Size(1486, 280);
            lblBreakMessage.TabIndex = 0;
            lblBreakMessage.Text = "休息时间";
            lblBreakMessage.TextAlign = ContentAlignment.MiddleCenter;

            // lblBreakTime
            lblBreakTime.Dock = DockStyle.Fill;
            lblBreakTime.Font = new Font("微软雅黑", 96F);
            lblBreakTime.ForeColor = Color.Green;
            lblBreakTime.Location = new Point(0, 280);
            lblBreakTime.Margin = new Padding(6, 0, 6, 0);
            lblBreakTime.Name = "lblBreakTime";
            lblBreakTime.Size = new Size(1486, 840);
            lblBreakTime.TabIndex = 1;
            lblBreakTime.Text = "05:00:00:0";
            lblBreakTime.TextAlign = ContentAlignment.MiddleCenter;

            // btnClose
            btnClose.Font = new Font("微软雅黑", 12F);
            btnClose.Location = new Point(0, 0);
            btnClose.Margin = new Padding(6);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(223, 93);
            btnClose.TabIndex = 2;
            btnClose.Text = "关闭";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += BtnClose_Click;

            // btnSkipBreak
            btnSkipBreak.Font = new Font("微软雅黑", 12F);
            btnSkipBreak.Location = new Point(0, 0);
            btnSkipBreak.Margin = new Padding(6);
            btnSkipBreak.Name = "btnSkipBreak";
            btnSkipBreak.Size = new Size(223, 93);
            btnSkipBreak.TabIndex = 3;
            btnSkipBreak.Text = "跳过休息";
            btnSkipBreak.UseVisualStyleBackColor = true;
            btnSkipBreak.Click += BtnSkipBreak_Click;

            // BreakForm
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1486, 1120);
            Controls.Add(btnSkipBreak);
            Controls.Add(btnClose);
            Controls.Add(lblBreakTime);
            Controls.Add(lblBreakMessage);
            Margin = new Padding(6);
            Name = "BreakForm";
            Text = "休息时间";
            FormClosing += BreakForm_FormClosing;
            ResumeLayout(false);
        }

        private void SetupForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.DoubleBuffered = true; // 启用双缓冲，解决闪烁问题

            // 添加窗口大小变化事件
            this.SizeChanged += BreakForm_SizeChanged;
        }

        private void TimerService_TimeUpdated(object? sender, EventArgs e)
        {
            UpdateTimeDisplay();

            // 检查休息时间是否结束
            CheckBreakTimeEnded();
        }

        private void TimerService_TimerStateChanged(object? sender, TimerStateChangedEventArgs e)
        {
            // 如果状态从休息切换到工作，自动关闭窗口
            if (e.State == TimerState.Work &&
                ((_breakType == BreakType.ShortBreak && e.PreviousState == TimerState.ShortBreak) ||
                 (_breakType == BreakType.LongBreak && e.PreviousState == TimerState.LongBreak)))
            {
                AutoCloseForm();
            }
        }

        private void CheckBreakTimeEnded()
        {
            // 如果休息时间结束，自动关闭窗口
            if (_timerService.TimeLeft <= TimeSpan.Zero &&
                _timerService.CurrentState == TimerState.Work)
            {
                AutoCloseForm();
            }
        }

        private void AutoCloseForm()
        {
            if (!_isAutoClosed && !this.IsDisposed)
            {
                _isAutoClosed = true;

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => this.Close()));
                }
                else
                {
                    this.Close();
                }
            }
        }

        public void RefreshUI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateUI));
            }
            else
            {
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            this.Text = LanguageManager.GetString("BreakTime") ?? "休息时间";

            if (_breakType == BreakType.ShortBreak)
            {
                lblBreakMessage!.Text = LanguageManager.GetString("ShortBreakMessage") ?? "短休息时间";
            }
            else
            {
                lblBreakMessage!.Text = LanguageManager.GetString("LongBreakMessage") ?? "长休息时间";
            }

            btnClose!.Text = LanguageManager.GetString("Close") ?? "关闭";
            btnSkipBreak!.Text = LanguageManager.GetString("SkipBreak") ?? "跳过休息";

            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateTimeDisplay));
                return;
            }

            var timeLeft = _timerService.TimeLeft;
            string timeText = string.Format("{0:00}:{1:00}:{2}",
                timeLeft.Minutes,
                timeLeft.Seconds,
                timeLeft.Milliseconds / 100);

            lblBreakTime!.Text = timeText;
        }

        private void BreakForm_SizeChanged(object? sender, EventArgs e)
        {
            const int marginRight = 80;
            const int marginBottom = 80;
            const int buttonSpacing = 40;

            if (btnClose != null && btnSkipBreak != null)
            {
                int buttonWidth = btnClose.Width;
                int buttonHeight = btnClose.Height;

                btnClose.Left = this.ClientSize.Width - marginRight - buttonWidth;
                btnClose.Top = this.ClientSize.Height - marginBottom - buttonHeight;

                btnSkipBreak.Left = btnClose.Left - buttonWidth - buttonSpacing;
                btnSkipBreak.Top = btnClose.Top;
            }
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnSkipBreak_Click(object? sender, EventArgs e)
        {
            _timerService.SkipBreak();
            this.Close();
        }

        private void BreakForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _timerService.TimeUpdated -= TimerService_TimeUpdated;
            _timerService.TimerStateChanged -= TimerService_TimerStateChanged;
        }
    }
}