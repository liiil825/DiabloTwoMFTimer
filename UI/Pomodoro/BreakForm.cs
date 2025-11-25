using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Models;

namespace DTwoMFTimerHelper.UI.Pomodoro {
    // 定义窗口模式
    public enum BreakFormMode {
        PomodoroBreak, // 休息模式：有倒计时，有Session统计
        StatisticsView // 查看模式：无倒计时，仅查看数据
    }

    public enum StatViewType {
        Session, // 刚才/本轮
        Today,   // 今日
        Week     // 本周
    }

    public partial class BreakForm : Form {
        private readonly PomodoroTimerService _timerService;
        private readonly BreakType _breakType;
        private readonly IProfileService _profileService;
        private readonly TimeSettings _timeSettings;
        private readonly StatisticsService _statsService;
        private readonly BreakFormMode _mode; // 当前窗口模式

        private StatViewType _currentViewType; // 当前显示的统计类型
        private bool _isAutoClosed = false;

        // UI控件
        private Panel pnlHeader;      // 顶部区域（放标题和切换按钮）
        private Label lblTitle;       // 标题
        private FlowLayoutPanel pnlToggles; // 切换按钮容器

        private Button btnToggleSession;
        private Button btnToggleToday;
        private Button btnToggleWeek;

        private Label lblMessage;     // 提示语 (休息模式显示，查看模式隐藏)
        private Label lblStats;       // 统计内容 (多行)
        private Label lblTimer;       // 倒计时 (查看模式隐藏)

        private Button btnClose;
        private Button btnSkip;       // 跳过 (查看模式隐藏)

        private readonly List<string> _shortBreakMessages = new List<string>
        {
            "站起来走两步，活动一下筋骨", "眺望远方，放松一下眼睛", "喝口水，补充水分", "深呼吸，放松肩膀"
        };
        private readonly List<string> _longBreakMessages = new List<string>
        {
            "休息时间长一点，去吃点水果吧", "这一轮辛苦了，彻底放松一下", "即使是奈非天也需要休息"
        };

        // 构造函数
        public BreakForm(PomodoroTimerService timerService, IProfileService profileService, BreakFormMode mode, BreakType breakType = BreakType.ShortBreak) {
            _timerService = timerService;
            _profileService = profileService;
            _mode = mode;
            _breakType = breakType;
            _timeSettings = timerService.Settings;
            _statsService = new StatisticsService();

            // 确定默认显示的视图
            _currentViewType = (_mode == BreakFormMode.PomodoroBreak) ? StatViewType.Session : StatViewType.Today;

            InitializeComponent();
            SetupForm();
            UpdateContent(); // 设置初始文本
            UpdateLayoutState(); // 根据模式显隐控件

            this.BackColor = Color.FromArgb(28, 28, 28);

            // 初始加载数据
            RefreshStatistics();

            // 仅在休息模式下订阅计时事件
            if (_mode == BreakFormMode.PomodoroBreak) {
                _timerService.TimeUpdated += TimerService_TimeUpdated;
                _timerService.TimerStateChanged += TimerService_TimerStateChanged;
            }
        }

        private void InitializeComponent() {
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1024, 768);
            this.Name = "BreakForm";
            this.Text = "统计信息";

            // 1. 顶部 Header 区域
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.Transparent };

            lblTitle = new Label {
                AutoSize = false,
                Size = new Size(300, 40),
                Font = new Font("微软雅黑", 14F, FontStyle.Bold),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = _mode == BreakFormMode.PomodoroBreak ? "REST & RECOVER" : "STATISTICS",
                Location = new Point(20, 20)
            };

            // 切换按钮容器
            pnlToggles = new FlowLayoutPanel {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Location = new Point(20, 60) // 在标题下方
            };

            btnToggleSession = CreateToggleButton("本轮战况", StatViewType.Session);
            btnToggleToday = CreateToggleButton("今日累计", StatViewType.Today);
            btnToggleWeek = CreateToggleButton("本周累计", StatViewType.Week);

            pnlToggles.Controls.Add(btnToggleSession);
            pnlToggles.Controls.Add(btnToggleToday);
            pnlToggles.Controls.Add(btnToggleWeek);

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(pnlToggles);

            // 2. 提示语 (仅休息模式)
            lblMessage = new Label {
                AutoSize = false,
                Size = new Size(800, 80),
                Font = new Font("微软雅黑", 24F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "休息一下"
            };

            // 3. 统计内容 (核心区域)
            lblStats = new Label {
                AutoSize = false,
                Size = new Size(800, 400),
                Font = new Font("Consolas", 12F),
                ForeColor = Color.Gold,
                TextAlign = ContentAlignment.TopCenter,
                Text = "Loading..."
            };

            // 4. 倒计时
            lblTimer = new Label {
                AutoSize = true,
                Font = new Font("Consolas", 20F, FontStyle.Bold),
                ForeColor = Color.LightGreen,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "00:00"
            };

            // 5. 底部按钮
            btnClose = CreateFlatButton("关闭", Color.IndianRed);
            btnClose.Click += (s, e) => this.Close();

            btnSkip = CreateFlatButton("跳过休息", Color.SteelBlue);
            btnSkip.Click += (s, e) => { _timerService.SkipBreak(); this.Close(); };

            // 添加控件
            this.Controls.Add(pnlHeader);
            this.Controls.Add(lblMessage);
            this.Controls.Add(lblStats);
            this.Controls.Add(lblTimer);
            this.Controls.Add(btnSkip);
            this.Controls.Add(btnClose);

            this.FormClosing += BreakForm_FormClosing;
            this.SizeChanged += BreakForm_SizeChanged;
        }

        private Button CreateToggleButton(string text, StatViewType type) {
            var btn = new Button {
                Text = text,
                Size = new Size(120, 35),
                Font = new Font("微软雅黑", 10F),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Tag = type // 存储类型
            };
            btn.FlatAppearance.BorderSize = 1;
            btn.Click += (s, e) => SwitchView(type);
            return btn;
        }

        private Button CreateFlatButton(string text, Color hoverColor) {
            var btn = new Button {
                Text = text,
                Size = new Size(160, 50),
                Font = new Font("微软雅黑", 11F),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(60, 60, 60)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = hoverColor;
            return btn;
        }

        private void SetupForm() {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.DoubleBuffered = true;
        }

        // 根据模式控制控件的显示/隐藏
        private void UpdateLayoutState() {
            if (_mode == BreakFormMode.StatisticsView) {
                lblMessage.Visible = false; // 查看模式不显示“休息一下”
                lblTimer.Visible = false;   // 查看模式不显示倒计时
                btnSkip.Visible = false;    // 查看模式不需要跳过
                btnToggleSession.Visible = false; // 查看模式下，通常看今日/本周，"本轮"概念较弱，也可保留
            }
            else {
                // 休息模式全显
                lblMessage.Visible = true;
                lblTimer.Visible = true;
                btnSkip.Visible = true;
                btnToggleSession.Visible = true;
            }
        }

        private void SwitchView(StatViewType type) {
            _currentViewType = type;
            RefreshStatistics();
        }

        private void UpdateButtonStyles() {
            // 高亮当前选中的按钮
            HighlightButton(btnToggleSession, _currentViewType == StatViewType.Session);
            HighlightButton(btnToggleToday, _currentViewType == StatViewType.Today);
            HighlightButton(btnToggleWeek, _currentViewType == StatViewType.Week);
        }

        private void HighlightButton(Button btn, bool isActive) {
            if (isActive) {
                btn.BackColor = Color.Gray;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = Color.White;
            }
            else {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.Gray;
                btn.FlatAppearance.BorderColor = Color.Gray;
            }
        }

        private void UpdateContent() {
            // 只有休息模式需要随机提示语
            if (_mode == BreakFormMode.PomodoroBreak) {
                var rnd = new Random();
                var list = _breakType == BreakType.ShortBreak ? _shortBreakMessages : _longBreakMessages;
                lblMessage.Text = list[rnd.Next(list.Count)];
            }
        }

        private void RefreshStatistics() {
            UpdateButtonStyles();

            if (_profileService == null || _profileService.CurrentProfile == null) {
                lblStats.Text = "暂无角色数据";
                return;
            }

            string title = "";
            string content = "";

            switch (_currentViewType) {
                case StatViewType.Session:
                    // 计算Session时间
                    DateTime sessionStart;
                    if (_breakType == BreakType.ShortBreak)
                        sessionStart = DateTime.Now.AddMinutes(-_timeSettings.WorkTimeMinutes - 5);
                    else {
                        int cycleMins = (_timeSettings.WorkTimeMinutes * 4) + (_timeSettings.ShortBreakMinutes * 3);
                        sessionStart = DateTime.Now.AddMinutes(-cycleMins - 10);
                    }
                    title = ">>> 本轮战况 <<<";
                    content = _statsService.GetDetailedSummary(_profileService, sessionStart, DateTime.Now);
                    break;

                case StatViewType.Today:
                    title = ">>> 今日战况 <<<";
                    content = _statsService.GetDetailedSummary(_profileService, DateTime.Today, DateTime.Now);
                    break;

                case StatViewType.Week:
                    title = ">>> 本周战况 <<<";
                    content = _statsService.GetDetailedSummary(_profileService, _statsService.GetStartOfWeek(), DateTime.Now);
                    break;
            }

            lblStats.Text = $"{title}\n\n{content}";
        }

        // 布局逻辑 (根据模式动态调整)
        private void BreakForm_SizeChanged(object? sender, EventArgs e) {
            int cx = this.ClientSize.Width / 2;
            int totalH = this.ClientSize.Height;

            // 1. Header (始终在顶部)
            // pnlHeader.Width = this.ClientSize.Width; // Dock Top 自动处理了
            // 调整 Toggle Buttons 居中
            pnlToggles.Left = (this.ClientSize.Width - pnlToggles.Width) / 2;

            int currentY = 120; // Header 下方的起始位置

            // 2. 提示语 (仅休息模式)
            if (_mode == BreakFormMode.PomodoroBreak) {
                lblMessage.Width = this.ClientSize.Width - 100;
                lblMessage.Location = new Point(50, currentY);
                currentY = lblMessage.Bottom + 20;
            }
            else {
                currentY += 40; // 查看模式稍微留点空
            }

            // 3. 统计内容 (占据主要空间)
            lblStats.Width = this.ClientSize.Width - 100;
            // 动态计算高度：底部留给按钮和倒计时
            int bottomMargin = 150;
            lblStats.Height = totalH - currentY - bottomMargin;
            lblStats.Location = new Point(50, currentY);

            // 4. 倒计时 & 按钮
            int btnY = totalH - 100;

            if (_mode == BreakFormMode.PomodoroBreak) {
                lblTimer.Location = new Point(cx - (lblTimer.Width / 2), lblStats.Bottom + 10);

                int spacing = 40;
                int totalBtnW = btnClose.Width + btnSkip.Width + spacing;
                btnSkip.Location = new Point(cx - (totalBtnW / 2), btnY);
                btnClose.Location = new Point(btnSkip.Right + spacing, btnY);
            }
            else {
                // 查看模式只有一个关闭按钮，居中
                btnClose.Location = new Point(cx - (btnClose.Width / 2), btnY);
            }
        }

        // ... Timer 事件处理保持不变 ...
        private void TimerService_TimeUpdated(object? sender, EventArgs e) {
            if (InvokeRequired) { Invoke(new Action<object?, EventArgs>(TimerService_TimeUpdated), sender, e); return; }
            var t = _timerService.TimeLeft;
            lblTimer.Text = $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
            CheckBreakTimeEnded();
        }

        // ... 其他方法保持不变 ...
        private void TimerService_TimerStateChanged(object? sender, TimerStateChangedEventArgs e) {
            // 只有在休息模式才自动关闭
            if (_mode == BreakFormMode.PomodoroBreak) { /* ...原逻辑... */ }
        }

        private void CheckBreakTimeEnded() {
            if (_timerService.TimeLeft <= TimeSpan.Zero && _timerService.CurrentState == TimerState.Work) {
                AutoCloseForm();
            }
        }

        private void AutoCloseForm() {
            if (!_isAutoClosed && !this.IsDisposed) {
                _isAutoClosed = true;
                if (this.InvokeRequired) this.Invoke(new Action(() => this.Close()));
                else this.Close();
            }
        }

        private void BreakForm_FormClosing(object? sender, FormClosingEventArgs e) {
            _timerService.TimeUpdated -= TimerService_TimeUpdated;
            _timerService.TimerStateChanged -= TimerService_TimerStateChanged;
        }
    }
}