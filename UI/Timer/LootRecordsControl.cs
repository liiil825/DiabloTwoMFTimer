using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer;

public partial class LootRecordsControl : UserControl
{
    private CharacterProfile? _currentProfile;
    private IProfileService _profileService = null!;
    private ISceneService _sceneService = null!;

    private List<LootRecord> _displayRecords = [];
    private string _currentScene = string.Empty;

    public event EventHandler? InteractionOccurred;

    public LootRecordsControl()
    {
        InitializeComponent();
    }

    public void Initialize(IProfileService profileService, ISceneService sceneService)
    {
        _profileService = profileService;
        _sceneService = sceneService;
    }

    private void GridLoot_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
    {
        if (_displayRecords == null || e.RowIndex >= _displayRecords.Count)
            return;

        var record = _displayRecords[e.RowIndex];
        switch (e.ColumnIndex)
        {
            case 0:
                e.Value = record.RunCount.ToString();
                break;
            case 1:
                e.Value = record.Name;
                break;
            case 2:
                e.Value = record.DropTime.ToString("MM-dd HH:mm");
                break;
        }
    }

    public void UpdateLootRecords(CharacterProfile? profile, string currentScene)
    {
        _currentProfile = profile;
        _currentScene = currentScene;

        if (_currentProfile == null)
        {
            _displayRecords = [];
            RefreshGrid();
            return;
        }

        string pureEnglishCurrentScene = _sceneService.GetEnglishSceneName(_currentScene);

        var query = string.IsNullOrEmpty(_currentScene)
            ? _currentProfile.LootRecords
            : _currentProfile.LootRecords.Where(r => r.SceneName == pureEnglishCurrentScene);

        // 修改 1: 改为升序排列 (Oldest -> Newest)
        _displayRecords = query.OrderBy(r => r.DropTime).ToList();

        RefreshGrid();
    }

    private void RefreshGrid()
    {
        gridLoot.SafeInvoke(() =>
        {
            gridLoot.ClearSelection();
            gridLoot.CurrentCell = null;
            gridLoot.RowCount = _displayRecords.Count;
            gridLoot.Invalidate();
        });
    }

    // 修改 2: 新增选中最后一行的方法 (用于添加新记录后)
    public void SelectLastRow()
    {
        // 检查是否有可见行
        if (!gridLoot.Visible || gridLoot.Height <= 0)
            return;

        gridLoot.SafeInvoke(() =>
        {
            if (gridLoot.RowCount > 0)
            {
                int lastIndex = gridLoot.RowCount - 1;
                // 确保滚动到最后一行
                gridLoot.FirstDisplayedScrollingRowIndex = lastIndex;
                gridLoot.ClearSelection();
                gridLoot.Rows[lastIndex].Selected = true;
                // 可选：将焦点设为该行，方便键盘操作
                gridLoot.CurrentCell = gridLoot.Rows[lastIndex].Cells[0];
            }
        });
    }

    public void ScrollToBottom()
    {
        // 检查是否有可见行
        if (!gridLoot.Visible || gridLoot.Height <= 0)
            return;

        gridLoot.SafeInvoke(() =>
        {
            if (gridLoot.RowCount > 0)
            {
                int displayCount = gridLoot.DisplayedRowCount(false);
                int firstVisible = gridLoot.RowCount - displayCount;
                if (firstVisible < 0)
                    firstVisible = 0;
                gridLoot.FirstDisplayedScrollingRowIndex = firstVisible;
            }
        });
    }

    public async Task<bool> DeleteSelectedLootAsync()
    {
        if (_currentProfile == null || gridLoot.SelectedRows.Count == 0)
            return false;

        int visualIndex = gridLoot.SelectedRows[0].Index;
        if (visualIndex < 0 || visualIndex >= _displayRecords.Count)
            return false;

        var recordToDelete = _displayRecords[visualIndex];
        bool removed = _currentProfile.LootRecords.Remove(recordToDelete);

        if (removed)
        {
            _profileService.SaveCurrentProfile();
            UpdateLootRecords(_currentProfile, _currentScene);
            return true;
        }
        return await Task.FromResult(false);
    }

    public void ClearSelection()
    {
        gridLoot.SafeInvoke(() =>
        {
            gridLoot.ClearSelection();
            gridLoot.CurrentCell = null;
        });
    }

    public bool HasFocus => gridLoot.ContainsFocus;
}
