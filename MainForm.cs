using System;
using System.Windows.Forms;
using System.Drawing;
using WinFormsDemo.Resources;

namespace WinFormsDemo
{
    public partial class MainForm : Form
    {
        // 各个功能控件
        private CountControl? countControl;
        private PomodoroControl? pomodoroControl;
        private SettingsControl? settingsControl;

        public MainForm()
        {
            InitializeComponent();
            InitializeControls();
            InitializeLanguageSupport();
            
            // 确保窗口可见并具有合理的大小和位置
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = true;
            this.Visible = true;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabCount = new System.Windows.Forms.TabPage();
            this.tabPomodoro = new System.Windows.Forms.TabPage();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.mainMenuStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabCount.SuspendLayout();
            this.tabPomodoro.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(300, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "mainMenuStrip";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabCount);
            this.tabControl.Controls.Add(this.tabPomodoro);
            this.tabControl.Controls.Add(this.tabSettings);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(300, 240);
            this.tabControl.TabIndex = 1;
            // 
            // tabCount
            // 
            this.tabCount.Location = new System.Drawing.Point(4, 24);
            this.tabCount.Name = "tabCount";
            this.tabCount.Padding = new System.Windows.Forms.Padding(3);
            this.tabCount.Size = new System.Drawing.Size(292, 212);
            this.tabCount.TabIndex = 0;
            this.tabCount.UseVisualStyleBackColor = true;
            // 
            // tabPomodoro
            // 
            this.tabPomodoro.Location = new System.Drawing.Point(4, 24);
            this.tabPomodoro.Name = "tabPomodoro";
            this.tabPomodoro.Padding = new System.Windows.Forms.Padding(3);
            this.tabPomodoro.Size = new System.Drawing.Size(292, 212);
            this.tabPomodoro.TabIndex = 1;
            this.tabPomodoro.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Location = new System.Drawing.Point(4, 24);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(292, 212);
            this.tabSettings.TabIndex = 2;
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 264);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabCount.ResumeLayout(false);
            this.tabPomodoro.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void InitializeControls()
        {
            // 初始化各个功能控件
            countControl = new CountControl();
            pomodoroControl = new PomodoroControl();
            settingsControl = new SettingsControl();

            // 设置控件的Dock属性
            countControl.Dock = DockStyle.Fill;
            pomodoroControl.Dock = DockStyle.Fill;
            settingsControl.Dock = DockStyle.Fill;

            // 添加到对应的Tab页面
            if (tabCount != null)
            {
                tabCount.Controls.Add(countControl);
                countControl.TimerStateChanged += OnCountTimerStateChanged;
            }
            if (tabPomodoro != null)
            {
                tabPomodoro.Controls.Add(pomodoroControl);
                pomodoroControl.TimerStateChanged += OnPomodoroTimerStateChanged;
                pomodoroControl.PomodoroCompleted += OnPomodoroCompleted;
            }
            if (tabSettings != null)
            {
                tabSettings.Controls.Add(settingsControl);
                settingsControl.WindowPositionChanged += OnWindowPositionChanged;
                settingsControl.LanguageChanged += OnLanguageChanged;
                settingsControl.AlwaysOnTopChanged += OnAlwaysOnTopChanged;
            }
        }
        
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 根据当前选中的选项卡更新UI
            UpdateUI();
        }

        private void InitializeLanguageSupport()
        {
            // 订阅语言改变事件
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
            
            // 初始化UI文本
            UpdateUI();
            
            // 初始化窗口置顶状态
            this.TopMost = true;
        }

        private void UpdateUI()
        {
            // 更新界面文本
            this.Text = LanguageManager.GetString("FormTitle");
            
            // 更新Tab页面标题
            tabCount!.Text = LanguageManager.GetString("TabCount");
            tabPomodoro!.Text = LanguageManager.GetString("TabPomodoro");
            tabSettings!.Text = LanguageManager.GetString("TabSettings");
            
            // 语言和窗口设置现在通过SettingsControl管理
            // 不再需要单独的菜单项
            
            // 更新各个控件的UI
            countControl?.UpdateUI();
            pomodoroControl?.UpdateUI();
            settingsControl?.UpdateUI();
        }

        // 事件处理方法
        private void OnCountTimerStateChanged(object? sender, EventArgs e)
        {
            // 计数计时器状态改变时的处理
            // 可以在这里添加跨组件的交互逻辑
        }

        private void OnPomodoroTimerStateChanged(object? sender, EventArgs e)
        {
            // 番茄时钟状态改变时的处理
            // 可以在这里添加跨组件的交互逻辑
        }

        private void OnPomodoroCompleted(object? sender, EventArgs e)
        {
            // 番茄时钟完成时的处理
            // 可以在这里添加提示或其他操作
        }

        private void OnWindowPositionChanged(object? sender, SettingsControl.WindowPositionChangedEventArgs e)
        {
            // 窗口位置改变时的处理
            settingsControl?.MoveWindowToPosition(this, e.Position);
        }
        
        private void OnLanguageChanged(object? sender, SettingsControl.LanguageChangedEventArgs e)
        {
            // 语言改变时的处理
            if (e.Language == SettingsControl.LanguageOption.Chinese)
            {
                LanguageManager.SwitchLanguage(LanguageManager.Chinese);
            }
            else
            {
                LanguageManager.SwitchLanguage(LanguageManager.English);
            }
        }
        
        private void OnAlwaysOnTopChanged(object? sender, SettingsControl.AlwaysOnTopChangedEventArgs e)
        {
            // 窗口置顶状态改变时的处理
            this.TopMost = e.IsAlwaysOnTop;
        }

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            // 语言改变时更新UI
            UpdateUI();
        }

        // 界面控件
        private System.ComponentModel.IContainer? components;
        private System.Windows.Forms.MenuStrip? mainMenuStrip;
        private System.Windows.Forms.TabControl? tabControl;
        private System.Windows.Forms.TabPage? tabCount;
        private System.Windows.Forms.TabPage? tabPomodoro;
        private System.Windows.Forms.TabPage? tabSettings;
    }
}