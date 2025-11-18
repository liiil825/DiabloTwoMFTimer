using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Timer
{
    public partial class StatisticsControl : UserControl
    {
        private Label? lblRunCount;
        private Label? lblFastestTime;
        private Label? lblAverageTime;

        // 统计数据属性
        public int RunCount
        {
            get; set;
        }
        public TimeSpan FastestTime
        {
            get; set;
        }
        public TimeSpan AverageTime
        {
            get; set;
        }

        public StatisticsControl()
        {
            InitializeComponent();
            // 注册语言变更事件
            Utils.LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
        }

        private void InitializeComponent()
        {
            // 运行次数显示
            lblRunCount = new Label
            {
                AutoSize = false,
                Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134))),
                Location = new Point(0, 0),
                Name = "lblRunCount",
                Size = new Size(290, 21),
                TextAlign = ContentAlignment.MiddleLeft,
                TabIndex = 0,
                Text = "--- Run count 0 (0) ---",
                Parent = this
            };

            // 最快时间显示
            lblFastestTime = new Label
            {
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134))),
                Location = new Point(0, 25),
                Name = "lblFastestTime",
                Size = new Size(120, 19),
                TabIndex = 1,
                Text = "Fastest time: --:--:--.-",
                Parent = this
            };

            // 平均时间显示
            lblAverageTime = new Label
            {
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134))),
                Location = new Point(0, 45),
                Name = "lblAverageTime",
                Size = new Size(125, 19),
                TabIndex = 2,
                Text = "Average time: --:--:--.-",
                Parent = this
            };

            // 控件设置
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Size = new Size(290, 75);
            Name = "StatisticsControl";
        }

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        public void UpdateStatistics(int runCount, TimeSpan fastestTime, List<TimeSpan> runHistory)
        {
            // 更新属性
            this.RunCount = runCount;
            this.FastestTime = fastestTime;

            // 计算平均时间
            if (runCount > 0 && runHistory.Count > 0)
            {
                TimeSpan totalTime = TimeSpan.Zero;
                foreach (var time in runHistory)
                {
                    totalTime += time;
                }
                this.AverageTime = new TimeSpan(totalTime.Ticks / runHistory.Count);
            }
            else
            {
                this.AverageTime = TimeSpan.Zero;
            }

            // 更新UI
            UpdateUI();
        }

        public void UpdateUI()
        {
            // 更新统计信息
            if (lblRunCount != null)
            {
                // 使用多语言显示运行次数
                string runCountText = Utils.LanguageManager.GetString("RunCount", RunCount, RunCount);
                if (string.IsNullOrEmpty(runCountText) || runCountText == "RunCount")
                {
                    // 如果未找到翻译，使用默认格式
                    runCountText = $"--- 运行次数 {RunCount} ({RunCount}) ---";
                }
                else
                {
                    runCountText = $"--- {runCountText} ---";
                }
                lblRunCount.Text = runCountText;
            }

            if (lblFastestTime != null)
            {
                if (RunCount > 0 && FastestTime != TimeSpan.MaxValue && FastestTime > TimeSpan.Zero)
                {
                    string fastestFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}",
                        FastestTime.Hours, FastestTime.Minutes, FastestTime.Seconds,
                        (int)(FastestTime.Milliseconds / 100));

                    string fastestTimeText = Utils.LanguageManager.GetString("FastestTime", fastestFormatted);
                    if (string.IsNullOrEmpty(fastestTimeText) || fastestTimeText == "FastestTime")
                    {
                        fastestTimeText = $"最快时间: {fastestFormatted}";
                    }

                    lblFastestTime.Text = fastestTimeText;
                }
                else
                {
                    string fastestTimeText = Utils.LanguageManager.GetString("FastestTimePlaceholder");
                    if (string.IsNullOrEmpty(fastestTimeText) || fastestTimeText == "FastestTimePlaceholder")
                    {
                        fastestTimeText = "最快时间: --:--:--.-";
                    }

                    lblFastestTime.Text = fastestTimeText;
                }
            }

            if (lblAverageTime != null)
            {
                if (RunCount > 0)
                {
                    string averageFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}",
                        AverageTime.Hours, AverageTime.Minutes, AverageTime.Seconds,
                        (int)(AverageTime.Milliseconds / 100));

                    string averageTimeText = Utils.LanguageManager.GetString("AverageTime", averageFormatted);
                    if (string.IsNullOrEmpty(averageTimeText) || averageTimeText == "AverageTime")
                    {
                        averageTimeText = $"平均时间: {averageFormatted}";
                    }

                    lblAverageTime.Text = averageTimeText;
                }
                else
                {
                    string averageTimeText = Utils.LanguageManager.GetString("AverageTimePlaceholder");
                    if (string.IsNullOrEmpty(averageTimeText) || averageTimeText == "AverageTimePlaceholder")
                    {
                        averageTimeText = "平均时间: --:--:--.-";
                    }

                    lblAverageTime.Text = averageTimeText;
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
    }
}