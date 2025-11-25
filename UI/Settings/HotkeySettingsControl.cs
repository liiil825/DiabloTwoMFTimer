using System;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Settings {
    public class HotkeySettingsControl : UserControl {
        private GroupBox grpHotkeys = null!;

        // 控件定义
        private Label lblStartNext = null!;
        private TextBox txtStartNext = null!;
        private Label lblPause = null!;
        private TextBox txtPause = null!;
        private Label lblDeleteHistory = null!;
        private TextBox txtDeleteHistory = null!;
        private Label lblRecordLoot = null!;
        private TextBox txtRecordLoot = null!;

        // 样式定义
        private readonly Color ColorNormal = Color.White;
        private readonly Color ColorEditing = Color.AliceBlue;

        //防止焦点回弹的标志位
        private bool _isUpdating = false;

        public Keys StartOrNextRunHotkey { get; private set; }
        public Keys PauseHotkey { get; private set; }
        public Keys DeleteHistoryHotkey { get; private set; }
        public Keys RecordLootHotkey { get; private set; }

        public HotkeySettingsControl() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            this.grpHotkeys = new System.Windows.Forms.GroupBox();
            this.lblStartNext = new System.Windows.Forms.Label();
            this.txtStartNext = new System.Windows.Forms.TextBox();
            this.lblPause = new System.Windows.Forms.Label();
            this.txtPause = new System.Windows.Forms.TextBox();
            this.lblDeleteHistory = new System.Windows.Forms.Label();
            this.txtDeleteHistory = new System.Windows.Forms.TextBox();
            this.lblRecordLoot = new System.Windows.Forms.Label();
            this.txtRecordLoot = new System.Windows.Forms.TextBox();
            this.grpHotkeys.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpHotkeys
            // 
            this.grpHotkeys.Controls.Add(this.txtRecordLoot);
            this.grpHotkeys.Controls.Add(this.lblRecordLoot);
            this.grpHotkeys.Controls.Add(this.txtDeleteHistory);
            this.grpHotkeys.Controls.Add(this.lblDeleteHistory);
            this.grpHotkeys.Controls.Add(this.txtPause);
            this.grpHotkeys.Controls.Add(this.lblPause);
            this.grpHotkeys.Controls.Add(this.txtStartNext);
            this.grpHotkeys.Controls.Add(this.lblStartNext);
            this.grpHotkeys.Location = new System.Drawing.Point(10, 10);
            this.grpHotkeys.Name = "grpHotkeys";
            this.grpHotkeys.Size = new System.Drawing.Size(330, 220);
            this.grpHotkeys.TabIndex = 0;
            this.grpHotkeys.TabStop = false;
            this.grpHotkeys.Text = "快捷键设置";
            // 
            // lblStartNext
            // 
            this.lblStartNext.AutoSize = true;
            this.lblStartNext.Location = new System.Drawing.Point(15, 34);
            this.lblStartNext.Name = "lblStartNext";
            this.lblStartNext.Size = new System.Drawing.Size(110, 20);
            this.lblStartNext.TabIndex = 0;
            this.lblStartNext.Text = "开始/下一局:";
            // 
            // txtStartNext
            // 
            this.txtStartNext.BackColor = ColorNormal;
            this.txtStartNext.Cursor = Cursors.Hand;
            this.txtStartNext.Location = new System.Drawing.Point(130, 30);
            this.txtStartNext.Name = "txtStartNext";
            this.txtStartNext.ReadOnly = true;
            this.txtStartNext.Size = new System.Drawing.Size(180, 27);
            this.txtStartNext.TabIndex = 1;
            this.txtStartNext.Tag = "StartNext";
            this.txtStartNext.KeyDown += new KeyEventHandler(this.OnHotkeyInput);
            this.txtStartNext.Enter += new EventHandler(this.OnTextBoxEnter);
            this.txtStartNext.Leave += new EventHandler(this.OnTextBoxLeave);
            // 
            // lblPause
            // 
            this.lblPause.AutoSize = true;
            this.lblPause.Location = new System.Drawing.Point(15, 74);
            this.lblPause.Name = "lblPause";
            this.lblPause.Size = new System.Drawing.Size(110, 20);
            this.lblPause.TabIndex = 2;
            this.lblPause.Text = "暂停/恢复:";
            // 
            // txtPause
            // 
            this.txtPause.BackColor = ColorNormal;
            this.txtPause.Cursor = Cursors.Hand;
            this.txtPause.Location = new System.Drawing.Point(130, 70);
            this.txtPause.Name = "txtPause";
            this.txtPause.ReadOnly = true;
            this.txtPause.Size = new System.Drawing.Size(180, 27);
            this.txtPause.TabIndex = 3;
            this.txtPause.Tag = "Pause";
            this.txtPause.KeyDown += new KeyEventHandler(this.OnHotkeyInput);
            this.txtPause.Enter += new EventHandler(this.OnTextBoxEnter);
            this.txtPause.Leave += new EventHandler(this.OnTextBoxLeave);
            // 
            // lblDeleteHistory
            // 
            this.lblDeleteHistory.AutoSize = true;
            this.lblDeleteHistory.Location = new System.Drawing.Point(15, 114);
            this.lblDeleteHistory.Name = "lblDeleteHistory";
            this.lblDeleteHistory.Size = new System.Drawing.Size(110, 20);
            this.lblDeleteHistory.TabIndex = 4;
            this.lblDeleteHistory.Text = "删除最后记录:";
            // 
            // txtDeleteHistory
            // 
            this.txtDeleteHistory.BackColor = ColorNormal;
            this.txtDeleteHistory.Cursor = Cursors.Hand;
            this.txtDeleteHistory.Location = new System.Drawing.Point(130, 110);
            this.txtDeleteHistory.Name = "txtDeleteHistory";
            this.txtDeleteHistory.ReadOnly = true;
            this.txtDeleteHistory.Size = new System.Drawing.Size(180, 27);
            this.txtDeleteHistory.TabIndex = 5;
            this.txtDeleteHistory.Tag = "Delete";
            this.txtDeleteHistory.KeyDown += new KeyEventHandler(this.OnHotkeyInput);
            this.txtDeleteHistory.Enter += new EventHandler(this.OnTextBoxEnter);
            this.txtDeleteHistory.Leave += new EventHandler(this.OnTextBoxLeave);
            // 
            // lblRecordLoot
            // 
            this.lblRecordLoot.AutoSize = true;
            this.lblRecordLoot.Location = new System.Drawing.Point(15, 154);
            this.lblRecordLoot.Name = "lblRecordLoot";
            this.lblRecordLoot.Size = new System.Drawing.Size(110, 20);
            this.lblRecordLoot.TabIndex = 6;
            this.lblRecordLoot.Text = "记录掉落:";
            // 
            // txtRecordLoot
            // 
            this.txtRecordLoot.BackColor = ColorNormal;
            this.txtRecordLoot.Cursor = Cursors.Hand;
            this.txtRecordLoot.Location = new System.Drawing.Point(130, 150);
            this.txtRecordLoot.Name = "txtRecordLoot";
            this.txtRecordLoot.ReadOnly = true;
            this.txtRecordLoot.Size = new System.Drawing.Size(180, 27);
            this.txtRecordLoot.TabIndex = 7;
            this.txtRecordLoot.Tag = "Record";
            this.txtRecordLoot.KeyDown += new KeyEventHandler(this.OnHotkeyInput);
            this.txtRecordLoot.Enter += new EventHandler(this.OnTextBoxEnter);
            this.txtRecordLoot.Leave += new EventHandler(this.OnTextBoxLeave);
            // 
            // HotkeySettingsControl
            // 
            this.Controls.Add(this.grpHotkeys);
            this.Name = "HotkeySettingsControl";
            this.Size = new System.Drawing.Size(350, 250);
            this.grpHotkeys.ResumeLayout(false);
            this.grpHotkeys.PerformLayout();
            this.ResumeLayout(false);
        }

        // --- 核心修复逻辑 ---

        private void OnTextBoxEnter(object? sender, EventArgs e) {
            if (sender is not TextBox textBox) return;

            // 修复关键：如果是刚刚修改完触发的焦点回弹，不要显示提示语，直接返回
            if (_isUpdating) {
                _isUpdating = false;
                return;
            }

            textBox.BackColor = ColorEditing;
            textBox.Text = "请按快捷键 (Esc取消)";
        }

        private void OnTextBoxLeave(object? sender, EventArgs e) {
            if (sender is not TextBox textBox) return;

            textBox.BackColor = ColorNormal;

            // 还原/更新文本
            string tag = textBox.Tag?.ToString() ?? "";
            Keys currentKey = Keys.None;
            switch (tag) {
                case "StartNext": currentKey = StartOrNextRunHotkey; break;
                case "Pause": currentKey = PauseHotkey; break;
                case "Delete": currentKey = DeleteHistoryHotkey; break;
                case "Record": currentKey = RecordLootHotkey; break;
            }
            textBox.Text = FormatKeyString(currentKey);

            // 确保标志位复位（防呆）
            _isUpdating = false;
        }

        private void OnHotkeyInput(object? sender, KeyEventArgs e) {
            if (sender is not TextBox textBox) return;

            e.SuppressKeyPress = true;

            // Esc: 取消
            if (e.KeyCode == Keys.Escape) {
                // 主动转移焦点到父控件，比 ActiveControl=null 更稳健
                this.Focus();
                return;
            }

            // Delete/Back: 清除
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete) {
                // 标记正在更新，防止Enter事件覆盖文本
                _isUpdating = true;
                UpdateHotkey(textBox, Keys.None);
                this.Focus();
                return;
            }

            // 忽略单一控制键
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu) {
                return;
            }

            Keys keyData = e.KeyCode;
            if (e.Control) keyData |= Keys.Control;
            if (e.Shift) keyData |= Keys.Shift;
            if (e.Alt) keyData |= Keys.Alt;

            // 标记正在更新，防止Enter事件覆盖文本
            _isUpdating = true;

            UpdateHotkey(textBox, keyData);

            // 尝试转移焦点，这会触发 Leave -> Enter(回弹)
            // 但因为我们设置了 _isUpdating = true，回弹的 Enter 会被忽略
            this.Focus();
        }

        private void UpdateHotkey(TextBox textBox, Keys newKey) {
            string tag = textBox.Tag?.ToString() ?? "";
            switch (tag) {
                case "StartNext": StartOrNextRunHotkey = newKey; break;
                case "Pause": PauseHotkey = newKey; break;
                case "Delete": DeleteHistoryHotkey = newKey; break;
                case "Record": RecordLootHotkey = newKey; break;
            }
            // 立即更新文本，让用户立刻看到结果
            textBox.Text = FormatKeyString(newKey);
            // 立即恢复颜色
            textBox.BackColor = ColorNormal;
        }

        private string FormatKeyString(Keys key) {
            if (key == Keys.None) return "无 (None)";
            var converter = new KeysConverter();
            return converter.ConvertToString(key) ?? "None";
        }

        public void LoadHotkeys(Services.IAppSettings settings) {
            StartOrNextRunHotkey = settings.HotkeyStartOrNext;
            PauseHotkey = settings.HotkeyPause;
            DeleteHistoryHotkey = settings.HotkeyDeleteHistory;
            RecordLootHotkey = settings.HotkeyRecordLoot;

            txtStartNext.Text = FormatKeyString(StartOrNextRunHotkey);
            txtPause.Text = FormatKeyString(PauseHotkey);
            txtDeleteHistory.Text = FormatKeyString(DeleteHistoryHotkey);
            txtRecordLoot.Text = FormatKeyString(RecordLootHotkey);
        }

        public void RefreshUI() {
            if (this.InvokeRequired) { this.Invoke(new Action(RefreshUI)); return; }
            if (grpHotkeys == null) return;
            try {
                grpHotkeys.Text = LanguageManager.GetString("HotkeySettingsGroup");
                lblStartNext.Text = LanguageManager.GetString("HotkeyStartNext");
                lblPause.Text = LanguageManager.GetString("HotkeyPause");
                lblDeleteHistory.Text = LanguageManager.GetString("HotkeyDeleteHistory");
                lblRecordLoot.Text = LanguageManager.GetString("HotkeyRecordLoot");

                LoadHotkeys(new Services.AppSettings {
                    HotkeyStartOrNext = StartOrNextRunHotkey,
                    HotkeyPause = PauseHotkey,
                    HotkeyDeleteHistory = DeleteHistoryHotkey,
                    HotkeyRecordLoot = RecordLootHotkey
                });
            }
            catch { }
        }
    }
}