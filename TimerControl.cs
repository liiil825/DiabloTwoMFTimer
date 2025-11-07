using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Data;
using DTwoMFTimerHelper.Resources;

namespace DTwoMFTimerHelper
{
    public partial class TimerControl : UserControl
    {
        // 计时器相关字段
        private bool isTimerRunning = false;
        private DateTime startTime = DateTime.MinValue;
        private System.Windows.Forms.Timer? timer;
        private Data.CharacterProfile? currentProfile = null;

        // 运行统计数据
        private int runCount = 0;
        private List<TimeSpan> runHistory = new List<TimeSpan>();
        private TimeSpan fastestTime = TimeSpan.MaxValue;

        // 事件
        public event EventHandler? TimerStateChanged;

        // 公共属性
        public bool IsTimerRunning => isTimerRunning;
        public Data.CharacterProfile? CurrentProfile => currentProfile;

        public TimerControl()
        {
            InitializeComponent();
            InitializeTimer();
            
            // 注册语言变更事件
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
            
            UpdateUI();
        }
        
        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 取消注册语言变更事件
                LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // 状态指示按钮
            btnStatusIndicator = new Button();
            
            // 主要计时显示标签
            lblTimeDisplay = new Label();
            lblTimeDisplay.AutoSize = false; // 设置为手动调整大小
            lblTimeDisplay.TextAlign = ContentAlignment.MiddleCenter; // 居中对齐
            
            // 运行统计信息
            lblRunCount = new Label();
            lblFastestTime = new Label();
            lblAverageTime = new Label();
            
            // 历史记录区域
            lstRunHistory = new ListBox();
            
            // 角色和场景显示标签
            lblCharacterDisplay = new Label();
            lblSceneDisplay = new Label();
            
            SuspendLayout();
            // 
            // btnStatusIndicator - 状态指示按钮
            // 
            btnStatusIndicator.Enabled = false;
            btnStatusIndicator.FlatStyle = FlatStyle.Flat;
            btnStatusIndicator.Size = new Size(16, 16);
            btnStatusIndicator.Location = new Point(15, 45);
            btnStatusIndicator.Name = "btnStatusIndicator";
            btnStatusIndicator.TabIndex = 0;
            btnStatusIndicator.TabStop = false;
            btnStatusIndicator.BackColor = Color.Red;
            btnStatusIndicator.FlatAppearance.BorderSize = 0;
            
            // 
            // lblTimeDisplay - 计时显示
            // 
            lblTimeDisplay.AutoSize = false;
            lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 30F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134))); // 减小默认字体大小
            lblTimeDisplay.Location = new Point(15, 30); // 调整位置更靠左
            lblTimeDisplay.Name = "lblTimeDisplay";
            lblTimeDisplay.Size = new Size(290, 64); // 增加宽度以确保显示完整的8位时间
            lblTimeDisplay.TextAlign = ContentAlignment.MiddleCenter; // 居中对齐
            lblTimeDisplay.TabIndex = 1;
            lblTimeDisplay.Text = "00:00:00:0";
            
            // 
            // lblRunCount - 运行次数显示
            // 
            lblRunCount.AutoSize = false;
            lblRunCount.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblRunCount.Location = new Point(15, 100);
            lblRunCount.Name = "lblRunCount";
            lblRunCount.Size = new Size(290, 21); // 增加宽度确保完整显示
            lblRunCount.TextAlign = ContentAlignment.MiddleLeft;
            lblRunCount.TabIndex = 2;
            lblRunCount.Text = "--- Run count 0 (0) ---";
            
            // 
            // lblFastestTime - 最快时间显示
            // 
            lblFastestTime.AutoSize = true;
            lblFastestTime.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblFastestTime.Location = new Point(15, 125);
            lblFastestTime.Name = "lblFastestTime";
            lblFastestTime.Size = new Size(120, 19);
            lblFastestTime.TabIndex = 3;
            lblFastestTime.Text = "Fastest time: --:--:--.-";
            
            // 
            // lblAverageTime - 平均时间显示
            // 
            lblAverageTime.AutoSize = true;
            lblAverageTime.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblAverageTime.Location = new Point(15, 145);
            lblAverageTime.Name = "lblAverageTime";
            lblAverageTime.Size = new Size(125, 19);
            lblAverageTime.TabIndex = 4;
            lblAverageTime.Text = "Average time: --:--:--.-";
            
            // 
            // lstRunHistory - 历史记录列表
            // 
            lstRunHistory.FormattingEnabled = true;
            lstRunHistory.ItemHeight = 15;
            lstRunHistory.Location = new Point(15, 190);
            lstRunHistory.Name = "lstRunHistory";
            lstRunHistory.Size = new Size(290, 119);
            lstRunHistory.TabIndex = 7;
            
            // 
            // lblCharacterDisplay - 角色显示
            // 
            lblCharacterDisplay.BorderStyle = BorderStyle.FixedSingle;
            lblCharacterDisplay.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblCharacterDisplay.Location = new Point(15, 165);
            lblCharacterDisplay.Name = "lblCharacterDisplay";
            lblCharacterDisplay.Size = new Size(140, 25);
            lblCharacterDisplay.TextAlign = ContentAlignment.MiddleCenter;
            lblCharacterDisplay.TabIndex = 5;
            lblCharacterDisplay.Text = LanguageManager.GetString("CharacterLabel") + " ";
            
            // 
            // lblSceneDisplay - 场景显示
            // 
            lblSceneDisplay.BorderStyle = BorderStyle.FixedSingle;
            lblSceneDisplay.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblSceneDisplay.Location = new Point(165, 165);
            lblSceneDisplay.Name = "lblSceneDisplay";
            lblSceneDisplay.Size = new Size(140, 25);
            lblSceneDisplay.TextAlign = ContentAlignment.MiddleCenter;
            lblSceneDisplay.TabIndex = 6;
            lblSceneDisplay.Text = LanguageManager.GetString("SceneLabel") + " ";
            
            // 
            // TimerControl - 主控件设置
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnStatusIndicator);
            Controls.Add(lblTimeDisplay);
            Controls.Add(lblRunCount);
            Controls.Add(lblFastestTime);
            Controls.Add(lblAverageTime);
            Controls.Add(lblCharacterDisplay);
            Controls.Add(lblSceneDisplay);
            Controls.Add(lstRunHistory);
            Name = "TimerControl";
            Size = new Size(320, 320);
            ResumeLayout(false);
            PerformLayout();
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 100; // 100毫秒
            timer.Tick += Timer_Tick;
        }

        private string currentCharacter = "";
        private string currentScene = "";
        
        // 设置当前角色和场景的方法
        public void SetCharacterAndScene(string character, string scene)
        {
            currentCharacter = character;
            currentScene = scene;
            UpdateUI();
        }
        
        public void UpdateUI()
        {
            // 更新状态指示按钮颜色
            if (btnStatusIndicator != null)
            {
                btnStatusIndicator.BackColor = isTimerRunning ? Color.Green : Color.Red;
            }
            
            // 更新时间显示
            if (isTimerRunning && startTime != DateTime.MinValue)
            {
                TimeSpan elapsed;
                
                if (isPaused && pauseStartTime != DateTime.MinValue)
                {
                    // 暂停状态，计算到暂停开始时的时间
                    elapsed = pauseStartTime - startTime - pausedDuration;
                }
                else
                {
                    // 运行状态，计算实际经过时间（扣除暂停时间）
                    elapsed = DateTime.Now - startTime - pausedDuration;
                }
                
                // 显示100毫秒格式
                string formattedTime = string.Format("{0:00}:{1:00}:{2:00}:{3}", 
                    elapsed.Hours, elapsed.Minutes, elapsed.Seconds, 
                    (int)(elapsed.Milliseconds / 100));
                    
                if (lblTimeDisplay != null) 
                {
                    // 根据时间长度调整字体大小确保显示完整
                    if (elapsed.Hours > 9)
                    {
                        // 小时数超过9时使用更小的字体
                        lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 24F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                    }
                    else if (elapsed.Hours > 0)
                    {
                        // 有小时数但不超过9时使用中等字体
                        lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 28F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                    }
                    else
                    {
                        // 没有小时数时使用合适的字体大小
                        lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 30F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                    }
                    
                    // 暂停时显示不同的样式
                    if (isPaused)
                    {
                        lblTimeDisplay.Text = formattedTime;
                    }
                    else
                    {
                        lblTimeDisplay.Text = formattedTime;
                    }
                }
            }
            else
            {
                if (lblTimeDisplay != null) 
                {
                    lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 30F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                    lblTimeDisplay.Text = "00:00:00:0";
                }
            }
            
            // 更新统计信息
            if (lblRunCount != null)
            {
                // 使用多语言显示运行次数
                string runCountText = LanguageManager.GetString("RunCount", runCount, runCount);
                if (string.IsNullOrEmpty(runCountText) || runCountText == "RunCount")
                {
                    // 如果未找到翻译，使用默认格式
                    runCountText = $"--- Run count {runCount} ({runCount}) ---";
                }
                else
                {
                    runCountText = $"--- {runCountText} ---";
                }
                lblRunCount.Text = runCountText;
            }
            
            if (lblFastestTime != null)
            {
                if (runCount > 0 && fastestTime != TimeSpan.MaxValue)
                {
                    string fastestFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}", 
                        fastestTime.Hours, fastestTime.Minutes, fastestTime.Seconds, 
                        (int)(fastestTime.Milliseconds / 100));
                    
                    string fastestTimeText = LanguageManager.GetString("FastestTime", fastestFormatted);
                    if (string.IsNullOrEmpty(fastestTimeText) || fastestTimeText == "FastestTime")
                    {
                        fastestTimeText = $"Fastest time: {fastestFormatted}";
                    }
                    
                    lblFastestTime.Text = fastestTimeText;
                }
                else
                {
                    string fastestTimeText = LanguageManager.GetString("FastestTimePlaceholder");
                    if (string.IsNullOrEmpty(fastestTimeText) || fastestTimeText == "FastestTimePlaceholder")
                    {
                        fastestTimeText = "Fastest time: --:--:--.-";
                    }
                    
                    lblFastestTime.Text = fastestTimeText;
                }
            }
            
            if (lblAverageTime != null)
            {
                if (runCount > 0)
                {
                    TimeSpan averageTime = TimeSpan.Zero;
                    foreach (var time in runHistory)
                    {
                        averageTime += time;
                    }
                    averageTime = new TimeSpan(averageTime.Ticks / runHistory.Count);
                    
                    string averageFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}", 
                        averageTime.Hours, averageTime.Minutes, averageTime.Seconds, 
                        (int)(averageTime.Milliseconds / 100));
                    
                    string averageTimeText = LanguageManager.GetString("AverageTime", averageFormatted);
                    if (string.IsNullOrEmpty(averageTimeText) || averageTimeText == "AverageTime")
                    {
                        averageTimeText = $"Average time: {averageFormatted}";
                    }
                    
                    lblAverageTime.Text = averageTimeText;
                }
                else
                {
                    string averageTimeText = LanguageManager.GetString("AverageTimePlaceholder");
                    if (string.IsNullOrEmpty(averageTimeText) || averageTimeText == "AverageTimePlaceholder")
                    {
                        averageTimeText = "Average time: --:--:--.-";
                    }
                    
                    lblAverageTime.Text = averageTimeText;
                }
            }
            
            // 更新历史记录列表
            if (lstRunHistory != null)
            {
                lstRunHistory.Items.Clear();
                for (int i = 0; i < runHistory.Count; i++)
                {
                    var time = runHistory[i];
                    string timeFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}", 
                        time.Hours, time.Minutes, time.Seconds, 
                        (int)(time.Milliseconds / 100));
                    string runText = LanguageManager.GetString("RunNumber", i + 1, timeFormatted);
                    if (string.IsNullOrEmpty(runText) || runText == "RunNumber")
                    {
                        runText = $"Run {i + 1}: {timeFormatted}";
                    }
                    lstRunHistory.Items.Add(runText);
                }
                
                // 确保最新记录在顶部
                if (lstRunHistory.Items.Count > 0)
                {
                    lstRunHistory.SelectedIndex = 0;
                }
            }
            
            // 更新角色和场景显示
            if (lblCharacterDisplay != null)
            {
                string characterLabel = LanguageManager.GetString("CharacterLabel");
                if (string.IsNullOrEmpty(currentCharacter))
                {
                    lblCharacterDisplay.Text = characterLabel + " ";
                }
                else
                {
                    lblCharacterDisplay.Text = characterLabel + " " + currentCharacter;
                }
            }
            
            if (lblSceneDisplay != null)
            {
                string sceneLabel = LanguageManager.GetString("SceneLabel");
                if (string.IsNullOrEmpty(currentScene))
                {
                    lblSceneDisplay.Text = sceneLabel + " ";
                }
                else
                {
                    lblSceneDisplay.Text = sceneLabel + " " + currentScene;
                }
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        // 提供给外部调用的开始/停止方法，用于快捷键触发
        public void ToggleTimer()
        {
            if (!isTimerRunning)
            {
                StartTimer();
            }
            else
            {
                StopTimer();
            }
        }
        
        // 提供给外部调用的暂停方法，用于快捷键触发
        public void TogglePause()
        {
            if (isTimerRunning)
            {
                if (isPaused)
                {
                    ResumeTimer();
                }
                else
                {
                    PauseTimer();
                }
            }
        }
        
        private void PauseTimer()
        {
            if (isTimerRunning && !isPaused)
            {
                isPaused = true;
                pauseStartTime = DateTime.Now;
                UpdateUI();
            }
        }
        
        private void ResumeTimer()
        {
            if (isTimerRunning && isPaused && pauseStartTime != DateTime.MinValue)
            {
                pausedDuration += DateTime.Now - pauseStartTime;
                isPaused = false;
                pauseStartTime = DateTime.MinValue;
                UpdateUI();
            }
        }
        
        // 提供给外部调用的重置方法
        public void ResetTimerExternally()
        {
            ResetTimer();
        }

        private void StartTimer()
        {
            isTimerRunning = true;
            isPaused = false;
            startTime = DateTime.Now;
            pausedDuration = TimeSpan.Zero;
            pauseStartTime = DateTime.MinValue;
            timer?.Start();
            
            UpdateUI();
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void StopTimer()
        {
            isTimerRunning = false;
            isPaused = false;
            timer?.Stop();
            
            // 记录本次运行时间
            if (startTime != DateTime.MinValue)
            {
                TimeSpan runTime = DateTime.Now - startTime - pausedDuration;
                runHistory.Insert(0, runTime); // 新记录插入到开头
                runCount++;
                
                // 更新最快时间
                if (runTime < fastestTime)
                {
                    fastestTime = runTime;
                }
            }
            
            UpdateUI();
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ResetTimer()
        {
            StopTimer();
            ResetTimerDisplay();
        }

        private void ResetTimerDisplay()
        {
            startTime = DateTime.MinValue;
            pausedDuration = TimeSpan.Zero;
            pauseStartTime = DateTime.MinValue;
            UpdateUI();
        }

        public void SetCurrentProfile(Data.CharacterProfile? profile)
        {
            currentProfile = profile;
            UpdateUI();
        }

        // 控件字段定义
        private Button? btnStatusIndicator;
        private Label? lblTimeDisplay;
        private Label? lblRunCount;
        private Label? lblFastestTime;
        private Label? lblAverageTime;
        private ListBox? lstRunHistory;
        private Label? lblCharacterDisplay;
        private Label? lblSceneDisplay;
        
        // 计时器状态字段
        private bool isPaused = false;
        private TimeSpan pausedDuration = TimeSpan.Zero;
        private DateTime pauseStartTime = DateTime.MinValue;
    }
}