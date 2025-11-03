using System;
using System.Windows.Forms;
using System.Drawing;
using WinFormsDemo.Resources;
using AntdUI;

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
            this.tabCountPage = new System.Windows.Forms.TabPage();
            this.tabPomodoroPage = new System.Windows.Forms.TabPage();
            this.tabSettingsPage = new System.Windows.Forms.TabPage();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(400, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "mainMenuStrip";
            // 
            // tabControl
            // 
            this.tabControl.TabPages.AddRange(new[] {
                this.tabCountPage,
                this.tabPomodoroPage,
                this.tabSettingsPage});
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.Size = new System.Drawing.Size(400, 276);
            this.tabControl.TabIndex = 1;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // tabCountPage
            // 
            this.tabCountPage.Location = new System.Drawing.Point(4, 24);
            this.tabCountPage.Name = "tabCountPage";
            this.tabCountPage.Size = new System.Drawing.Size(392, 248);
            this.tabCountPage.TabIndex = 0;
            this.tabCountPage.Text = "";
            this.tabCountPage.UseVisualStyleBackColor = true;
            // 
            // tabPomodoroPage
            // 
            this.tabPomodoroPage.Location = new System.Drawing.Point(4, 24);
            this.tabPomodoroPage.Name = "tabPomodoroPage";
            this.tabPomodoroPage.Size = new System.Drawing.Size(392, 248);
            this.tabPomodoroPage.TabIndex = 1;
            this.tabPomodoroPage.Text = "";
            this.tabPomodoroPage.UseVisualStyleBackColor = true;
            // 
            // tabSettingsPage
            // 
            this.tabSettingsPage.Location = new System.Drawing.Point(4, 24);
            this.tabSettingsPage.Name = "tabSettingsPage";
            this.tabSettingsPage.Size = new System.Drawing.Size(392, 248);
            this.tabSettingsPage.TabIndex = 2;
            this.tabSettingsPage.Text = "";
            this.tabSettingsPage.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
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

            // 标签页已经在InitializeComponent中添加，这里不需要重复添加
            
            // 添加到对应的TabPage
            if (tabCountPage != null)
            {
                tabCountPage.Controls.Add(countControl);
                countControl.TimerStateChanged += OnCountTimerStateChanged;
            }
            if (tabPomodoroPage != null)
            {
                tabPomodoroPage.Controls.Add(pomodoroControl);
                pomodoroControl.TimerStateChanged += OnPomodoroTimerStateChanged;
                pomodoroControl.PomodoroCompleted += OnPomodoroCompleted;
            }
            if (tabSettingsPage != null)
            {
                tabSettingsPage.Controls.Add(settingsControl);
                settingsControl.WindowPositionChanged += OnWindowPositionChanged;
                settingsControl.LanguageChanged += OnLanguageChanged;
                settingsControl.AlwaysOnTopChanged += OnAlwaysOnTopChanged;
            }
        }
        
        private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // 当标签页切换时，可以在这里添加额外的逻辑
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
            
            // 更新选项卡标题
            if (tabControl != null && tabControl.TabPages.Count >= 3)
            {
                tabControl.TabPages[0].Text = LanguageManager.GetString("TabCount");
                tabControl.TabPages[1].Text = LanguageManager.GetString("TabPomodoro");
                tabControl.TabPages[2].Text = LanguageManager.GetString("TabSettings");
            }
            
            // 更新各功能控件的UI
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
        private System.Windows.Forms.TabPage? tabCountPage;
        private System.Windows.Forms.TabPage? tabPomodoroPage;
        private System.Windows.Forms.TabPage? tabSettingsPage;
    }
}