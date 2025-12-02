using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Form;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedMessageBox : BaseForm
{
    private Label lblMessage;
    private MessageBoxButtons _buttons;

    public ThemedMessageBox(string message, string title, MessageBoxButtons buttons)
    {
        this._buttons = buttons;
        this.Text = title;
        this.Size = new Size(400, 200); // 默认大小

        // 消息内容
        lblMessage = new Label
        {
            Text = message,
            ForeColor = AppTheme.TextColor,
            Font = AppTheme.MainFont,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(20),
        };

        // 这是一个 Hack，因为 BaseForm 已经加了 TitleBar 和 Buttons
        // 我们需要把 Message 插到它们中间。
        // BaseForm 的控件顺序是 TitleBar(Top), btnCancel/Confirm(Bottom/Anchor)
        // 所以直接 Add 会盖在上面，或者用 Panel 调整。
        // 这里简单处理：直接 Add，并在 OnLoad 调整位置。
        this.Controls.Add(lblMessage);

        // 按钮逻辑
        SetupButtons(buttons);
    }

    private void SetupButtons(MessageBoxButtons buttons)
    {
        // BaseForm 默认有 Confirm 和 Cancel
        switch (buttons)
        {
            case MessageBoxButtons.OK:
                btnConfirm.Text = "OK";
                btnConfirm.Visible = true;
                btnCancel.Visible = false;
                // 居中 Confirm 按钮
                btnConfirm.Location = new Point(
                    (this.ClientSize.Width - btnConfirm.Width) / 2,
                    btnConfirm.Location.Y
                );
                break;

            case MessageBoxButtons.OKCancel:
                btnConfirm.Text = "OK";
                btnCancel.Text = "Cancel";
                break;

            case MessageBoxButtons.YesNo:
                btnConfirm.Text = "Yes";
                btnCancel.Text = "No";
                break;
        }
    }

    // 静态调用方法
    public static DialogResult Show(
        string message,
        string title = "Message",
        MessageBoxButtons buttons = MessageBoxButtons.OK
    )
    {
        using var msgBox = new ThemedMessageBox(message, title, buttons);
        return msgBox.ShowDialog();
    }

    // 确保 Label 在 TitleBar 下面
    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);
        if (lblMessage != null)
        {
            lblMessage.BringToFront(); // 可能会盖住标题栏，需要调整
            // 手动调整区域：TitleBar 高 35，底部留 50 给按钮
            lblMessage.SetBounds(0, 35, this.ClientSize.Width, this.ClientSize.Height - 35 - 50);
        }
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
