using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms; // 确保引用
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedDateTimePicker : DateTimePicker
{
    private const int WM_PAINT = 0xF;
    private bool _isHovered = false;

    public ThemedDateTimePicker()
    {
        this.Format = DateTimePickerFormat.Custom;
        this.CustomFormat = "yyyy-MM-dd HH:mm";
        this.Font = new Font("微软雅黑", 10F);

        this.CalendarMonthBackground = AppTheme.SurfaceColor;
        this.CalendarTitleBackColor = AppTheme.AccentColor;
        this.CalendarTitleForeColor = Color.Black;
        this.CalendarForeColor = AppTheme.TextColor;
        this.CalendarTrailingForeColor = Color.Gray;

        this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }

    // --- 修改处：改用 F4 模拟 ---
    public void OpenDropdown()
    {
        this.Focus(); // 先获取焦点
        SendKeys.Send("{F4}"); // 模拟 F4 键，这是系统标准的展开快捷键，比 SendMessage 更稳健
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _isHovered = true;
        this.Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _isHovered = false;
        this.Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        if (m.Msg == WM_PAINT)
        {
            using (var g = Graphics.FromHwnd(this.Handle))
            {
                PaintCustomControl(g);
            }
        }
    }

    private void PaintCustomControl(Graphics g)
    {
        // ... (保持原有的绘制代码不变) ...
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        using (var bgBrush = new SolidBrush(AppTheme.SurfaceColor))
        {
            g.FillRectangle(bgBrush, 0, 0, this.Width, this.Height);
        }

        Color borderColor = _isHovered ? AppTheme.AccentColor : AppTheme.BorderColor;
        using (var borderPen = new Pen(borderColor))
        {
            g.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
        }

        int iconWidth = 30;
        Rectangle iconRect = new Rectangle(this.Width - iconWidth, 0, iconWidth, this.Height);

        int arrowSize = 5;
        Point center = new Point(iconRect.X + iconRect.Width / 2, iconRect.Y + iconRect.Height / 2);
        Point[] arrowPoints = new Point[]
        {
            new Point(center.X - arrowSize, center.Y - 2),
            new Point(center.X + arrowSize, center.Y - 2),
            new Point(center.X, center.Y + 3),
        };

        using (var arrowBrush = new SolidBrush(AppTheme.AccentColor))
        {
            g.FillPolygon(arrowBrush, arrowPoints);
        }

        string text = this.Value.ToString(this.CustomFormat);
        Rectangle textRect = new Rectangle(5, 0, this.Width - iconWidth - 5, this.Height);
        TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.SingleLine;

        TextRenderer.DrawText(g, text, this.Font, textRect, AppTheme.TextColor, flags);
    }
}
