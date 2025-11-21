using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;

namespace DTwoMFTimerHelper.UI.Timer {
    public partial class HistoryControl : UserControl {
        private ListBox? lstRunHistory;

        // 历史记录服务
        private readonly ITimerHistoryService? _historyService;

        // 分页相关变量
        private const int PageSize = 20; // 每页显示20条记录
        private int _displayStartIndex = 0; // 当前显示的起始索引
        private bool _isLoading = false; // 是否正在加载数据
        private Label? _loadingIndicator; // 加载状态指示器

        // 新增：防止重复处理事件的标志
        private bool _isProcessingHistoryChange = false;

        private CharacterProfile? _currentProfile = null;
        private string? _currentCharacterName = null;
        private string? _currentScene = null;
        private GameDifficulty _currentDifficulty = GameDifficulty.Hell;

        public int RunCount => _historyService?.RunCount ?? 0;
        public TimeSpan FastestTime => _historyService?.FastestTime ?? TimeSpan.Zero;
        public TimeSpan AverageTime => _historyService?.AverageTime ?? TimeSpan.Zero;
        public List<TimeSpan> RunHistory => _historyService?.RunHistory ?? [];

        public HistoryControl() {
            InitializeComponent();
        }
        public HistoryControl(ITimerHistoryService historyService) : this() {
            _historyService = historyService;

            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
            _historyService.HistoryDataChanged += OnHistoryDataChanged;
        }

        /// <summary>
        /// 添加单条运行记录
        /// </summary>
        public void AddRunRecord(TimeSpan runTime) {
            if (_historyService == null) return;
            _historyService.AddRunRecord(runTime);
        }

        /// <summary>
        /// 更新历史记录
        /// </summary>
        public void UpdateHistory(List<TimeSpan> runHistory) {
            if (_historyService == null) return;
            _historyService.UpdateHistory(runHistory);
        }

        /// <summary>
        /// 删除选中的运行记录
        /// </summary>
        public async Task<bool> DeleteSelectedRecordAsync() {
            if (_historyService == null) return false;

            if (lstRunHistory == null || lstRunHistory.SelectedIndex == -1 || _currentProfile == null)
                return false;

            try {
                _isLoading = true;
                // 获取实际的历史记录索引
                int actualIndex = _displayStartIndex + lstRunHistory.SelectedIndex;

                // 检查索引是否有效
                if (actualIndex >= 0 && actualIndex < _historyService.RunHistory.Count && _currentScene != null) {
                    // 直接调用新方法，根据索引删除记录
                    bool deleteSuccess = _historyService.DeleteHistoryRecordByIndex(
                        _currentProfile,
                        _currentScene,
                        _currentDifficulty,
                        actualIndex);

                    if (deleteSuccess) {
                        // 保存到YAML文件 - SaveProfile方法只需要profile参数
                        Services.DataService.SaveProfile(_currentProfile);

                        // 更新显示起始索引，确保它在有效范围内
                        _displayStartIndex = Math.Max(0, Math.Min(_displayStartIndex, _historyService.RunHistory.Count - 1));

                        // 重置加载状态，确保UI能更新
                        _isLoading = false;
                        await UpdateUIAsync();
                        return true;
                    }
                }
                return false;
            }
            finally {
                _isLoading = false;
            }
        }

        /// <summary>
        /// 从角色档案加载特定场景的历史数据（异步分页加载）
        /// </summary>
        public async Task<bool> LoadProfileHistoryDataAsync(
            CharacterProfile? profile, string scene, string characterName, GameDifficulty difficulty) {
            if (_historyService == null) return false;

            LogManager.WriteDebugLog("HistoryControl", $"开始LoadProfileHistoryDataAsync - 场景: {scene}");

            // 设置加载状态
            _isLoading = true;
            if (_loadingIndicator != null) {
                if (InvokeRequired) {
                    Invoke(new Action(() => {
                        _loadingIndicator.Visible = true;
                    }));
                }
                else {
                    _loadingIndicator.Visible = true;
                }
            }

            try {
                // 保存当前配置信息，用于后续保存操作
                _currentProfile = profile;
                _currentCharacterName = characterName;
                _currentScene = scene;
                _currentDifficulty = difficulty;

                bool result = _historyService.LoadProfileHistoryData(profile, scene, characterName, difficulty);
                if (result) {
                    // 重置显示起始索引，初始只显示最近的记录
                    _displayStartIndex = Math.Max(0, _historyService.RunHistory.Count - PageSize);
                    LogManager.WriteDebugLog("HistoryControl", $"LoadProfileHistoryDataAsync - 设置显示起始索引: {_displayStartIndex}");
                    await UpdateUIAsync();
                }
                LogManager.WriteDebugLog("HistoryControl", $"LoadProfileHistoryDataAsync完成 - 结果: {result}");
                return result;
            }
            finally {
                // 隐藏加载指示器
                _isLoading = false;
                if (_loadingIndicator != null) {
                    if (InvokeRequired) {
                        Invoke(new Action(() => {
                            _loadingIndicator.Visible = false;
                        }));
                    }
                    else {
                        _loadingIndicator.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// 同步版本的加载方法，供需要立即执行的场景使用
        /// </summary>
        public bool LoadProfileHistoryData(CharacterProfile? profile, string scene, string characterName, GameDifficulty difficulty) {
            if (_historyService == null) return false;

            LogManager.WriteDebugLog("HistoryControl", $"开始LoadProfileHistoryData - 场景: {scene}");

            // 保存当前配置信息，用于后续保存操作
            _currentProfile = profile;
            _currentCharacterName = characterName;
            _currentScene = scene;
            _currentDifficulty = difficulty;

            bool result = _historyService.LoadProfileHistoryData(profile, scene, characterName, difficulty);
            if (result) {
                // 重置显示起始索引，初始只显示最近的记录
                _displayStartIndex = Math.Max(0, _historyService.RunHistory.Count - PageSize);
                LogManager.WriteDebugLog("HistoryControl", $"LoadProfileHistoryData - 设置显示起始索引: {_displayStartIndex}");

                // 使用异步更新，但同步等待完成
                UpdateUIAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            LogManager.WriteDebugLog("HistoryControl", $"LoadProfileHistoryData完成 - 结果: {result}");
            return result;
        }

        /// <summary>
        /// 异步更新UI显示
        /// </summary>
        private async Task UpdateUIAsync() {
            if (_historyService == null) return;

            if (lstRunHistory == null || _historyService == null)
                return;

            // 防止重复更新
            if (_isLoading) {
                LogManager.WriteDebugLog("HistoryControl", "正在加载中，跳过UpdateUIAsync");
                return;
            }

            _isLoading = true;

            try {
                LogManager.WriteDebugLog("HistoryControl", $"开始UpdateUIAsync - 当前列表项数: {lstRunHistory.Items.Count}, 显示起始索引: {_displayStartIndex}");

                // 在UI线程清空列表
                if (InvokeRequired) {
                    Invoke(new Action(() => {
                        lstRunHistory.Items.Clear();
                    }));
                }
                else {
                    lstRunHistory.Items.Clear();
                }

                LogManager.WriteDebugLog("HistoryControl", $"清空后列表项数: {lstRunHistory.Items.Count}");

                // 获取要显示的数据范围
                var currentHistory = _historyService.RunHistory;
                if (currentHistory == null) {
                    LogManager.WriteDebugLog("HistoryControl", "currentHistory为null");
                    return;
                }

                LogManager.WriteDebugLog("HistoryControl", $"RunHistory总记录数: {currentHistory.Count}");

                // 确保_displayStartIndex在有效范围内
                _displayStartIndex = Math.Max(0, Math.Min(_displayStartIndex, currentHistory.Count - 1));
                int displayCount = Math.Min(currentHistory.Count - _displayStartIndex, PageSize);

                LogManager.WriteDebugLog("HistoryControl", $"显示范围: 起始索引={_displayStartIndex}, 显示数量={displayCount}");

                // 异步处理数据格式化
                var itemsToAdd = await Task.Run(() => {
                    var items = new List<string>(displayCount);
                    for (int i = _displayStartIndex; i < _displayStartIndex + displayCount; i++) {
                        if (i >= 0 && i < currentHistory.Count) {
                            var time = currentHistory[i];
                            string timeFormatted = FormatTime(time);
                            string runText = GetRunText(i + 1, timeFormatted);
                            items.Add(runText);
                            LogManager.WriteDebugLog("HistoryControl", $"格式化记录 #{i + 1}: {runText}");
                        }
                    }
                    return items;
                });

                LogManager.WriteDebugLog("HistoryControl", $"准备添加 {itemsToAdd.Count} 项到列表");

                // 在UI线程添加项目
                if (InvokeRequired) {
                    Invoke(new Action(() => {
                        foreach (var item in itemsToAdd) {
                            lstRunHistory.Items.Add(item);
                        }
                        LogManager.WriteDebugLog("HistoryControl", $"添加后列表项数: {lstRunHistory.Items.Count}");

                        // 滚动到最新记录
                        if (lstRunHistory.Items.Count > 0) {
                            lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
                            lstRunHistory.TopIndex = Math.Max(0, lstRunHistory.Items.Count - 1);
                        }
                    }));
                }
                else {
                    foreach (var item in itemsToAdd) {
                        lstRunHistory.Items.Add(item);
                    }
                    LogManager.WriteDebugLog("HistoryControl", $"添加后列表项数: {lstRunHistory.Items.Count}");

                    // 滚动到最新记录
                    if (lstRunHistory.Items.Count > 0) {
                        lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
                        lstRunHistory.TopIndex = Math.Max(0, lstRunHistory.Items.Count - 1);
                    }
                }

                LogManager.WriteDebugLog("HistoryControl", "UpdateUIAsync完成");
            }
            finally {
                _isLoading = false;
            }
        }

        /// <summary>
        /// 只更新UI显示，不重新加载数据
        /// </summary>
        public async Task RefreshUIAsync() {
            if (_historyService == null) return;

            if (lstRunHistory == null || _historyService == null)
                return;

            await Task.Run(() => {
                // 只更新现有项目的文本（用于语言切换等）
                var currentHistory = _historyService.RunHistory;
                if (currentHistory == null)
                    return;

                if (InvokeRequired) {
                    Invoke(new Action(() => {
                        UpdateListItems(currentHistory);
                    }));
                }
                else {
                    UpdateListItems(currentHistory);
                }
            });
        }

        private void UpdateListItems(List<TimeSpan> currentHistory) {
            if (_historyService == null || lstRunHistory == null)
                return;

            for (int i = 0; i < lstRunHistory.Items.Count; i++) {
                int actualIndex = _displayStartIndex + i;
                if (actualIndex < currentHistory.Count) {
                    var time = currentHistory[actualIndex];
                    string timeFormatted = FormatTime(time);
                    string runText = GetRunText(actualIndex + 1, timeFormatted);
                    lstRunHistory.Items[i] = runText;
                }
            }
        }

        // 保留原始方法以保持向后兼容性
        public void RefreshUI() {
            _ = RefreshUIAsync();
        }

        /// <summary>
        /// 添加单条记录 - 只添加不刷新（类似React的单项更新）
        /// </summary>
        private void AddSingleRunRecord(TimeSpan runTime) {
            if (_historyService == null || lstRunHistory == null)
                return;

            // 获取新记录的索引
            var currentHistory = _historyService.RunHistory;
            if (currentHistory == null)
                return;

            int newIndex = currentHistory.Count - 1;

            // 确保显示最新的记录
            _displayStartIndex = Math.Max(0, newIndex - PageSize + 1);

            // 格式化时间
            string timeFormatted = FormatTime(runTime);
            string runText = GetRunText(newIndex + 1, timeFormatted);

            // 如果列表已满，移除最旧的记录
            if (lstRunHistory.Items.Count >= PageSize) {
                lstRunHistory.Items.RemoveAt(0);
            }

            // 添加新记录到列表
            lstRunHistory.Items.Add(runText);

            // 滚动到最新记录
            lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
            lstRunHistory.TopIndex = Math.Max(0, lstRunHistory.Items.Count - 1);
        }

        /// <summary>
        /// 滚动事件处理，用于异步加载更多历史记录
        /// </summary>
        private async void LstRunHistory_MouseWheel(object? sender, MouseEventArgs e) {
            if (_historyService == null || lstRunHistory == null)
                return;

            // 检查是否需要加载更多历史记录
            // 当向上滚动（e.Delta > 0）并且接近列表顶部时加载更多
            if (e != null && e.Delta > 0 && _displayStartIndex > 0 && !_isLoading && lstRunHistory != null && lstRunHistory.TopIndex < 5) {
                await LoadMoreHistoryAsync();
            }
        }

        /// <summary>
        /// 异步加载更多历史记录（向上加载）
        /// </summary>
        private async Task LoadMoreHistoryAsync() {
            if (_historyService == null || lstRunHistory == null || _loadingIndicator == null || _displayStartIndex <= 0 || _isLoading)
                return;

            _isLoading = true;

            // 在UI线程显示加载指示器
            if (InvokeRequired) {
                Invoke(new Action(() => {
                    if (_loadingIndicator != null)
                        _loadingIndicator.Visible = true;
                }));
            }
            else {
                _loadingIndicator.Visible = true;
            }

            try {
                // 计算要加载的记录范围
                int newStartIndex = Math.Max(0, _displayStartIndex - PageSize);
                int addedCount = _displayStartIndex - newStartIndex;

                // 获取当前历史记录的副本
                var currentHistory = _historyService.RunHistory;
                if (currentHistory == null || currentHistory.Count == 0)
                    return;

                // 异步处理数据格式化
                var newItems = await Task.Run(() => {
                    var items = new List<string>(addedCount);
                    for (int i = newStartIndex; i < _displayStartIndex; i++) {
                        if (i >= 0 && i < currentHistory.Count) {
                            var time = currentHistory[i];
                            string timeFormatted = FormatTime(time);
                            string runText = GetRunText(i + 1, timeFormatted);
                            items.Add(runText);
                        }
                    }
                    return items;
                });

                // 如果新加载的项目为空，则直接返回
                if (newItems.Count == 0)
                    return;

                // 更新起始索引
                _displayStartIndex = newStartIndex;

                // 保存当前显示的项
                var currentDisplayItems = new List<string>();
                foreach (var item in lstRunHistory.Items) {
                    currentDisplayItems.Add(item?.ToString() ?? string.Empty);
                }

                // 清空列表并重新填充
                lstRunHistory.Items.Clear();

                // 添加新加载的历史记录（较早的记录）
                foreach (var item in newItems) {
                    lstRunHistory.Items.Add(item);
                }

                // 添加之前显示的记录（较新的记录）
                foreach (var item in currentDisplayItems) {
                    lstRunHistory.Items.Add(item);
                }

                // 调整滚动位置，让用户看到新加载的内容
                if (newItems.Count > 0) {
                    lstRunHistory.TopIndex = 0; // 滚动到顶部显示新加载的内容
                    lstRunHistory.SelectedIndex = -1; // 取消选择
                }
            }
            finally {
                _isLoading = false;

                // 在UI线程隐藏加载指示器
                if (InvokeRequired) {
                    Invoke(new Action(() => {
                        _loadingIndicator.Visible = false;
                    }));
                }
                else {
                    _loadingIndicator.Visible = false;
                }
            }
        }

        private string FormatTime(TimeSpan time) {
            return string.Format("{0:00}:{1:00}:{2:00}.{3}",
                time.Hours, time.Minutes, time.Seconds,
                (int)(time.Milliseconds / 100));
        }

        private string GetRunText(int runNumber, string timeFormatted) {
            string runText = Utils.LanguageManager.GetString("RunNumber", runNumber, timeFormatted);
            if (string.IsNullOrEmpty(runText) || runText == "RunNumber") {
                runText = $"Run {runNumber}: {timeFormatted}";
            }
            return runText;
        }

        /// <summary>
        /// 处理历史数据变更事件 - 根据变更类型决定如何更新UI
        /// </summary>
        private void OnHistoryDataChanged(object? sender, HistoryChangedEventArgs e) {
            // 确保e不为null
            if (e == null)
                return;

            if (InvokeRequired) {
                // 使用Action<T>确保参数传递正确
                BeginInvoke(new Action<HistoryChangedEventArgs>(args => {
                    ProcessHistoryChange(args);
                }), e);
            }
            else {
                ProcessHistoryChange(e);
            }
        }

        /// <summary>
        /// 处理历史记录变更
        /// </summary>
        private async void ProcessHistoryChange(HistoryChangedEventArgs e) {
            // 确保e不为null
            if (e == null)
                return;

            // 防止重复处理
            if (_isProcessingHistoryChange) {
                LogManager.WriteDebugLog("HistoryControl", "正在处理历史变更，跳过重复调用");
                return;
            }

            _isProcessingHistoryChange = true;

            try {
                LogManager.WriteDebugLog("HistoryControl", $"ProcessHistoryChange - 变更类型: {e.ChangeType}, 新增记录: {e.AddedRecord}");

                switch (e.ChangeType) {
                    case HistoryChangeType.Add:
                        // 只添加单条记录，不刷新整个列表
                        if (e.AddedRecord.HasValue) {
                            LogManager.WriteDebugLog("HistoryControl", "执行单条记录添加");
                            AddSingleRunRecord(e.AddedRecord.Value);
                        }
                        break;
                    case HistoryChangeType.FullRefresh:
                    default:
                        LogManager.WriteDebugLog("HistoryControl", "执行全量刷新");
                        // 确保重置加载状态
                        _isLoading = false;

                        // 异步全量刷新UI，显示加载状态
                        if (_loadingIndicator != null) {
                            if (InvokeRequired) {
                                Invoke(new Action(() => {
                                    _loadingIndicator.Visible = true;
                                }));
                            }
                            else {
                                _loadingIndicator.Visible = true;
                            }
                        }

                        try {
                            if (_historyService != null && _historyService.RunHistory != null) {
                                _displayStartIndex = Math.Max(0, _historyService.RunHistory.Count - PageSize);
                                LogManager.WriteDebugLog("HistoryControl", $"全量刷新 - 设置显示起始索引: {_displayStartIndex}");

                                // 统一使用异步更新
                                await UpdateUIAsync();
                            }
                        }
                        finally {
                            // 隐藏加载指示器
                            if (_loadingIndicator != null) {
                                if (InvokeRequired) {
                                    Invoke(new Action(() => {
                                        _loadingIndicator.Visible = false;
                                    }));
                                }
                                else {
                                    _loadingIndicator.Visible = false;
                                }
                            }
                        }
                        break;
                }
            }
            finally {
                _isProcessingHistoryChange = false;
            }
        }

        private void InitializeComponent() {
            // 历史记录列表
            lstRunHistory = new ListBox {
                FormattingEnabled = true,
                ItemHeight = 15,
                Location = new Point(0, 0),
                Name = "lstRunHistory",
                Size = new Size(290, 90),
                TabIndex = 0,
                HorizontalScrollbar = true
            };
            // 创建并初始化加载指示器
            _loadingIndicator = new Label {
                Text = "加载中...",
                AutoSize = true,
                Visible = false,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            Controls.Add(_loadingIndicator);

            // 添加鼠标滚轮事件来检测滚动行为
            if (lstRunHistory != null) {
                lstRunHistory.MouseWheel += LstRunHistory_MouseWheel;
            }

            // 添加到控件
            Controls.Add(lstRunHistory);

            // 控件设置
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Size = new Size(290, 90);
            Name = "HistoryControl";
        }

        private async void LanguageManager_OnLanguageChanged(object? sender, EventArgs e) {
            // 确保e不为null
            if (e == null)
                return;

            await RefreshUIAsync(); // 只刷新显示文本，不重新加载数据
        }

        protected override void Dispose(bool disposing) {

            if (disposing) {
                // 安全地移除事件订阅
                LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;

                _historyService.HistoryDataChanged -= OnHistoryDataChanged;

                // 移除鼠标滚轮事件订阅
                if (lstRunHistory != null) {
                    lstRunHistory.MouseWheel -= LstRunHistory_MouseWheel;
                }
            }
            base.Dispose(disposing);
        }
    }
}