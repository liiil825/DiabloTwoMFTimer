using System;
using System.Collections.Generic;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Interfaces;

public interface IStatisticsService
{
    /// <summary>
    /// 获取指定时间段内的场景统计数据
    /// </summary>
    public List<SceneStatDto> GetSceneStatistics(DateTime startTime, DateTime endTime, bool sortByCount);

    /// <summary>
    /// 获取指定时间段内的掉落统计数据
    /// </summary>
    public List<LootStatDto> GetLootStatistics(DateTime startTime, DateTime endTime);

    /// <summary>
    /// 获取指定时间段内的详细统计摘要
    /// </summary>
    public string GetDetailedSummary(DateTime startTime, DateTime endTime);

    /// <summary>
    /// 获取本周的开始时间（周一00:00:00）
    /// </summary>
    public DateTime GetStartOfWeek();
}
