using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Utils;
using DiabloTwoMFTimer.Models; // 引用消息命名空间
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Settings;

public partial class HotkeySettingsControl : UserControl
{
    private readonly Color ColorNormal = AppTheme.SurfaceColor;
    private readonly Color ColorEditing = Color.FromArgb(60, 60, 70);

    private bool _isUpdating = false;
    private IMessenger? _messenger; // 新增 Messenger 引用

    public Keys StartOrNextRunHotkey { get; private set; }
    public Keys PauseHotkey { get; private set; }
    public Keys DeleteHistoryHotkey { get; private set; }
    public Keys RecordLootHotkey { get; private set; }

    public HotkeySettingsControl()
    {
        InitializeComponent();
        InitializeTextBoxStyles();
    }

    // 依赖注入方法
    public void SetMessenger(IMessenger messenger)
    {
        _messenger = messenger;
    }

    private void InitializeTextBoxStyles()
    {
        ApplyStyle(txtStartNext);
        ApplyStyle(txtPause);
        ApplyStyle(txtDeleteHistory);
        ApplyStyle(txtRecordLoot);
    }

    private void ApplyStyle(TextBox tb)
    {
        tb.BackColor = ColorNormal;
        tb.ForeColor = AppTheme.TextColor;
        tb.BorderStyle = BorderStyle.FixedSingle;
    }

    private void OnTextBoxEnter(object? sender, EventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        if (_isUpdating)
        {
            _isUpdating = false;
            return;
        }

        // 进入编辑模式：暂停全局热键
        _messenger?.Publish(new SuspendHotkeysMessage());

        textBox.BackColor = ColorEditing;
        textBox.ForeColor = AppTheme.AccentColor;
        textBox.Text = LanguageManager.GetString("HotkeyPressToSet") ?? "请按快捷键 (Esc取消)";
    }

    private void OnTextBoxLeave(object? sender, EventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        // 离开编辑模式：恢复全局热键
        _messenger?.Publish(new ResumeHotkeysMessage());

        textBox.BackColor = ColorNormal;
        textBox.ForeColor = AppTheme.TextColor;

        // 还原显示当前保存的键值 (如果是Esc取消，这里会自动还原)
        string tag = textBox.Tag?.ToString() ?? "";
        Keys currentKey = Keys.None;
        switch (tag)
        {
            case "StartNext":
                currentKey = StartOrNextRunHotkey;
                break;
            case "Pause":
                currentKey = PauseHotkey;
                break;
            case "Delete":
                currentKey = DeleteHistoryHotkey;
                break;
            case "Record":
                currentKey = RecordLootHotkey;
                break;
        }
        textBox.Text = FormatKeyString(currentKey);
        _isUpdating = false;
    }

    private void OnHotkeyInput(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        e.SuppressKeyPress = true;

        // Esc: 取消 (通过转移焦点触发 Leave 事件来还原)
        if (e.KeyCode == Keys.Escape)
        {
            // 关键修复：将焦点移交给父容器（如 GroupBox 或 TabPage），强制 TextBox 失去焦点
            this.Parent?.Focus();
            return;
        }

        // Back/Delete: 清除热键
        if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
        {
            _isUpdating = true;
            UpdateHotkey(textBox, Keys.None);
            this.Parent?.Focus(); // 清除后也自动退出编辑
            return;
        }

        // 忽略单一控制键
        if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu)
        {
            return;
        }

        Keys keyData = e.KeyCode;
        if (e.Control) keyData |= Keys.Control;
        if (e.Shift) keyData |= Keys.Shift;
        if (e.Alt) keyData |= Keys.Alt;

        _isUpdating = true;
        UpdateHotkey(textBox, keyData);

        // 设置完成后自动退出编辑
        this.Parent?.Focus();
    }

    private void UpdateHotkey(TextBox textBox, Keys newKey)
    {
        string tag = textBox.Tag?.ToString() ?? "";
        switch (tag)
        {
            case "StartNext":
                StartOrNextRunHotkey = newKey;
                break;
            case "Pause":
                PauseHotkey = newKey;
                break;
            case "Delete":
                DeleteHistoryHotkey = newKey;
                break;
            case "Record":
                RecordLootHotkey = newKey;
                break;
        }
        textBox.Text = FormatKeyString(newKey);
        textBox.BackColor = ColorNormal;
        textBox.ForeColor = AppTheme.TextColor;
    }

    private string FormatKeyString(Keys key)
    {
        if (key == Keys.None)
            return "无 (None)";
        var converter = new KeysConverter();
        return converter.ConvertToString(key) ?? "None";
    }

    public void LoadHotkeys(IAppSettings settings)
    {
        StartOrNextRunHotkey = settings.HotkeyStartOrNext;
        PauseHotkey = settings.HotkeyPause;
        DeleteHistoryHotkey = settings.HotkeyDeleteHistory;
        RecordLootHotkey = settings.HotkeyRecordLoot;

        txtStartNext.Text = FormatKeyString(StartOrNextRunHotkey);
        txtPause.Text = FormatKeyString(PauseHotkey);
        txtDeleteHistory.Text = FormatKeyString(DeleteHistoryHotkey);
        txtRecordLoot.Text = FormatKeyString(RecordLootHotkey);
    }

    public void RefreshUI()
    {
        this.SafeInvoke(() =>
        {
            if (grpHotkeys == null)
                return;
            try
            {
                grpHotkeys.Text = LanguageManager.GetString("HotkeySettingsGroup");
                lblStartNext.Text = LanguageManager.GetString("HotkeyStartNext");
                lblPause.Text = LanguageManager.GetString("HotkeyPause");
                lblDeleteHistory.Text = LanguageManager.GetString("HotkeyDeleteHistory");
                lblRecordLoot.Text = LanguageManager.GetString("HotkeyRecordLoot");

                LoadHotkeys(
                    new Services.AppSettings
                    {
                        HotkeyStartOrNext = StartOrNextRunHotkey,
                        HotkeyPause = PauseHotkey,
                        HotkeyDeleteHistory = DeleteHistoryHotkey,
                        HotkeyRecordLoot = RecordLootHotkey,
                    }
                );
            }
            catch { }
        });
    }
}