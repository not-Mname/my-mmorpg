namespace Const
{
    public enum EventId
    {
        None,
        on_money_change,
        on_map_change,
        on_progress_bar_change,//参数1： bool 代表增加还是减少，参数2：float 代表进度值
        on_battle_target_change,//参数1：BattleUnit 代表目标的战斗单位
        on_player_lock_target,//参数1：GameObject 代表锁定的目标
        on_skill_updata,//无参
        on_battle_target_updata,//无参，仅在有目标时有效
        on_battle_player_updata,
    }
}
