using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components
{
    public class ThemedTabControl : TabControl
    {
        private int _hoveredIndex = -1;

        public int ActionTabIndex { get; set; } = -1;
        public event EventHandler? ActionTabClicked;

        public ThemedTabControl()
        {
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Padding = new Point(24, 8);
            this.SizeMode = TabSizeMode.Fixed;
            this.ItemSize = new Size(100, 40);

            // 1. 开启双缓冲优化 (注意：TabControl 不能随意开启 AllPaintingInWmPaint，否则内容区会消失)
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, false); // 保持 false，让系统处理 TabPage 切换
        }

        // 2. 核心防闪烁：拦截 WM_ERASEBKGND 消息
        // 禁止系统自动擦除背景，避免“先白后图”导致的闪烁
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0014) // WM_ERASEBKGND
            {
                m.Result = (IntPtr)1;
                return;
            }
            base.WndProc(ref m);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (ActionTabIndex >= 0 && ActionTabIndex < this.TabCount)
            {
                Rectangle rect = this.GetTabRect(ActionTabIndex);
                if (rect.Contains(e.Location))
                {
                    ActionTabClicked?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int newHover = -1;
            for (int i = 0; i < this.TabCount; i++)
            {
                if (this.GetTabRect(i).Contains(e.Location))
                {
                    newHover = i;
                    break;
                }
            }

            if (newHover != _hoveredIndex)
            {
                // 3. 核心优化：局部重绘
                // 只重绘“上一个高亮的Tab”和“当前高亮的Tab”，而不是整个控件

                // 清除旧的高亮
                if (_hoveredIndex != -1 && _hoveredIndex < this.TabCount)
                    this.Invalidate(this.GetTabRect(_hoveredIndex));

                // 绘制新的高亮
                if (newHover != -1 && newHover < this.TabCount)
                    this.Invalidate(this.GetTabRect(newHover));

                _hoveredIndex = newHover;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoveredIndex != -1)
            {
                // 同样只重绘受影响的区域
                if (_hoveredIndex < this.TabCount)
                    this.Invalidate(this.GetTabRect(_hoveredIndex));

                _hoveredIndex = -1;
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            var g = e.Graphics;
            var tabRect = this.GetTabRect(e.Index);
            var page = this.TabPages[e.Index];
            bool isSelected = (this.SelectedIndex == e.Index);
            bool isHovered = (e.Index == _hoveredIndex);
            bool isActionTab = (e.Index == ActionTabIndex);

            Color backColor;
            if (isActionTab && isHovered)
            {
                backColor = Color.FromArgb(232, 17, 35);
            }
            else if (isSelected)
            {
                backColor = AppTheme.SurfaceColor;
            }
            else
            {
                backColor = AppTheme.BackColor;
            }

            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, tabRect);
            }

            if (isSelected && !isActionTab)
            {
                using var pen = new Pen(AppTheme.AccentColor, 3);
                g.DrawLine(pen, tabRect.Left, tabRect.Bottom - 2, tabRect.Right, tabRect.Bottom - 2);
            }

            string text = page.Text;
            var textSize = g.MeasureString(text, this.Font);

            Color textColor;
            if (isActionTab && isHovered)
                textColor = Color.White;
            else if (isSelected)
                textColor = AppTheme.AccentColor;
            else
                textColor = AppTheme.TextSecondaryColor;

            using var textBrush = new SolidBrush(textColor);
            float x = tabRect.X + (tabRect.Width - textSize.Width) / 2;
            float y = tabRect.Y + (tabRect.Height - textSize.Height) / 2;
            g.DrawString(text, this.Font, textBrush, x, y);
        }
    }
}