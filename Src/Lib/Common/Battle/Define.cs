namespace Common.Battle
{
    public enum AttributesType
    {
        None = -1,
        //最大生命
        MaxHp = 0,
        //最大魔法
        MaxMp = 1,
        //力量
        STR = 2,
        //智力
        INT = 3,
        //敏捷
        DEX = 4,
        //物理攻击
        AD = 5,
        //法术攻击
        AP = 6,
        //物理防御
        DEF = 7,
        //魔法防御
        MDEF = 8,
        //攻击速度
        SPD = 9,
        //暴击
        CRI = 10,
        Max
    }

    public enum TargetType
    {
        None = 0,
        Target = 1,
        Self = 2,
        Position = 3,
    }

    public enum BuffEffect
    {
        None = 0,
        Stun = 1,        
    }

    public enum SkillType
    {
        Normal = 0,
        Skill = 1,
    }
}
