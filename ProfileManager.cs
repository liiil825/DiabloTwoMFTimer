using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using DTwoMFTimerHelper.Data;
using DTwoMFTimerHelper.Settings;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper
{
    public partial class ProfileManager : UserControl
    {
        // MF记录功能相关字段
        private bool isTimerRunning = false;
        private DateTime startTime = DateTime.MinValue;
        private Timer? timer;
        private Data.CharacterProfile? currentProfile = null;
        private Data.MFRecord? currentRecord = null;
        private List<Data.FarmingScene> farmingScenes = new List<Data.FarmingScene>();

        // 事件
        public event EventHandler? TimerStateChanged;

        // 公共属性
        public bool IsTimerRunning => isTimerRunning;
        public Data.CharacterProfile? CurrentProfile => currentProfile;
        public Data.MFRecord? CurrentRecord => currentRecord;
        
        // 当前选中的场景
        public string CurrentScene {
            get {
                if (cmbScene != null)
                    return cmbScene.Text;
                return string.Empty;
            }
        }
        
        // 当前选中的难度
        public Data.GameDifficulty CurrentDifficulty {
            get {
                return GetSelectedDifficulty();
            }
        }

        public ProfileManager()
        {
            InitializeComponent();
            InitializeTimer();
            LoadFarmingScenes();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            // 角色管理按钮
            btnCreateCharacter = new Button();
            btnSwitchCharacter = new Button();
            btnDeleteCharacter = new Button();
            
            // 场景和难度选择
            lblScene = new Label();
            cmbScene = new ComboBox();
            lblDifficulty = new Label();
            cmbDifficulty = new ComboBox();
            
            // 计时控制按钮 - 只保留开始按钮
            btnStartStop = new Button();
            
            // 状态显示标签
            lblCurrentProfile = new Label();
            lblTime = new Label();
            lblStats = new Label();
            
            SuspendLayout();
            // 
            // btnCreateCharacter
            // 
            btnCreateCharacter.Location = new System.Drawing.Point(30, 30);
            btnCreateCharacter.Margin = new Padding(6);
            btnCreateCharacter.Name = "btnCreateCharacter";
            btnCreateCharacter.Size = new System.Drawing.Size(140, 40);
            btnCreateCharacter.TabIndex = 0;
            btnCreateCharacter.UseVisualStyleBackColor = true;
            btnCreateCharacter.Click += btnCreateCharacter_Click;
            // 
            // btnSwitchCharacter
            // 
            btnSwitchCharacter.Location = new System.Drawing.Point(190, 30);
            btnSwitchCharacter.Margin = new Padding(6);
            btnSwitchCharacter.Name = "btnSwitchCharacter";
            btnSwitchCharacter.Size = new System.Drawing.Size(140, 40);
            btnSwitchCharacter.TabIndex = 1;
            btnSwitchCharacter.UseVisualStyleBackColor = true;
            btnSwitchCharacter.Click += btnSwitchCharacter_Click;
            // 
            // btnDeleteCharacter
            // 
            btnDeleteCharacter.Location = new System.Drawing.Point(350, 30);
            btnDeleteCharacter.Margin = new Padding(6);
            btnDeleteCharacter.Name = "btnDeleteCharacter";
            btnDeleteCharacter.Size = new System.Drawing.Size(140, 40);
            btnDeleteCharacter.TabIndex = 2;
            btnDeleteCharacter.UseVisualStyleBackColor = true;
            btnDeleteCharacter.Click += btnDeleteCharacter_Click;
            btnDeleteCharacter.Enabled = false;
            // 
            // lblScene
            // 
            lblScene.AutoSize = true;
            lblScene.Location = new System.Drawing.Point(30, 100);
            lblScene.Margin = new Padding(6, 0, 6, 0);
            lblScene.Name = "lblScene";
            lblScene.Size = new System.Drawing.Size(80, 28);
            lblScene.TabIndex = 3;
            // 
            // cmbScene
            // 
            cmbScene.Location = new System.Drawing.Point(130, 100);
            cmbScene.Margin = new Padding(6);
            cmbScene.Name = "cmbScene";
            cmbScene.Size = new System.Drawing.Size(250, 34);
            cmbScene.TabIndex = 4;
            cmbScene.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbScene.SelectedIndexChanged += cmbScene_SelectedIndexChanged;
            // 
            // lblDifficulty
            // 
            lblDifficulty.AutoSize = true;
            lblDifficulty.Location = new System.Drawing.Point(30, 160);
            lblDifficulty.Margin = new Padding(6, 0, 6, 0);
            lblDifficulty.Name = "lblDifficulty";
            lblDifficulty.Size = new System.Drawing.Size(80, 28);
            lblDifficulty.TabIndex = 5;
            // 
            // cmbDifficulty
            // 
            cmbDifficulty.Location = new System.Drawing.Point(130, 160);
            cmbDifficulty.Margin = new Padding(6);
            cmbDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDifficulty.SelectedIndexChanged += cmbDifficulty_SelectedIndexChanged;
            cmbDifficulty.Name = "cmbDifficulty";
            cmbDifficulty.Size = new System.Drawing.Size(200, 34);
            cmbDifficulty.TabIndex = 6;
            cmbDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            // 添加难度选项
            foreach (Data.GameDifficulty difficulty in Enum.GetValues(typeof(Data.GameDifficulty)))
            {
                cmbDifficulty.Items.Add(GetLocalizedDifficultyName(difficulty));
            }
            if (cmbDifficulty.Items.Count > 0)
                cmbDifficulty.SelectedIndex = 2; // 默认地狱难度
            // 
            // btnStartStop
            // 
            btnStartStop.Location = new System.Drawing.Point(30, 220);
            btnStartStop.Margin = new Padding(6);
            btnStartStop.Name = "btnStartStop";
            btnStartStop.Size = new System.Drawing.Size(130, 50);
            btnStartStop.TabIndex = 7;
            btnStartStop.UseVisualStyleBackColor = true;
            btnStartStop.Click += BtnStartStop_Click;
            btnStartStop.Enabled = false;
            // 
            // lblCurrentProfile
            // 
            lblCurrentProfile.AutoSize = true;
            lblCurrentProfile.Location = new System.Drawing.Point(30, 300);
            lblCurrentProfile.Margin = new Padding(6, 0, 6, 0);
            lblCurrentProfile.Name = "lblCurrentProfile";
            lblCurrentProfile.Size = new System.Drawing.Size(120, 28);
            lblCurrentProfile.TabIndex = 10;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Location = new System.Drawing.Point(30, 340);
            lblTime.Margin = new Padding(6, 0, 6, 0);
            lblTime.Name = "lblTime";
            lblTime.Size = new System.Drawing.Size(120, 28);
            lblTime.TabIndex = 11;
            // 
            // lblStats
            // 
            lblStats.AutoSize = true;
            lblStats.Location = new System.Drawing.Point(30, 380);
            lblStats.Margin = new Padding(6, 0, 6, 0);
            lblStats.Name = "lblStats";
            lblStats.Size = new System.Drawing.Size(120, 28);
            lblStats.TabIndex = 12;
            // 
            // ProfileManager (档案管理界面)
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnCreateCharacter);
            Controls.Add(btnSwitchCharacter);
            Controls.Add(btnDeleteCharacter);
            Controls.Add(lblScene);
            Controls.Add(cmbScene);
            Controls.Add(lblDifficulty);
            Controls.Add(cmbDifficulty);
            Controls.Add(btnStartStop);
            Controls.Add(lblCurrentProfile);
            Controls.Add(lblTime);
            Controls.Add(lblStats);
            Margin = new Padding(6);
            Name = "ProfileManager";
            Size = new System.Drawing.Size(542, 450);
            ResumeLayout(false);
            PerformLayout();
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 1000; // 1秒
            timer.Tick += Timer_Tick;
        }

        private void LoadFarmingScenes()
        {
            StringBuilder debugInfo = new StringBuilder();
            debugInfo.AppendLine("===== 开始加载场景列表到下拉框 =====");
            debugInfo.AppendLine("调用DataManager.LoadFarmingSpots()...");
            
            farmingScenes = DataManager.LoadFarmingSpots();
            debugInfo.AppendLine($"获取到的场景列表数量: {farmingScenes.Count}");
            
            debugInfo.AppendLine("清空下拉框...");
            cmbScene?.Items.Clear();
            
            if (farmingScenes.Count == 0)
            {
                debugInfo.AppendLine("警告: 场景列表为空，无法添加到下拉框");
                MessageBox.Show(debugInfo.ToString(), "场景加载警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                debugInfo.AppendLine("开始遍历场景并添加到下拉框...");
                int addedCount = 0;
                
                foreach (var scene in farmingScenes)
                {
                    string displayName = GetSceneDisplayName(scene);
                    debugInfo.AppendLine($"添加场景: {displayName}");
                    if (cmbScene != null) cmbScene.Items.Add(displayName);
                    addedCount++;
                    
                    // 限制显示的场景数量，避免MessageBox过大
                    if (debugInfo.Length > 1000)
                    {
                        debugInfo.AppendLine($"...还有 {farmingScenes.Count - addedCount} 个场景未显示");
                        break;
                    }
                }
                
                debugInfo.AppendLine($"成功添加 {addedCount} 个场景到下拉框");
                
                if (cmbScene != null && cmbScene.Items.Count > 0)
                {
                    cmbScene.SelectedIndex = 0;
                    debugInfo.AppendLine($"默认选中第一个场景: {cmbScene.SelectedItem}");
                }
                
                // 只输出到控制台，不再显示弹窗
                string summary = $"场景列表加载完成\n场景总数: {farmingScenes.Count}\n成功添加到下拉框: {(cmbScene != null ? cmbScene.Items.Count : 0)}";
                Console.WriteLine(summary);
            }
            
            // 尝试加载上次使用的场景、难度和角色档案
            try
            {
                var settings = DTwoMFTimerHelper.Settings.SettingsManager.LoadSettings();
                
                // 加载上次使用的场景
                if (!string.IsNullOrEmpty(settings.LastUsedScene) && cmbScene != null)
                {
                    for (int i = 0; i < cmbScene.Items.Count; i++)
                    {
                        var itemText = cmbScene.Items[i]?.ToString();
                        if (itemText != null && itemText == settings.LastUsedScene)
                        {
                            cmbScene.SelectedIndex = i;
                            debugInfo.AppendLine($"已恢复上次使用的场景: {settings.LastUsedScene}");
                            break;
                        }
                    }
                }
                
                // 尝试加载上次使用的难度
                if (!string.IsNullOrEmpty(settings.LastUsedDifficulty) && cmbDifficulty != null)
                {
                    if (Enum.TryParse<DTwoMFTimerHelper.Data.GameDifficulty>(settings.LastUsedDifficulty, out var difficulty))
                    {
                        // 根据难度值设置下拉框索引
                        switch (difficulty)
                        {
                            case DTwoMFTimerHelper.Data.GameDifficulty.Normal:
                                cmbDifficulty.SelectedIndex = 0;
                                break;
                            case DTwoMFTimerHelper.Data.GameDifficulty.Nightmare:
                                cmbDifficulty.SelectedIndex = 1;
                                break;
                            case DTwoMFTimerHelper.Data.GameDifficulty.Hell:
                                cmbDifficulty.SelectedIndex = 2;
                                break;
                        }
                        debugInfo.AppendLine($"已恢复上次使用的难度: {settings.LastUsedDifficulty}");
                    }
                }
                
                // 尝试加载上次使用的角色档案
                if (!string.IsNullOrEmpty(settings.LastUsedProfile))
                {
                    var profile = DataManager.FindProfileByName(settings.LastUsedProfile);
                    if (profile != null)
                    {
                        currentProfile = profile;
                        if (btnDeleteCharacter != null) btnDeleteCharacter.Enabled = true;
                        if (btnStartStop != null) btnStartStop.Enabled = true;
                        debugInfo.AppendLine($"已恢复上次使用的角色档案: {settings.LastUsedProfile}");
                        // 更新界面显示
                        UpdateUI();
                    }
                }
            }
            catch (Exception ex)
            {
                debugInfo.AppendLine($"加载上次设置失败: {ex.Message}");
            }
            
            debugInfo.AppendLine("===== 场景列表加载到下拉框完成 =====");
        }

        private string GetSceneDisplayName(Data.FarmingScene scene)
        {
            string actText = $"ACT {scene.ACT}";
            // 根据当前语言获取场景名称
            string language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.StartsWith("zh") ? "Chinese" : "English";
            string name = scene.GetSceneName(language);
            return $"{actText}: {name}";
        }

        private string GetLocalizedDifficultyName(Data.GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case Data.GameDifficulty.Normal: return DTwoMFTimerHelper.Utils.LanguageManager.GetString("DifficultyNormal");
        case Data.GameDifficulty.Nightmare: return DTwoMFTimerHelper.Utils.LanguageManager.GetString("DifficultyNightmare");
        case Data.GameDifficulty.Hell: return DTwoMFTimerHelper.Utils.LanguageManager.GetString("DifficultyHell");
                default: return difficulty.ToString();
            }
        }

        private Data.GameDifficulty GetSelectedDifficulty()
        {
            if (cmbDifficulty?.SelectedIndex >= 0)
            {
                switch (cmbDifficulty.SelectedIndex)
                {
                    case 0: return Data.GameDifficulty.Normal;
                    case 1: return Data.GameDifficulty.Nightmare;
                    case 2: return Data.GameDifficulty.Hell;
                    default: return Data.GameDifficulty.Hell;
                }
            }
            return Data.GameDifficulty.Hell;
        }

        private Data.FarmingScene? GetSelectedScene()
        {
            if (cmbScene?.SelectedIndex >= 0 && cmbScene.SelectedIndex < farmingScenes.Count)
                return farmingScenes[cmbScene.SelectedIndex];
            return null;
        }

        private string GetCurrentCharacterName()
        {
            if (currentProfile != null)
            {
                string className = Utils.LanguageManager.GetLocalizedClassName(currentProfile.Class);
                return $"{currentProfile.Name} ({className})";
            }
            return "未选择";
        }
        
        private string GetCurrentSceneName()
        {
            var selectedScene = GetSelectedScene();
            if (selectedScene != null)
            {
                return GetSceneDisplayName(selectedScene);
            }
            return "未选择";
        }
        
        public void UpdateUI()
        {
            // 更新按钮文本
            if (btnCreateCharacter != null) btnCreateCharacter.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("CreateCharacter");
            if (btnSwitchCharacter != null) btnSwitchCharacter.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("SwitchCharacter");
            if (btnDeleteCharacter != null) btnDeleteCharacter.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("DeleteCharacter");
            
            // 根据是否有未完成记录设置开始按钮文本
            if (btnStartStop != null)
            {
                // 添加日志记录开始检查过程
                DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", "开始检查未完成记录");
                
                // 检查是否有未完成记录
                bool hasIncompleteRecord = false;
                if (currentProfile != null)
                {
                    DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", $"当前角色: {currentProfile.Name}");
                    
                    var selectedScene = GetSelectedScene();
                    if (selectedScene != null)
                    {
                        var difficulty = GetSelectedDifficulty();
                        DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", $"选中难度: {difficulty}");
                        
                        // 获取场景的纯英文名称（与记录存储格式一致）
                        string sceneDisplayName = GetSceneDisplayName(selectedScene);
                        DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", $"场景显示名称: {sceneDisplayName}");
                        
                        string pureSceneName = sceneDisplayName;
                        if (sceneDisplayName.StartsWith("ACT ") || sceneDisplayName.StartsWith("Act ") || sceneDisplayName.StartsWith("act "))
                        {
                            int colonIndex = sceneDisplayName.IndexOf(':');
                            if (colonIndex > 0)
                            {
                                pureSceneName = sceneDisplayName.Substring(colonIndex + 1).Trim();
                                DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", $"提取纯场景名称: {pureSceneName}");
                            }
                        }
                        
                        string pureEnglishSceneName = DTwoMFTimerHelper.Utils.LanguageManager.GetPureEnglishSceneName(sceneDisplayName);
                        DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", $"获取纯英文场景名称: {pureEnglishSceneName}");
                        
                        // 记录当前配置文件中的记录数量
                        DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", $"当前角色记录数量: {currentProfile.Records.Count}");
                              
                        // 查找同场景、同难度、未完成的记录
                        hasIncompleteRecord = currentProfile.Records.Any(r => 
                            r.SceneName == pureEnglishSceneName && 
                            r.Difficulty == difficulty && 
                            !r.IsCompleted);
                        
                        DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", $"是否存在未完成记录: {hasIncompleteRecord}");
                        
                        // 记录第一条匹配场景和难度的记录详细信息（用于调试）
                        var matchingRecord = currentProfile.Records.FirstOrDefault(r => 
                            r.SceneName == pureEnglishSceneName && 
                            r.Difficulty == difficulty);
                        if (matchingRecord != null)
                        {
                            DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", $"找到匹配记录 - 场景: {matchingRecord.SceneName}, 难度: {matchingRecord.Difficulty}, 完成状态: {matchingRecord.IsCompleted}");
                        }
                    }
                    else
                    {
                        DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", "未选中场景");
                    }
                }
                else
                {
                    DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", "当前没有选择角色配置");
                }
                
                // 根据是否有未完成记录设置按钮文本
                if (hasIncompleteRecord)
                {
                    btnStartStop.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("ContinueFarm");
                    DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", "设置按钮文本为: ContinueFarm");
                }
                else
                {
                    btnStartStop.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("StartTimer");
                    DTwoMFTimerHelper.Utils.LogManager.WriteDebugLog("ProfileManager", "设置按钮文本为: StartTimer");
                }
            }
            if (lblScene != null) lblScene.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("SelectScene");
            if (lblDifficulty != null) lblDifficulty.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("DifficultyLabel");
            
            // 更新当前角色显示
            if (currentProfile != null)
            {
                string className = Utils.LanguageManager.GetLocalizedClassName(currentProfile.Class);
                if (lblCurrentProfile != null) lblCurrentProfile.Text = $"当前角色: {currentProfile.Name} ({className})";
            }
            else
            {
                if (lblCurrentProfile != null) lblCurrentProfile.Text = "当前角色: 未选择";
            }
            
            // 隐藏时间和统计信息标签，去掉红色标注
            if (lblTime != null) 
            {
                lblTime.Visible = false;
            }
            if (lblStats != null) 
            {
                lblStats.Visible = false;
            }
            
            // 更新按钮状态
            if (btnDeleteCharacter != null) btnDeleteCharacter.Enabled = currentProfile != null;
            if (btnStartStop != null) btnStartStop.Enabled = currentProfile != null;
        }
        
        // 使用LanguageManager中的GetLocalizedClassName方法

        private string FormatTime(double seconds)
        {
            int hours = (int)(seconds / 3600);
            int minutes = (int)((seconds % 3600) / 60);
            int secs = (int)(seconds % 60);
            
            if (hours > 0)
                return $"{hours}时{minutes}分";
            else if (minutes > 0)
                return $"{minutes}分{secs}秒";
            else
                return $"{secs}秒";
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        private void btnCreateCharacter_Click(object? sender, EventArgs e)
        {
            using (var form = new CreateCharacterForm())
            {
                if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(form.CharacterName))
                {
                    try
                    {
                        Console.WriteLine("开始创建新角色...");
                        
                        // 确保cmbScene不为null
                        if (cmbScene != null)
                        {
                            cmbScene.SelectedIndex = 0; // 重置场景选择
                        }

                        string characterName = form.CharacterName;
                        Console.WriteLine($"角色名称: {characterName}");
                        
                        // 获取选中的职业并确保类型转换正确
                        var selectedClass = form.GetSelectedClass();
                        if (!selectedClass.HasValue)
                        {
                            throw new InvalidOperationException("未选择有效的角色职业");
                        }
                        
                        // 显式转换为Data命名空间下的CharacterClass
                        DTwoMFTimerHelper.Data.CharacterClass charClass = (DTwoMFTimerHelper.Data.CharacterClass)selectedClass.Value;
                        Console.WriteLine($"角色职业: {charClass}");
                        
                        // 创建新角色档案
                        currentProfile = Data.DataManager.CreateNewProfile(characterName, charClass);
                        
                        // 验证创建结果
                        if (currentProfile == null)
                        {
                            throw new InvalidOperationException("创建角色失败，返回的配置文件为null");
                        }
                        
                        Console.WriteLine($"角色创建成功: {currentProfile.Name}");

                        // 更新上次使用的角色档案设置
                        var settings = SettingsManager.LoadSettings();
                        settings.LastUsedProfile = currentProfile.Name;
                        SettingsManager.SaveSettings(settings);

                        // 清除当前记录并更新UI
                        currentRecord = null;
                        UpdateUI();
                        
                        // 显示成功消息
                        MessageBox.Show($"角色 '{characterName}' 创建成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"创建角色失败: {ex.Message}");
                        Console.WriteLine($"异常堆栈: {ex.StackTrace}");
                        MessageBox.Show($"创建角色失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSwitchCharacter_Click(object? sender, EventArgs e)
        {            
            using (var form = new SwitchCharacterForm())
            {                
                if (form.ShowDialog() == DialogResult.OK && form.SelectedProfile != null)
                {
                    try
                    {
                        Console.WriteLine("开始切换角色...");
                        
                        // 停止当前计时
                        StopTimer();
                        
                        // 切换角色，确保类型正确
                        var selectedProfile = form.SelectedProfile;
                        
                        // 验证角色数据
                        if (selectedProfile == null || string.IsNullOrWhiteSpace(selectedProfile.Name))
                        {
                            throw new InvalidOperationException("无效的角色数据");
                        }
                        
                        // 确保使用的是Data命名空间下的CharacterProfile
                        // 由于我们已经修改了CharacterProfile的命名空间，现在应该可以直接使用
                        currentProfile = selectedProfile;
                        currentRecord = null;
                        
                        // 更新上次使用的角色档案设置
                        var settings = SettingsManager.LoadSettings();
                        settings.LastUsedProfile = currentProfile.Name;
                        SettingsManager.SaveSettings(settings);
                        
                        Console.WriteLine($"成功切换到角色: {currentProfile.Name}");
                        
                        // 更新UI显示新角色信息
                        UpdateUI();
                        
                        // 同步更新TimerControl
                        SyncTimerControl();
                        
                        // 显示成功消息
                        MessageBox.Show($"已成功切换到角色 '{currentProfile.Name}'", "切换成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"切换角色失败: {ex.Message}");
                        Console.WriteLine($"异常堆栈: {ex.StackTrace}");
                        MessageBox.Show($"切换角色失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDeleteCharacter_Click(object? sender, EventArgs e)
        {
            if (currentProfile == null) return;
            
            string confirmMsg = $"确定要删除角色: {currentProfile.Name}?";
            if (MessageBox.Show(confirmMsg, "删除角色", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 停止计时
                StopTimer();
                
                // 删除角色档案
                DataManager.DeleteProfile(currentProfile);
                currentProfile = null;
                currentRecord = null;
                UpdateUI();
            }
        }

        // 使用LogManager类记录日志
        private void WriteDebugLog(string message)
        {
            LogManager.WriteDebugLog("ProfileManager", message);
        }
        
        private void BtnStartStop_Click(object? sender, EventArgs e)
        {            // 获取当前选择的场景和角色信息
            string selectedScene = cmbScene?.Text ?? "";
            string characterName = currentProfile != null ? GetCurrentCharacterName() : "";
            
            // 获取不带ACT前缀的英文场景名称
            string pureSceneName = selectedScene;
            if (selectedScene.StartsWith("ACT ") || selectedScene.StartsWith("Act ") || selectedScene.StartsWith("act "))
            {
                int colonIndex = selectedScene.IndexOf(':');
                if (colonIndex > 0)
                {
                    pureSceneName = selectedScene.Substring(colonIndex + 1).Trim();
                }
            }
            
            // 使用DataManager获取英文场景名称
            string englishSceneName = DTwoMFTimerHelper.Data.DataManager.GetEnglishSceneName(pureSceneName);
            
            // 确保场景名称不为空
            if (string.IsNullOrEmpty(englishSceneName))
            {
                englishSceneName = "UnknownScene"; // 设置默认值
                WriteDebugLog($"警告: 获取英文场景名称失败，pureSceneName: '{pureSceneName}'，使用默认值 '{englishSceneName}'");
            }
            
            // 记录点击开始计时时的角色档案信息
            WriteDebugLog($"btnStartStop_Click 调用（点击开始计时按钮），默认用户档案名称: {characterName}, 当前选择的场景: {selectedScene}, 英文纯名称: {englishSceneName}");
            WriteDebugLog($"当前CurrentProfile: {(currentProfile != null ? currentProfile.Name : "null")}");
            
            // 跳转到计时器页面
            TabControl? tabControl = ParentForm?.Controls.Find("tabControl", true).FirstOrDefault() as TabControl;
            if (tabControl != null)
            {
                tabControl.SelectedIndex = 1;
                WriteDebugLog("已切换到计时器页面");
            }
            else
            {
                WriteDebugLog("未找到tabControl");
            }
            
            // 查找TimerControl并设置角色和场景信息，然后启动计时器
            TimerControl? timerControl = ParentForm?.Controls.Find("timerControl", true).FirstOrDefault() as TimerControl;
            if (timerControl != null)
            {
                WriteDebugLog($"已找到TimerControl，准备设置角色和场景: {characterName}, {englishSceneName}");
                timerControl.SetCharacterAndScene(characterName, englishSceneName);
                timerControl.StartTimer();
            }
            else
            {
                WriteDebugLog("未找到timerControl");
            }
        }
        
        // 场景选择变更事件处理
        private void cmbScene_SelectedIndexChanged(object? sender, EventArgs e)
        {            
            // 当场景改变时，同步更新TimerControl
            if (cmbScene != null)
            {
                Console.WriteLine($"场景已变更为: {cmbScene.Text}");
                SyncTimerControl();
                // 更新按钮文本，检查是否有未完成记录
                UpdateUI();
            }
        }
        
        private void cmbDifficulty_SelectedIndexChanged(object? sender, EventArgs e)
        {            
            // 当难度改变时，更新按钮文本，检查是否有未完成记录
            UpdateUI();
        }
        
        /// <summary>
        /// 同步更新TimerControl的角色和场景信息
        /// </summary>
        private void SyncTimerControl()
        {            
            try
            {                
                // 获取主窗口
                var mainForm = this.FindForm() as MainForm;
                if (mainForm != null && mainForm.TimerControl != null)
                {                    
                    // 调用TimerControl的同步方法
                    mainForm.TimerControl.SyncWithProfileManager();
                    Console.WriteLine($"已同步TimerControl的角色和场景信息");
                }
            }
            catch (Exception ex)
            {                
                Console.WriteLine($"同步TimerControl失败: {ex.Message}");
                Console.WriteLine($"异常堆栈: {ex.StackTrace}");
            }
        }
        
        private void StartTimer()
        {
            if (!isTimerRunning && currentProfile != null)
            {
                // 创建新记录
                var selectedScene = GetSelectedScene();
                if (selectedScene != null)
                {
                    startTime = DateTime.Now;
                    isTimerRunning = true;
                    
                    // 创建新的MF记录
                    currentRecord = new Data.MFRecord
                    {
                        SceneName = selectedScene.enUS, // 只保留SceneName字段，保存英文名称
                        ACT = selectedScene.ACT,
                        Difficulty = GetSelectedDifficulty(),
                        StartTime = startTime
                    };
                    
                    timer?.Start();
                    UpdateUI();
                    
                    // 触发事件
                    TimerStateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        
        private void StopTimer()
        {
            if (isTimerRunning)
            {
                timer?.Stop();
                isTimerRunning = false;
                UpdateUI();
                
                // 触发事件
                TimerStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        /// <summary>
        /// 加载上次使用的角色档案
        /// </summary>
        public void LoadLastUsedProfile()
        {            WriteDebugLog("LoadLastUsedProfile 开始执行");
            try
            {                // 加载设置并获取上次使用的角色档案名称
                var settings = SettingsManager.LoadSettings();
                string lastUsedProfileName = settings.LastUsedProfile;
                WriteDebugLog($"从配置文件加载设置: LastUsedProfile={lastUsedProfileName}");
                
                if (!string.IsNullOrWhiteSpace(lastUsedProfileName))
                {                    WriteDebugLog($"尝试加载上次使用的角色档案: {lastUsedProfileName}");
                    Console.WriteLine($"尝试加载上次使用的角色档案: {lastUsedProfileName}");
                    
                    // 加载所有角色档案
                    var allProfiles = DataManager.LoadAllProfiles(false);
                    WriteDebugLog($"已加载所有角色档案，数量: {allProfiles.Count}");
                    
                    // 查找上次使用的角色档案
                    var profile = allProfiles.FirstOrDefault(p => p.Name == lastUsedProfileName);
                    if (profile != null)
                    {                        currentProfile = profile;
                        currentRecord = null;
                        UpdateUI();
                        WriteDebugLog($"成功加载上次使用的角色档案: {lastUsedProfileName}, profile.Name={profile.Name}");
                        Console.WriteLine($"成功加载上次使用的角色档案: {lastUsedProfileName}");
                    }
                    else
                    {                        WriteDebugLog($"未找到上次使用的角色档案: {lastUsedProfileName}");
                        Console.WriteLine($"未找到上次使用的角色档案: {lastUsedProfileName}");
                    }
                }
                else
                {                    WriteDebugLog("没有保存的上次使用角色档案");
                    Console.WriteLine("没有保存的上次使用角色档案");
                }
            }
            catch (Exception ex)
            {                WriteDebugLog($"加载上次使用角色档案失败: {ex.Message}, 堆栈: {ex.StackTrace}");
                Console.WriteLine($"加载上次使用角色档案失败: {ex.Message}");
            }
        }
        
        // 控件字段 - 标记为可为null以修复CS8618警告
        private Button? btnCreateCharacter;
        private Button? btnSwitchCharacter;
        private Button? btnDeleteCharacter;
        private Label? lblScene;
        private ComboBox? cmbScene;
        private Label? lblDifficulty;
        private ComboBox? cmbDifficulty;
        private Button? btnStartStop;
        private Label? lblCurrentProfile;
        private Label? lblTime;
        private Label? lblStats;
    }
}