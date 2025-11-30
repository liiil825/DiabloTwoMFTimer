using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedTextBox : TextBox
{
    public ThemedTextBox()
    {
        this.BackColor = AppTheme.SurfaceColor;
        this.ForeColor = AppTheme.TextColor;
        this.BorderStyle = BorderStyle.FixedSingle;
        this.Font = AppTheme.MainFont;
    }
}