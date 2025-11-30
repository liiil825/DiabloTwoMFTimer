using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedCheckBox : CheckBox
{
    public ThemedCheckBox()
    {
        this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        this.Cursor = Cursors.Hand;
        this.Font = AppTheme.MainFont;
        this.ForeColor = AppTheme.TextColor;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        // 1. 绘制方框
        int boxSize = 16;
        int yOffset = (this.Height - boxSize) / 2;
        var boxRect = new Rectangle(0, yOffset, boxSize, boxSize);

        using (var pen = new Pen(AppTheme.AccentColor))
        using (var brush = new SolidBrush(AppTheme.SurfaceColor))
        {
            g.FillRectangle(brush, boxRect);
            g.DrawRectangle(pen, boxRect);
        }

        // 2. 绘制勾选
        if (this.Checked)
        {
            using (var pen = new Pen(AppTheme.AccentColor, 2))
            {
                // 画个对勾
                g.DrawLine(pen, boxRect.Left + 3, boxRect.Top + 8, boxRect.Left + 6, boxRect.Bottom - 4);
                g.DrawLine(pen, boxRect.Left + 6, boxRect.Bottom - 4, boxRect.Right - 3, boxRect.Top + 4);
            }
        }

        // 3. 绘制文字
        using (var brush = new SolidBrush(this.ForeColor))
        {
            g.DrawString(this.Text, this.Font, brush, boxSize + 6, yOffset); // 稍微垂直居中
        }
    }
}