#nullable disable

using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Pomodoro;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Timer
{
    partial class TimerControl
    {
        private TableLayoutPanel mainLayout;
        // 组件引用
        private StatisticsControl statisticsControl;
        private HistoryControl historyControl;
        private CharacterSceneControl characterSceneControl;
        private LootRecordsControl lootRecordsControl;
        private ThemedLabel labelTime1;

        // 控件字段定义
        private Label btnStatusIndicator;
        private ThemedButton toggleLootButton;
        private PomodoroTimeDisplayLabel pomodoroTime;
        private ThemedLabel lblTimeDisplay;

        private void InitializeComponent()
        {
            this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
            // 1. 初始化控件
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.topLayout = new System.Windows.Forms.TableLayoutPanel();
            this.bottomInfoLayout = new System.Windows.Forms.TableLayoutPanel();

            this.btnStatusIndicator = new System.Windows.Forms.Label();
            this.labelTime1 = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.pomodoroTime = new DiabloTwoMFTimer.UI.Pomodoro.PomodoroTimeDisplayLabel();
            this.toggleLootButton = new DiabloTwoMFTimer.UI.Components.ThemedButton();
            this.lblTimeDisplay = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

            this.statisticsControl = new DiabloTwoMFTimer.UI.Timer.StatisticsControl();
            this.historyControl = new DiabloTwoMFTimer.UI.Timer.HistoryControl();
            this.characterSceneControl = new DiabloTwoMFTimer.UI.Timer.CharacterSceneControl();
            this.lootRecordsControl = new DiabloTwoMFTimer.UI.Timer.LootRecordsControl();

            this.SuspendLayout();

            // ---------------------------------------------------------
            // 1. 主布局容器 (mainLayout)
            // ---------------------------------------------------------
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

            // 重要：先清除样式，防止重复添加导致索引错乱
            this.mainLayout.RowStyles.Clear();
            this.mainLayout.RowCount = 6;

            // --- 必须严格按照索引 0-5 的顺序添加 RowStyle ---

            // Row 0: Top Bar (50px)
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));

            // Row 1: Time Display (80px)
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));

            // Row 2: Stats (120px)
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));

            // Row 3: History (70%)
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));

            // Row 4: Loot (掉落记录) - [关键] 必须在这里，代码逻辑会控制它的大小
            // 初始设为 0F (隐藏)
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));

            // Row 5: Bottom (场景+按钮) - [关键] 必须是最后一个，固定 85px
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));

            // --- 添加控件 (注意第二个参数是 Column, 第三个参数是 Row) ---

            this.mainLayout.Controls.Add(this.topLayout, 0, 0);
            this.mainLayout.Controls.Add(this.lblTimeDisplay, 0, 1);
            this.mainLayout.Controls.Add(this.statisticsControl, 0, 2);
            this.mainLayout.Controls.Add(this.historyControl, 0, 3);

            // [关键] 确保 Loot 在第 4 行
            this.mainLayout.Controls.Add(this.lootRecordsControl, 0, 4);
            // [关键] 确保 Bottom Info 在第 5 行
            this.mainLayout.Controls.Add(this.bottomInfoLayout, 0, 5);

            // ---------------------------------------------------------
            // 2. 顶部布局
            // ---------------------------------------------------------
            this.topLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topLayout.Margin = new System.Windows.Forms.Padding(0);
            this.topLayout.ColumnCount = 3;
            this.topLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.topLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.topLayout.RowCount = 1;
            this.topLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            // 左侧容器
            System.Windows.Forms.FlowLayoutPanel topLeftFlow = new System.Windows.Forms.FlowLayoutPanel();
            topLeftFlow.WrapContents = false;
            topLeftFlow.AutoSize = true;
            topLeftFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            topLeftFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            topLeftFlow.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            topLeftFlow.Controls.Add(this.btnStatusIndicator);
            topLeftFlow.Controls.Add(this.labelTime1);
            topLeftFlow.Margin = new System.Windows.Forms.Padding(0);

            // 指示灯
            this.btnStatusIndicator.AutoSize = false;
            this.btnStatusIndicator.Size = new System.Drawing.Size(16, 16);
            this.btnStatusIndicator.BackColor = System.Drawing.Color.Red;
            this.btnStatusIndicator.Margin = new System.Windows.Forms.Padding(15, 17, 0, 0);

            // 日期时间
            this.labelTime1.AutoSize = true;
            this.labelTime1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.labelTime1.ForeColor = System.Drawing.Color.LightGray;
            this.labelTime1.Text = "12:00";
            this.labelTime1.Margin = new System.Windows.Forms.Padding(8, 8, 0, 0);

            // 番茄钟
            this.pomodoroTime.AutoSize = true;
            this.pomodoroTime.Dock = System.Windows.Forms.DockStyle.Right;
            this.pomodoroTime.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.pomodoroTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.pomodoroTime.ForeColor = System.Drawing.Color.White;
            this.pomodoroTime.ShowMilliseconds = false;

            this.topLayout.Controls.Add(topLeftFlow, 0, 0);
            this.topLayout.Controls.Add(this.pomodoroTime, 2, 0);

            // ---------------------------------------------------------
            // 3. 主计时器
            // ---------------------------------------------------------
            this.lblTimeDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTimeDisplay.AutoSize = false;
            this.lblTimeDisplay.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.BigTimeFont;
            this.lblTimeDisplay.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.AccentColor;
            this.lblTimeDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTimeDisplay.Text = "00:00:00.0";

            // ---------------------------------------------------------
            // 4. 底部布局 (场景+按钮)
            // ---------------------------------------------------------
            this.bottomInfoLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomInfoLayout.Margin = new System.Windows.Forms.Padding(0);
            this.bottomInfoLayout.ColumnCount = 2;
            this.bottomInfoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bottomInfoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.bottomInfoLayout.RowCount = 1;
            this.bottomInfoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.characterSceneControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.characterSceneControl.Margin = new System.Windows.Forms.Padding(5);

            this.toggleLootButton.Size = new System.Drawing.Size(120, 36);
            this.toggleLootButton.Text = "Show Loot";
            this.toggleLootButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.toggleLootButton.Margin = new System.Windows.Forms.Padding(0, 0, 10, 25);
            this.toggleLootButton.Click += new System.EventHandler(this.ToggleLootButton_Click);

            this.bottomInfoLayout.Controls.Add(this.characterSceneControl, 0, 0);
            this.bottomInfoLayout.Controls.Add(this.toggleLootButton, 1, 0);

            // ---------------------------------------------------------
            // 5. 其他
            // ---------------------------------------------------------
            this.statisticsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lootRecordsControl.Dock = System.Windows.Forms.DockStyle.Fill;

            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainLayout);
            this.Name = "TimerControl";
            this.Size = new System.Drawing.Size(500, 600);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel topLayout;
        private System.Windows.Forms.TableLayoutPanel bottomInfoLayout;
    }
}