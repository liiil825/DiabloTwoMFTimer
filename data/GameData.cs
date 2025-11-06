using System;
using System.Collections.Generic;
using System.Linq;

namespace DTwoMFTimerHelper.Data
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
        public string SceneEnName { get; set; } = string.Empty;
        public string SceneZhName { get; set; } = string.Empty;
        public int ACT { get; set; } = 0;
        public GameDifficulty Difficulty { get; set; } = GameDifficulty.Normal;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double DurationSeconds => EndTime.HasValue ? (EndTime.Value - StartTime).TotalSeconds : 0;
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
        public double TotalPlayTimeSeconds => Records.Sum(r => r.DurationSeconds);
        public double AverageGameTimeSeconds => Records.Count > 0 ? Records.Where(r => r.IsCompleted).Average(r => r.DurationSeconds) : 0;
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