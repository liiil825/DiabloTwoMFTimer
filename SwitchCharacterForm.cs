using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DTwoMFTimerHelper.Data;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper
{
    public class SwitchCharacterForm : Form
    {
        private ListBox? lstCharacters;
        private Button? btnSelect;
        private Button? btnCancel;
        private Label? lblCharacters;
        
        // 属性
        public Data.CharacterProfile? SelectedProfile { get; private set; }
        
        public SwitchCharacterForm()
        {
            InitializeComponent();
            UpdateUI();
            LoadProfiles();
        }
        
        private void InitializeComponent()
        {
            // 设置窗口属性
            this.Text = "切换角色档案";
            this.Size = new System.Drawing.Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            
            // 初始化控件
            lblCharacters = new Label();
            lstCharacters = new ListBox();
            btnSelect = new Button();
            btnCancel = new Button();
            
            // 设置控件位置和大小
            lblCharacters.Location = new System.Drawing.Point(30, 20);
            lblCharacters.Size = new System.Drawing.Size(100, 25);
            
            lstCharacters.Location = new System.Drawing.Point(30, 50);
            lstCharacters.Size = new System.Drawing.Size(320, 160);
            lstCharacters.DisplayMember = "DisplayName";
            
            btnSelect.Location = new System.Drawing.Point(120, 230);
            btnSelect.Size = new System.Drawing.Size(80, 30);
            btnSelect.Click += btnSelect_Click;
            
            btnCancel.Location = new System.Drawing.Point(250, 230);
            btnCancel.Size = new System.Drawing.Size(80, 30);
            btnCancel.Click += btnCancel_Click;
            
            // 添加控件到表单
            this.Controls.Add(lblCharacters);
            this.Controls.Add(lstCharacters);
            this.Controls.Add(btnSelect);
            this.Controls.Add(btnCancel);
        }
        
        public void UpdateUI()
        {
            this.Text = LanguageManager.GetString("SwitchCharacter") ?? "切换角色档案";
            lblCharacters!.Text = LanguageManager.GetString("SelectCharacter") ?? "选择角色:";
            btnSelect!.Text = LanguageManager.GetString("Select") ?? "选择";
            btnCancel!.Text = LanguageManager.GetString("Cancel") ?? "取消";
        }
        
        private void LoadProfiles()
        {
            try
            {
                Console.WriteLine("[详细调试] SwitchCharacterForm: 开始加载角色档案...");
                // 明确指定只加载非隐藏角色
                var profiles = DataManager.LoadAllProfiles(includeHidden: false);
                Console.WriteLine("[详细调试] SwitchCharacterForm: 从DataManager获取到角色档案");
                
                lstCharacters!.Items.Clear();
                Console.WriteLine("[详细调试] SwitchCharacterForm: 已清空角色列表");
                
                Console.WriteLine($"[详细调试] SwitchCharacterForm: 找到 {profiles.Count} 个角色档案");
                
                // 显示每个角色的详细信息
                foreach (var profile in profiles)
                {
                    Console.WriteLine($"[详细调试] SwitchCharacterForm: 加载角色: {profile.Name}, 职业: {profile.Class}, IsHidden: {profile.IsHidden}");
                    var profileItem = new ProfileItem(profile);
                    Console.WriteLine($"[详细调试] SwitchCharacterForm: 创建ProfileItem: {profileItem.DisplayName}");
                    lstCharacters.Items.Add(profileItem);
                    Console.WriteLine($"[详细调试] SwitchCharacterForm: 已添加到列表，当前列表项数: {lstCharacters.Items.Count}");
                }
                
                if (lstCharacters.Items.Count > 0)
                {
                    lstCharacters.SelectedIndex = 0;
                    Console.WriteLine($"[详细调试] SwitchCharacterForm: 已选中第一个角色: {lstCharacters.SelectedItem}");
                }
                else
                {
                    Console.WriteLine("[详细调试] SwitchCharacterForm: 角色列表为空");
                }
                
                // 更新UI显示
                Console.WriteLine("[详细调试] SwitchCharacterForm: 准备更新空列表消息");
                UpdateEmptyListMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载角色档案异常: {ex.Message}");
                Console.WriteLine($"异常堆栈: {ex.StackTrace}");
                // 使用本地化的错误消息
                MessageBox.Show($"{LanguageManager.GetString("ErrorLoadingProfiles") ?? "加载角色档案失败"}: {ex.Message}", 
                    LanguageManager.GetString("Error") ?? "错误");
            }
        }
        
        private void UpdateEmptyListMessage()
        {
            if (lstCharacters!.Items.Count == 0)
            {
                // 确保使用本地化的空列表消息
                string emptyMessage = LanguageManager.GetString("NoCharactersFound") ?? "没有找到角色档案";
                Console.WriteLine($"角色列表为空，显示消息: {emptyMessage}");
                lstCharacters.Items.Add(emptyMessage);
                btnSelect!.Enabled = false;
            }
            else
            {
                // 禁用按钮只有在选中项目是占位符消息时才需要
                btnSelect!.Enabled = true;
            }
        }
        
        private void btnSelect_Click(object? sender, EventArgs e)
        {
            if (lstCharacters!.SelectedItem is ProfileItem profileItem)
            {
                // 验证角色数据有效性
                if (profileItem.Profile != null && !string.IsNullOrEmpty(profileItem.Profile.Name))
                {
                    SelectedProfile = profileItem.Profile;
                    Console.WriteLine($"已选择角色: {profileItem.Profile.Name}");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    Console.WriteLine("选中的角色数据无效");
                    MessageBox.Show(LanguageManager.GetString("InvalidCharacter") ?? "选中的角色数据无效", 
                        LanguageManager.GetString("Warning") ?? "警告");
                }
            }
            else if (lstCharacters.Items.Count == 1 && lstCharacters.SelectedItem is string)
            {
                // 如果只有一项且是字符串（空列表提示消息），给出更明确的提示
                Console.WriteLine("用户尝试选择空列表提示消息");
                MessageBox.Show(LanguageManager.GetString("NoCharactersAvailable") ?? "没有可用的角色档案，请先创建角色。", 
                    LanguageManager.GetString("Information") ?? "提示");
            }
            else
            {
                Console.WriteLine("请先选择角色");
                MessageBox.Show(LanguageManager.GetString("SelectCharacterFirst") ?? "请先选择一个角色", 
                    LanguageManager.GetString("Information") ?? "提示");
            }
        }
        
        private void btnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        // 包装类，用于显示友好的角色信息
        private class ProfileItem
        {
            public Data.CharacterProfile Profile { get; }
            public string DisplayName { get; }
            
            public ProfileItem(Data.CharacterProfile profile)
            {
                Profile = profile;
                
                // 获取本地化的职业名称
                string className = DTwoMFTimerHelper.Utils.LanguageManager.GetLocalizedClassName(profile.Class);
                
                // 显示角色名称、职业和游戏统计
                DisplayName = $"{profile.Name} - {className} (游戏局数: {profile.CompletedGamesCount}, 总时间: {FormatTime(profile.TotalPlayTimeSeconds)})";
            }
            
            // 使用Utils.LanguageManager中的GetLocalizedClassName方法
            
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
        }
    }
}