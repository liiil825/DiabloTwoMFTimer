using System;
using System.ComponentModel;
using System.Windows.Forms;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.UI.Common;

namespace DTwoMFTimerHelper.UI.Pomodoro {
    public class PomodoroSettingsForm : BaseForm {
        // 主要标签
        private Label lblWorkTime = null!;
        private Label lblShortBreakTime = null!;
        private Label lblLongBreakTime = null!;

        // 单位标签 (分)
        private Label lblWorkMinUnit = null!;
        private Label lblShortBreakMinUnit = null!;
        private Label lblLongBreakMinUnit = null!;

        // 单位标签 (秒)
        private Label lblWorkSecUnit = null!;
        private Label lblShortBreakSecUnit = null!;
        private Label lblLongBreakSecUnit = null!;

        // 输入框
        private NumericUpDown nudWorkTimeMin = null!;
        private NumericUpDown nudWorkTimeSec = null!;
        private NumericUpDown nudShortBreakTimeMin = null!;
        private NumericUpDown nudShortBreakTimeSec = null!;
        private NumericUpDown nudLongBreakTimeMin = null!;
        private NumericUpDown nudLongBreakTimeSec = null!;

        private readonly IContainer components = null!;

        // 属性
        public int WorkTimeMinutes { get; private set; }
        public int WorkTimeSeconds { get; private set; }
        public int ShortBreakMinutes { get; private set; }
        public int ShortBreakSeconds { get; private set; }
        public int LongBreakMinutes { get; private set; }
        public int LongBreakSeconds { get; private set; }

        public PomodoroSettingsForm() {
            InitializeComponent();
        }

        public PomodoroSettingsForm(int workTime, int shortBreakTime, int longBreakTime) : this() {
            WorkTimeMinutes = workTime;
            ShortBreakMinutes = shortBreakTime;
            LongBreakMinutes = longBreakTime;
        }

        public PomodoroSettingsForm(int workTimeMinutes, int workTimeSeconds, int shortBreakMinutes, int shortBreakSeconds, int longBreakMinutes, int longBreakSeconds) : this() {
            WorkTimeMinutes = workTimeMinutes;
            WorkTimeSeconds = workTimeSeconds;
            ShortBreakMinutes = shortBreakMinutes;
            ShortBreakSeconds = shortBreakSeconds;
            LongBreakMinutes = longBreakMinutes;
            LongBreakSeconds = longBreakSeconds;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            if (!this.DesignMode) {
                // 如果是默认初始化（全0），尝试加载配置
                if (WorkTimeMinutes == 0 && ShortBreakMinutes == 0 && LongBreakMinutes == 0) {
                    LoadSettings();
                }
                UpdateUI();
            }
        }

        private void LoadSettings() {
            try {
                var settings = SettingsManager.LoadSettings();
                WorkTimeMinutes = settings.WorkTimeMinutes;
                WorkTimeSeconds = settings.WorkTimeSeconds;
                ShortBreakMinutes = settings.ShortBreakMinutes;
                ShortBreakSeconds = settings.ShortBreakSeconds;
                LongBreakMinutes = settings.LongBreakMinutes;
                LongBreakSeconds = settings.LongBreakSeconds;
            }
            catch { /* Ignore */ }
        }

        private void InitializeComponent() {
            // 初始化控件
            this.lblWorkTime = new System.Windows.Forms.Label();
            this.lblShortBreakTime = new System.Windows.Forms.Label();
            this.lblLongBreakTime = new System.Windows.Forms.Label();

            this.lblWorkMinUnit = new System.Windows.Forms.Label();
            this.lblShortBreakMinUnit = new System.Windows.Forms.Label();
            this.lblLongBreakMinUnit = new System.Windows.Forms.Label();

            this.lblWorkSecUnit = new System.Windows.Forms.Label();
            this.lblShortBreakSecUnit = new System.Windows.Forms.Label();
            this.lblLongBreakSecUnit = new System.Windows.Forms.Label();

            this.nudWorkTimeMin = new System.Windows.Forms.NumericUpDown();
            this.nudWorkTimeSec = new System.Windows.Forms.NumericUpDown();
            this.nudShortBreakTimeMin = new System.Windows.Forms.NumericUpDown();
            this.nudShortBreakTimeSec = new System.Windows.Forms.NumericUpDown();
            this.nudLongBreakTimeMin = new System.Windows.Forms.NumericUpDown();
            this.nudLongBreakTimeSec = new System.Windows.Forms.NumericUpDown();

            ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeSec)).BeginInit();

            this.SuspendLayout();

            // 布局常量
            int labelX = 30;         // 标题X坐标
            int inputMinX = 140;     // 分钟输入框X坐标
            int labelMinX = 215;     // "分"字X坐标
            int inputSecX = 250;     // 秒输入框X坐标
            int labelSecX = 325;     // "秒"字X坐标

            int row1Y = 30;          // 第一行Y
            int row2Y = 70;          // 第二行Y
            int row3Y = 110;         // 第三行Y

            // 文本对齐偏移量：Label通常比InputBox位置要靠下一点点才能视觉居中
            int textOffsetY = 4;

            // --- 第一行：工作时间 ---

            // 标题
            this.lblWorkTime.AutoSize = true;
            this.lblWorkTime.Location = new System.Drawing.Point(labelX, row1Y + textOffsetY);
            this.lblWorkTime.Name = "lblWorkTime";
            this.lblWorkTime.Size = new System.Drawing.Size(100, 15);
            this.lblWorkTime.Text = "工作时间:"; // 设计时默认显示

            // 分钟输入
            this.nudWorkTimeMin.Location = new System.Drawing.Point(inputMinX, row1Y);
            this.nudWorkTimeMin.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            this.nudWorkTimeMin.Name = "nudWorkTimeMin";
            this.nudWorkTimeMin.Size = new System.Drawing.Size(70, 25);

            // 分钟单位
            this.lblWorkMinUnit.AutoSize = true;
            this.lblWorkMinUnit.Location = new System.Drawing.Point(labelMinX, row1Y + textOffsetY);
            this.lblWorkMinUnit.Name = "lblWorkMinUnit";
            this.lblWorkMinUnit.Size = new System.Drawing.Size(22, 15);
            this.lblWorkMinUnit.Text = "分";

            // 秒输入
            this.nudWorkTimeSec.Location = new System.Drawing.Point(inputSecX, row1Y);
            this.nudWorkTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            this.nudWorkTimeSec.Name = "nudWorkTimeSec";
            this.nudWorkTimeSec.Size = new System.Drawing.Size(70, 25);

            // 秒单位
            this.lblWorkSecUnit.AutoSize = true;
            this.lblWorkSecUnit.Location = new System.Drawing.Point(labelSecX, row1Y + textOffsetY);
            this.lblWorkSecUnit.Name = "lblWorkSecUnit";
            this.lblWorkSecUnit.Size = new System.Drawing.Size(22, 15);
            this.lblWorkSecUnit.Text = "秒";

            // --- 第二行：短休息 ---

            this.lblShortBreakTime.AutoSize = true;
            this.lblShortBreakTime.Location = new System.Drawing.Point(labelX, row2Y + textOffsetY);
            this.lblShortBreakTime.Name = "lblShortBreakTime";
            this.lblShortBreakTime.Text = "短休息时间:";

            this.nudShortBreakTimeMin.Location = new System.Drawing.Point(inputMinX, row2Y);
            this.nudShortBreakTimeMin.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            this.nudShortBreakTimeMin.Name = "nudShortBreakTimeMin";
            this.nudShortBreakTimeMin.Size = new System.Drawing.Size(70, 25);

            this.lblShortBreakMinUnit.AutoSize = true;
            this.lblShortBreakMinUnit.Location = new System.Drawing.Point(labelMinX, row2Y + textOffsetY);
            this.lblShortBreakMinUnit.Name = "lblShortBreakMinUnit";
            this.lblShortBreakMinUnit.Text = "分";

            this.nudShortBreakTimeSec.Location = new System.Drawing.Point(inputSecX, row2Y);
            this.nudShortBreakTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            this.nudShortBreakTimeSec.Name = "nudShortBreakTimeSec";
            this.nudShortBreakTimeSec.Size = new System.Drawing.Size(70, 25);

            this.lblShortBreakSecUnit.AutoSize = true;
            this.lblShortBreakSecUnit.Location = new System.Drawing.Point(labelSecX, row2Y + textOffsetY);
            this.lblShortBreakSecUnit.Name = "lblShortBreakSecUnit";
            this.lblShortBreakSecUnit.Text = "秒";

            // --- 第三行：长休息 ---

            this.lblLongBreakTime.AutoSize = true;
            this.lblLongBreakTime.Location = new System.Drawing.Point(labelX, row3Y + textOffsetY);
            this.lblLongBreakTime.Name = "lblLongBreakTime";
            this.lblLongBreakTime.Text = "长休息时间:";

            this.nudLongBreakTimeMin.Location = new System.Drawing.Point(inputMinX, row3Y);
            this.nudLongBreakTimeMin.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            this.nudLongBreakTimeMin.Name = "nudLongBreakTimeMin";
            this.nudLongBreakTimeMin.Size = new System.Drawing.Size(70, 25);

            this.lblLongBreakMinUnit.AutoSize = true;
            this.lblLongBreakMinUnit.Location = new System.Drawing.Point(labelMinX, row3Y + textOffsetY);
            this.lblLongBreakMinUnit.Name = "lblLongBreakMinUnit";
            this.lblLongBreakMinUnit.Text = "分";

            this.nudLongBreakTimeSec.Location = new System.Drawing.Point(inputSecX, row3Y);
            this.nudLongBreakTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            this.nudLongBreakTimeSec.Name = "nudLongBreakTimeSec";
            this.nudLongBreakTimeSec.Size = new System.Drawing.Size(70, 25);

            this.lblLongBreakSecUnit.AutoSize = true;
            this.lblLongBreakSecUnit.Location = new System.Drawing.Point(labelSecX, row3Y + textOffsetY);
            this.lblLongBreakSecUnit.Name = "lblLongBreakSecUnit";
            this.lblLongBreakSecUnit.Text = "秒";

            // --- 按钮 (继承自 BaseForm) ---
            this.btnConfirm.Location = new System.Drawing.Point(147, 180);
            this.btnConfirm.Text = "保存";

            this.btnCancel.Location = new System.Drawing.Point(273, 180);

            // --- Form 设置 ---
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 240); // 调整了窗口大小以适应紧凑布局
            this.Name = "PomodoroSettingsForm";
            this.Text = "番茄时钟设置";

            // 添加控件
            this.Controls.Add(this.lblWorkTime);
            this.Controls.Add(this.nudWorkTimeMin);
            this.Controls.Add(this.lblWorkMinUnit);
            this.Controls.Add(this.nudWorkTimeSec);
            this.Controls.Add(this.lblWorkSecUnit);

            this.Controls.Add(this.lblShortBreakTime);
            this.Controls.Add(this.nudShortBreakTimeMin);
            this.Controls.Add(this.lblShortBreakMinUnit);
            this.Controls.Add(this.nudShortBreakTimeSec);
            this.Controls.Add(this.lblShortBreakSecUnit);

            this.Controls.Add(this.lblLongBreakTime);
            this.Controls.Add(this.nudLongBreakTimeMin);
            this.Controls.Add(this.lblLongBreakMinUnit);
            this.Controls.Add(this.nudLongBreakTimeSec);
            this.Controls.Add(this.lblLongBreakSecUnit);

            // 保持按钮在最上层
            this.Controls.SetChildIndex(this.btnConfirm, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);

            ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeSec)).EndInit();

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected override void UpdateUI() {
            base.UpdateUI();

            // 1. 设置数值
            nudWorkTimeMin.Value = Math.Max(nudWorkTimeMin.Minimum, Math.Min(nudWorkTimeMin.Maximum, WorkTimeMinutes));
            nudWorkTimeSec.Value = Math.Max(nudWorkTimeSec.Minimum, Math.Min(nudWorkTimeSec.Maximum, WorkTimeSeconds));

            nudShortBreakTimeMin.Value = Math.Max(nudShortBreakTimeMin.Minimum, Math.Min(nudShortBreakTimeMin.Maximum, ShortBreakMinutes));
            nudShortBreakTimeSec.Value = Math.Max(nudShortBreakTimeSec.Minimum, Math.Min(nudShortBreakTimeSec.Maximum, ShortBreakSeconds));

            nudLongBreakTimeMin.Value = Math.Max(nudLongBreakTimeMin.Minimum, Math.Min(nudLongBreakTimeMin.Maximum, LongBreakMinutes));
            nudLongBreakTimeSec.Value = Math.Max(nudLongBreakTimeSec.Minimum, Math.Min(nudLongBreakTimeSec.Maximum, LongBreakSeconds));

            // 2. 本地化文本 - 所有 Label 都需要更新
            this.Text = LanguageManager.GetString("PomodoroSettings") ?? "番茄时钟设置";

            // 行标题
            lblWorkTime.Text = LanguageManager.GetString("WorkTime") ?? "工作时间:";
            lblShortBreakTime.Text = LanguageManager.GetString("ShortBreakTime") ?? "短休息时间:";
            lblLongBreakTime.Text = LanguageManager.GetString("LongBreakTime") ?? "长休息时间:";

            // 单位 (分)
            string strMin = LanguageManager.GetString("Minutes") ?? "分";
            lblWorkMinUnit.Text = strMin;
            lblShortBreakMinUnit.Text = strMin;
            lblLongBreakMinUnit.Text = strMin;

            // 单位 (秒)
            string strSec = LanguageManager.GetString("Seconds") ?? "秒";
            lblWorkSecUnit.Text = strSec;
            lblShortBreakSecUnit.Text = strSec;
            lblLongBreakSecUnit.Text = strSec;

            // 按钮
            if (btnConfirm != null) btnConfirm.Text = LanguageManager.GetString("Save") ?? "保存";
            if (btnCancel != null) btnCancel.Text = LanguageManager.GetString("Cancel") ?? "取消";
        }

        protected override void BtnConfirm_Click(object? sender, EventArgs e) {
            WorkTimeMinutes = (int)nudWorkTimeMin.Value;
            WorkTimeSeconds = (int)nudWorkTimeSec.Value;
            ShortBreakMinutes = (int)nudShortBreakTimeMin.Value;
            ShortBreakSeconds = (int)nudShortBreakTimeSec.Value;
            LongBreakMinutes = (int)nudLongBreakTimeMin.Value;
            LongBreakSeconds = (int)nudLongBreakTimeSec.Value;

            base.BtnConfirm_Click(sender, e);
        }

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}