using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Form;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedMessageBox : BaseForm
{
    private Label lblMessage;
    private MessageBoxButtons _buttons;

    public ThemedMessageBox(string message, string title, MessageBoxButtons buttons)
    {
        this._buttons = buttons;
        this.Text = title;

        // --- 1. 构建消息内容 ---
        lblMessage = new Label
        {
            Text = message,
            ForeColor = AppTheme.TextColor,
            Font = AppTheme.MainFont,
            AutoSize = true,
            MaximumSize = new Size(UISizeConstants.BaseFormWidth - 60, 0),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
        };

        TableLayoutPanel tlp = new TableLayoutPanel();
        tlp.Dock = DockStyle.Fill;
        tlp.AutoSize = true;
        tlp.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tlp.ColumnCount = 1;
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlp.RowCount = 1;
        tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        tlp.Controls.Add(lblMessage, 0, 0);
        tlp.Padding = new Padding(10, 20, 10, 20);

        this.pnlContent.Controls.Add(tlp);

        ConfigureButtons();
    }

    private void ConfigureButtons()
    {
        switch (_buttons)
        {
            case MessageBoxButtons.OK:
                btnConfirm.Visible = true;
                btnCancel.Visible = false;
                break;
            case MessageBoxButtons.OKCancel:
            case MessageBoxButtons.YesNo:
                btnConfirm.Visible = true;
                btnCancel.Visible = true;
                break;
        }
    }

    // 重写 UpdateUI
    protected override void UpdateUI()
    {
        base.UpdateUI(); // 这里已经将按钮设置为了图标 (Confirm=\uE73E, Cancel=\uE711)

        // 针对特定按钮类型，如果有特殊需求可以在这里覆盖，
        // 但既然统一改为图标，BaseForm 的默认设置已经满足 OK, OKCancel, YesNo 的视觉需求。
        // OK       -> 显示 √
        // OKCancel -> 显示 √ 和 ×
        // YesNo    -> 显示 √ 和 ×

        // 确保字体设置正确（双重保险）
        if (btnConfirm != null)
            btnConfirm.Font = _iconFont;
        if (btnCancel != null)
            btnCancel.Font = _iconFont;
    }

    public static DialogResult Show(
        string message,
        string title = "Message",
        MessageBoxButtons buttons = MessageBoxButtons.OK
    )
    {
        using var msgBox = new ThemedMessageBox(message, title, buttons);
        return msgBox.ShowDialog();
    }

    protected override void BtnConfirm_Click(object? sender, EventArgs e)
    {
        if (_buttons == MessageBoxButtons.YesNo)
        {
            this.DialogResult = DialogResult.Yes;
        }
        else
        {
            this.DialogResult = DialogResult.OK;
        }
        this.Close();
    }

    protected override void BtnCancel_Click(object? sender, EventArgs e)
    {
        if (_buttons == MessageBoxButtons.YesNo)
        {
            this.DialogResult = DialogResult.No;
        }
        else
        {
            this.DialogResult = DialogResult.Cancel;
        }
        this.Close();
    }
}
