using System;
using System.Windows.Forms;
using DTwoMFTimerHelper.Data;
using DTwoMFTimerHelper.Resources;

namespace DTwoMFTimerHelper
{
    public class CreateCharacterForm : Form
    {
        private Label? lblCharacterName;
        private TextBox? txtCharacterName;
        private Label? lblCharacterClass;
        private ComboBox? cmbCharacterClass;
        private Button? btnConfirm;
        private Button? btnCancel;
        
        // 属性
        public string? CharacterName => txtCharacterName?.Text.Trim();
        public CharacterClass? SelectedClass {
            get {
                if (cmbCharacterClass?.SelectedItem is CharacterClass charClass)
                    return charClass;
                return null;
            }
        }
        
        public CreateCharacterForm()
        {
            InitializeComponent();
            UpdateUI();
        }
        
        private void InitializeComponent()
        {
            // 设置窗口属性
            this.Text = "创建角色档案";
            this.Size = new System.Drawing.Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            
            // 初始化控件
            lblCharacterName = new Label();
            txtCharacterName = new TextBox();
            lblCharacterClass = new Label();
            cmbCharacterClass = new ComboBox();
            btnConfirm = new Button();
            btnCancel = new Button();
            
            // 设置控件位置和大小
            lblCharacterName.Location = new System.Drawing.Point(50, 30);
            lblCharacterName.Size = new System.Drawing.Size(100, 25);
            
            txtCharacterName.Location = new System.Drawing.Point(150, 30);
            txtCharacterName.Size = new System.Drawing.Size(180, 25);
            
            lblCharacterClass.Location = new System.Drawing.Point(50, 70);
            lblCharacterClass.Size = new System.Drawing.Size(100, 25);
            
            cmbCharacterClass.Location = new System.Drawing.Point(150, 70);
            cmbCharacterClass.Size = new System.Drawing.Size(180, 25);
            cmbCharacterClass.DropDownStyle = ComboBoxStyle.DropDownList;
            
            // 添加职业选项
            foreach (CharacterClass charClass in Enum.GetValues(typeof(CharacterClass)))
            {
                cmbCharacterClass.Items.Add(charClass);
            }
            if (cmbCharacterClass.Items.Count > 0)
                cmbCharacterClass.SelectedIndex = 0;
            
            btnConfirm.Location = new System.Drawing.Point(120, 230);
            btnConfirm.Size = new System.Drawing.Size(80, 30);
            btnConfirm.Click += btnConfirm_Click;
            
            btnCancel.Location = new System.Drawing.Point(250, 230);
            btnCancel.Size = new System.Drawing.Size(80, 30);
            btnCancel.Click += btnCancel_Click;
            
            // 添加控件到表单
            this.Controls.Add(lblCharacterName);
            this.Controls.Add(txtCharacterName);
            this.Controls.Add(lblCharacterClass);
            this.Controls.Add(cmbCharacterClass);
            this.Controls.Add(btnConfirm);
            this.Controls.Add(btnCancel);
        }
        
        public void UpdateUI()
        {
            this.Text = LanguageManager.GetString("CreateCharacter") ?? "创建角色档案";
            lblCharacterName!.Text = LanguageManager.GetString("CharacterName") ?? "角色名称:";
            lblCharacterClass!.Text = LanguageManager.GetString("CharacterClass") ?? "职业:";
            btnConfirm!.Text = LanguageManager.GetString("Confirm") ?? "确认";
            btnCancel!.Text = LanguageManager.GetString("Cancel") ?? "取消";
            
            // 更新职业显示名称
            if (cmbCharacterClass != null && cmbCharacterClass.Items.Count > 0)
            {
                int selectedIndex = cmbCharacterClass.SelectedIndex;
                cmbCharacterClass.Items.Clear();
                
                // 添加本地化的职业名称
                foreach (CharacterClass charClass in Enum.GetValues(typeof(CharacterClass)))
                {
                    cmbCharacterClass.Items.Add(GetLocalizedClassName(charClass));
                }
                
                if (selectedIndex >= 0 && selectedIndex < cmbCharacterClass.Items.Count)
                    cmbCharacterClass.SelectedIndex = selectedIndex;
            }
        }
        
        private string GetLocalizedClassName(CharacterClass charClass)
        {
            switch (charClass)
            {
                case CharacterClass.Barbarian: return LanguageManager.GetString("Barbarian") ?? "野蛮人";
                case CharacterClass.Sorceress: return LanguageManager.GetString("Sorceress") ?? "法师";
                case CharacterClass.Assassin: return LanguageManager.GetString("Assassin") ?? "刺客";
                case CharacterClass.Druid: return LanguageManager.GetString("Druid") ?? "德鲁伊";
                case CharacterClass.Paladin: return LanguageManager.GetString("Paladin") ?? "圣骑士";
                case CharacterClass.Amazon: return LanguageManager.GetString("Amazon") ?? "亚马逊";
                case CharacterClass.Necromancer: return LanguageManager.GetString("Necromancer") ?? "死灵法师";
                default: return charClass.ToString();
            }
        }
        
        private CharacterClass GetCharacterClassFromLocalizedName(string localizedName)
        {
            // 反向映射
            foreach (CharacterClass charClass in Enum.GetValues(typeof(CharacterClass)))
            {
                if (GetLocalizedClassName(charClass).Equals(localizedName))
                    return charClass;
            }
            return CharacterClass.Barbarian; // 默认值
        }
        
        private void btnConfirm_Click(object? sender, EventArgs e)
        {
            // 验证输入
            if (string.IsNullOrEmpty(CharacterName))
            {
                MessageBox.Show(LanguageManager.GetString("EnterCharacterName") ?? "请输入角色名称", "提示");
                return;
            }
            
            // 检查角色是否已存在
            if (DataManager.FindProfileByName(CharacterName, true) != null)
            {
                MessageBox.Show(LanguageManager.GetString("CharacterExists") ?? "该角色名称已存在", "提示");
                return;
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
        private void btnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        // 重写SelectedClass以支持本地化
        public CharacterClass? GetSelectedClass()
        {
            if (cmbCharacterClass?.SelectedItem != null)
            {
                string localizedName = cmbCharacterClass.SelectedItem.ToString()!;
                return GetCharacterClassFromLocalizedName(localizedName);
            }
            return null;
        }
    }
}