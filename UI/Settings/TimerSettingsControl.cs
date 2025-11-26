using System;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Settings {
    public class TimerSettingsControl : UserControl {
        private GroupBox grpTimerSettings = null!;

        // 控件定义
        private CheckBox chkShowPomodoro = null!;
        private CheckBox chkShowLootDrops = null!;
        private CheckBox chkSyncStartPomodoro = null!;
        private CheckBox chkSyncPausePomodoro = null!;
        private CheckBox chkGenerateRoomName = null!;

        // 属性定义
        public bool TimerShowPomodoro { get; private set; }
        public bool TimerShowLootDrops { get; private set; }
        public bool TimerSyncStartPomodoro { get; private set; }
        public bool TimerSyncPausePomodoro { get; private set; }
        public bool GenerateRoomName { get; private set; }

        public TimerSettingsControl() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            this.grpTimerSettings = new System.Windows.Forms.GroupBox();
            this.chkSyncPausePomodoro = new System.Windows.Forms.CheckBox();
            this.chkGenerateRoomName = new System.Windows.Forms.CheckBox();
            this.chkShowPomodoro = new System.Windows.Forms.CheckBox();
            this.chkShowLootDrops = new System.Windows.Forms.CheckBox();
            this.chkSyncStartPomodoro = new System.Windows.Forms.CheckBox();
            this.grpTimerSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpTimerSettings
            // 
            this.grpTimerSettings.Controls.Add(this.chkGenerateRoomName);
            this.grpTimerSettings.Controls.Add(this.chkSyncPausePomodoro);
            this.grpTimerSettings.Controls.Add(this.chkSyncStartPomodoro);
            this.grpTimerSettings.Controls.Add(this.chkShowLootDrops);
            this.grpTimerSettings.Controls.Add(this.chkShowPomodoro);
            this.grpTimerSettings.Location = new System.Drawing.Point(10, 10);
            this.grpTimerSettings.Name = "grpTimerSettings";
            this.grpTimerSettings.Size = new System.Drawing.Size(330, 250);
            this.grpTimerSettings.TabIndex = 0;
            this.grpTimerSettings.TabStop = false;
            this.grpTimerSettings.Text = "计时器设置";
            // 
            // chkShowPomodoro
            // 
            this.chkShowPomodoro.AutoSize = true;
            this.chkShowPomodoro.Location = new System.Drawing.Point(15, 34);
            this.chkShowPomodoro.Name = "chkShowPomodoro";
            this.chkShowPomodoro.Size = new System.Drawing.Size(150, 24);
            this.chkShowPomodoro.TabIndex = 0;
            this.chkShowPomodoro.Text = "是否显示番茄钟";
            this.chkShowPomodoro.UseVisualStyleBackColor = true;
            this.chkShowPomodoro.CheckedChanged += new System.EventHandler(this.OnShowPomodoroChanged);
            // 
            // chkShowLootDrops
            // 
            this.chkShowLootDrops.AutoSize = true;
            this.chkShowLootDrops.Location = new System.Drawing.Point(15, 74);
            this.chkShowLootDrops.Name = "chkShowLootDrops";
            this.chkShowLootDrops.Size = new System.Drawing.Size(150, 24);
            this.chkShowLootDrops.TabIndex = 1;
            this.chkShowLootDrops.Text = "是否展示掉落";
            this.chkShowLootDrops.UseVisualStyleBackColor = true;
            this.chkShowLootDrops.CheckedChanged += new System.EventHandler(this.OnShowLootDropsChanged);
            // 
            // chkSyncStartPomodoro
            // 
            this.chkSyncStartPomodoro.AutoSize = true;
            this.chkSyncStartPomodoro.Location = new System.Drawing.Point(15, 114);
            this.chkSyncStartPomodoro.Name = "chkSyncStartPomodoro";
            this.chkSyncStartPomodoro.Size = new System.Drawing.Size(240, 24);
            this.chkSyncStartPomodoro.TabIndex = 2;
            this.chkSyncStartPomodoro.Text = "同步开启番茄钟";
            this.chkSyncStartPomodoro.UseVisualStyleBackColor = true;
            this.chkSyncStartPomodoro.CheckedChanged += new System.EventHandler(this.OnSyncStartPomodoroChanged);
            // 
            // chkSyncPausePomodoro
            // 
            this.chkSyncPausePomodoro.AutoSize = true;
            this.chkSyncPausePomodoro.Location = new System.Drawing.Point(15, 154);
            this.chkSyncPausePomodoro.Name = "chkSyncPausePomodoro";
            this.chkSyncPausePomodoro.Size = new System.Drawing.Size(240, 24);
            this.chkSyncPausePomodoro.TabIndex = 3;
            this.chkSyncPausePomodoro.Text = "同步暂停番茄钟";
            this.chkSyncPausePomodoro.UseVisualStyleBackColor = true;
            this.chkSyncPausePomodoro.CheckedChanged += new System.EventHandler(this.OnSyncPausePomodoroChanged);
            // 
            // chkGenerateRoomName
            // 
            this.chkGenerateRoomName.AutoSize = true;
            this.chkGenerateRoomName.Location = new System.Drawing.Point(15, 194);
            this.chkGenerateRoomName.Name = "chkGenerateRoomName";
            this.chkGenerateRoomName.Size = new System.Drawing.Size(150, 24);
            this.chkGenerateRoomName.TabIndex = 4;
            this.chkGenerateRoomName.Text = "生成房间名称";
            this.chkGenerateRoomName.UseVisualStyleBackColor = true;
            this.chkGenerateRoomName.CheckedChanged += new System.EventHandler(this.OnGenerateRoomNameChanged);
            // 
            // TimerSettingsControl
            // 
            this.Controls.Add(this.grpTimerSettings);
            this.Name = "TimerSettingsControl";
            this.Size = new System.Drawing.Size(350, 270);
            this.grpTimerSettings.ResumeLayout(false);
            this.grpTimerSettings.PerformLayout();
            this.ResumeLayout(false);
        }

        // 事件处理
        private void OnShowPomodoroChanged(object? sender, EventArgs e) {
            if (sender is CheckBox checkBox) {
                TimerShowPomodoro = checkBox.Checked;
            }
        }

        private void OnShowLootDropsChanged(object? sender, EventArgs e) {
            if (sender is CheckBox checkBox) {
                TimerShowLootDrops = checkBox.Checked;
            }
        }

        private void OnSyncStartPomodoroChanged(object? sender, EventArgs e) {
            if (sender is CheckBox checkBox) {
                TimerSyncStartPomodoro = checkBox.Checked;
            }
        }

        private void OnSyncPausePomodoroChanged(object? sender, EventArgs e) {
            if (sender is CheckBox checkBox) {
                TimerSyncPausePomodoro = checkBox.Checked;
            }
        }

        private void OnGenerateRoomNameChanged(object? sender, EventArgs e) {
            if (sender is CheckBox checkBox) {
                GenerateRoomName = checkBox.Checked;
            }
        }

        // 加载设置
        public void LoadSettings(Services.IAppSettings settings) {
            TimerShowPomodoro = settings.TimerShowPomodoro;
            TimerShowLootDrops = settings.TimerShowLootDrops;
            TimerSyncStartPomodoro = settings.TimerSyncStartPomodoro;
            TimerSyncPausePomodoro = settings.TimerSyncPausePomodoro;
            GenerateRoomName = settings.GenerateRoomName;

            chkShowPomodoro.Checked = TimerShowPomodoro;
            chkShowLootDrops.Checked = TimerShowLootDrops;
            chkSyncStartPomodoro.Checked = TimerSyncStartPomodoro;
            chkSyncPausePomodoro.Checked = TimerSyncPausePomodoro;
            chkGenerateRoomName.Checked = GenerateRoomName;
        }

        // 刷新UI（支持国际化）
        public void RefreshUI() {
            if (this.InvokeRequired) { this.Invoke(new Action(RefreshUI)); return; }
            if (grpTimerSettings == null) return;
            try {
                grpTimerSettings.Text = LanguageManager.GetString("TimerSettingsGroup");
                chkShowPomodoro.Text = LanguageManager.GetString("TimerShowPomodoro");
                chkShowLootDrops.Text = LanguageManager.GetString("TimerShowLootDrops");
                chkSyncStartPomodoro.Text = LanguageManager.GetString("TimerSyncStartPomodoro");
                chkSyncPausePomodoro.Text = LanguageManager.GetString("TimerSyncPausePomodoro");
                chkGenerateRoomName.Text = LanguageManager.GetString("TimerGenerateRoomName");
            }
            catch { }
        }
    }
}