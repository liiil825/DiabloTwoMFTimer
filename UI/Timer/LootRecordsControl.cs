using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.UI;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Timer
{
    public partial class LootRecordsControl : UserControl
    {
        private System.ComponentModel.IContainer components = null!;
        private Panel lootPanel = null!;
        private ListView lootListView = null!;
        private List<LootRecord> _lootRecords = [];
        private string _currentScene = string.Empty;

        // Loot记录属性
        public List<LootRecord> LootRecords
        {
            get { return _lootRecords; }
            set
            {
                _lootRecords = value;
                UpdateLootRecordsDisplay();
            }
        }

        // 当前场景属性，用于过滤掉落记录
        public string CurrentScene
        {
            get { return _currentScene; }
            set
            {
                _currentScene = value;
                UpdateLootRecordsDisplay();
            }
        }

        public LootRecordsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var height = UISizeConstants.LootRecordsControlHeight;
            components = new System.ComponentModel.Container();
            lootPanel = new Panel();
            lootListView = new ListView();
            lootPanel.SuspendLayout();
            SuspendLayout();
            //
            // lootPanel
            //
            lootPanel.Controls.Add(lootListView);
            lootPanel.Dock = DockStyle.Fill;
            lootPanel.Location = new Point(0, 0);
            lootPanel.Name = "lootPanel";
            lootPanel.Size = new Size(605, height);
            lootPanel.TabIndex = 0;
            //
            // lootListView
            //
            lootListView.Dock = DockStyle.Fill;
            lootListView.View = View.List;
            lootListView.LabelWrap = false;
            lootListView.Location = new Point(0, 0);
            lootListView.Name = "lootListView";
            lootListView.Size = new Size(605, height);
            lootListView.TabIndex = 0;
            lootListView.UseCompatibleStateImageBehavior = false;
            //
            // LootRecordsControl
            //
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lootPanel);
            Margin = new Padding(4);
            Name = "LootRecordsControl";
            Size = new Size(605, height);
            lootPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        public void UpdateLootRecords(List<LootRecord> lootRecords)
        {
            this._lootRecords = lootRecords;
            UpdateLootRecordsDisplay();
        }

        // 更新掉落记录并设置当前场景
        public void UpdateLootRecords(List<LootRecord> lootRecords, string currentScene)
        {
            this._lootRecords = lootRecords;
            this._currentScene = currentScene;
            UpdateLootRecordsDisplay();
        }

        private void UpdateLootRecordsDisplay()
        {
            // 清空现有项
            lootListView.Items.Clear();
            string pureEnglishCurrentScene = SceneHelper.GetEnglishSceneName(_currentScene);

            // 获取要显示的记录：如果指定了场景，则只显示该场景的记录，否则显示所有记录
            var recordsToDisplay = string.IsNullOrEmpty(_currentScene)
                ? _lootRecords
                : _lootRecords.Where(r => r.SceneName == pureEnglishCurrentScene);

            // 添加数据项，按掉落时间降序排列
            foreach (var record in recordsToDisplay.OrderByDescending(r => r.DropTime))
            {
                // 使用国际化格式字符串显示掉落记录，直接使用GetString的格式化功能
                string lootText = LanguageManager.GetString("RunNumber", record.RunCount, record.Name);
                lootListView.Items.Add(lootText);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
