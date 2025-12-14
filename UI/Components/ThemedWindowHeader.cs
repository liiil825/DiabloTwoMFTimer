using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedWindowHeader : UserControl
{
    private Label _lblTitle = null!;
    private FlowLayoutPanel _flpToggles = null!;

    // 【新增】公开这个属性，以便父窗体可以在 SizeChanged 中调整它的 Left 属性实现居中
    public FlowLayoutPanel TogglePanel => _flpToggles;

    public string Title
    {
        get => _lblTitle.Text;
        set => _lblTitle.Text = value;
    }

    public ThemedWindowHeader()
    {
        this.DoubleBuffered = true;
        this.BackColor = Color.Transparent;
        // 关闭 AutoSize，由父容器手动控制高度，防止布局抖动
        this.AutoSize = false;
        this.Height = ScaleHelper.Scale(100); // 给定一个默认高度

        InitializeSubControls();
    }

    private void InitializeSubControls()
    {
        // 1. 标题
        _lblTitle = new Label
        {
            AutoSize = true,
            Font = AppTheme.TitleFont,
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(ScaleHelper.Scale(20), ScaleHelper.Scale(20)),
            Text = "TITLE",
        };

        // 2. 按钮容器
        _flpToggles = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
            // 初始位置，稍后会被 LootHistoryForm_SizeChanged 修改
            Location = new Point(ScaleHelper.Scale(20), ScaleHelper.Scale(55)),
        };

        this.Controls.Add(_lblTitle);
        this.Controls.Add(_flpToggles);
    }

    public void AddToggleButton(Control button)
    {
        _flpToggles.Controls.Add(button);
    }
}
