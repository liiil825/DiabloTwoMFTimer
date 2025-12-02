using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.UI.Form;

public class QuickMenuForm : System.Windows.Forms.Form
{
    private List<CommandAction> _commands;

    // 定义颜色常量
    private readonly Color _accentColor = ColorTranslator.FromHtml("#C7B377"); // 流沙金
    private readonly Color _bgColor = Color.FromArgb(32, 32, 32);       // 深灰背景(接近黑)
    private readonly Color _textColor = Color.FromArgb(220, 220, 220);  // 浅灰白文字

    public QuickMenuForm(List<CommandAction> commands)
    {
        _commands = commands;

        // 1. 窗体基础设置
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = _bgColor;
        this.Padding = new Padding(20, 15, 20, 15); // 内边距
        this.KeyPreview = true;
        this.ShowInTaskbar = false;

        // 自动根据内容计算高度 (基础高度 + 每行高度)
        int rowHeight = 30;
        int headerHeight = 40;
        this.Size = new Size(350, headerHeight + (_commands.Count * rowHeight) + 40);

        InitializeFlatUI();
    }

    private void InitializeFlatUI()
    {
        // 2. 顶部提示标题
        Label lblTitle = new Label();
        lblTitle.Text = "WAITING FOR INPUT"; // 英文大写显得更极简/专业
        lblTitle.ForeColor = _accentColor;
        lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold); // 小而精的标题
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Height = 30;
        lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        this.Controls.Add(lblTitle);

        // 分割线 (用一个高度1px的Panel模拟)
        Panel line = new Panel();
        line.Height = 1;
        line.BackColor = Color.FromArgb(60, 60, 60); // 弱化的分割线
        line.Dock = DockStyle.Top;
        this.Controls.Add(line);
        // 加一点间距
        Panel spacer = new Panel { Height = 10, Dock = DockStyle.Top, BackColor = Color.Transparent };
        this.Controls.Add(spacer);

        // 3. 使用 TableLayoutPanel 实现整齐的网格布局
        TableLayoutPanel grid = new TableLayoutPanel();
        grid.Dock = DockStyle.Fill;
        grid.ColumnCount = 2;
        grid.RowCount = _commands.Count;

        // 设置列宽：第一列(按键)固定宽度，第二列(描述)自动填充
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        int currentRow = 0;
        foreach (var cmd in _commands)
        {
            // 左侧：按键 (例如 "S")
            Label lblKey = new Label();
            lblKey.Text = $"[{cmd.TriggerKey}]"; // 或者直接 cmd.TriggerKey.ToString()
            lblKey.ForeColor = _accentColor;     // 按键用金色高亮
            lblKey.Font = new Font("Consolas", 11, FontStyle.Bold); // 等宽字体
            lblKey.TextAlign = ContentAlignment.MiddleLeft;
            lblKey.Dock = DockStyle.Fill;
            lblKey.Margin = new Padding(0);

            // 右侧：描述
            Label lblDesc = new Label();
            lblDesc.Text = cmd.Description;
            lblDesc.ForeColor = _textColor;
            lblDesc.Font = new Font("Segoe UI", 10); // 无衬线字体，易读
            lblDesc.TextAlign = ContentAlignment.MiddleLeft;
            lblDesc.Dock = DockStyle.Fill;
            lblDesc.Margin = new Padding(0);

            // 添加到网格
            grid.Controls.Add(lblKey, 0, currentRow);
            grid.Controls.Add(lblDesc, 1, currentRow);

            currentRow++;
        }

        this.Controls.Add(grid);

        // 确保网格最后被添加到控件列表里（即显示在最下方），
        // 但由于Dock顺序是反的，我们要把 grid BringToFront 
        grid.BringToFront();
    }

    // 4. 手绘金色边框
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // 绘制 1px 的边框
        using (Pen p = new Pen(_accentColor, 1))
        {
            // 矩形收缩 1 像素以防绘制到外面被切掉
            Rectangle rect = this.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;
            e.Graphics.DrawRectangle(p, rect);
        }
    }

    // 5. 按键处理逻辑 (保持不变)
    protected override void OnKeyDown(KeyEventArgs e)
    {
        // 允许用户按 ESC 或 Space 再次关闭
        if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Space)
        {
            this.Close();
            return;
        }

        var matchedCmd = _commands.Find(c => c.TriggerKey == e.KeyCode);
        if (matchedCmd != null)
        {
            this.Close();
            matchedCmd.ActionToRun?.Invoke();
        }
    }
}