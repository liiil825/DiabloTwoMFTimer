using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.Models
{
    // 游戏难度枚举
    public enum GameDifficulty
    {
        Normal,
        Nightmare,
        Hell
    }

    // 角色职业枚举
    public enum CharacterClass
    {
        Barbarian,
        Sorceress,
        Assassin,
        Druid,
        Paladin,
        Amazon,
        Necromancer
    }

    // MF记录类
    public class MFRecord
    {
        public string SceneName { get; set; } = string.Empty;
        public int ACT { get; set; } = 0;
        public GameDifficulty Difficulty { get; set; } = GameDifficulty.Normal;
        [YamlDotNet.Serialization.YamlMember(Alias = "startTime")]
        public DateTime StartTime { get; set; }
        [YamlDotNet.Serialization.YamlMember(Alias = "endTime")]
        public DateTime? EndTime { get; set; }
        [YamlDotNet.Serialization.YamlMember(Alias = "latestTime")]
        public DateTime? LatestTime { get; set; }
        [YamlDotNet.Serialization.YamlMember(Alias = "durationSeconds")]
        public double DurationSeconds { get; set; } = 0;
        
        /// <summary>
        /// 计算的持续时间（秒）
        /// </summary>
        [YamlIgnore]
        public double CalculatedDurationSeconds
        {
            get
            {
                try
                {
                    if (IsCompleted || !LatestTime.HasValue) // 已完成的记录或LatestTime为空
                    {
                        // 对于已完成的记录，直接读取DurationSeconds
                        double result = DurationSeconds;
                        LogManager.WriteDebugLog("GameData", $"[CalculatedDurationSeconds] 计算路径: 已完成记录，直接读取DurationSeconds = {result}");
                        return result;
                    }
                    else // 未完成记录
                    {
                        // 未完成记录，使用DurationSeconds + (现在时间 - LatestTime)
                        double result = DurationSeconds;
                        if (LatestTime.HasValue)
                        {
                            double latestToNow = (DateTime.Now - LatestTime.Value).TotalSeconds;
                            result += latestToNow;
                            LogManager.WriteDebugLog("GameData", $"[CalculatedDurationSeconds] 计算路径: 未完成记录，DurationSeconds({DurationSeconds}) + (Now - LatestTime)({latestToNow}) = {result}");
                        }
                        else
                        {
                            LogManager.WriteDebugLog("GameData", $"[CalculatedDurationSeconds] 计算路径: 未完成记录，仅使用DurationSeconds = {result}");
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteErrorLog("GameData", "[CalculatedDurationSeconds] 计算持续时间时发生错误", ex);
                    return 0;
                }
            }
        }
        public bool IsCompleted => EndTime.HasValue;
    }

    // 角色档案类
    public class CharacterProfile
    {
        public string Name { get; set; } = string.Empty;
        public CharacterClass Class { get; set; }
        public bool IsHidden { get; set; } = false;
        public List<MFRecord> Records { get; set; } = new List<MFRecord>();
        
        // 计算属性
        public double TotalPlayTimeSeconds {
            get {                try {                    return Records?.Sum(r => r.CalculatedDurationSeconds) ?? 0;                } catch {                    return 0;                }            }
        }
        public double AverageGameTimeSeconds {
            get {                try {                    var completedRecords = Records.Where(r => r.IsCompleted).ToList();                    return completedRecords.Count > 0 ? completedRecords.Average(r => r.CalculatedDurationSeconds) : 0;                } catch {                    return 0;                }            }
        }
        public int CompletedGamesCount => Records.Count(r => r.IsCompleted);
        public int TotalGamesCount => Records.Count;
    }

    // 场景类
    public class FarmingScene
    {
        public int ACT { get; set; } = 0;
        public string enUS { get; set; } = string.Empty;
        public string zhCN { get; set; } = string.Empty;
        
        // 根据当前语言获取场景名称
        public string GetSceneName(string language)
        {
            return language == "English" ? enUS : zhCN;
        }
    }

    // 场景数据容器
    public class FarmingSpotsData
    {
        // 只使用一个属性，并通过YamlMember特性指定别名，避免重复映射
        [YamlDotNet.Serialization.YamlMember(Alias = "farmingSpots", ApplyNamingConventions = false)]
        public List<FarmingScene> FarmingSpots { get; set; } = new List<FarmingScene>();
    }
}