using System.Collections.Generic;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Interfaces;

public interface IKeyMapRepository
{
    /// <summary>
    /// 加载按键映射配置。如果文件不存在，会生成默认配置。
    /// </summary>
    /// <returns>根节点列表</returns>
    List<KeyMapNode> LoadKeyMap();

    /// <summary>
    /// 保存按键映射配置
    /// </summary>
    void SaveKeyMap(List<KeyMapNode> nodes);
}
