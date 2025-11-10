using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using DTwoMFTimerHelper.Data;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Settings;

namespace DTwoMFTimerHelper
{
    public partial class TimerControl : UserControl
    {
        // 计时器相关字段
        private bool isTimerRunning = false;
        private DateTime startTime = DateTime.MinValue;
        private System.Windows.Forms.Timer? timer;
        private Data.CharacterProfile? currentProfile = null;
        private Data.MFRecord? inProgressRecord = null;
        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 100; // 100毫秒
            timer.Tick += Timer_Tick;
        }

        private string currentCharacter = "";
        private string currentScene = "";
        // 运行统计数据
        private int runCount = 0;
        private List<TimeSpan> runHistory = new List<TimeSpan>();
        private TimeSpan fastestTime = TimeSpan.MaxValue;

        // 事件
        public event EventHandler? TimerStateChanged;

        // 公共属性
        public bool IsTimerRunning => isTimerRunning;
        public Data.CharacterProfile? CurrentProfile => currentProfile;
        public Data.MFRecord? InProgressRecord => inProgressRecord;

        public TimerControl()
        {
            InitializeComponent();
            InitializeTimer();
            
            // 注册语言变更事件
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
            
            // 在界面初始化时，绑定最近一条未完成的记录到对应属性
            BindLatestIncompleteRecord();
            
            UpdateUI();
        }
        
        /// <summary>
        /// 绑定最近一条未完成的记录到对应属性
        /// </summary>
        private void BindLatestIncompleteRecord()
        {
            LogManager.WriteDebugLog("TimerControl", $"触发函数 BindLatestIncompleteRecord");

            try
            {
                // 获取当前难度信息
                var currentDifficulty = GetCurrentDifficulty();
                
                // 查找最近一条未完成的记录
                var incompleteRecord = FindIncompleteRecordForCurrentScene();
                
                if (incompleteRecord != null)
                {
                    LogManager.WriteDebugLog("TimerControl", $"绑定最近未完成记录到属性: Scene={incompleteRecord.SceneName}, Difficulty={incompleteRecord.Difficulty}, ElapsedTime={incompleteRecord.ElapsedTime}秒");
                    
                    // 将记录信息绑定到对应的属性
                    this.inProgressRecord = incompleteRecord;
                    
                    // 如果有elapsedTime值，调整startTime以便正确显示
                    if (incompleteRecord.ElapsedTime.HasValue && incompleteRecord.ElapsedTime.Value > 0)
                    {
                        LogManager.WriteDebugLog("TimerControl", $"从yaml文件加载elapsedTime: {incompleteRecord.ElapsedTime}秒，调整startTime");
                        // 调整startTime，使得计算出的elapsed等于yaml中的elapsedTime
                        if (isPaused && pauseStartTime != DateTime.MinValue)
                        {
                            this.startTime = pauseStartTime - TimeSpan.FromSeconds(incompleteRecord.ElapsedTime.Value) - pausedDuration;
                        }
                        else
                        {
                            this.startTime = DateTime.Now - TimeSpan.FromSeconds(incompleteRecord.ElapsedTime.Value) - pausedDuration;
                        }
                        LogManager.WriteDebugLog("TimerControl", $"startTime: {this.startTime}");
                    }
                }
                else
                {
                    LogManager.WriteDebugLog("TimerControl", "没有找到未完成的记录");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"绑定未完成记录失败: {ex.Message}");
                LogManager.WriteDebugLog("TimerControl", $"绑定未完成记录失败: {ex.Message}, 堆栈: {ex.StackTrace}");
            }
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
            // lstRunHistory - 历史记录列表
            // 
            lstRunHistory.FormattingEnabled = true;
            lstRunHistory.ItemHeight = 15;
            lstRunHistory.Location = new Point(15, 170);
            lstRunHistory.Name = "lstRunHistory";
            lstRunHistory.Size = new Size(290, 90);
            lstRunHistory.TabIndex = 7;
            
            // 
            // lblCharacterDisplay - 角色显示
            // 
            lblCharacterDisplay.BorderStyle = BorderStyle.None;
            lblCharacterDisplay.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblCharacterDisplay.Location = new Point(15, 265);
            lblCharacterDisplay.Name = "lblCharacterDisplay";
            lblCharacterDisplay.Size = new Size(290, 25);
            lblCharacterDisplay.TextAlign = ContentAlignment.MiddleLeft;
            lblCharacterDisplay.TabIndex = 5;
            
            // 
            // lblSceneDisplay - 场景显示
            // 
            lblSceneDisplay.BorderStyle = BorderStyle.None;
            lblSceneDisplay.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblSceneDisplay.Location = new Point(15, 290);
            lblSceneDisplay.Name = "lblSceneDisplay";
            lblSceneDisplay.Size = new Size(290, 25);
            lblSceneDisplay.TextAlign = ContentAlignment.MiddleLeft;
            lblSceneDisplay.TabIndex = 6;
            
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
            Controls.Add(lstRunHistory);
            Controls.Add(lblCharacterDisplay);
            Controls.Add(lblSceneDisplay);
            Name = "TimerControl";
            Size = new Size(320, 320);
            ResumeLayout(false);
            PerformLayout();
        }
        
        // 使用LogManager进行日志记录
        
        /// <summary>
        /// 设置角色和场景信息
        /// </summary>
        /// <param name="character">角色名称</param>
        /// <param name="scene">场景名称</param>
        public void SetCharacterAndScene(string character, string scene)
        {
            currentCharacter = character;
            
            // 使用DataManager获取对应的英文场景名称
            string englishSceneName = DTwoMFTimerHelper.Data.DataManager.GetEnglishSceneName(scene);
            
            currentScene = englishSceneName;
            
            LogManager.WriteDebugLog("TimerControl", $"SetCharacterAndScene 调用: 角色={character}, 原始场景={scene}, 英文场景={englishSceneName}");
            
            // 尝试根据角色名称查找对应的角色档案
            if (!string.IsNullOrEmpty(character))
            {
                try
                {
                    LogManager.WriteDebugLog("TimerControl", $"尝试根据角色名称 '{character}' 查找对应的角色档案");
                    currentProfile = DTwoMFTimerHelper.Data.DataManager.FindProfileByName(character);
                    if (currentProfile != null)
                    {
                        LogManager.WriteDebugLog("TimerControl", $"已关联角色档案: {character}, 档案名称={currentProfile.Name}");
                        LogManager.WriteDebugLog("TimerControl", $"已关联角色档案: {character}");
                        
                        // 保存当前角色、场景和难度到设置
                        var settings = DTwoMFTimerHelper.Settings.SettingsManager.LoadSettings();
                        // 提取纯角色名称，去除可能包含的职业信息 (如 "AAA (刺客)" -> "AAA")
                        string pureCharacterName = character;
                        if (character.Contains(" ("))
                        {
                            int index = character.IndexOf(" (");
                            pureCharacterName = character.Substring(0, index);
                            LogManager.WriteDebugLog("TimerControl", $"已从角色名称中提取纯名称: 原名称='{character}', 提取后='{pureCharacterName}'");
                        }
                        settings.LastUsedProfile = pureCharacterName;
                        settings.LastUsedScene = scene;
                        settings.LastUsedDifficulty = GetCurrentDifficulty().ToString();
                        DTwoMFTimerHelper.Settings.SettingsManager.SaveSettings(settings);
                        LogManager.WriteDebugLog("TimerControl", $"已保存设置到配置文件: LastUsedProfile={pureCharacterName}, LastUsedScene={scene}, LastUsedDifficulty={settings.LastUsedDifficulty}");
                    }
                    else
                    {
                        LogManager.WriteDebugLog("TimerControl", $"未找到角色档案: {character}");
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteDebugLog("TimerControl", $"查找角色档案失败: {ex.Message}");
                    LogManager.WriteDebugLog("TimerControl", $"查找角色档案失败: {ex.Message}");
                }
            }
            
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
                
                // 优先使用inProgressRecord中的elapsedTime值（来自yaml文件）
                if (inProgressRecord != null && inProgressRecord.ElapsedTime.HasValue && inProgressRecord.ElapsedTime.Value > 0)
                {
                    LogManager.WriteDebugLog("TimerControl", $"使用inProgressRecord中的elapsedTime值: {inProgressRecord.ElapsedTime}秒");
                    elapsed = TimeSpan.FromSeconds(inProgressRecord.ElapsedTime.Value);
                    
                    // 如果不是暂停状态，需要加上从LatestTime到现在的时间
                    if (!isPaused && inProgressRecord.LatestTime.HasValue)
                    {
                        double timeSinceLatest = (DateTime.Now - inProgressRecord.LatestTime.Value).TotalSeconds;
                        elapsed = elapsed.Add(TimeSpan.FromSeconds(timeSinceLatest));
                    }
                }
                else if (isPaused && pauseStartTime != DateTime.MinValue)
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
                    LogManager.WriteDebugLog("TimerControl", "[计算平均时间] 开始计算");
                    TimeSpan averageTime = TimeSpan.Zero;
                    double totalSeconds = 0;
                    for (int i = 0; i < runHistory.Count; i++)
                    {
                        var time = runHistory[i];
                        averageTime += time;
                        totalSeconds += time.TotalSeconds;
                        LogManager.WriteDebugLog("TimerControl", $"[计算平均时间] 记录 #{i + 1}: {time}, 累计: {totalSeconds}秒");
                    }
                    averageTime = new TimeSpan(averageTime.Ticks / runHistory.Count);
                    LogManager.WriteDebugLog("TimerControl", $"[计算平均时间] 总时间: {totalSeconds}秒, 记录数: {runHistory.Count}, 平均时间: {averageTime}, 平均秒数: {totalSeconds / runHistory.Count}");
                    
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
                LogManager.WriteDebugLog("TimerControl", "[更新历史记录列表] 开始更新UI显示");
                lstRunHistory.Items.Clear();
                for (int i = 0; i < runHistory.Count; i++)
                {
                    var time = runHistory[i];
                    string timeFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}", 
                        time.Hours, time.Minutes, time.Seconds, 
                        (int)(time.Milliseconds / 100));
                    LogManager.WriteDebugLog("TimerControl", $"[更新历史记录列表] 记录 #{i + 1}: {timeFormatted}");
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
                if (string.IsNullOrEmpty(currentCharacter))
                {
                    lblCharacterDisplay.Text = "";
                }
                else
                {
                    // 获取角色职业信息
                    string characterClass = "";
                    if (currentProfile != null)
                    {
                        // 使用LogManager中的统一方法获取本地化职业名称
                        characterClass = Utils.LanguageManager.GetLocalizedClassName(currentProfile.Class);
                    }
                    
                    // 检查currentCharacter是否已经包含职业信息格式
                    if (!string.IsNullOrEmpty(characterClass))
                    {
                        // 如果currentCharacter已经包含括号格式，只显示纯角色名称加职业
                        string displayName = currentCharacter;
                        if (currentCharacter.Contains(" ("))
                        {
                            int index = currentCharacter.IndexOf(" (");
                            displayName = currentCharacter.Substring(0, index);
                        }
                        lblCharacterDisplay.Text = $"{displayName} ({characterClass})";
                    }
                    else
                    {
                        // 如果没有职业信息，只显示角色名称
                        lblCharacterDisplay.Text = currentCharacter;
                    }
                }
            }
            
            if (lblSceneDisplay != null)
            {
                if (string.IsNullOrEmpty(currentScene))
                {
                    lblSceneDisplay.Text = "";
                }
                else
                {
                    // 获取游戏难度
                    string difficultyText = GetCurrentDifficultyText();
                    
                    // 直接使用LanguageManager.GetString获取本地化的场景名称
                    // LanguageManager.cs中已增强了场景名称的自动翻译功能
                    string localizedSceneName = Utils.LanguageManager.GetString(currentScene);
                    
                    // 在场景名称前添加难度
                    lblSceneDisplay.Text = $"{difficultyText} {localizedSceneName}";
                }
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        /// <summary>
        /// 通过快捷键触发开始/停止计时
        /// 当停止时：保存记录到角色档案，并立即开始下一场计时
        /// </summary>
        public void ToggleTimer()
        {   
            LogManager.WriteDebugLog("TimerControl", $"ToggleTimer 调用（快捷键触发），当前状态: isTimerRunning={isTimerRunning}");
            if (!isTimerRunning)
            {   
                LogManager.WriteDebugLog("TimerControl", $"通过快捷键开始计时，当前角色档案: {(currentProfile != null ? currentProfile.Name : "null")}");
                StartTimer();
            }
            else
            {   
                // 停止当前计时并保存记录
                LogManager.WriteDebugLog("TimerControl", $"通过快捷键停止计时");
                StopTimer(true); // 传入true表示通过快捷键触发，需要保存并自动开始下一场
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
                
                // 更新未完成记录的LatestTime和ElapsedTime
                UpdateIncompleteRecord();
                
                // 保存暂停状态到设置
                SaveTimerState();
            }
        }
        
        private void ResumeTimer()
        {            
            if (isTimerRunning && isPaused && pauseStartTime != DateTime.MinValue)
            {
                // 在计算暂停时间之前，先更新记录，使用pauseStartTime作为更新时间点
                UpdateIncompleteRecord(pauseStartTime);
                
                pausedDuration += DateTime.Now - pauseStartTime;
                isPaused = false;
                pauseStartTime = DateTime.MinValue;
                UpdateUI();
                
                // 只更新latestTime，不影响elapsedTime计算
                if (currentProfile != null)
                {
                    var record = FindIncompleteRecordForCurrentScene();
                    if (record != null)
                    {
                        // 只更新LatestTime，保持ElapsedTime不变
                        record.LatestTime = DateTime.Now;
                        DTwoMFTimerHelper.Data.DataManager.UpdateMFRecord(currentProfile, record);
                        LogManager.WriteDebugLog("TimerControl", $"已更新未完成记录的LatestTime: 场景={currentScene}, 更新时间点={DateTime.Now}");
                    }
                }
                
                // 保存恢复状态到设置
                SaveTimerState();
            }
        }
        
        // 提供给外部调用的重置方法
        public void ResetTimerExternally()
        {
            ResetTimer();
        }

        /// <summary>
        /// 开始计时器
        /// </summary>
        public void StartTimer()
        {
            if (isTimerRunning)
                return;
            
            // 如果没有设置角色和场景，尝试从当前打开的ProfileManager获取
            if (string.IsNullOrEmpty(currentCharacter) || string.IsNullOrEmpty(currentScene))
            {
                LogManager.WriteDebugLog("TimerControl", "角色或场景为空，尝试从主窗口获取档案信息");
                TryGetProfileInfoFromMainForm();
            }
            
            LogManager.WriteDebugLog("TimerControl", $"[快捷键触发] 开始计时前的角色档案信息: currentCharacter={currentCharacter}, currentProfile={(currentProfile != null ? currentProfile.Name : "null")}");
            LogManager.WriteDebugLog("TimerControl", $"[快捷键触发] 当前场景: {currentScene}, 当前时间: {DateTime.Now}");
            
            isTimerRunning = true;
            isPaused = false;
            startTime = DateTime.Now;
            pausedDuration = TimeSpan.Zero;
            pauseStartTime = DateTime.MinValue;
            timer?.Start();
            
            // 在开始计时时创建一条记录
            CreateStartRecord();
            
            // 保存当前角色、场景和难度到设置
            if (!string.IsNullOrEmpty(currentCharacter) && !string.IsNullOrEmpty(currentScene))
            {
                try
                {
                    var settings = SettingsManager.LoadSettings();
                    // 提取纯角色名称，去除可能包含的职业信息 (如 "AAA (刺客)" -> "AAA")
                    string pureCharacterName = currentCharacter;
                    if (currentCharacter.Contains(" ("))
                    {
                        int index = currentCharacter.IndexOf(" (");
                        pureCharacterName = currentCharacter.Substring(0, index);
                        LogManager.WriteDebugLog("TimerControl", $"已从角色名称中提取纯名称: 原名称='{currentCharacter}', 提取后='{pureCharacterName}'");
                    }
                    settings.LastUsedProfile = pureCharacterName;
                    settings.LastUsedScene = currentScene;
                    settings.LastUsedDifficulty = GetCurrentDifficulty().ToString();
                    
                    // 保存计时状态
                    SaveTimerStateToSettings(settings);
                    
                    SettingsManager.SaveSettings(settings);
                    Console.WriteLine($"已保存设置: 角色={pureCharacterName}, 场景={currentScene}, 难度={settings.LastUsedDifficulty}");
                    LogManager.WriteDebugLog("TimerControl", $"已保存设置到配置文件: LastUsedProfile={pureCharacterName}, LastUsedScene={currentScene}, LastUsedDifficulty={settings.LastUsedDifficulty}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"保存设置失败: {ex.Message}");
                }
            }
            
            UpdateUI();
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// 尝试从主窗口获取角色和场景信息
        /// </summary>
        private void TryGetProfileInfoFromMainForm()
        {   
            LogManager.WriteDebugLog("TimerControl", "TryGetProfileInfoFromMainForm 开始执行");
            try
            {   
                // 获取主窗口
                var mainForm = this.FindForm() as MainForm;
                LogManager.WriteDebugLog("TimerControl", $"获取到主窗口: {(mainForm != null ? "是" : "否")}");
                
                if (mainForm != null && mainForm.ProfileManager != null)
                {   
                    var profileManager = mainForm.ProfileManager;
                    LogManager.WriteDebugLog("TimerControl", $"获取到ProfileManager: 是, CurrentProfile是否为null: {(profileManager.CurrentProfile == null ? "是" : "否")}");
                    
                    if (profileManager.CurrentProfile != null)
                    {   
                        currentProfile = profileManager.CurrentProfile;
                        currentCharacter = profileManager.CurrentProfile.Name;
                        
                        // 尝试获取当前选择的场景
                        if (profileManager is ProfileManager pm)
                        {
                            // 假设ProfileManager有获取当前场景的方法或属性
                            // 这里简化处理，尝试获取场景下拉框的值
                            try
                            {
                                // 使用反射获取场景选择
                                LogManager.WriteDebugLog("TimerControl", $"尝试通过反射获取场景选择");
                                var sceneComboBox = pm.GetType().GetProperty("CurrentScene")?.GetValue(pm) as string;
                                if (!string.IsNullOrEmpty(sceneComboBox))
                                {
                                    currentScene = sceneComboBox;
                                    LogManager.WriteDebugLog("TimerControl", $"已从ProfileManager获取场景信息: {currentScene}");
                                    Console.WriteLine($"已从ProfileManager获取场景信息: {currentScene}");
                                }
                                else
                                {
                                    LogManager.WriteDebugLog("TimerControl", $"通过反射获取的场景信息为空");
                                }
                            }
                            catch (Exception ex)
                            {   
                                LogManager.WriteDebugLog("TimerControl", $"获取场景信息失败，尝试其他方式: {ex.Message}");
                                Console.WriteLine($"获取场景信息失败，尝试其他方式");
                                // 如果反射失败，尝试直接从控件获取（如果可以访问）
                            }
                        }
                        
                        LogManager.WriteDebugLog("TimerControl", $"已从ProfileManager获取角色信息: currentCharacter={currentCharacter}, currentProfile.Name={currentProfile.Name}");
                        Console.WriteLine($"已从ProfileManager获取角色信息: {currentCharacter}");
                    }
                    else
                    {
                        LogManager.WriteDebugLog("TimerControl", "ProfileManager.CurrentProfile 为 null，无法获取角色信息");
                    }
                }
                else
                {
                    LogManager.WriteDebugLog("TimerControl", $"mainForm 或 ProfileManager 为 null: mainForm={(mainForm != null ? "非null" : "null")}, ProfileManager={(mainForm != null && mainForm.ProfileManager != null ? "非null" : "null")}");
                }
            }
            catch (Exception ex)
            {   
                LogManager.WriteDebugLog("TimerControl", $"获取角色信息失败: {ex.Message}, 堆栈: {ex.StackTrace}");
                Console.WriteLine($"获取角色信息失败: {ex.Message}");
                Console.WriteLine($"异常堆栈: {ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 同步更新角色和场景信息
        /// 当ProfileManager中的角色或场景改变时调用此方法
        /// </summary>
        public void SyncWithProfileManager()
        {   
            TryGetProfileInfoFromMainForm();
            LoadProfileHistoryData();
            UpdateUI();
            BindLatestIncompleteRecord(); // 添加这一行，加载未完成记录
            Console.WriteLine($"已同步角色和场景信息: {currentCharacter} - {currentScene}");
        }
        
        /// <summary>
        /// 当切换到计时器Tab时调用此方法
        /// 自动加载角色档案中对应的场景数据并显示
        /// </summary>
        public void OnTabSelected()
        {   
            SyncWithProfileManager();
            Console.WriteLine("计时器Tab被选中，已自动加载角色档案数据");
        }
        
        /// <summary>
        /// 从角色档案加载特定场景的历史数据
        /// </summary>
        private void LoadProfileHistoryData()
        {   
            // 重置当前的统计数据
            ResetTimerDisplay();
            runHistory.Clear();
            runCount = 0;
            fastestTime = TimeSpan.MaxValue;
            
            // 如果有当前角色档案和场景，加载历史数据
            if (currentProfile != null && !string.IsNullOrEmpty(currentScene))
            {   
                try
                {   
                    // 获取当前难度
                    var currentDifficulty = GetCurrentDifficulty();
                    
                    // 添加详细调试日志
                    LogManager.WriteDebugLog("TimerControl", $"调试 - 当前场景名称: '{currentScene}'");
                    LogManager.WriteDebugLog("TimerControl", $"调试 - 档案总记录数: {currentProfile.Records.Count}");
                    
                    // 输出档案中所有记录的场景名称用于调试
                    for (int i = 0; i < currentProfile.Records.Count; i++)
                    {
                        var record = currentProfile.Records[i];
                        LogManager.WriteDebugLog("TimerControl", $"调试 - 档案记录 {i}: {record.SceneName}, 难度: {record.Difficulty}, 完成: {record.IsCompleted}");
                    }
                    
                    // 从角色档案中过滤出同场景记录
                    // 使用LanguageManager获取纯英文场景名称进行匹配
                    string pureEnglishSceneName = DTwoMFTimerHelper.Utils.LanguageManager.GetPureEnglishSceneName(currentScene);
                    
                    // 日志记录当前匹配的场景名称
                    LogManager.WriteDebugLog("TimerControl", $"匹配场景名称: '{pureEnglishSceneName}'");
                    
                    // 修改过滤条件，处理记录场景名称可能为空的情况，或使用配置中的sceneName
                    var sceneRecords = currentProfile.Records.Where(r => 
                        (string.IsNullOrEmpty(r.SceneName) || 
                         r.SceneName.Equals(pureEnglishSceneName, StringComparison.OrdinalIgnoreCase) ||
                         r.SceneName.Trim('"', '\'').Equals(pureEnglishSceneName, StringComparison.OrdinalIgnoreCase)) && 
                        r.IsCompleted).ToList();
                    
                    LogManager.WriteDebugLog("TimerControl", $"从角色档案中加载了 {sceneRecords.Count} 条记录: {currentCharacter} - {pureEnglishSceneName}, 难度: {currentDifficulty}");
                    Console.WriteLine($"从角色档案中加载了 {sceneRecords.Count} 条记录: {currentCharacter} - {pureEnglishSceneName}, 难度: {currentDifficulty}");
                    
                    // 如果有记录，更新统计数据
                    if (sceneRecords.Count > 0)
                    {   
                        runCount = sceneRecords.Count;
                        
                        // 转换为TimeSpan并添加到历史记录
                        foreach (var record in sceneRecords)
                        {   
                            // 添加详细的初始值日志，记录record对象在赋值前的状态
                            LogManager.WriteDebugLog("TimerControl", $"[加载记录前状态] ID: {record.GetHashCode()}, StartTime: {record.StartTime}, EndTime: {(record.EndTime.HasValue ? record.EndTime.Value.ToString() : "null")}, LatestTime: {(record.LatestTime.HasValue ? record.LatestTime.Value.ToString() : "null")}, ElapsedTime: {(record.ElapsedTime.HasValue ? record.ElapsedTime.Value.ToString() : "null")}, SceneName: {record.SceneName}");
                            
                            // 添加详细的调试日志
                            Console.WriteLine($"[调试] 加载记录 - StartTime: {record.StartTime}, EndTime: {record.EndTime}, LatestTime: {record.LatestTime}, ElapsedTime: {record.ElapsedTime}");
                            
                            // 手动计算正确的持续时间
                            double correctDuration;
                            int recordIndex = sceneRecords.IndexOf(record);
                            LogManager.WriteDebugLog("TimerControl", $"[加载记录 #{recordIndex + 1}] 开始计算持续时间");
                            LogManager.WriteDebugLog("TimerControl", $"[加载记录 #{recordIndex + 1}] StartTime: {record.StartTime}, EndTime: {record.EndTime}, LatestTime: {record.LatestTime}, ElapsedTime: {record.ElapsedTime}");
                            
                            if (record.ElapsedTime.HasValue && record.ElapsedTime.Value > 0 && record.LatestTime.HasValue && record.EndTime.HasValue)
                            {
                                double latestToEnd = (record.EndTime.Value - record.LatestTime.Value).TotalSeconds;
                                correctDuration = record.ElapsedTime.Value + latestToEnd;
                                LogManager.WriteDebugLog("TimerControl", $"[加载记录 #{recordIndex + 1}] 计算路径: ElapsedTime + (EndTime - LatestTime) = {record.ElapsedTime.Value} + {latestToEnd} = {correctDuration}秒");
                            }
                            else if (record.EndTime.HasValue)
                            {
                                correctDuration = (record.EndTime.Value - record.StartTime).TotalSeconds;
                                LogManager.WriteDebugLog("TimerControl", $"[加载记录 #{recordIndex + 1}] 计算路径: EndTime - StartTime = {correctDuration}秒");
                            }
                            else
                            {
                                correctDuration = 0;
                                LogManager.WriteDebugLog("TimerControl", $"[加载记录 #{recordIndex + 1}] 计算路径: 持续时间为0");
                            }
                            
                            TimeSpan duration = TimeSpan.FromSeconds(correctDuration);
                            runHistory.Add(duration);
                            LogManager.WriteDebugLog("TimerControl", $"[加载记录 #{recordIndex + 1}] 成功添加到runHistory: {duration}");
                            
                            // 更新最快时间
                            if (duration < fastestTime)
                            {   
                                fastestTime = duration;
                                LogManager.WriteDebugLog("TimerControl", $"[加载记录 #{recordIndex + 1}] 更新最快时间为: {fastestTime}");
                            }
                        }
                        
                        // 按时间从旧到新排序，让时间靠前的记录排在前面
                        runHistory.Sort((a, b) => a.CompareTo(b));
                        
                        LogManager.WriteDebugLog("TimerControl", "[加载完成] runHistory排序后内容:");
                        for (int i = 0; i < runHistory.Count; i++)
                        {
                            LogManager.WriteDebugLog("TimerControl", $"[加载完成] 记录 #{i + 1}: {runHistory[i]}");
                        }
                        LogManager.WriteDebugLog("TimerControl", $"[加载完成] 运行次数: {runCount}, 最快时间: {fastestTime}");
                        Console.WriteLine($"加载完成，运行次数: {runCount}, 最快时间: {fastestTime}");
                    }
                }
                catch (Exception ex)
                {   
                    Console.WriteLine($"加载历史数据失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 将当前计时记录保存到角色档案
        /// </summary>
        private void SaveToProfile()
        {   
            // 如果没有设置当前角色档案，尝试从主窗口获取
            if (currentProfile == null)
            {
                TryGetProfileInfoFromMainForm();
                // 如果仍然获取不到，记录日志并返回
                if (currentProfile == null)
                {
                    Console.WriteLine("无法保存记录：未找到当前角色档案");
                    return;
                }
            }
            
            if (string.IsNullOrEmpty(currentCharacter) || string.IsNullOrEmpty(currentScene) || startTime == DateTime.MinValue)
                return;

            try
            {
                // 从场景名称中提取ACT值
                int actValue = ExtractActFromSceneName(currentScene);
                
                // 获取难度信息
                var difficulty = GetCurrentDifficulty();
                
                // 根据当前语言设置正确的场景中英文名称
                string sceneEnName = currentScene; // 默认值
                string sceneZhName = currentScene; // 默认值
                
                // 从设置中获取当前语言
                bool isChineseScene = SettingsManager.StringToLanguage(SettingsManager.LoadSettings().Language) == DTwoMFTimerHelper.SettingsControl.LanguageOption.Chinese;
                
                // 如果是中文场景，需要区分中英文
                if (isChineseScene || currentScene.StartsWith("ACT ") || currentScene.StartsWith("Act ") || currentScene.StartsWith("act "))
                {
                    // 当前显示的是中文，所以SceneZhName就是currentScene
                    sceneZhName = currentScene;
                    
                    // 尝试获取对应的英文名称
                    sceneEnName = DTwoMFTimerHelper.Data.DataManager.GetEnglishSceneName(currentScene);
                    LogManager.WriteDebugLog("TimerControl", $"场景中英文映射: 中文='{sceneZhName}', 英文='{sceneEnName}'");
                }
                else
                {
                    // 当前显示的是英文，所以SceneEnName就是currentScene
                    sceneEnName = currentScene;
                    sceneZhName = currentScene; // 如果无法获取中文，保持一致
                }
                
                // 计算实际持续时间
                TimeSpan actualDuration = DateTime.Now - startTime - pausedDuration;
                double durationSeconds = actualDuration.TotalSeconds;
                
                // 确保场景名称不为空
                if (string.IsNullOrEmpty(sceneEnName))
                {
                    sceneEnName = "UnknownScene"; // 设置默认值
                    LogManager.WriteDebugLog("TimerControl", $"警告: SaveToProfile中sceneEnName为空，使用默认值 '{sceneEnName}'");
                }
                
                // 创建新的MF记录，确保设置正确的LatestTime和ElapsedTime
                var newRecord = new DTwoMFTimerHelper.Data.MFRecord
                {
                    SceneName = sceneEnName, // 使用英文名称作为SceneName
                    ACT = actValue,
                    Difficulty = difficulty,
                    StartTime = startTime,
                    EndTime = DateTime.Now,
                    LatestTime = DateTime.Now, // 设置LatestTime为结束时间
                    ElapsedTime = durationSeconds // 设置ElapsedTime为实际计算的持续时间
                    // IsCompleted是只读属性，通过设置EndTime来自动计算
                };

                // 获取英文场景名称并移除ACT前缀，与CreateStartRecord方法保持一致的格式
                string sceneEnNameForSearch = DTwoMFTimerHelper.Data.DataManager.GetEnglishSceneName(currentScene);
                string pureEnglishSceneNameForSearch = DTwoMFTimerHelper.Utils.LanguageManager.GetPureEnglishSceneName(currentScene);
                
                LogManager.WriteDebugLog("TimerControl", $"查找未完成记录: 原始场景名='{currentScene}', 纯英文场景名='{pureEnglishSceneNameForSearch}'");
                
                // 查找同场景同难度的未完成记录（使用与CreateStartRecord一致的纯英文场景名称格式）
                var existingRecord = currentProfile.Records.FirstOrDefault(r => 
                    r.SceneName == pureEnglishSceneNameForSearch && 
                    r.Difficulty == difficulty && 
                    !r.IsCompleted);
                
                if (existingRecord != null)
                {
                    // 计算实际持续时间（使用不同的变量名避免作用域冲突）
                    TimeSpan existingRecordDuration = DateTime.Now - existingRecord.StartTime - pausedDuration;
                    double existingRecordSeconds = existingRecordDuration.TotalSeconds;
                    
                    // 更新现有记录，确保设置正确的值
                    existingRecord.EndTime = DateTime.Now;
                    existingRecord.LatestTime = DateTime.Now; // 设置LatestTime为结束时间
                    existingRecord.ElapsedTime = existingRecordSeconds; // 设置ElapsedTime为实际计算的持续时间
                    // IsCompleted是只读属性，通过设置EndTime来自动计算
                    existingRecord.ACT = actValue;
                    existingRecord.Difficulty = difficulty;
                    
                    // 更新现有记录
                    DTwoMFTimerHelper.Data.DataManager.UpdateMFRecord(currentProfile, existingRecord);
                    LogManager.WriteDebugLog("TimerControl", $"[更新现有记录] {currentCharacter} - {currentScene}, ACT: {actValue}, 难度: {difficulty}, 开始时间: {existingRecord.StartTime}, 结束时间: {DateTime.Now}, ElapsedTime: {existingRecord.ElapsedTime}, 计算时间: {existingRecord.DurationSeconds}秒");
                }
                else
                {
                    // 添加新记录
                    DTwoMFTimerHelper.Data.DataManager.AddMFRecord(currentProfile, newRecord);
                    LogManager.WriteDebugLog("TimerControl", $"[添加新记录] {currentCharacter} - {currentScene}, ACT: {actValue}, 难度: {difficulty}, 开始时间: {startTime}, 结束时间: {DateTime.Now}, ElapsedTime: {newRecord.ElapsedTime}, 计算时间: {newRecord.DurationSeconds}秒");
                }

                // 记录日志
                Console.WriteLine($"已保存计时记录到角色档案: {currentCharacter} - {currentScene}, ACT: {actValue}, ElapsedTime: {newRecord.ElapsedTime}, 计算时间: {newRecord.DurationSeconds}秒");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存计时记录失败: {ex.Message}");
                Console.WriteLine($"异常堆栈: {ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 从场景名称中提取ACT值
        /// </summary>
        private int ExtractActFromSceneName(string sceneName)
        {
            try
            {
                // 首先尝试从"ACT X: 场景名"格式中提取ACT值
                if (sceneName.StartsWith("ACT ") || sceneName.StartsWith("Act ") || sceneName.StartsWith("act "))
                {
                    // 提取ACT后面的数字部分（处理"ACT X:"格式）
                    int colonIndex = sceneName.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        string actPart = sceneName.Substring(0, colonIndex).Trim();
                        if (int.TryParse(actPart.Split(' ')[1], out int act))
                        {
                            return act;
                        }
                    }
                }
                
                // 移除ACT前缀（如果有），提取纯场景名称
                string pureSceneName = sceneName;
                if (sceneName.StartsWith("ACT ") || sceneName.StartsWith("Act ") || sceneName.StartsWith("act "))
                {
                    int colonIndex = sceneName.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        pureSceneName = sceneName.Substring(colonIndex + 1).Trim();
                    }
                }
                
                // 使用DataManager获取场景到ACT值的映射
                var sceneActMappings = DTwoMFTimerHelper.Data.DataManager.GetSceneActMappings();
                
                // 尝试在映射中查找纯场景名称对应的ACT值
                if (sceneActMappings.TryGetValue(pureSceneName, out int actValue))
                {
                    return actValue;
                }
                
                // 尝试查找包含场景名称的键
                foreach (var mapping in sceneActMappings)
                {
                    if (pureSceneName.Contains(mapping.Key, StringComparison.OrdinalIgnoreCase) || 
                        mapping.Key.Contains(pureSceneName, StringComparison.OrdinalIgnoreCase))
                    {
                        return mapping.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"提取ACT值失败: {ex.Message}");
            }
            return 0; // 默认返回0
        }
        
        /// <summary>
        /// 获取当前游戏难度
        /// </summary>
        private DTwoMFTimerHelper.Data.GameDifficulty GetCurrentDifficulty()
        {
            try
            {
                // 尝试从主窗口的ProfileManager获取难度信息
                var mainForm = this.FindForm() as MainForm;
                if (mainForm != null && mainForm.ProfileManager != null)
                {
                    // 这里简化处理，实际应该调用ProfileManager的相关属性
                    // 暂时默认返回地狱难度
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取难度信息失败: {ex.Message}");
            }
            return DTwoMFTimerHelper.Data.GameDifficulty.Hell; // 默认地狱难度
        }
        
        // GetSceneEnglishName方法已被移除，现在使用DTwoMFTimerHelper.Data.DataManager.GetEnglishSceneName和DTwoMFTimerHelper.Resources.LanguageManager.GetPureEnglishSceneName
        
        /// <summary>
        /// 获取当前游戏难度的中文显示文本
        /// </summary>
        /// <returns>难度的中文文本</returns>
        private string GetCurrentDifficultyText()
        {
            DTwoMFTimerHelper.Data.GameDifficulty difficulty = GetCurrentDifficulty();
            
            switch (difficulty)
            {
                case DTwoMFTimerHelper.Data.GameDifficulty.Normal:
                    return DTwoMFTimerHelper.Utils.LanguageManager.GetString("DifficultyNormal");
                case DTwoMFTimerHelper.Data.GameDifficulty.Nightmare:
                    return DTwoMFTimerHelper.Utils.LanguageManager.GetString("DifficultyNightmare");
                case DTwoMFTimerHelper.Data.GameDifficulty.Hell:
                    return DTwoMFTimerHelper.Utils.LanguageManager.GetString("DifficultyHell");
                default:
                    return DTwoMFTimerHelper.Utils.LanguageManager.GetString("DifficultyUnknown");
            }
        }

        private void StopTimer(bool autoStartNext = false)
        {            
            isTimerRunning = false;
            isPaused = false;
            timer?.Stop();
            
            // 清除计时状态设置
            ClearTimerState();
            
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
                
                // 保存记录到角色档案
                // 首先尝试获取当前角色档案（如果为null）
                if (currentProfile == null)
                {
                    LogManager.WriteDebugLog("TimerControl", "StopTimer: currentProfile为null，尝试获取角色档案");
                    TryGetProfileInfoFromMainForm();
                }
                
                // 调用SaveToProfile方法保存记录（SaveToProfile内部也会检查并尝试获取角色档案）
                SaveToProfile();
            }
            
            UpdateUI();
            TimerStateChanged?.Invoke(this, EventArgs.Empty);

            // 如果是通过快捷键触发，自动开始下一场计时
            if (autoStartNext)
            {
                // 短暂延迟后自动开始下一场
                System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke((Action)StartTimer);
                    }
                    else
                    {
                        StartTimer();
                    }
                });
            }
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
            
            // 清除计时状态设置
            ClearTimerState();
            
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
        
        /// <summary>
        /// 在开始计时时创建一条记录
        /// 即使没有完整的角色和场景信息，也会创建一条基本记录
        /// </summary>
        private void CreateStartRecord()
        {
            // 如果没有角色档案，尝试从主窗口获取
            if (currentProfile == null)
            {
                LogManager.WriteDebugLog("TimerControl", "CreateStartRecord: currentProfile为null，尝试获取角色档案");
                TryGetProfileInfoFromMainForm();
                
                // 如果仍然没有档案，创建一个临时的基本记录
                if (currentProfile == null)
                {
                    LogManager.WriteDebugLog("TimerControl", "仍然没有角色档案，创建临时记录");
                }
            }

            // 确保currentCharacter和currentScene有默认值
            if (string.IsNullOrEmpty(currentCharacter))
                currentCharacter = "Unknown Character";
            if (string.IsNullOrEmpty(currentScene))
                currentScene = "Unknown Scene";

            try
            {
                // 从场景名称中提取ACT值
                int actValue = ExtractActFromSceneName(currentScene);
                
                // 获取难度信息
                var difficulty = GetCurrentDifficulty();
                
                // 获取英文场景名称和纯英文场景名称（移除ACT前缀）
                string sceneEnName = DTwoMFTimerHelper.Data.DataManager.GetEnglishSceneName(currentScene);
                string pureEnglishSceneName = DTwoMFTimerHelper.Utils.LanguageManager.GetPureEnglishSceneName(currentScene);
                
                LogManager.WriteDebugLog("TimerControl", $"保存场景名称: 原始='{currentScene}', 英文='{sceneEnName}', 纯英文='{pureEnglishSceneName}'");
                
                // 确保场景名称不为空
                if (string.IsNullOrEmpty(pureEnglishSceneName))
                {
                    pureEnglishSceneName = "UnknownScene"; // 设置默认值
                    LogManager.WriteDebugLog("TimerControl", $"警告: CreateStartRecord中pureEnglishSceneName为空，使用默认值 '{pureEnglishSceneName}'");
                }
                
                // 创建新的MF记录（未完成）
                var newRecord = new DTwoMFTimerHelper.Data.MFRecord
                {
                    SceneName = pureEnglishSceneName, // 使用不带ACT前缀的英文名称作为SceneName
                    ACT = actValue,
                    Difficulty = difficulty,
                    StartTime = startTime,
                    EndTime = null, // 未完成，结束时间设为null
                    LatestTime = startTime, // 初始化LatestTime为开始时间，而不是null
                    ElapsedTime = 0.0 // 开始时已用时间为0
                };

                // 只有当有角色档案时才添加记录
                if (currentProfile != null)
                {
                    DTwoMFTimerHelper.Data.DataManager.AddMFRecord(currentProfile, newRecord);
                    
                    // 记录日志
                    Console.WriteLine($"已创建开始记录到角色档案: {currentCharacter} - {currentScene}, ACT: {actValue}");
                    LogManager.WriteDebugLog("TimerControl", $"已创建开始记录到角色档案: {currentCharacter} - {currentScene}, ACT: {actValue}, 开始时间: {startTime}");
                }
                else
                {
                    // 记录日志但不添加到档案
                    LogManager.WriteDebugLog("TimerControl", $"已创建临时记录但未保存到档案: {currentCharacter} - {currentScene}, ACT: {actValue}, 开始时间: {startTime}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建开始记录失败: {ex.Message}");
                LogManager.WriteDebugLog("TimerControl", $"创建开始记录失败: {ex.Message}, 堆栈: {ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 查找用户同场景同难度的最近一条未完成记录
        /// </summary>
        private DTwoMFTimerHelper.Data.MFRecord? FindIncompleteRecordForCurrentScene()
        {
            if (currentProfile == null || string.IsNullOrEmpty(currentScene))
                return null;
                
            var difficulty = GetCurrentDifficulty();
            
            // 使用与创建记录时相同的场景名称处理逻辑
            string pureEnglishSceneName = DTwoMFTimerHelper.Utils.LanguageManager.GetPureEnglishSceneName(currentScene);
            
            // 查找同场景、同难度、未完成的最近一条记录
            return currentProfile.Records
                .Where(r => r.SceneName == pureEnglishSceneName && r.Difficulty == difficulty && !r.IsCompleted)
                .OrderByDescending(r => r.StartTime)
                .FirstOrDefault();
        }
        
        /// <summary>
        /// 更新未完成记录的LatestTime和ElapsedTime
        /// </summary>
        /// <param name="updateTime">用于更新的时间点，默认为当前时间</param>
        private void UpdateIncompleteRecord(DateTime? updateTime = null)
        {
            if (!isTimerRunning || currentProfile == null)
                return;
                
            var record = FindIncompleteRecordForCurrentScene();
            if (record == null)
                return;
                
            try
            {
                // 保存更新前的累计时间和上次更新时间
                double previousElapsedTime = record.ElapsedTime ?? 0;
                DateTime? previousLatestTime = record.LatestTime;
                
                // 使用提供的时间点或当前时间
                DateTime now = updateTime ?? DateTime.Now;
                
                // 计算新的ElapsedTime，确保只增不减
                double newElapsedTime;
                
                // 确保LatestTime始终有值
                if (!record.LatestTime.HasValue)
                {
                    // 如果LatestTime为空，初始化它为StartTime
                    record.LatestTime = record.StartTime;
                    previousLatestTime = record.StartTime;
                    LogManager.WriteDebugLog("TimerControl", $"[更新记录] 初始化LatestTime为StartTime: {record.StartTime}");
                }
                
                // 如果之前有LatestTime，计算新的ElapsedTime
                if (previousLatestTime.HasValue)
                {
                    // 确保updateTime不早于previousLatestTime
                    DateTime effectiveUpdateTime = now > previousLatestTime.Value ? now : previousLatestTime.Value;
                    // ElapsedTime = (更新时间 - 上次LatestTime) + 已有的ElapsedTime
                    double additionalSeconds = (effectiveUpdateTime - previousLatestTime.Value).TotalSeconds;
                    newElapsedTime = previousElapsedTime + additionalSeconds;
                    LogManager.WriteDebugLog("TimerControl", $"[更新记录] 基于LatestTime计算: 上次时间={previousLatestTime.Value}, 当前时间={effectiveUpdateTime}, 增加时间={additionalSeconds}, 总计={newElapsedTime}");
                }
                else
                {
                    // 第一次更新，从StartTime开始计算
                    newElapsedTime = (now - record.StartTime).TotalSeconds;
                    LogManager.WriteDebugLog("TimerControl", $"[更新记录] 基于StartTime计算: 开始时间={record.StartTime}, 当前时间={now}, 总计={newElapsedTime}");
                }
                
                // 确保累计时间不会减少并且始终大于0
                if (newElapsedTime > previousElapsedTime || previousElapsedTime == 0)
                {
                    record.ElapsedTime = newElapsedTime;
                    LogManager.WriteDebugLog("TimerControl", $"[更新记录] ElapsedTime已更新: {previousElapsedTime} -> {newElapsedTime}");
                }
                else
                {
                    // 如果计算出的时间小于等于之前的时间，保持原值
                    newElapsedTime = previousElapsedTime;
                    LogManager.WriteDebugLog("TimerControl", $"[更新记录] 保持原有ElapsedTime: {previousElapsedTime}");
                }
                
                // 更新LatestTime
                record.LatestTime = now;
                LogManager.WriteDebugLog("TimerControl", $"[更新记录] LatestTime已更新: {(previousLatestTime.HasValue ? previousLatestTime.Value.ToString() : "null")} -> {now}");
                
                // 更新记录
                DTwoMFTimerHelper.Data.DataManager.UpdateMFRecord(currentProfile, record);
                
                // 记录更详细的日志信息，包含上次累计时间和上次更新时间
                LogManager.WriteDebugLog("TimerControl", $"已更新未完成记录: 场景={currentScene}, 上次累计时间={previousElapsedTime}秒, 当前累计时间={newElapsedTime}秒, 上次更新时间={previousLatestTime}, 更新时间点={now}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新未完成记录失败: {ex.Message}");
                LogManager.WriteDebugLog("TimerControl", $"更新未完成记录失败: {ex.Message}, 堆栈: {ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 保存计时状态到设置
        /// </summary>
        private void SaveTimerState()
        {            
            try
            {
                var settings = SettingsManager.LoadSettings();
                SaveTimerStateToSettings(settings);
                SettingsManager.SaveSettings(settings);
                LogManager.WriteDebugLog("TimerControl", $"已保存计时状态: isTimerRunning={isTimerRunning}, isPaused={isPaused}, character={currentCharacter}, scene={currentScene}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存计时状态失败: {ex.Message}");
                LogManager.WriteDebugLog("TimerControl", $"保存计时状态失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 将计时状态保存到设置对象
        /// </summary>
        private void SaveTimerStateToSettings(AppSettings settings)
        {            
            settings.IsTimerInProgress = isTimerRunning;
            settings.TimerStartTime = startTime;
            settings.TimerPausedDuration = pausedDuration.TotalMilliseconds;
            settings.IsTimerPaused = isPaused;
            settings.TimerPauseStartTime = pauseStartTime;
            settings.InProgressCharacter = currentCharacter;
            settings.InProgressScene = currentScene;
        }
        
        /// <summary>
        /// 清除计时状态设置
        /// </summary>
        private void ClearTimerState()
        {            
            try
            {
                var settings = SettingsManager.LoadSettings();
                settings.IsTimerInProgress = false;
                settings.TimerStartTime = DateTime.MinValue;
                settings.TimerPausedDuration = 0;
                settings.IsTimerPaused = false;
                settings.TimerPauseStartTime = DateTime.MinValue;
                settings.InProgressCharacter = "";
                settings.InProgressScene = "";
                SettingsManager.SaveSettings(settings);
                LogManager.WriteDebugLog("TimerControl", "已清除计时状态");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"清除计时状态失败: {ex.Message}");
                LogManager.WriteDebugLog("TimerControl", $"清除计时状态失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 尝试加载未完成的计时状态
        /// </summary>
        public void TryLoadPendingTimerState()
        {            
            try
            {
                var settings = SettingsManager.LoadSettings();
                
                // 如果有未完成的计时，且当前选择的角色和场景匹配
                if (settings.IsTimerInProgress && 
                    !string.IsNullOrEmpty(settings.InProgressCharacter) && 
                    !string.IsNullOrEmpty(settings.InProgressScene) &&
                    settings.TimerStartTime != DateTime.MinValue)
                {
                    LogManager.WriteDebugLog("TimerControl", $"发现未完成的计时状态: character={settings.InProgressCharacter}, scene={settings.InProgressScene}");
                    
                    // 检查当前选择的角色和场景是否匹配
                    if (string.IsNullOrEmpty(currentCharacter) || string.IsNullOrEmpty(currentScene))
                    {
                        // 如果当前没有选择角色和场景，尝试使用保存的
                        TryGetProfileInfoFromMainForm();
                    }
                    
                    // 提取纯角色名称用于比较
                    string savedPureCharacterName = settings.InProgressCharacter;
                    if (savedPureCharacterName.Contains(" ("))
                    {
                        int index = savedPureCharacterName.IndexOf(" (");
                        savedPureCharacterName = savedPureCharacterName.Substring(0, index);
                    }
                    
                    string currentPureCharacterName = currentCharacter;
                    if (!string.IsNullOrEmpty(currentPureCharacterName) && currentPureCharacterName.Contains(" ("))
                    {
                        int index = currentPureCharacterName.IndexOf(" (");
                        currentPureCharacterName = currentPureCharacterName.Substring(0, index);
                    }
                    
                    // 如果角色和场景匹配，恢复计时
                    if (currentPureCharacterName.Equals(savedPureCharacterName, StringComparison.OrdinalIgnoreCase) && 
                        currentScene.Equals(settings.InProgressScene, StringComparison.OrdinalIgnoreCase))
                    {
                        LogManager.WriteDebugLog("TimerControl", "角色和场景匹配，恢复计时状态");
                        
                        // 恢复计时状态
                        isTimerRunning = true;
                        startTime = settings.TimerStartTime;
                        pausedDuration = TimeSpan.FromMilliseconds(settings.TimerPausedDuration);
                        isPaused = settings.IsTimerPaused;
                        pauseStartTime = settings.IsTimerPaused ? settings.TimerPauseStartTime : DateTime.MinValue;
                        
                        // 查找同场景同难度的未完成记录
                        var incompleteRecord = FindIncompleteRecordForCurrentScene();
                        if (incompleteRecord != null && incompleteRecord.ElapsedTime.HasValue && incompleteRecord.ElapsedTime.Value > 0)
                        {
                            LogManager.WriteDebugLog("TimerControl", $"发现未完成记录，elapsedTime={incompleteRecord.ElapsedTime}秒，调整计时器显示");
                            
                            // 根据elapsedTime调整startTime，以便正确计算当前时间
                            double elapsedSeconds = incompleteRecord.ElapsedTime.Value;
                            
                            // 如果记录有LatestTime，从LatestTime开始计算当前应有的开始时间
                            if (incompleteRecord.LatestTime.HasValue)
                            {
                                TimeSpan timeSinceLatest = DateTime.Now - incompleteRecord.LatestTime.Value;
                                startTime = DateTime.Now - TimeSpan.FromSeconds(elapsedSeconds) - timeSinceLatest;
                            }
                            else
                            {
                                // 如果没有LatestTime，直接根据elapsedSeconds调整startTime
                                startTime = DateTime.Now - TimeSpan.FromSeconds(elapsedSeconds);
                            }
                        }
                        
                        // 如果不是暂停状态，启动计时器
                        if (!isPaused)
                        {
                            timer?.Start();
                        }
                        
                        UpdateUI();
                        TimerStateChanged?.Invoke(this, EventArgs.Empty);
                        
                        LogManager.WriteDebugLog("TimerControl", "计时状态恢复成功");
                    }
                    else
                    {
                        LogManager.WriteDebugLog("TimerControl", $"角色或场景不匹配，不恢复计时: current={currentPureCharacterName} - {currentScene}, saved={savedPureCharacterName} - {settings.InProgressScene}");
                        // 清除不匹配的计时状态
                        ClearTimerState();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载未完成计时状态失败: {ex.Message}");
                LogManager.WriteDebugLog("TimerControl", $"加载未完成计时状态失败: {ex.Message}, 堆栈: {ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 在应用程序关闭时调用，保存计时状态
        /// </summary>
        public void OnApplicationClosing()
        {            
            if (isTimerRunning)
            {
                // 如果计时器正在运行，更新未完成记录
                if (!isPaused)
                {
                    UpdateIncompleteRecord();
                }
                
                SaveTimerState();
                LogManager.WriteDebugLog("TimerControl", "应用程序关闭时保存了计时状态和未完成记录");
            }
        }
    }
}