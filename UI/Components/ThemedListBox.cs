using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedListBox : ListBox
{
    public ThemedListBox()
    {
        this.BackColor = AppTheme.SurfaceColor;
        this.ForeColor = AppTheme.TextColor;
        this.BorderStyle = BorderStyle.FixedSingle;
        this.Font = AppTheme.MainFont;
        this.DrawMode = DrawMode.OwnerDrawFixed;
        this.ItemHeight = 24;
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0)
            return;

        e.DrawBackground();
        bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

        // 背景色
        Color backColor = isSelected ? AppTheme.AccentColor : AppTheme.SurfaceColor;
        using (var brush = new SolidBrush(backColor))
        {
            e.Graphics.FillRectangle(brush, e.Bounds);
        }

        // 文本色
        Color textColor = isSelected ? Color.Black : AppTheme.TextColor;
        string text = this.Items[e.Index].ToString() ?? "";

        using (var brush = new SolidBrush(textColor))
        {
            // 垂直居中
            float y = e.Bounds.Y + (e.Bounds.Height - e.Font!.Height) / 2;
            e.Graphics.DrawString(text, e.Font, brush, e.Bounds.X + 5, y);
        }
        // e.DrawFocusRectangle(); // 不需要虚线框
    }
}
