using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Common
{
    public class BaseForm : Form
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

        public BaseForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnConfirm = new ThemedButton();
            this.btnCancel = new ThemedButton();
            this.pnlTitleBar = new Panel();
            this.lblTitle = new Label();
            this.btnClose = new Button();

            this.SuspendLayout();

            //
            // pnlTitleBar
            //
            this.pnlTitleBar.BackColor = AppTheme.SurfaceColor;
            this.pnlTitleBar.Dock = DockStyle.Top;
            this.pnlTitleBar.Height = 35;
            this.pnlTitleBar.MouseDown += PnlTitleBar_MouseDown;

            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = DockStyle.Fill;
            this.lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            this.lblTitle.ForeColor = AppTheme.AccentColor;
            this.lblTitle.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            this.lblTitle.Padding = new Padding(10, 0, 0, 0);
            this.lblTitle.MouseDown += PnlTitleBar_MouseDown;

            //
            // btnClose
            //
            this.btnClose.Dock = DockStyle.Right;
            this.btnClose.Width = 40;
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 17, 35);
            this.btnClose.ForeColor = AppTheme.TextSecondaryColor;
            this.btnClose.Text = "×";
            this.btnClose.Font = new Font("Arial", 12F);
            this.btnClose.Cursor = Cursors.Hand;
            this.btnClose.Click += (s, e) => this.Close();

            this.pnlTitleBar.Controls.Add(this.lblTitle);
            this.pnlTitleBar.Controls.Add(this.btnClose);

            //
            // btnConfirm
            //
            this.btnConfirm.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            this.btnConfirm.Location = new Point(197, 260); // 默认位置
            this.btnConfirm.Size = new Size(80, 30);
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.Click += new EventHandler(this.BtnConfirm_Click);

            //
            // btnCancel
            //
            this.btnCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            this.btnCancel.Location = new Point(292, 260); // 默认位置
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(this.BtnCancel_Click);

            //
            // BaseForm
            //
            this.BackColor = AppTheme.BackColor;
            this.ClientSize = new Size(400, 310);
            this.Controls.Add(this.pnlTitleBar);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.FormBorderStyle = FormBorderStyle.None;
            // 【核心修改】 改为屏幕居中
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Name = "BaseForm";
            this.ResumeLayout(false);
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
            using (var pen = new Pen(AppTheme.AccentColor, 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

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
                lblTitle.Text = this.Text;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }
    }
}
