using System;
using System.ComponentModel;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.UI.Form;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Profiles;

public partial class SwitchCharacterForm : BaseForm
{
    private readonly IProfileService _profileService;

    public Models.CharacterProfile? SelectedProfile { get; private set; }

    public SwitchCharacterForm(IProfileService profileService)
    {
        _profileService = profileService;
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!this.DesignMode)
        {
            UpdateUI();
            LoadProfiles();
            // 确保列表获得焦点，以便立即响应按键
            lstCharacters.Focus();
        }
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();
        this.Text = LanguageManager.GetString("SwitchCharacter") ?? "切换角色档案";

        if (lblCharacters != null)
            lblCharacters.Text = LanguageManager.GetString("SelectCharacter") ?? "选择角色:";

        // 【核心修复】删除下面这行代码
        // BaseForm 已经将按钮设置为图标样式 (Segoe MDL2 Assets)，
        // 如果这里强行设置中文文字 "选择"，会导致显示乱码或方框。
        // if (btnConfirm != null)
        //    btnConfirm.Text = LanguageManager.GetString("Select") ?? "选择";
    }

    // 【核心修复】绑定 W/S 为上下方向键
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        const int WM_KEYDOWN = 0x100;
        const int WM_SYSKEYDOWN = 0x104;

        if (msg.Msg == WM_KEYDOWN || msg.Msg == WM_SYSKEYDOWN)
        {
            switch (keyData)
            {
                // W -> Up
                case Keys.W:
                    msg.WParam = (IntPtr)Keys.Up;
                    return base.ProcessCmdKey(ref msg, Keys.Up);

                // S -> Down
                case Keys.S:
                    msg.WParam = (IntPtr)Keys.Down;
                    return base.ProcessCmdKey(ref msg, Keys.Down);
            }
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void LoadProfiles()
    {
        try
        {
            LogManager.WriteDebugLog("SwitchCharacterForm", "[详细调试] 开始加载角色档案名称...");
            var profileNames = _profileService.GetAllProfileNames();
            string? currentName = _profileService.CurrentProfile?.Name;
            int targetIndex = 0;
            lstCharacters.Items.Clear();
            foreach (var profileName in profileNames)
            {
                var profileItem = new ProfileItem(profileName);
                lstCharacters.Items.Add(profileItem);

                if (string.Equals(profileName, currentName, StringComparison.OrdinalIgnoreCase))
                {
                    targetIndex = lstCharacters.Items.Count - 1;
                }
            }

            if (lstCharacters.Items.Count > 0)
            {
                lstCharacters.SelectedIndex = targetIndex;
            }

            UpdateEmptyListMessage();
        }
        catch (Exception ex)
        {
            LogManager.WriteDebugLog("SwitchCharacterForm", $"加载角色档案异常: {ex.Message}");
            MessageBox.Show(
                $"{LanguageManager.GetString("ErrorLoadingProfiles") ?? "加载角色档案失败"}: {ex.Message}",
                LanguageManager.GetString("Error") ?? "错误"
            );
        }
    }

    private void UpdateEmptyListMessage()
    {
        if (lstCharacters.Items.Count == 0)
        {
            string emptyMessage = LanguageManager.GetString("NoCharactersFound") ?? "没有找到角色档案";
            lstCharacters.Items.Add(emptyMessage);
            btnConfirm.Enabled = false;
        }
        else
        {
            btnConfirm.Enabled = !(lstCharacters.Items.Count == 1 && lstCharacters.Items[0] is string);
        }
    }

    protected override void BtnConfirm_Click(object? sender, EventArgs e)
    {
        if (lstCharacters.SelectedItem is ProfileItem profileItem)
        {
            var selectedProfile = _profileService.FindProfileByName(profileItem.ProfileName);

            if (selectedProfile != null && !string.IsNullOrEmpty(selectedProfile.Name))
            {
                SelectedProfile = selectedProfile;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                Utils.Toast.Warning(LanguageManager.GetString("InvalidCharacter") ?? "选中的角色数据无效");
            }
        }
        else if (lstCharacters.Items.Count > 0 && lstCharacters.Items[0] is string)
        {
            Utils.Toast.Warning(
                LanguageManager.GetString("NoCharactersAvailable") ?? "没有可用的角色档案，请先创建角色。"
            );
        }
        else
        {
            Utils.Toast.Warning(LanguageManager.GetString("SelectCharacterFirst") ?? "请先选择一个角色");
        }
    }

    private class ProfileItem
    {
        public string ProfileName { get; }
        public string DisplayName { get; }

        public ProfileItem(string profileName)
        {
            ProfileName = profileName;
            DisplayName = profileName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
