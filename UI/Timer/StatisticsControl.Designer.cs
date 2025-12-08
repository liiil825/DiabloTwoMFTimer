namespace DiabloTwoMFTimer.UI.Timer
{
    partial class StatisticsControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                Utils.LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblRunCount = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblFastestTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblAverageTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

            this.tlpMain.SuspendLayout();
            this.SuspendLayout();

            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tlpMain.Controls.Add(this.lblRunCount, 0, 0);
            this.tlpMain.Controls.Add(this.lblFastestTime, 0, 1);
            this.tlpMain.Controls.Add(this.lblAverageTime, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            // 三行均分
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpMain.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 60);
            this.tlpMain.TabIndex = 0;

            // 
            // lblRunCount
            // 
            this.lblRunCount.AutoSize = true;
            this.lblRunCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRunCount.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.SmallTitleFont; // 大一点的字体
            this.lblRunCount.Location = new System.Drawing.Point(0, 0);
            this.lblRunCount.Name = "lblRunCount";
            this.lblRunCount.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 20);
            this.lblRunCount.TabIndex = 0;
            this.lblRunCount.Text = "--- Run count 0 ---";
            this.lblRunCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // lblFastestTime
            // 
            this.lblFastestTime.AutoSize = true;
            this.lblFastestTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFastestTime.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.MainFont;
            this.lblFastestTime.Location = new System.Drawing.Point(0, 22);
            this.lblFastestTime.Name = "lblFastestTime";
            this.lblFastestTime.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 20);
            this.lblFastestTime.TabIndex = 1;
            this.lblFastestTime.Text = "Fastest: --:--:--.-";
            this.lblFastestTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // lblAverageTime
            // 
            this.lblAverageTime.AutoSize = true;
            this.lblAverageTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAverageTime.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.MainFont;
            this.lblAverageTime.Location = new System.Drawing.Point(0, 44);
            this.lblAverageTime.Name = "lblAverageTime";
            this.lblAverageTime.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 20);
            this.lblAverageTime.TabIndex = 2;
            this.lblAverageTime.Text = "Average: --:--:--.-";
            this.lblAverageTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // StatisticsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tlpMain);
            this.Name = "StatisticsControl";
            this.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 70);

            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblRunCount;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblFastestTime;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblAverageTime;
    }
}