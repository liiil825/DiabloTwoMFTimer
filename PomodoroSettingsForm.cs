using System;
using System.Windows.Forms;
using WinFormsDemo.Resources;

namespace WinFormsDemo
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
            
            // 初始化当前设置值
            WorkTimeMinutes = workTime;
            WorkTimeSeconds = 0;
            ShortBreakMinutes = shortBreakTime;
            ShortBreakSeconds = 0;
            LongBreakMinutes = longBreakTime;
            LongBreakSeconds = 0;
            
            InitializeComponent();
            UpdateUI();
        }
        
        public PomodoroSettingsForm(int workTimeMinutes, int workTimeSeconds, int shortBreakMinutes, int shortBreakSeconds, int longBreakMinutes, int longBreakSeconds)
        {
            // 设置窗口属性
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            
            // 初始化当前设置值
            WorkTimeMinutes = workTimeMinutes;
            WorkTimeSeconds = workTimeSeconds;
            ShortBreakMinutes = shortBreakMinutes;
            ShortBreakSeconds = shortBreakSeconds;
            LongBreakMinutes = longBreakMinutes;
            LongBreakSeconds = longBreakSeconds;
            
            InitializeComponent();
            UpdateUI();
        }
        
        private void InitializeComponent()
        {
            // 创建控件
            this.lblWorkTime = new System.Windows.Forms.Label();
            this.lblShortBreakTime = new System.Windows.Forms.Label();
            this.lblLongBreakTime = new System.Windows.Forms.Label();
            this.lblWorkTimeSec = new System.Windows.Forms.Label();
            this.lblShortBreakTimeSec = new System.Windows.Forms.Label();
            this.lblLongBreakTimeSec = new System.Windows.Forms.Label();
            this.nudWorkTimeMin = new System.Windows.Forms.NumericUpDown();
            this.nudShortBreakTimeMin = new System.Windows.Forms.NumericUpDown();
            this.nudLongBreakTimeMin = new System.Windows.Forms.NumericUpDown();
            this.nudWorkTimeSec = new System.Windows.Forms.NumericUpDown();
            this.nudShortBreakTimeSec = new System.Windows.Forms.NumericUpDown();
            this.nudLongBreakTimeSec = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            
            // 设置控件属性
            this.Text = "番茄时钟设置";
            this.Size = new System.Drawing.Size(380, 250);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            
            // 设置数值选择框属性 - 分钟
            this.nudWorkTimeMin.Minimum = 0;
            this.nudWorkTimeMin.Maximum = 60;
            this.nudWorkTimeMin.Value = WorkTimeMinutes;
            this.nudShortBreakTimeMin.Minimum = 0;
            this.nudShortBreakTimeMin.Maximum = 30;
            this.nudShortBreakTimeMin.Value = ShortBreakMinutes;
            this.nudLongBreakTimeMin.Minimum = 0;
            this.nudLongBreakTimeMin.Maximum = 60;
            this.nudLongBreakTimeMin.Value = LongBreakMinutes;
            
            // 设置数值选择框属性 - 秒
            this.nudWorkTimeSec.Minimum = 0;
            this.nudWorkTimeSec.Maximum = 59;
            this.nudWorkTimeSec.Value = WorkTimeSeconds;
            this.nudShortBreakTimeSec.Minimum = 0;
            this.nudShortBreakTimeSec.Maximum = 59;
            this.nudShortBreakTimeSec.Value = ShortBreakSeconds;
            this.nudLongBreakTimeSec.Minimum = 0;
            this.nudLongBreakTimeSec.Maximum = 59;
            this.nudLongBreakTimeSec.Value = LongBreakSeconds;
            
            // 设置按钮事件
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            
            // 设置控件位置
            this.lblWorkTime.Location = new System.Drawing.Point(30, 30);
            this.nudWorkTimeMin.Location = new System.Drawing.Point(120, 30);
            this.nudWorkTimeMin.Width = 70;
            this.lblWorkTimeSec.Location = new System.Drawing.Point(200, 30);
            this.nudWorkTimeSec.Location = new System.Drawing.Point(240, 30);
            this.nudWorkTimeSec.Width = 70;
            
            this.lblShortBreakTime.Location = new System.Drawing.Point(30, 70);
            this.nudShortBreakTimeMin.Location = new System.Drawing.Point(120, 70);
            this.nudShortBreakTimeMin.Width = 70;
            this.lblShortBreakTimeSec.Location = new System.Drawing.Point(200, 70);
            this.nudShortBreakTimeSec.Location = new System.Drawing.Point(240, 70);
            this.nudShortBreakTimeSec.Width = 70;
            
            this.lblLongBreakTime.Location = new System.Drawing.Point(30, 110);
            this.nudLongBreakTimeMin.Location = new System.Drawing.Point(120, 110);
            this.nudLongBreakTimeMin.Width = 70;
            this.lblLongBreakTimeSec.Location = new System.Drawing.Point(200, 110);
            this.nudLongBreakTimeSec.Location = new System.Drawing.Point(240, 110);
            this.nudLongBreakTimeSec.Width = 70;
            
            this.btnSave.Location = new System.Drawing.Point(100, 160);
            this.btnSave.Width = 80;
            
            this.btnCancel.Location = new System.Drawing.Point(200, 160);
            this.btnCancel.Width = 80;
            
            // 添加控件到窗体
            this.Controls.Add(this.lblWorkTime);
            this.Controls.Add(this.nudWorkTimeMin);
            this.Controls.Add(this.lblWorkTimeSec);
            this.Controls.Add(this.nudWorkTimeSec);
            this.Controls.Add(this.lblShortBreakTime);
            this.Controls.Add(this.nudShortBreakTimeMin);
            this.Controls.Add(this.lblShortBreakTimeSec);
            this.Controls.Add(this.nudShortBreakTimeSec);
            this.Controls.Add(this.lblLongBreakTime);
            this.Controls.Add(this.nudLongBreakTimeMin);
            this.Controls.Add(this.lblLongBreakTimeSec);
            this.Controls.Add(this.nudLongBreakTimeSec);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
        }
        
        public void UpdateUI()
        {
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
    }
}