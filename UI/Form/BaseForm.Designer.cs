using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Form;

partial class BaseForm : System.Windows.Forms.Form
{
    protected ThemedButton btnConfirm = null!;
    protected ThemedButton btnCancel = null!;
    private Panel pnlTitleBar = null!;
    private Label lblTitle = null!;
    private Button btnClose = null!;
    private IContainer components = null!;

    [Browsable(true)]
    [Category("Appearance")]
    public string ConfirmButtonText { get; set; } = "确认";

    [Browsable(true)]
    [Category("Appearance")]
    public string CancelButtonText { get; set; } = "取消";

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        btnConfirm = new ThemedButton();
        btnCancel = new ThemedButton();
        pnlTitleBar = new Panel();
        lblTitle = new Label();
        btnClose = new Button();

        this.SuspendLayout();

        //
        // pnlTitleBar
        //
        pnlTitleBar.BackColor = AppTheme.SurfaceColor;
        pnlTitleBar.Dock = DockStyle.Top;
        pnlTitleBar.Height = 35;
        pnlTitleBar.MouseDown += PnlTitleBar_MouseDown;

        //
        // lblTitle
        //
        lblTitle.AutoSize = false;
        lblTitle.Dock = DockStyle.Fill;
        lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        lblTitle.ForeColor = AppTheme.AccentColor;
        lblTitle.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
        lblTitle.Padding = new Padding(10, 0, 0, 0);
        lblTitle.MouseDown += PnlTitleBar_MouseDown;

        //
        // btnClose
        //
        btnClose.Dock = DockStyle.Right;
        btnClose.Width = 40;
        btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 17, 35);
        btnClose.ForeColor = AppTheme.TextSecondaryColor;
        btnClose.Text = "×";
        btnClose.Font = new Font("Arial", 12F);
        btnClose.Cursor = Cursors.Hand;
        btnClose.Click += (s, e) => this.Close();

        pnlTitleBar.Controls.Add(lblTitle);
        pnlTitleBar.Controls.Add(btnClose);

        //
        // btnConfirm
        //
        btnConfirm.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnConfirm.Location = new Point(197, 260); // 默认位置
        btnConfirm.Size = new Size(80, 30);
        btnConfirm.Text = "Confirm";
        btnConfirm.Click += new EventHandler(BtnConfirm_Click);

        //
        // btnCancel
        //
        btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancel.Location = new Point(292, 260); // 默认位置
        btnCancel.Size = new Size(80, 30);
        btnCancel.Text = "Cancel";
        btnCancel.Click += new EventHandler(BtnCancel_Click);

        //
        // BaseForm
        //
        this.BackColor = AppTheme.BackColor;
        this.ClientSize = new Size(400, 310);
        this.Controls.Add(pnlTitleBar);
        this.Controls.Add(btnCancel);
        this.Controls.Add(btnConfirm);
        this.FormBorderStyle = FormBorderStyle.None;
        // 【核心修改】 改为屏幕居中
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Name = "BaseForm";
        this.ResumeLayout(false);
    }
}