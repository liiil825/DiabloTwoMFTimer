using System;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Timer
{
    public partial class CharacterSceneControl : UserControl
    {
        private IProfileService? _profileService;
        private bool _isInitialized = false; // 用于防止重复订阅

        public CharacterSceneControl()
        {
            InitializeComponent();
        }

        public void Initialize(IProfileService profileService)
        {
            if (_isInitialized || profileService == null)
                return;

            // 赋值服务
            _profileService = profileService;

            // 注册语言变更事件（LanguageManager 假设是静态的，设计器中可能也需要）
            Utils.LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;

            // 注册ProfileService事件（只有在服务不为空时才订阅）
            _profileService.CurrentProfileChangedEvent += OnProfileChanged;
            _profileService.CurrentSceneChangedEvent += OnSceneChanged;
            _profileService.CurrentDifficultyChangedEvent += OnDifficultyChanged;

            _isInitialized = true;
            UpdateUI();
        }

        private Label lblCharacterDisplay = null!;
        private Label lblSceneDisplay = null!;

        protected override void Dispose(bool disposing)
        {
            if (_profileService == null)
                return;

            if (disposing)
            { // 取消注册语言变更事件
                Utils.LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
                // 取消注册ProfileService事件
                _profileService.CurrentProfileChangedEvent -= OnProfileChanged;
                _profileService.CurrentSceneChangedEvent -= OnSceneChanged;
                _profileService.CurrentDifficultyChangedEvent -= OnDifficultyChanged;
            }
            base.Dispose(disposing);
        }

        private void OnProfileChanged(CharacterProfile? profile) => UpdateUI();

        private void OnSceneChanged(string scene) => UpdateUI();

        private void OnDifficultyChanged(GameDifficulty difficulty) => UpdateUI();

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e) => UpdateUI();

        public void UpdateCharacterSceneInfo() => UpdateUI();

        private void InitializeComponent()
        {
            this.lblCharacterDisplay = new System.Windows.Forms.Label();
            this.lblSceneDisplay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lblCharacterDisplay
            //
            this.lblCharacterDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblCharacterDisplay.Font = new System.Drawing.Font(
                "Microsoft YaHei UI",
                12F,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point,
                ((byte)(134))
            );
            this.lblCharacterDisplay.Location = new System.Drawing.Point(0, 0);
            this.lblCharacterDisplay.Name = "lblCharacterDisplay";
            this.lblCharacterDisplay.Size = new System.Drawing.Size(290, 25);
            this.lblCharacterDisplay.TabIndex = 0;
            this.lblCharacterDisplay.Text = "初始化角色";
            this.lblCharacterDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSceneDisplay
            //
            this.lblSceneDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblSceneDisplay.Font = new System.Drawing.Font(
                "Microsoft YaHei UI",
                12F,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point,
                ((byte)(134))
            );
            this.lblSceneDisplay.Location = new System.Drawing.Point(0, 25);
            this.lblSceneDisplay.Name = "lblSceneDisplay";
            this.lblSceneDisplay.Size = new System.Drawing.Size(290, 25);
            this.lblSceneDisplay.TabIndex = 1;
            this.lblSceneDisplay.Text = "初始化场景";
            this.lblSceneDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // CharacterSceneControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            // 关键修改：使用 Controls.Add 而不是 Parent = this
            this.Controls.Add(this.lblSceneDisplay);
            this.Controls.Add(this.lblCharacterDisplay);
            this.Name = "CharacterSceneControl";
            this.Size = new System.Drawing.Size(290, 50);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// 保存角色和场景设置
        /// </summary>
        /// <param name="character">角色名称</param>
        /// <param name="scene">场景名称</param>
        public void UpdateUI()
        {
            if (_profileService == null)
                return;

            // 更新角色显示 (直接使用变量，无需判空，因为 InitializeComponent 保证了它们存在)
            var profile = _profileService.CurrentProfile;
            if (profile == null)
            {
                this.lblCharacterDisplay.Text = "";
            }
            else
            {
                string characterClass = Utils.LanguageManager.GetLocalizedClassName(profile.Class);
                this.lblCharacterDisplay.Text = $"{profile.Name} ({characterClass})";
            }

            // 更新场景显示
            string currentScene = _profileService.CurrentScene;
            if (string.IsNullOrEmpty(currentScene))
            {
                this.lblSceneDisplay.Text = "";
            }
            else
            {
                string localizedSceneName = Utils.LanguageManager.GetString(currentScene);
                string localizedDifficultyName = _profileService.CurrentDifficultyLocalized;
                this.lblSceneDisplay.Text = $"{localizedDifficultyName} {localizedSceneName}";
            }
        }
    }
}
