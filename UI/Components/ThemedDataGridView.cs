using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedDataGridView : DataGridView
{
    public ThemedDataGridView()
    {
        // 1. 基础外观设置
        this.Dock = DockStyle.Fill;
        this.BackgroundColor = AppTheme.BackColor; // 表格背景色
        this.BorderStyle = BorderStyle.None;
        this.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        this.GridColor = Color.FromArgb(60, 60, 60); // 网格线颜色

        // 2. 行为设置
        this.ColumnHeadersVisible = true;
        this.RowHeadersVisible = false;
        this.AllowUserToAddRows = false;
        this.AllowUserToDeleteRows = false;
        this.AllowUserToResizeRows = false;
        this.ReadOnly = true;
        this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.MultiSelect = false;
        this.VirtualMode = true; // 保持虚拟模式

        // 双缓冲，防止闪烁
        this.DoubleBuffered = true;

        // --- 关键：深色主题样式 ---
        // 3. 表头样式 (必须禁用系统样式才能生效)
        this.EnableHeadersVisualStyles = false;
        DataGridViewCellStyle headerStyle = new()
        {
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            BackColor = AppTheme.SurfaceColor, // 表头深灰
            ForeColor = AppTheme.TextSecondaryColor, // 表头文字灰白
            SelectionBackColor = AppTheme.SurfaceColor, // 表头选中不变色
            SelectionForeColor = AppTheme.TextSecondaryColor,
            Font = new Font("微软雅黑", 9F, FontStyle.Bold),
            WrapMode = DataGridViewTriState.True,
        };
        this.ColumnHeadersDefaultCellStyle = headerStyle;
        this.ColumnHeadersHeight = 35; // 调整表头高度
        this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        // 4. 单元格（数据行）样式
        DataGridViewCellStyle cellStyle = new()
        {
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            // Font = new Font("微软雅黑", 10F, FontStyle.Regular),
            Font = new Font("微软雅黑", 9F, FontStyle.Regular),
            BackColor = AppTheme.BackColor, // 数据行深黑
            ForeColor = AppTheme.TextColor, // 数据文字亮白
            SelectionBackColor = AppTheme.AccentColor, // 选中背景（暗金）
            SelectionForeColor = Color.Black, // 选中文字（黑色）
            Padding = new Padding(5, 0, 0, 0),
        };
        this.DefaultCellStyle = cellStyle;

        // 行高设置
        this.RowTemplate.Height = 35; // 调整数据行高度
        // this.RowHeadersWidth = 30; // 调整行头宽度（如果可见）
    }

    // 可以在这里重写 OnPaint 做更高级的绘制，但在 DataGridView 中通常不需要
}
