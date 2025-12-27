using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer;

public partial class LootHistoryForm : System.Windows.Forms.Form
{
    private readonly IProfileService _profileService;
    private readonly ISceneService _sceneService;
    private readonly IStatisticsService _statisticsService;

    private Button btnToday = null!;
    private Button btnWeek = null!;
    private Button btnCustom = null!;

    private System.Windows.Forms.Timer _fadeInTimer;
    private readonly List<Label> _shortcutBadges = new();
    private bool _showShortcuts = false;

    private enum ViewMode
    {
        Today,
        Week,
        Custom,
    }

    private ViewMode _currentMode = ViewMode.Today;

    public LootHistoryForm(
            IProfileService profileService,
            ISceneService sceneService,
            IStatisticsService statisticsService
        )
    {
        _profileService = profileService;
        _sceneService = sceneService;
        _statisticsService = statisticsService;

        InitializeComponent();
        InitializeToggleButtons();
        ApplyScaledLayout();

        // 绑定滚动条
        D2ScrollHelper.Attach(this.gridLoot, this.pnlGridContainer);

        LanguageManager.OnLanguageChanged += LanguageChanged;

        // Resize 事件负责动态调整 Grid 宽度
        this.Resize += LootHistoryForm_Resize;

        // 键盘支持
        this.KeyPreview = true;
        this.KeyDown += LootHistoryForm_KeyDown;

        AttachKeyBadge(dtpStart, "Q", () => dtpStart.OpenDropdown());
        AttachKeyBadge(dtpEnd, "W", () => dtpEnd.OpenDropdown());
        AttachKeyBadge(btnSearch, "R", () => btnSearch.PerformClick());
        AttachKeyBadge(btnClose, "D", () => btnClose.PerformClick());

        this.CancelButton = btnClose;

        this.Opacity = 0;
        _fadeInTimer = new System.Windows.Forms.Timer { Interval = 15 };
        _fadeInTimer.Tick += FadeInTimer_Tick;

        UpdateLanguageText();
        SwitchMode(ViewMode.Today);
    }

    private void ApplyScaledLayout()
    {
        // 1. Header (固定高度)
        mainLayout.RowStyles[0].Height = ScaleHelper.Scale(110);

        // 2. Date Panel (固定高度 60)
        mainLayout.RowStyles[1] = new RowStyle(SizeType.Absolute, ScaleHelper.Scale(60));

        // 3. 统一日期栏控件高度
        int ctrlHeight = ScaleHelper.Scale(20);

        dtpStart.Size = new Size(ScaleHelper.Scale(160), ctrlHeight);
        dtpEnd.Size = new Size(ScaleHelper.Scale(160), ctrlHeight);

        // --- 核心修复：关闭自动大小，强制应用高度 ---
        btnSearch.AutoSize = false;
        btnSearch.Size = new Size(ScaleHelper.Scale(80), ctrlHeight);

        // 分隔符
        lblSeparator.AutoSize = false;
        lblSeparator.Size = new Size(ScaleHelper.Scale(20), ctrlHeight);
        lblSeparator.TextAlign = ContentAlignment.MiddleCenter;
        lblSeparator.Text = "-";

        // 间距微调
        int spacing = ScaleHelper.Scale(10);
        dtpStart.Margin = new Padding(0, 0, spacing, 0);
        lblSeparator.Margin = new Padding(0, 0, spacing, 0);
        dtpEnd.Margin = new Padding(0, 0, spacing, 0);

        // 4. Grid Container
        pnlGridContainer.Margin = new Padding(0, ScaleHelper.Scale(10), 0, 0);

        // 5. Bottom Buttons
        mainLayout.RowStyles[3] = new RowStyle(SizeType.AutoSize);
        panelButtons.Padding = new Padding(ScaleHelper.Scale(5));
        panelButtons.Margin = new Padding(0, 0, 0, ScaleHelper.Scale(60));
        btnClose.Margin = new Padding(ScaleHelper.Scale(10));
    }

    private void LootHistoryForm_Resize(object? sender, EventArgs e)
    {
        // 1. Header 居中逻辑
        if (headerControl != null && headerControl.TogglePanel != null)
        {
            headerControl.TogglePanel.Left = (headerControl.Width - headerControl.TogglePanel.Width) / 2;
        }

        // 2. Grid 宽度控制逻辑 (还原原版比例)
        if (pnlGridContainer != null)
        {
            // 最大宽度限制，避免在 4K 屏过宽
            int maxGridWidth = ScaleHelper.Scale(1200);
            // 保持左右至少有留白
            int availableWidth = this.ClientSize.Width - ScaleHelper.Scale(60);

            // 计算最终宽度
            int targetWidth = Math.Min(availableWidth, maxGridWidth);

            // 设置宽度 (由于 Anchor=Top|Bottom，TLP 会自动将其水平居中)
            pnlGridContainer.Width = targetWidth;
        }
    }

    private void SwitchMode(ViewMode mode)
    {
        _currentMode = mode;
        UpdateButtonStyles();

        // 切换日期面板可见性
        // 因为 RowStyle 是 Absolute，隐藏 Panel 后，行高度保留，显示为空白
        pnlCustomDate.Visible = (mode == ViewMode.Custom);

        // 同步 Badge 状态
        if (_showShortcuts)
        {
            ToggleShortcutsVisibility(); // 刷新逻辑
        }

        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;

        switch (mode)
        {
            case ViewMode.Today:
                start = DateTime.Today;
                LoadData(start, end);
                break;
            case ViewMode.Week:
                start = _statisticsService.GetStartOfWeek();
                LoadData(start, end);
                break;
            case ViewMode.Custom:
                dtpStart.Value = DateTime.Today.AddDays(-1);
                dtpEnd.Value = DateTime.Now;
                LoadData(dtpStart.Value, dtpEnd.Value);
                break;
        }
    }

    // --- 其余代码 (键盘事件、数据加载等) 保持不变 ---

    private void LootHistoryForm_KeyDown(object? sender, KeyEventArgs e)
    {
        // 注意：E, S, D, F 的导航逻辑已经移至 ProcessCmdKey
        // 这里只需要处理功能性按键
        switch (e.KeyCode)
        {
            case Keys.H:
                ToggleShortcutsVisibility();
                e.SuppressKeyPress = true;
                break;

            case Keys.D1:
            case Keys.NumPad1:
                btnToday?.PerformClick();
                break;
            case Keys.D2:
            case Keys.NumPad2:
                btnWeek?.PerformClick();
                break;
            case Keys.D3:
            case Keys.NumPad3:
                btnCustom?.PerformClick();
                break;

            // Q -> 展开并聚焦开始时间
            case Keys.Q:
                if (pnlCustomDate.Visible)
                {
                    dtpStart.OpenDropdown();
                    e.SuppressKeyPress = true; // 消除提示音
                }
                break;

            // W -> 展开并聚焦结束时间
            case Keys.W:
                if (pnlCustomDate.Visible)
                {
                    dtpEnd.OpenDropdown();
                    e.SuppressKeyPress = true;
                }
                break;

            // R -> 查询
            case Keys.R:
                if (pnlCustomDate.Visible)
                {
                    btnSearch.PerformClick();
                    e.SuppressKeyPress = true;
                }
                break;

            // D -> 关闭窗口
            case Keys.D:
                // 这里不需要额外的逻辑判断！
                // 原理：
                // 1. 如果焦点在时间控件上，ProcessCmdKey 会把 'D' 变成 'Down'，
                //    控件会消耗掉这个 'Down' 消息，所以代码根本不会运行到这里。
                // 2. 如果焦点不在时间控件上，ProcessCmdKey 会放行 'D'，
                //    代码就会运行到这里，执行关闭操作。
                btnClose.PerformClick();
                e.SuppressKeyPress = true;
                break;
        }
    }

    // 核心修复：使用 ProcessCmdKey 进行底层的按键替换
    // 这比 SendKeys.Send 更快、更稳，而且完美解决焦点问题
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // 1. 检查上下文
        if (pnlCustomDate.Visible && (dtpStart.ContainsFocus || dtpEnd.ContainsFocus))
        {
            // 2. 只有在按下键盘消息时才处理 (防止处理 WM_KEYUP 等其他消息导致异常)
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if (msg.Msg == WM_KEYDOWN || msg.Msg == WM_SYSKEYDOWN)
            {
                switch (keyData)
                {
                    // E -> Up
                    case Keys.W:
                        msg.WParam = (IntPtr)Keys.Up; // 【核心修复】直接修改底层消息参数
                        return base.ProcessCmdKey(ref msg, Keys.Up);

                    // D -> Down
                    case Keys.S:
                        msg.WParam = (IntPtr)Keys.Down; // 【核心修复】把 'D' 变成 'Down'
                        return base.ProcessCmdKey(ref msg, Keys.Down);

                    // S -> Left
                    case Keys.A:
                        msg.WParam = (IntPtr)Keys.Left; // 【核心修复】
                        return base.ProcessCmdKey(ref msg, Keys.Left);

                    // F -> Right
                    case Keys.D:
                        msg.WParam = (IntPtr)Keys.Right; // 【核心修复】
                        return base.ProcessCmdKey(ref msg, Keys.Right);
                }
            }
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void ToggleShortcutsVisibility()
    {
        _showShortcuts = !_showShortcuts;
        foreach (var badge in _shortcutBadges)
        {
            // 智能显隐：如果父控件被隐藏了（比如日期栏隐藏了），那么对应的 Badge 也不应该显示
            if (badge.Parent != null && badge.Parent.Visible)
            {
                badge.Visible = _showShortcuts;
            }
            else
            {
                badge.Visible = false;
            }
        }
    }

    private void AttachKeyBadge(Control target, string keyText, Action triggerAction)
    {
        var lblBadge = new Label
        {
            Text = keyText,
            Font = new Font("Consolas", 8F, FontStyle.Bold),
            ForeColor = Color.Gold,
            BackColor = Color.FromArgb(180, 0, 0, 0),
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand,
            Visible = false // 默认隐藏
        };

        // 计算位置：右上角
        // 修复：从 Width - 15 改为 Width - 25，防止贴边被裁剪或遮挡控件图标
        // Y 轴设为 2，稍微留点顶边距
        lblBadge.Location = new Point(target.Width - 25, 2);

        // 保持右上角锚定，以防控件大小改变
        lblBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        lblBadge.Click += (s, e) => triggerAction();

        target.Controls.Add(lblBadge);
        lblBadge.BringToFront();

        _shortcutBadges.Add(lblBadge);
    }

    private void InitializeToggleButtons()
    {
        btnToday = CreateToggleButton("Today", ViewMode.Today);
        AttachKeyBadge(btnToday, "1", () => btnToday.PerformClick());

        btnWeek = CreateToggleButton("Week", ViewMode.Week);
        AttachKeyBadge(btnWeek, "2", () => btnWeek.PerformClick());

        btnCustom = CreateToggleButton("Custom", ViewMode.Custom);
        AttachKeyBadge(btnCustom, "3", () => btnCustom.PerformClick());

        headerControl.AddToggleButton(btnToday);
        headerControl.AddToggleButton(btnWeek);
        headerControl.AddToggleButton(btnCustom);
    }

    private Button CreateToggleButton(string text, ViewMode tag)
    {
        var btn = new Button
        {
            Text = text,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(ScaleHelper.Scale(25), ScaleHelper.Scale(5), ScaleHelper.Scale(25), ScaleHelper.Scale(5)),
            MinimumSize = new Size(0, ScaleHelper.Scale(43)),
            Font = AppTheme.MainFont,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter,
            UseCompatibleTextRendering = true,
            Tag = tag,
        };
        btn.FlatAppearance.BorderSize = 1;
        btn.Click += (s, e) => SwitchMode(tag);
        return btn;
    }

    private void UpdateButtonStyles()
    {
        HighlightButton(btnToday, _currentMode == ViewMode.Today);
        HighlightButton(btnWeek, _currentMode == ViewMode.Week);
        HighlightButton(btnCustom, _currentMode == ViewMode.Custom);
    }

    private void HighlightButton(Button btn, bool isActive)
    {
        if (isActive)
        {
            btn.BackColor = Color.Gray;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderColor = Color.White;
        }
        else
        {
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Color.Gray;
            btn.FlatAppearance.BorderColor = Color.Gray;
        }
    }

    private void LanguageChanged(object? sender, EventArgs e)
    {
        UpdateLanguageText();
    }

    private void UpdateLanguageText()
    {
        this.SafeInvoke(() =>
        {
            headerControl.Title = LanguageManager.GetString("LootHistoryTitle");
            btnToday.Text = LanguageManager.GetString("LootToday");
            btnWeek.Text = LanguageManager.GetString("LootThisWeek");
            btnCustom.Text = LanguageManager.GetString("LootCustom");
            btnSearch.Text = LanguageManager.GetString("LootSearch");
            btnClose.Text = LanguageManager.GetString("LootClose");
            SetupGridColumns();
        });
    }

    private void BtnSearch_Click(object sender, EventArgs e)
    {
        LoadData(dtpStart.Value, dtpEnd.Value);
    }

    private void BtnClose_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void SetupGridColumns()
    {
        gridLoot.Columns.Clear();
        gridLoot.ColumnHeadersHeight = ScaleHelper.Scale(40);
        gridLoot.RowTemplate.Height = ScaleHelper.Scale(35);
        gridLoot.BackgroundColor = Color.FromArgb(32, 32, 32);

        var colTime = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableDropTime"),
            Width = ScaleHelper.Scale(200),
            DataPropertyName = "DropTime",
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "yyyy-MM-dd HH:mm",
                Alignment = DataGridViewContentAlignment.MiddleCenter,
            },
        };

        var colName = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableItemName"),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            DataPropertyName = "Name",
        };
        colName.DefaultCellStyle.Font = new Font(AppTheme.MainFont, FontStyle.Bold);
        colName.DefaultCellStyle.ForeColor = AppTheme.AccentColor;

        var colScene = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableScene"),
            Width = ScaleHelper.Scale(220),
            DataPropertyName = "SceneName",
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
        };

        var colRun = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableRun"),
            Width = ScaleHelper.Scale(100),
            DataPropertyName = "RunCount",
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
        };

        gridLoot.Columns.AddRange([colTime, colName, colScene, colRun]);
    }

    private void LoadData(DateTime start, DateTime end)
    {
        this.SuspendLayout();
        try
        {
            var profile = _profileService.CurrentProfile;
            if (profile == null)
                return;

            var records = profile
                .LootRecords.Where(r => r.DropTime >= start && r.DropTime <= end)
                .OrderByDescending(r => r.DropTime)
                .ToList();

            var displayList = records
                .Select(r => new
                {
                    r.DropTime,
                    r.Name,
                    SceneName = _sceneService.GetLocalizedShortSceneName(r.SceneName),
                    r.RunCount,
                })
                .ToList();

            var bindingSource = new BindingSource();
            bindingSource.DataSource = displayList;
            gridLoot.DataSource = bindingSource;
        }
        finally
        {
            this.ResumeLayout();
        }
    }

    private void FadeInTimer_Tick(object? sender, EventArgs e)
    {
        if (this.Opacity < 1)
        {
            this.Opacity += 0.08;
        }
        else
        {
            this.Opacity = 1;
            _fadeInTimer.Stop();
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        _fadeInTimer.Start();
        // 确保布局刷新
        LootHistoryForm_Resize(this, EventArgs.Empty);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _fadeInTimer.Stop();
        LanguageManager.OnLanguageChanged -= LanguageChanged;
        base.OnFormClosed(e);
    }
}