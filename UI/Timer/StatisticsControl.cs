using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer;

public partial class StatisticsControl : UserControl
{
    // 统计数据属性
    public int RunCount { get; set; }
    public TimeSpan FastestTime { get; set; }
    public TimeSpan AverageTime { get; set; }

    public StatisticsControl()
    {
        InitializeComponent();
        // 注册语言变更事件
        Utils.LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
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
            string runCountText = Utils.LanguageManager.GetString("RunCount", RunCount);
            runCountText = $" {runCountText} ";

            lblRunCount.Text = runCountText;
        }

        if (lblFastestTime != null)
        {
            if (RunCount > 0 && FastestTime != TimeSpan.MaxValue && FastestTime > TimeSpan.Zero)
            {
                string fastestFormatted = string.Format(
                    "{0:00}:{1:00}:{2:00}.{3}",
                    FastestTime.Hours,
                    FastestTime.Minutes,
                    FastestTime.Seconds,
                    (int)(FastestTime.Milliseconds / 100)
                );

                string fastestTimeText = Utils.LanguageManager.GetString("FastestTime", fastestFormatted);
                lblFastestTime.Text = fastestTimeText;
            }
            else
            {
                string fastestTimeText = Utils.LanguageManager.GetString("FastestTimePlaceholder");
                lblFastestTime.Text = fastestTimeText;
            }
        }

        if (lblAverageTime != null)
        {
            if (RunCount > 0)
            {
                string averageFormatted = string.Format(
                    "{0:00}:{1:00}:{2:00}.{3}",
                    AverageTime.Hours,
                    AverageTime.Minutes,
                    AverageTime.Seconds,
                    (int)(AverageTime.Milliseconds / 100)
                );

                string averageTimeText = Utils.LanguageManager.GetString("AverageTime", averageFormatted);
                lblAverageTime.Text = averageTimeText;
            }
            else
            {
                string averageTimeText = Utils.LanguageManager.GetString("AverageTimePlaceholder");
                lblAverageTime.Text = averageTimeText;
            }
        }
    }
}
