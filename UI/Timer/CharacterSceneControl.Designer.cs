namespace DiabloTwoMFTimer.UI.Timer
{
    partial class CharacterSceneControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (_profileService == null)
                return;

            if (disposing && (components != null))
            { // 取消注册语言变更事件
                Utils.LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
                // 取消注册ProfileService事件
                _profileService.CurrentProfileChangedEvent -= OnProfileChanged;
                _profileService.CurrentSceneChangedEvent -= OnSceneChanged;
                _profileService.CurrentDifficultyChangedEvent -= OnDifficultyChanged;
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblSceneDisplay = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblCharacterDisplay = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

            this.tlpMain.SuspendLayout();
            this.SuspendLayout();

            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.lblSceneDisplay, 0, 0);
            this.tlpMain.Controls.Add(this.lblCharacterDisplay, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            // 两行均分高度，或者使用 AutoSize
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 60);
            this.tlpMain.TabIndex = 0;

            // 
            // lblSceneDisplay
            // 
            this.lblSceneDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSceneDisplay.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.SmallTitleFont; // 使用小标题字体
            this.lblSceneDisplay.Location = new System.Drawing.Point(3, 0);
            this.lblSceneDisplay.Name = "lblSceneDisplay";
            this.lblSceneDisplay.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 30);
            this.lblSceneDisplay.TabIndex = 0;
            this.lblSceneDisplay.Text = "初始化场景";
            this.lblSceneDisplay.TextAlign = System.Drawing.ContentAlignment.BottomLeft; // 底部对齐

            // 
            // lblCharacterDisplay
            // 
            this.lblCharacterDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCharacterDisplay.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.MainFont; // 使用主字体
            this.lblCharacterDisplay.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextSecondaryColor; // 次要颜色
            this.lblCharacterDisplay.Location = new System.Drawing.Point(3, 30);
            this.lblCharacterDisplay.Name = "lblCharacterDisplay";
            this.lblCharacterDisplay.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 30);
            this.lblCharacterDisplay.TabIndex = 1;
            this.lblCharacterDisplay.Text = "初始化角色";
            this.lblCharacterDisplay.TextAlign = System.Drawing.ContentAlignment.TopLeft; // 顶部对齐

            // 
            // CharacterSceneControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tlpMain);
            this.Name = "CharacterSceneControl";
            this.Size = new System.Drawing.Size(290, 60);

            this.tlpMain.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblSceneDisplay;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblCharacterDisplay;
    }
}