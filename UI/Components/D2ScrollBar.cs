using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Components
{
    public class D2ScrollBar : Control
    {
        // === D2 风格配色 ===
        private Color _thumbColor = ColorTranslator.FromHtml("#C7B377"); // 砂金色
        private Color _trackColor = ColorTranslator.FromHtml("#1E1E1E"); // 深灰色背景
        private Color _hoverColor = ColorTranslator.FromHtml("#D4C495"); // 悬停稍亮

        private int _value = 0;
        private int _maximum = 100;
        private int _largeChange = 10;
        private int _thumbHeight = 20;
        private bool _isDragging = false;
        private int _clickPointY;
        private int _thumbRectY;

        public event EventHandler? Scroll;

        public D2ScrollBar()
        {
            this.DoubleBuffered = true;
            this.Width = 10; // 细长风格
            this.BackColor = _trackColor;
            this.Cursor = Cursors.Default;
        }

        // === 核心属性 ===
        public int Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = Math.Max(0, Math.Min(value, _maximum - _largeChange + 1)); // 范围限制
                Invalidate();
                Scroll?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                Invalidate();
            }
        }

        public int LargeChange
        {
            get => _largeChange;
            set
            {
                _largeChange = value;
                Invalidate();
            }
        }

        // === 绘图逻辑 (修改后支持 Padding) ===
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            // 1. 绘制滑道 (Track) - 依然画满背景，或者你可以选择只画 Padding 内部
            using (SolidBrush b = new SolidBrush(_trackColor))
            {
                g.FillRectangle(b, ClientRectangle);
            }

            // 2. 计算滑块位置和高度 (确保 recalculate 逻辑是最新的)
            CalculateThumbDimensions();

            // 【关键修改】滑块的 X 和 Y 都要加上 Padding 的偏移
            Rectangle thumbRect = new Rectangle(
                1 + Padding.Left, // X: 稍微往里缩一点，防止贴边太紧
                _thumbRectY + Padding.Top, // Y: 加上顶部内边距
                Width - 2 - Padding.Horizontal, // Width: 减去左右内边距
                _thumbHeight
            );

            // 3. 绘制滑块 (Thumb)
            Color c = _isDragging ? _hoverColor : _thumbColor;
            using (SolidBrush b = new SolidBrush(c))
            {
                g.FillRectangle(b, thumbRect);
            }

            // 描边 (可选)
            using (Pen p = new Pen(Color.FromArgb(100, 255, 255, 255)))
            {
                g.DrawRectangle(p, thumbRect);
            }
        }

        private void CalculateThumbDimensions()
        {
            // 【关键修改】可用高度要减去上下 Padding
            int trackHeight = Height - Padding.Vertical;
            if (trackHeight <= 0)
                return;

            int scrollableRange = _maximum;
            if (scrollableRange <= 0)
                scrollableRange = 1;

            // 计算滑块高度
            float viewableRatio = (float)_largeChange / _maximum;
            _thumbHeight = Math.Max((int)(trackHeight * viewableRatio), 20);

            // 计算滑块 Y 坐标 (相对 Padding.Top 的偏移量)
            int movableTrackHeight = trackHeight - _thumbHeight;
            int maxScrollValue = _maximum - _largeChange;

            if (maxScrollValue <= 0)
            {
                _thumbRectY = 0;
                return;
            }

            float scrollPercent = (float)_value / maxScrollValue;
            _thumbRectY = (int)(scrollPercent * movableTrackHeight);
        }

        // === 鼠标交互 (也需要考虑 Padding) ===
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            // 判定区域也要加上 Padding 偏移
            Rectangle thumbRect = new Rectangle(0, _thumbRectY + Padding.Top, Width, _thumbHeight);

            if (thumbRect.Contains(e.Location))
            {
                _isDragging = true;
                _clickPointY = e.Y - (_thumbRectY + Padding.Top); // 记录相对滑块顶部的点击位置
            }
            else
            {
                // 点击滑道逻辑
                int trackHeight = Height - Padding.Vertical;
                int movableTrackHeight = trackHeight - _thumbHeight;

                // 点击位置减去顶部 Padding
                int clickY_Relative = e.Y - Padding.Top;

                float clickPercent = (float)clickY_Relative / movableTrackHeight;
                int maxScrollValue = _maximum - _largeChange;
                Value = (int)(clickPercent * maxScrollValue);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                int trackHeight = Height - Padding.Vertical;
                int movableTrackHeight = trackHeight - _thumbHeight;
                if (movableTrackHeight <= 0)
                    return;

                // 计算新的 Y 位置 (减去点击偏移和顶部 Padding)
                int newThumbY = e.Y - _clickPointY - Padding.Top;
                newThumbY = Math.Max(0, Math.Min(newThumbY, movableTrackHeight));

                float scrollPercent = (float)newThumbY / movableTrackHeight;
                int maxScrollValue = _maximum - _largeChange;

                Value = (int)(scrollPercent * maxScrollValue);
            }
        }
    }
}
