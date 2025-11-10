using System;
using System.Windows.Forms;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Settings;

namespace DTwoMFTimerHelper
{
    public class PomodoroSettingsForm : Form
    {
        // 控件声明
        private Label? lblWorkTime;
        private Label? lblShortBreakTime;
        private Label? lblLongBreakTime;
        private Label? lblWorkTimeSec;
        private Label? lblShortBreakTimeSec;
        private Label? lblLongBreakTimeSec;
        private NumericUpDown? nudWorkTimeMin;
        private NumericUpDown? nudShortBreakTimeMin;
        private NumericUpDown? nudLongBreakTimeMin;
        private NumericUpDown? nudWorkTimeSec;
        private NumericUpDown? nudShortBreakTimeSec;
        private NumericUpDown? nudLongBreakTimeSec;
        private Button? btnSave;
        private Button? btnCancel;

        // 属性，用于获取设置的值
        public int WorkTimeMinutes { get; private set; }
        public int WorkTimeSeconds { get; private set; }
        public int ShortBreakMinutes { get; private set; }
        public int ShortBreakSeconds { get; private set; }
        public int LongBreakMinutes { get; private set; }
        public int LongBreakSeconds { get; private set; }

        public PomodoroSettingsForm(int workTime, int shortBreakTime, int longBreakTime)
        {
            // 设置窗口属性
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;

            // 从配置文件加载默认设置
            var settings = SettingsManager.LoadSettings();
            
            // 使用传入的值作为当前值，但从配置文件获取默认值
            WorkTimeMinutes = workTime;
            WorkTimeSeconds = settings.WorkTimeSeconds;
            ShortBreakMinutes = shortBreakTime;
            ShortBreakSeconds = settings.ShortBreakSeconds;
            LongBreakMinutes = longBreakTime;
            LongBreakSeconds = settings.LongBreakSeconds;

            InitializeComponent();
            UpdateUI();
        }

        public PomodoroSettingsForm(int workTimeMinutes, int workTimeSeconds, int shortBreakMinutes, int shortBreakSeconds, int longBreakMinutes, int longBreakSeconds)
        {
            // 设置窗口属性
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;

            // 从配置文件加载默认设置
            var settings = SettingsManager.LoadSettings();
            
            // 使用传入的值，但如果传入的值为默认值，则使用配置文件中的值
            WorkTimeMinutes = workTimeMinutes;
            WorkTimeSeconds = workTimeSeconds > 0 ? workTimeSeconds : settings.WorkTimeSeconds;
            ShortBreakMinutes = shortBreakMinutes;
            ShortBreakSeconds = shortBreakSeconds > 0 ? shortBreakSeconds : settings.ShortBreakSeconds;
            LongBreakMinutes = longBreakMinutes;
            LongBreakSeconds = longBreakSeconds > 0 ? longBreakSeconds : settings.LongBreakSeconds;

            InitializeComponent();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            lblWorkTime = new Label();
            lblShortBreakTime = new Label();
            lblLongBreakTime = new Label();
            lblWorkTimeSec = new Label();
            lblShortBreakTimeSec = new Label();
            lblLongBreakTimeSec = new Label();
            nudWorkTimeMin = new NumericUpDown();
            nudShortBreakTimeMin = new NumericUpDown();
            nudLongBreakTimeMin = new NumericUpDown();
            nudWorkTimeSec = new NumericUpDown();
            nudShortBreakTimeSec = new NumericUpDown();
            nudLongBreakTimeSec = new NumericUpDown();
            btnSave = new Button();
            btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)nudWorkTimeMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudShortBreakTimeMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLongBreakTimeMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWorkTimeSec).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudShortBreakTimeSec).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLongBreakTimeSec).BeginInit();
            SuspendLayout();
            // 
            // lblWorkTime
            // 
            lblWorkTime.Location = new System.Drawing.Point(41, 28);
            lblWorkTime.Name = "lblWorkTime";
            lblWorkTime.Size = new System.Drawing.Size(100, 36);
            lblWorkTime.TabIndex = 0;
            // 
            // lblShortBreakTime
            // 
            lblShortBreakTime.Location = new System.Drawing.Point(41, 68);
            lblShortBreakTime.Name = "lblShortBreakTime";
            lblShortBreakTime.Size = new System.Drawing.Size(100, 36);
            lblShortBreakTime.TabIndex = 4;
            // 
            // lblLongBreakTime
            // 
            lblLongBreakTime.Location = new System.Drawing.Point(41, 104);
            lblLongBreakTime.Name = "lblLongBreakTime";
            lblLongBreakTime.Size = new System.Drawing.Size(100, 40);
            lblLongBreakTime.TabIndex = 8;
            // 
            // lblWorkTimeSec
            // 
            lblWorkTimeSec.Location = new System.Drawing.Point(253, 32);
            lblWorkTimeSec.Name = "lblWorkTimeSec";
            lblWorkTimeSec.Size = new System.Drawing.Size(47, 32);
            lblWorkTimeSec.TabIndex = 2;
            // 
            // lblShortBreakTimeSec
            // 
            lblShortBreakTimeSec.Location = new System.Drawing.Point(253, 70);
            lblShortBreakTimeSec.Name = "lblShortBreakTimeSec";
            lblShortBreakTimeSec.Size = new System.Drawing.Size(47, 23);
            lblShortBreakTimeSec.TabIndex = 6;
            // 
            // lblLongBreakTimeSec
            // 
            lblLongBreakTimeSec.Location = new System.Drawing.Point(253, 110);
            lblLongBreakTimeSec.Name = "lblLongBreakTimeSec";
            lblLongBreakTimeSec.Size = new System.Drawing.Size(47, 32);
            lblLongBreakTimeSec.TabIndex = 10;
            lblLongBreakTimeSec.Click += lblLongBreakTimeSec_Click;
            // 
            // nudWorkTimeMin
            // 
            nudWorkTimeMin.Location = new System.Drawing.Point(177, 30);
            nudWorkTimeMin.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            nudWorkTimeMin.Name = "nudWorkTimeMin";
            nudWorkTimeMin.Size = new System.Drawing.Size(70, 34);
            nudWorkTimeMin.TabIndex = 1;
            nudWorkTimeMin.ValueChanged += nudWorkTimeMin_ValueChanged;
            // 
            // nudShortBreakTimeMin
            // 
            nudShortBreakTimeMin.Location = new System.Drawing.Point(177, 70);
            nudShortBreakTimeMin.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            nudShortBreakTimeMin.Name = "nudShortBreakTimeMin";
            nudShortBreakTimeMin.Size = new System.Drawing.Size(70, 34);
            nudShortBreakTimeMin.TabIndex = 5;
            // 
            // nudLongBreakTimeMin
            // 
            nudLongBreakTimeMin.Location = new System.Drawing.Point(177, 110);
            nudLongBreakTimeMin.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            nudLongBreakTimeMin.Name = "nudLongBreakTimeMin";
            nudLongBreakTimeMin.Size = new System.Drawing.Size(70, 34);
            nudLongBreakTimeMin.TabIndex = 9;
            // 
            // nudWorkTimeSec
            // 
            nudWorkTimeSec.Location = new System.Drawing.Point(306, 30);
            nudWorkTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            nudWorkTimeSec.Name = "nudWorkTimeSec";
            nudWorkTimeSec.Size = new System.Drawing.Size(70, 34);
            nudWorkTimeSec.TabIndex = 3;
            // 
            // nudShortBreakTimeSec
            // 
            nudShortBreakTimeSec.Location = new System.Drawing.Point(306, 70);
            nudShortBreakTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            nudShortBreakTimeSec.Name = "nudShortBreakTimeSec";
            nudShortBreakTimeSec.Size = new System.Drawing.Size(70, 34);
            nudShortBreakTimeSec.TabIndex = 7;
            // 
            // nudLongBreakTimeSec
            // 
            nudLongBreakTimeSec.Location = new System.Drawing.Point(306, 110);
            nudLongBreakTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            nudLongBreakTimeSec.Name = "nudLongBreakTimeSec";
            nudLongBreakTimeSec.Size = new System.Drawing.Size(70, 34);
            nudLongBreakTimeSec.TabIndex = 11;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(147, 177);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(80, 23);
            btnSave.TabIndex = 12;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(273, 177);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(80, 23);
            btnCancel.TabIndex = 13;
            btnCancel.Click += btnCancel_Click;
            // 
            // PomodoroSettingsForm
            // 
            ClientSize = new System.Drawing.Size(504, 334);
            Controls.Add(lblWorkTime);
            Controls.Add(nudWorkTimeMin);
            Controls.Add(lblWorkTimeSec);
            Controls.Add(nudWorkTimeSec);
            Controls.Add(lblShortBreakTime);
            Controls.Add(nudShortBreakTimeMin);
            Controls.Add(lblShortBreakTimeSec);
            Controls.Add(nudShortBreakTimeSec);
            Controls.Add(lblLongBreakTime);
            Controls.Add(nudLongBreakTimeMin);
            Controls.Add(lblLongBreakTimeSec);
            Controls.Add(nudLongBreakTimeSec);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "PomodoroSettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "番茄时钟设置";
            ((System.ComponentModel.ISupportInitialize)nudWorkTimeMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudShortBreakTimeMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLongBreakTimeMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWorkTimeSec).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudShortBreakTimeSec).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLongBreakTimeSec).EndInit();
            ResumeLayout(false);
        }

        public void UpdateUI()
        {
            // 设置数值控件的值
            if (nudWorkTimeMin != null) nudWorkTimeMin.Value = WorkTimeMinutes;
            if (nudWorkTimeSec != null) nudWorkTimeSec.Value = WorkTimeSeconds;
            if (nudShortBreakTimeMin != null) nudShortBreakTimeMin.Value = ShortBreakMinutes;
            if (nudShortBreakTimeSec != null) nudShortBreakTimeSec.Value = ShortBreakSeconds;
            if (nudLongBreakTimeMin != null) nudLongBreakTimeMin.Value = LongBreakMinutes;
            if (nudLongBreakTimeSec != null) nudLongBreakTimeSec.Value = LongBreakSeconds;
            // 更新界面文本
            this.Text = LanguageManager.GetString("PomodoroSettings") ?? "番茄时钟设置";
            this.lblWorkTime!.Text = LanguageManager.GetString("WorkTime") ?? "工作时间(分):";
            this.lblWorkTimeSec!.Text = "秒:";
            this.lblShortBreakTime!.Text = LanguageManager.GetString("ShortBreakTime") ?? "短休息时间(分):";
            this.lblShortBreakTimeSec!.Text = "秒:";
            this.lblLongBreakTime!.Text = LanguageManager.GetString("LongBreakTime") ?? "长休息时间(分):";
            this.lblLongBreakTimeSec!.Text = "秒:";
            this.btnSave!.Text = LanguageManager.GetString("Save") ?? "保存";
            this.btnCancel!.Text = LanguageManager.GetString("Cancel") ?? "取消";
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            // 保存设置
            WorkTimeMinutes = (int)this.nudWorkTimeMin!.Value;
            WorkTimeSeconds = (int)this.nudWorkTimeSec!.Value;
            ShortBreakMinutes = (int)this.nudShortBreakTimeMin!.Value;
            ShortBreakSeconds = (int)this.nudShortBreakTimeSec!.Value;
            LongBreakMinutes = (int)this.nudLongBreakTimeMin!.Value;
            LongBreakSeconds = (int)this.nudLongBreakTimeSec!.Value;

            // 设置对话框结果为OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            // 设置对话框结果为Cancel
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void nudWorkTimeMin_ValueChanged(object? sender, EventArgs e)
        {

        }

        private void lblLongBreakTimeSec_Click(object? sender, EventArgs e)
        {

        }
    }
}