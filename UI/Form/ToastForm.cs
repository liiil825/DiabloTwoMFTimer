using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Form;

public class ToastForm : System.Windows.Forms.Form
{
    private System.Windows.Forms.Timer _timerLife = null!;
    private System.Windows.Forms.Timer _timerAnim = null!;
    private int _lifeTime = 3000; // 默认显示3秒
    private bool _isClosing = false;

    // UI 组件
    // private Label? lblTitle; // 移除标题
    private Label? lblMessage;
    private Panel? pnlColorStrip;

    // 移除 title 参数，因为它不再被显示
    public ToastForm(string message, ToastType type)
    {
        this.StartPosition = FormStartPosition.Manual;

        // 基础窗体设置
        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;
        this.ShowInTaskbar = false;

        // --- 尺寸设置 ---
        // 增加宽度，高度设置为适应两行文字的大致高度 (例如 60px)
        // 使用 ScaleHelper 确保在高分屏下大小合适
        int w = ScaleHelper.Scale(380);
        int h = ScaleHelper.Scale(60);
        this.Size = new Size(w, h);

        this.BackColor = AppTheme.BackColor;
        this.Opacity = 0;

        // 绘制边框
        this.Paint += (s, e) =>
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, AppTheme.BorderColor, ButtonBorderStyle.Solid);
        };

        InitializeControls(message, type);

        _timerAnim = new System.Windows.Forms.Timer { Interval = 20 };
        _timerAnim.Tick += TimerAnim_Tick;

        _timerLife = new System.Windows.Forms.Timer { Interval = _lifeTime };
        _timerLife.Tick += (s, e) =>
        {
            CloseToast();
        };
    }

    protected override bool ShowWithoutActivation => true;

    private void InitializeControls(string message, ToastType type)
    {
        // 1. 确定颜色
        Color typeColor = AppTheme.TextColor;
        switch (type)
        {
            case ToastType.Success:
                typeColor = AppTheme.SetColor;
                break; // 绿
            case ToastType.Warning:
                typeColor = AppTheme.RareColor;
                break; // 黄
            case ToastType.Info:
                typeColor = AppTheme.MagicColor;
                break; // 蓝
            case ToastType.Error:
                typeColor = AppTheme.ErrorColor;
                break; // 红
        }

        // 2. 左侧色条
        pnlColorStrip = new Panel
        {
            Dock = DockStyle.Left,
            Width = ScaleHelper.Scale(6), // 稍微加宽一点点
            BackColor = typeColor,
        };

        // 3. 消息内容
        lblMessage = new Label
        {
            Text = message,
            Font = AppTheme.MainFont,
            ForeColor = AppTheme.TextColor,

            // --- 核心布局逻辑 ---
            AutoSize = false, // 必须关闭 AutoSize 才能控制换行和省略号
            AutoEllipsis = true, // 超出显示 ...
            TextAlign = ContentAlignment.MiddleLeft, // 垂直居中，左对齐

            // 填充剩余空间
            Dock = DockStyle.Fill,
        };

        // 计算 padding 以实现 "上下合适的 Margin"
        // 左侧留出与色条的间距，右侧留出边距
        int hPadding = ScaleHelper.Scale(12);
        int vPadding = ScaleHelper.Scale(8);
        lblMessage.Padding = new Padding(hPadding, vPadding, hPadding, vPadding);

        // 强制限制高度逻辑 (虽然 Form 高度限制了，但为了保险起见)
        // 如果你需要非常严格的“最多2行”，可以通过测量字体高度来微调 Form 的高度
        // 当前 Form 高度 60px - 上下边框/Padding，大约能容纳 2 行 10pt 的文字

        // 点击关闭事件
        this.Click += (s, e) => CloseToast();
        lblMessage.Click += (s, e) => CloseToast();
        pnlColorStrip.Click += (s, e) => CloseToast();

        // 添加控件 (注意顺序，先 Dock Left，再 Dock Fill)
        this.Controls.Add(lblMessage);
        this.Controls.Add(pnlColorStrip);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        _timerAnim.Start();
        _timerLife.Start();
    }

    private void TimerAnim_Tick(object? sender, EventArgs e)
    {
        if (_isClosing)
        {
            this.Opacity -= 0.1;
            if (this.Opacity <= 0)
            {
                _timerAnim.Stop();
                this.Close();
            }
        }
        else
        {
            if (this.Opacity < 1)
                this.Opacity += 0.1;
            else
                _timerAnim.Stop();
        }
    }

    private void CloseToast()
    {
        if (!_isClosing) // 防止多次触发
        {
            _isClosing = true;
            _timerLife.Stop();
            _timerAnim.Start();
        }
    }
}
