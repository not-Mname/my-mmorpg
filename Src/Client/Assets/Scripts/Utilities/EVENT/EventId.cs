using System.Collections.Generic;
using UnityEngine;

namespace Const
{
    /// <summary>
    /// 事件ID枚举，定义系统中所有标准事件类型
    /// 用于强类型事件订阅和触发
    /// </summary>
    public enum EventId
    {
        None,
        on_money_change,           // 金钱变化事件
        on_map_change,             // 地图切换事件
        on_battle_target_change,   // 战斗目标变化事件：参数1(BattleUnit)-目标的战斗单位
        on_player_lock_target,     // 玩家锁定目标事件：参数1(GameObject)-锁定的目标对象
        on_skill_updata,           // 技能更新事件（无参数）
        on_battle_target_updata,   // 战斗目标更新事件（无参数，仅在存在目标时有效）
        on_battle_player_updata,   // 战斗玩家更新事件
    }
}