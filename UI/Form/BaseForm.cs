using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Form;

public partial class BaseForm : System.Windows.Forms.Form
{
    public BaseForm()
    {
        InitializeComponent();
    }

    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;
            if (lblTitle != null)
                lblTitle.Text = value;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(AppTheme.AccentColor, 1);
        e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
    }

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern int SendMessage(nint hWnd, int Msg, int wParam, int lParam);

    private void PnlTitleBar_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ReleaseCapture();
            SendMessage(Handle, 0xA1, 0x2, 0);
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!this.DesignMode)
            UpdateUI();
    }

    protected virtual void UpdateUI()
    {
        if (btnConfirm != null)
            btnConfirm.Text = LanguageManager.GetString("Confirm") ?? ConfirmButtonText;
        if (btnCancel != null)
            btnCancel.Text = LanguageManager.GetString("Cancel") ?? CancelButtonText;
        if (lblTitle != null)
            lblTitle.Text = Text;
    }
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            BtnCancel_Click(null, EventArgs.Empty);
            return true;
        }
        if (keyData == Keys.Enter)
        {
            // 如果焦点在按钮上，让按钮自己处理，否则默认触发确认
            if (!btnConfirm!.Focused && !btnCancel!.Focused)
            {
                BtnConfirm_Click(null, EventArgs.Empty);
                return true;
            }
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected virtual void BtnConfirm_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    protected virtual void BtnCancel_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }

}
