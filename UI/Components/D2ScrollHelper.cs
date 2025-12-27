using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Components;

public static class D2ScrollHelper
{
    // ==========================================
    // P/Invoke definitions for RichTextBox
    // ==========================================
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, ref Point lParam);

    private const int WM_USER = 0x400;
    private const int EM_GETSCROLLPOS = WM_USER + 221;
    private const int EM_SETSCROLLPOS = WM_USER + 222;

    /// <summary>
    /// 将 D2 风格滚动条附加到指定的 DataGridView 上
    /// </summary>
    public static void Attach(DataGridView grid, Control parent)
    {
        if (grid.Tag is D2ScrollBar)
            return;

        var scrollBar = new D2ScrollBar
        {
            Dock = DockStyle.Right,
            Width = 10,
            Padding = new Padding(0, grid.ColumnHeadersHeight, 0, 0),
        };

        parent.Controls.Add(scrollBar);
        scrollBar.SendToBack();
        grid.BringToFront();

        grid.ScrollBars = ScrollBars.None;
        grid.Tag = scrollBar;

        // Events
        scrollBar.Scroll += (s, e) =>
        {
            if (grid.RowCount > 0)
            {
                try
                {
                    int targetRow = Math.Min(scrollBar.Value, grid.RowCount - 1);
                    grid.FirstDisplayedScrollingRowIndex = targetRow;
                }
                catch { }
            }
        };

        grid.Scroll += (s, e) =>
        {
            if (grid.FirstDisplayedScrollingRowIndex >= 0)
            {
                if (Math.Abs(scrollBar.Value - grid.FirstDisplayedScrollingRowIndex) > 0)
                    scrollBar.Value = grid.FirstDisplayedScrollingRowIndex;
            }
        };

        grid.MouseWheel += (s, e) =>
        {
            if (scrollBar.Maximum <= 0)
                return;
            int step = (e.Delta / 120) * -1;
            scrollBar.Value += step;
        };

        EventHandler updateParams = (s, e) => UpdateScrollBarParams(grid, scrollBar);
        grid.RowsAdded += (s, e) => UpdateScrollBarParams(grid, scrollBar);
        grid.RowsRemoved += (s, e) => UpdateScrollBarParams(grid, scrollBar);
        grid.Resize += (s, e) => UpdateScrollBarParams(grid, scrollBar);

        UpdateScrollBarParams(grid, scrollBar);
    }

    private static void UpdateScrollBarParams(DataGridView grid, D2ScrollBar scrollBar)
    {
        int topOffset = grid.ColumnHeadersHeight;
        scrollBar.Padding = new Padding(0, topOffset, 0, 0);

        int visibleRows = grid.DisplayedRowCount(false);

        if (grid.RowCount > 0)
            scrollBar.Maximum = grid.RowCount - 1;
        else
            scrollBar.Maximum = 0;

        scrollBar.LargeChange = visibleRows > 0 ? visibleRows : 1;
        scrollBar.Visible = grid.RowCount > visibleRows;
    }

    /// <summary>
    /// 【新增】将 D2 风格滚动条附加到 RichTextBox 上
    /// </summary>
    public static void Attach(RichTextBox rtb, Control parent)
    {
        if (rtb.Tag is D2ScrollBar)
            return;

        // 1. 创建并配置滚动条
        var scrollBar = new D2ScrollBar
        {
            Dock = DockStyle.Right,
            Width = 10,
            Padding = new Padding(0), // RTB 通常不需要顶部 Padding
        };

        // 2. 布局
        parent.Controls.Add(scrollBar);
        scrollBar.SendToBack(); // 沉底，占据右侧
        rtb.BringToFront(); // RTB 浮起，填充剩余

        // 3. 禁用原生滚动条
        rtb.ScrollBars = RichTextBoxScrollBars.None;
        rtb.Tag = scrollBar;

        // 4. 事件绑定

        // A. 滚动条 -> RTB (拖动滑块)
        scrollBar.Scroll += (s, e) =>
        {
            SetRTBScrollPos(rtb, scrollBar.Value);
        };

        // B. RTB -> 滚动条 (鼠标滚轮)
        rtb.MouseWheel += (s, e) =>
        {
            if (!scrollBar.Visible)
                return;
            // 每次滚动约 40 像素，可根据需要调整手感
            int step = (e.Delta / 120) * -40;
            scrollBar.Value += step; // 这里会自动触发 Scroll 事件，反过来更新 RTB
        };

        // C. RTB -> 滚动条 (键盘光标移动导致的内容滚动)
        // SelectionChanged 是捕获光标移动最可靠的事件
        rtb.SelectionChanged += (s, e) =>
        {
            if (!scrollBar.Visible)
                return;

            // 检查当前 RTB 实际滚动位置，如果与滚动条不一致，同步滚动条
            int currentY = GetRTBScrollPos(rtb).Y;
            if (Math.Abs(scrollBar.Value - currentY) > 5) // 5px 容差
            {
                scrollBar.Value = currentY;
            }
        };

        // D. 内容变动或大小变动 -> 更新滚动条参数 (Max, LargeChange)
        EventHandler updateParams = (s, e) => UpdateRTBParams(rtb, scrollBar);

        rtb.ContentsResized += (s, e) => UpdateRTBParams(rtb, scrollBar);
        rtb.Resize += updateParams;
        rtb.TextChanged += updateParams;

        // 初始化
        UpdateRTBParams(rtb, scrollBar);
    }

    private static void UpdateRTBParams(RichTextBox rtb, D2ScrollBar scrollBar)
    {
        // 1. 计算内容总高度
        int totalHeight = 0;

        // 方法：利用 GetPositionFromCharIndex 获取最后一个字符的坐标
        // 注意：该坐标是相对于当前视口左上角的，所以要加上当前的 ScrollPos.Y 才是绝对高度
        if (rtb.TextLength > 0)
        {
            try
            {
                Point lastCharPos = rtb.GetPositionFromCharIndex(rtb.TextLength - 1);
                Point scrollPos = GetRTBScrollPos(rtb);
                int fontHeight = rtb.Font.Height; // 加上最后一行的字体高度
                totalHeight = lastCharPos.Y + scrollPos.Y + fontHeight + 10; // +10 底部留白
            }
            catch
            {
                totalHeight = 0;
            }
        }

        // 2. 视口高度
        int viewHeight = rtb.ClientSize.Height;

        // 3. 设置滚动条参数
        // D2ScrollBar 的逻辑是：Value 可到达 Maximum - LargeChange + 1
        // 对于像素滚动：Maximum 设为内容总高度，LargeChange 设为视口高度
        scrollBar.Maximum = totalHeight;
        scrollBar.LargeChange = viewHeight;

        // 4. 可见性
        scrollBar.Visible = totalHeight > viewHeight;
    }

    private static Point GetRTBScrollPos(RichTextBox rtb)
    {
        Point pt = new Point();
        SendMessage(rtb.Handle, EM_GETSCROLLPOS, IntPtr.Zero, ref pt);
        return pt;
    }

    private static void SetRTBScrollPos(RichTextBox rtb, int y)
    {
        Point pt = new Point(0, y);
        SendMessage(rtb.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref pt);
    }

    /// <summary>
    /// 将 D2 风格滚动条附加到普通 Panel/UserControl 上
    /// </summary>
    public static void Attach(ScrollableControl viewport, Control content)
    {
        if (viewport.Tag is D2ScrollBar)
            return;

        viewport.AutoScroll = false;

        var scrollBar = new D2ScrollBar
        {
            Dock = DockStyle.Right,
            Width = 10,
            Padding = new Padding(0, 0, 0, 0),
        };

        viewport.Controls.Add(scrollBar);
        scrollBar.SendToBack();

        // 确保滚动条在 Z-Order 正确位置 (视需求而定)
        // scrollBar.BringToFront();

        viewport.Tag = scrollBar;

        scrollBar.Scroll += (s, e) =>
        {
            if (content.Height > viewport.Height)
            {
                content.Top = -scrollBar.Value;
            }
        };

        MouseEventHandler wheelHandler = (s, e) =>
        {
            if (!scrollBar.Visible)
                return;
            int step = (e.Delta / 120) * -1 * 40;
            scrollBar.Value += step;
        };
        viewport.MouseWheel += wheelHandler;
        content.MouseWheel += wheelHandler;

        EventHandler updateParams = (s, e) => UpdatePanelParams(viewport, content, scrollBar);

        viewport.Resize += updateParams;
        content.Resize += updateParams;

        UpdatePanelParams(viewport, content, scrollBar);
    }

    private static void UpdatePanelParams(ScrollableControl viewport, Control content, D2ScrollBar scrollBar)
    {
        int availableHeight = viewport.Height;
        int maxScroll = content.Height - availableHeight;

        // 兼容 D2ScrollBar 的像素模式：Maximum 设为内容总高度，LargeChange 设为视口
        // 这样 Value 最大值 = Content - View，正好对应 Top 的最大偏移
        scrollBar.Maximum = content.Height;
        scrollBar.LargeChange = availableHeight;

        if (scrollBar.Maximum < availableHeight)
            scrollBar.Maximum = availableHeight; // 防止负数

        bool canScroll = content.Height > availableHeight;
        scrollBar.Visible = canScroll;

        if (!canScroll)
        {
            content.Top = 0;
            scrollBar.Value = 0;
        }
        else
        {
            if (-content.Top > (content.Height - availableHeight))
            {
                scrollBar.Value = content.Height - availableHeight;
            }
        }
    }
}
