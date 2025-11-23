using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.UI;

namespace DTwoMFTimerHelper.UI.Timer {
    public partial class LootRecordsControl : UserControl {
        private System.ComponentModel.IContainer components;
        private Panel lootPanel;
        private ListView lootListView;
        private List<LootRecord> _lootRecords = new List<LootRecord>();

        // Loot记录属性
        public List<LootRecord> LootRecords {
            get { return _lootRecords; }
            set {
                _lootRecords = value;
                UpdateLootRecordsDisplay();
            }
        }

        public LootRecordsControl() {
            InitializeComponent();
        }

        private void InitializeComponent() {
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

        public void UpdateLootRecords(List<LootRecord> lootRecords) {
            this._lootRecords = lootRecords;
            UpdateLootRecordsDisplay();
        }

        private void UpdateLootRecordsDisplay() {
            // 清空现有项
            lootListView.Items.Clear();
            
            // 添加数据项，按掉落时间降序排列
            foreach (var record in _lootRecords.OrderByDescending(r => r.DropTime)) {
                lootListView.Items.Add($"第{record.RunCount}次:{record.Name}");
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}