using System;

namespace DiabloTwoMFTimer.Models;

// 掉落物品类
public class LootRecord
{
    public string Name { get; set; } = string.Empty; // 名称
    public string SceneName { get; set; } = string.Empty; // 掉落的场景
    public int RunCount { get; set; } = 0; // 在多少次同场景中掉落的
    public DateTime DropTime { get; set; } = DateTime.Now; // 掉落的时间
}