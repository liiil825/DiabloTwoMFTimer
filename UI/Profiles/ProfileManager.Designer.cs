namespace DiabloTwoMFTimer.UI.Profiles
{
    partial class ProfileManager
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();

            this.tlpTopButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnCreateCharacter = new DiabloTwoMFTimer.UI.Components.ThemedButton();
            this.btnSwitchCharacter = new DiabloTwoMFTimer.UI.Components.ThemedButton();
            this.btnDeleteCharacter = new DiabloTwoMFTimer.UI.Components.ThemedButton();

            this.tlpScene = new System.Windows.Forms.TableLayoutPanel();
            this.lblScene = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.cmbScene = new DiabloTwoMFTimer.UI.Components.ThemedComboBox();

            this.tlpDifficulty = new System.Windows.Forms.TableLayoutPanel();
            this.lblDifficulty = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.cmbDifficulty = new DiabloTwoMFTimer.UI.Components.ThemedComboBox();

            this.tlpActionButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnStartFarm = new DiabloTwoMFTimer.UI.Components.ThemedButton();
            this.btnShowLootHistory = new DiabloTwoMFTimer.UI.Components.ThemedButton();
            this.btnShowStats = new DiabloTwoMFTimer.UI.Components.ThemedButton();

            this.lblCurrentProfile = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblStats = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

            this.tlpMain.SuspendLayout();
            this.tlpTopButtons.SuspendLayout();
            this.tlpScene.SuspendLayout();
            this.tlpDifficulty.SuspendLayout();
            this.tlpActionButtons.SuspendLayout();
            this.SuspendLayout();

            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.tlpTopButtons, 0, 0);
            this.tlpMain.Controls.Add(this.tlpScene, 0, 1);
            this.tlpMain.Controls.Add(this.tlpDifficulty, 0, 2);
            this.tlpMain.Controls.Add(this.tlpActionButtons, 0, 3);
            this.tlpMain.Controls.Add(this.lblCurrentProfile, 0, 4);
            this.tlpMain.Controls.Add(this.lblTime, 0, 5);
            this.tlpMain.Controls.Add(this.lblStats, 0, 6);

            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.Padding = new System.Windows.Forms.Padding(20);
            this.tlpMain.RowCount = 8;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(542, 450);
            this.tlpMain.TabIndex = 0;

            // -----------------------------------------------------------
            // 1. 顶部按钮组 (tlpTopButtons)
            // -----------------------------------------------------------
            this.tlpTopButtons.AutoSize = true;
            this.tlpTopButtons.ColumnCount = 3;
            // 均分 33.33%
            this.tlpTopButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpTopButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpTopButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));

            this.tlpTopButtons.Controls.Add(this.btnCreateCharacter, 0, 0);
            this.tlpTopButtons.Controls.Add(this.btnSwitchCharacter, 1, 0);
            this.tlpTopButtons.Controls.Add(this.btnDeleteCharacter, 2, 0);

            this.tlpTopButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTopButtons.Location = new System.Drawing.Point(23, 23);
            this.tlpTopButtons.Margin = new System.Windows.Forms.Padding(3, 3, 3, 15);
            this.tlpTopButtons.Name = "tlpTopButtons";
            this.tlpTopButtons.RowCount = 1;
            this.tlpTopButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpTopButtons.Size = new System.Drawing.Size(496, 48);
            this.tlpTopButtons.TabIndex = 0;

            // 统一设置顶部按钮样式
            System.Windows.Forms.Padding commonMargin = new System.Windows.Forms.Padding(5, 5, 5, 10); // 统一间距 3px

            this.btnCreateCharacter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCreateCharacter.Height = 40;
            this.btnCreateCharacter.Margin = commonMargin;
            this.btnCreateCharacter.Click += BtnCreateCharacter_Click;

            this.btnSwitchCharacter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSwitchCharacter.Height = 40;
            this.btnSwitchCharacter.Margin = commonMargin;
            this.btnSwitchCharacter.Click += BtnSwitchCharacter_Click;

            this.btnDeleteCharacter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDeleteCharacter.Height = 40;
            this.btnDeleteCharacter.Margin = commonMargin;
            this.btnDeleteCharacter.Click += BtnDeleteCharacter_Click;

            // 
            // tlpScene
            // 
            this.tlpScene.AutoSize = true;
            this.tlpScene.ColumnCount = 2;
            this.tlpScene.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpScene.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpScene.Controls.Add(this.lblScene, 0, 0);
            this.tlpScene.Controls.Add(this.cmbScene, 1, 0);
            this.tlpScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpScene.Location = new System.Drawing.Point(23, 89);
            this.tlpScene.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.tlpScene.Name = "tlpScene";
            this.tlpScene.RowCount = 1;
            this.tlpScene.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpScene.Size = new System.Drawing.Size(496, 41);
            this.tlpScene.TabIndex = 1;

            this.lblScene.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblScene.AutoSize = true;
            this.lblScene.Text = "Scene:";

            this.cmbScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbScene.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScene.Height = 35;

            // 
            // tlpDifficulty
            // 
            this.tlpDifficulty.AutoSize = true;
            this.tlpDifficulty.ColumnCount = 2;
            this.tlpDifficulty.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpDifficulty.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDifficulty.Controls.Add(this.lblDifficulty, 0, 0);
            this.tlpDifficulty.Controls.Add(this.cmbDifficulty, 1, 0);
            this.tlpDifficulty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDifficulty.Location = new System.Drawing.Point(23, 143);
            this.tlpDifficulty.Margin = new System.Windows.Forms.Padding(3, 3, 3, 20);
            this.tlpDifficulty.Name = "tlpDifficulty";
            this.tlpDifficulty.RowCount = 1;
            this.tlpDifficulty.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpDifficulty.Size = new System.Drawing.Size(496, 41);
            this.tlpDifficulty.TabIndex = 2;

            this.lblDifficulty.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDifficulty.AutoSize = true;
            this.lblDifficulty.Text = "Diff:";

            this.cmbDifficulty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDifficulty.Height = 35;

            // -----------------------------------------------------------
            // 2. 底部操作按钮组 (tlpActionButtons)
            // -----------------------------------------------------------
            this.tlpActionButtons.AutoSize = true;
            this.tlpActionButtons.ColumnCount = 3;
            // 同样均分 33.33%，确保和上面对齐
            this.tlpActionButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpActionButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpActionButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));

            this.tlpActionButtons.Controls.Add(this.btnStartFarm, 0, 0);
            this.tlpActionButtons.Controls.Add(this.btnShowLootHistory, 1, 0);
            this.tlpActionButtons.Controls.Add(this.btnShowStats, 2, 0);

            this.tlpActionButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpActionButtons.Location = new System.Drawing.Point(23, 207);
            this.tlpActionButtons.Margin = new System.Windows.Forms.Padding(3, 3, 3, 20);
            this.tlpActionButtons.Name = "tlpActionButtons";
            this.tlpActionButtons.RowCount = 1;
            this.tlpActionButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpActionButtons.Size = new System.Drawing.Size(496, 54);
            this.tlpActionButtons.TabIndex = 3;

            // 核心修改：使用与上面相同的 Margin
            this.btnStartFarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStartFarm.Height = 50;
            this.btnStartFarm.Margin = commonMargin; // 使用 3px
            this.btnStartFarm.Click += BtnStartFarm_Click;

            this.btnShowLootHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnShowLootHistory.Height = 50;
            this.btnShowLootHistory.Margin = commonMargin; // 使用 3px
            this.btnShowLootHistory.Click += BtnShowLootHistory_Click;

            this.btnShowStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnShowStats.Height = 50;
            this.btnShowStats.Margin = commonMargin; // 使用 3px
            this.btnShowStats.Click += BtnShowStats_Click;

            // 
            // Labels
            // 
            this.lblCurrentProfile.AutoSize = true;
            this.lblCurrentProfile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentProfile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblTime.AutoSize = true;
            this.lblTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblStats.AutoSize = true;
            this.lblStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // ProfileManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
            this.Controls.Add(this.tlpMain);
            this.Name = "ProfileManager";
            this.Size = new System.Drawing.Size(542, 450);

            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.tlpTopButtons.ResumeLayout(false);
            this.tlpScene.ResumeLayout(false);
            this.tlpScene.PerformLayout();
            this.tlpDifficulty.ResumeLayout(false);
            this.tlpDifficulty.PerformLayout();
            this.tlpActionButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpTopButtons;
        private System.Windows.Forms.TableLayoutPanel tlpScene;
        private System.Windows.Forms.TableLayoutPanel tlpDifficulty;
        private System.Windows.Forms.TableLayoutPanel tlpActionButtons;

        private DiabloTwoMFTimer.UI.Components.ThemedButton btnCreateCharacter;
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnSwitchCharacter;
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnDeleteCharacter;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblScene;
        private DiabloTwoMFTimer.UI.Components.ThemedComboBox cmbScene;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblDifficulty;
        private DiabloTwoMFTimer.UI.Components.ThemedComboBox cmbDifficulty;

        private DiabloTwoMFTimer.UI.Components.ThemedButton btnStartFarm;
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnShowLootHistory;
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnShowStats;

        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblCurrentProfile;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblTime;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblStats;
    }
}