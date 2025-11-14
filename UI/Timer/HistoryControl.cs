using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;

namespace DTwoMFTimerHelper.UI.Timer
{
    public partial class HistoryControl : UserControl
    {
        private ListBox? lstRunHistory;

        // 历史记录服务
        private readonly TimerHistoryService _historyService;
        
        // 历史记录统计信息（通过服务暴露）
        public int RunCount => _historyService.RunCount;
        public TimeSpan FastestTime => _historyService.FastestTime;
        public TimeSpan AverageTime => _historyService.AverageTime;
        public List<TimeSpan> RunHistory => _historyService.RunHistory;

        public HistoryControl()
        {            
            InitializeComponent();
            // 使用历史记录服务的单例实例
            _historyService = TimerHistoryService.Instance;
            // 注册语言变更事件
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
        }
        
        /// <summary>
        /// 从角色档案加载特定场景的历史数据
        /// </summary>
        /// <param name="profile">角色档案</param>
        /// <param name="scene">场景名称</param>
        /// <param name="characterName">角色名称</param>
        /// <param name="difficulty">游戏难度</param>
        /// <returns>是否成功加载历史数据</returns>
        public bool LoadProfileHistoryData(CharacterProfile? profile, string scene, string characterName, GameDifficulty difficulty)
        {   
            bool result = _historyService.LoadProfileHistoryData(profile, scene, characterName, difficulty);
            
            // 如果加载成功，更新UI
            if (result)
            {
                UpdateUI();
            }
            
            return result;
        }
        
        /// <summary>
        /// 重置历史数据
        /// </summary>
        private void ResetHistoryData()
        {   
            _historyService.ResetHistoryData();
        }
        
        public void UpdateUI()
        {   // 更新历史记录列表
            if (lstRunHistory != null)
            {
                lstRunHistory.Items.Clear();
                for (int i = 0; i < RunHistory.Count; i++)
                {
                    var time = RunHistory[i];
                    string timeFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}",
                        time.Hours, time.Minutes, time.Seconds,
                        (int)(time.Milliseconds / 100));

                    string runText = Utils.LanguageManager.GetString("RunNumber", i + 1, timeFormatted);
                    if (string.IsNullOrEmpty(runText) || runText == "RunNumber")
                    {
                        runText = $"Run {i + 1}: {timeFormatted}";
                    }
                    lstRunHistory.Items.Add(runText);
                }

                // 确保选中最后一条记录并滚动到底部
                if (lstRunHistory.Items.Count > 0)
                {
                    lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
                    lstRunHistory.TopIndex = lstRunHistory.Items.Count - 1;
                }
            }
        }
        
        protected override void Dispose(bool disposing)
        {   
            if (disposing)
            {
                // 取消注册语言变更事件
                Utils.LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // 历史记录列表
            lstRunHistory = new ListBox
            {
                FormattingEnabled = true,
                ItemHeight = 15,
                Location = new Point(0, 0),
                Name = "lstRunHistory",
                Size = new Size(290, 90),
                TabIndex = 0,
                Parent = this
            };

            // 控件设置
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Size = new Size(290, 90);
            Name = "HistoryControl";
        }

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        public void UpdateHistory(List<TimeSpan> runHistory)
        {   
            // 使用服务更新历史记录数据
            _historyService.UpdateHistory(runHistory);
            
            // 更新UI
            UpdateUI();
        }
        
        /// <summary>
        /// 添加新的运行记录
        /// </summary>
        /// <param name="runTime">运行时间</param>
        public void AddRunRecord(TimeSpan runTime)
        {            // 使用服务添加运行记录
            _historyService.AddRunRecord(runTime);
            
            // 更新UI
            UpdateUI();
        }
    }
}