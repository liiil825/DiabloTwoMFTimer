using System.Drawing;
using System;
using System.Windows.Forms;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;

namespace DTwoMFTimerHelper.UI.Timer
{
    public partial class CharacterSceneControl : UserControl
    {
        private Label? lblCharacterDisplay;
        private Label? lblSceneDisplay;

        public CharacterSceneControl()
        {
            InitializeComponent();
            // 注册语言变更事件
            Utils.LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
            // 注册ProfileService事件
            ProfileService.Instance.CurrentProfileChanged += OnProfileChanged;
            ProfileService.Instance.CurrentSceneChanged += OnSceneChanged;
            ProfileService.Instance.CurrentDifficultyChanged += OnDifficultyChanged;
        }
        
        protected override void Dispose(bool disposing)
        {            if (disposing)
            {                // 取消注册语言变更事件
                Utils.LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
                // 取消注册ProfileService事件
                ProfileService.Instance.CurrentProfileChanged -= OnProfileChanged;
                ProfileService.Instance.CurrentSceneChanged -= OnSceneChanged;
                ProfileService.Instance.CurrentDifficultyChanged -= OnDifficultyChanged;
            }
            base.Dispose(disposing);
        }
        
        private void OnProfileChanged(CharacterProfile? profile)
        {            UpdateUI();
        }
        
        private void OnSceneChanged(string scene)
        {            UpdateUI();
        }
        
        private void OnDifficultyChanged(GameDifficulty difficulty)
        {            UpdateUI();
        }

        private void InitializeComponent()
        {
            // 角色显示
            lblCharacterDisplay = new Label
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134))),
                Location = new Point(0, 0),
                Name = "lblCharacterDisplay",
                Size = new Size(290, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                TabIndex = 0,
                Parent = this
            };

            // 场景显示
            lblSceneDisplay = new Label
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134))),
                Location = new Point(0, 25),
                Name = "lblSceneDisplay",
                Size = new Size(290, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                TabIndex = 1,
                Parent = this
            };

            // 控件设置
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Size = new Size(290, 50);
            Name = "CharacterSceneControl";
        }

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        public void UpdateCharacterSceneInfo(string characterName, CharacterProfile? profile, string sceneName, string difficultyText)
        {            // 通过ProfileService更新数据
            if (profile != null)
            {                ProfileService.Instance.SwitchCharacter(profile);
            }
            ProfileService.Instance.CurrentScene = sceneName;
            
            // 更新UI
            UpdateUI();
        }
        
        /// <summary>
        /// 设置角色和场景信息
        /// </summary>
        /// <param name="character">角色名称</param>
        /// <param name="scene">场景名称</param>
        public void SetCharacterAndScene(string character, string scene)
        {            Utils.LogManager.WriteDebugLog("CharacterSceneControl", $"SetCharacterAndScene 调用: 角色={character}, 原始场景={scene}");
            
            // 通过ProfileService更新场景
            ProfileService.Instance.CurrentScene = scene;
            
            // 尝试根据角色名称查找对应的角色档案并切换
            if (!string.IsNullOrEmpty(character))
            {                try
                {                    Utils.LogManager.WriteDebugLog("CharacterSceneControl", $"尝试根据角色名称 '{character}' 查找对应的角色档案");
                    var profile = ProfileService.Instance.FindProfileByName(character);
                    if (profile != null)
                    {                        ProfileService.Instance.SwitchCharacter(profile);
                        Utils.LogManager.WriteDebugLog("CharacterSceneControl", $"已关联角色档案: {character}, 档案名称={profile.Name}");
                    }                    else
                    {                        Utils.LogManager.WriteDebugLog("CharacterSceneControl", $"未找到角色档案: {character}");
                    }                }
                catch (Exception ex)
                {                    Utils.LogManager.WriteDebugLog("CharacterSceneControl", $"查找角色档案失败: {ex.Message}");
                }            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// 保存角色和场景设置
        /// </summary>
        /// <param name="character">角色名称</param>
        /// <param name="scene">场景名称</param>
        // 不再需要SaveCharacterSceneSettings方法，因为ProfileService会自动保存设置

        public void UpdateUI()
        {            // 更新角色显示
            if (lblCharacterDisplay != null)
            {                var profile = ProfileService.Instance.CurrentProfile;
                if (profile == null)
                {                    lblCharacterDisplay.Text = "";
                }                else
                {                    // 获取角色职业信息
                    string characterClass = Utils.LanguageManager.GetLocalizedClassName(profile.Class);

                    // 显示角色名称加职业
                    lblCharacterDisplay.Text = $"{profile.Name} ({characterClass})";
                }            }

            // 更新场景显示
            if (lblSceneDisplay != null)
            {                string currentScene = ProfileService.Instance.CurrentScene;
                if (string.IsNullOrEmpty(currentScene))
                {                    lblSceneDisplay.Text = "";
                }                else
                {                    // 获取本地化的场景名称
                    string localizedSceneName = Utils.LanguageManager.GetString(currentScene);
                    // 获取本地化的难度名称
                    string localizedDifficultyName = Utils.LanguageManager.GetString(ProfileService.Instance.CurrentDifficulty.ToString());

                    // 在场景名称前添加难度
                    lblSceneDisplay.Text = $"{localizedDifficultyName} {localizedSceneName}";
                }            }
        }
    }
}