namespace Const
{
    public enum EventId
    {
        None,
        on_money_change,
        on_map_change,
        on_progress_bar_change,//参数1： bool 代表增加还是减少，参数2：float 代表进度值
        on_battle_target_change,//参数1：BattleUnit 代表目标的战斗单位
    }
}
