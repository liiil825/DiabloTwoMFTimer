using System;
using System.Collections.Generic;

namespace DiabloTwoMFTimer.Models;

// 游戏难度枚举
public enum GameDifficulty
{
    Normal,
    Nightmare,
    Hell,
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
    Necromancer,
}

// 场景类
public class FarmingScene
{
    public int ACT { get; set; } = 0;

    [YamlDotNet.Serialization.YamlMember(Alias = "enUS")]
    public string EnUS { get; set; } = string.Empty;

    [YamlDotNet.Serialization.YamlMember(Alias = "zhCN")]
    public string ZhCN { get; set; } = string.Empty;

    [YamlDotNet.Serialization.YamlMember(Alias = "shortName")]
    public string ShortName { get; set; } = string.Empty;

    [YamlDotNet.Serialization.YamlMember(Alias = "shortEnName")]
    public string ShortEnName { get; set; } = string.Empty;

    [YamlDotNet.Serialization.YamlMember(Alias = "shortZhCN")]
    public string ShortZhCN { get; set; } = string.Empty;

    // 根据当前语言获取场景名称
    public string GetSceneName(string language)
    {
        return language == "English" ? EnUS : ZhCN;
    }
}

// 场景数据容器
public class FarmingSpotsData
{
    // 只使用一个属性，并通过YamlMember特性指定别名，避免重复映射
    [YamlDotNet.Serialization.YamlMember(Alias = "farmingSpots", ApplyNamingConventions = false)]
    public List<FarmingScene> FarmingSpots { get; set; } = [];
}

