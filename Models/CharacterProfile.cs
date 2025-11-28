using System;
using System.Collections.Generic;
using System.Linq;

namespace DiabloTwoMFTimer.Models;
// 角色档案类
public class CharacterProfile
{
    public string Name { get; set; } = string.Empty;
    public CharacterClass Class { get; set; }
    public List<MFRecord> Records { get; set; } = [];
    public List<LootRecord> LootRecords { get; set; } = []; // 存储掉落的历史记录

    public string LastRunScene { get; set; } = string.Empty;
    public GameDifficulty LastRunDifficulty { get; set; } = GameDifficulty.Hell;

    // 计算属性
    public double TotalPlayTimeSeconds
    {
        get
        {
            try
            {
                return Records?.Sum(r => r.DurationSeconds) ?? 0;
            }
            catch
            {
                return 0;
            }
        }
    }
    public double AverageGameTimeSeconds
    {
        get
        {
            try
            {
                var completedRecords = Records.Where(r => r.IsCompleted).ToList();
                return completedRecords.Count > 0 ? completedRecords.Average(r => r.DurationSeconds) : 0;
            }
            catch
            {
                return 0;
            }
        }
    }
    public int CompletedGamesCount => Records.Count(r => r.IsCompleted);
    public int TotalGamesCount => Records.Count;
}
