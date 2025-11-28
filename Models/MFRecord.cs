using System;

namespace DiabloTwoMFTimer.Models;

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

    public bool IsCompleted => EndTime.HasValue;
}