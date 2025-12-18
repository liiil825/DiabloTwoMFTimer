using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedModalButton : ThemedButton
{
    public ThemedModalButton()
    {
        // 预设为 BreakForm/LootHistoryForm 底部按钮的样式
        this.Size = new Size(ScaleHelper.Scale(160), ScaleHelper.Scale(50));
        this.BackColor = Color.FromArgb(60, 60, 60);
        this.ForeColor = Color.White;

        // 动态计算内边距
        this.Padding = new Padding(
            ScaleHelper.Scale(10),
            ScaleHelper.Scale(5),
            ScaleHelper.Scale(10),
            ScaleHelper.Scale(5)
        );

        this.MinimumSize = new Size(0, ScaleHelper.Scale(43));
    }

    // 提供一个便捷方法设置“危险操作”样式（如关闭/删除）
    public void SetThemeDanger()
    {
        this.FlatAppearance.MouseOverBackColor = Color.IndianRed;
    }

    // 提供一个便捷方法设置“主要操作”样式（如跳过/保存）
    public void SetThemePrimary()
    {
        this.FlatAppearance.MouseOverBackColor = Color.SteelBlue;
    }
}
