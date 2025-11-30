#nullable disable

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Components;

namespace DiabloTwoMFTimer.UI.Timer
{
    partial class HistoryControl
    {

        private DataGridView gridRunHistory;

        private void InitializeComponent()
        {
            this.gridRunHistory = new ThemedDataGridView();

            // 【关键】当网格被点击或获得焦点时，触发交互事件
            this.gridRunHistory.Enter += (s, e) => InteractionOccurred?.Invoke(this, EventArgs.Empty);
            this.gridRunHistory.Click += (s, e) => InteractionOccurred?.Invoke(this, EventArgs.Empty);

            this.gridRunHistory.CellValueNeeded += GridRunHistory_CellValueNeeded;

            // ... 列配置保持不变 ...
            DataGridViewTextBoxColumn colIndex = new DataGridViewTextBoxColumn();
            colIndex.HeaderText = "#";
            colIndex.Width = 50;
            colIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewTextBoxColumn colTime = new DataGridViewTextBoxColumn();
            colTime.HeaderText = "Time";
            colTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.gridRunHistory.Columns.AddRange(new DataGridViewColumn[] { colIndex, colTime });

            this.AutoScaleDimensions = new SizeF(9F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.gridRunHistory);
            this.Name = "HistoryControl";
            this.Size = new Size(290, 90);

            ((System.ComponentModel.ISupportInitialize)(this.gridRunHistory)).EndInit();
            this.ResumeLayout(false);
        }


    }
}