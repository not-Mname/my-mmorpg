using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace Common.Battle
{
    public class Attributes
    {
        private AttributeData _inital = new AttributeData();
        private AttributeData _growth = new AttributeData();
        private AttributeData _equip = new AttributeData();
        public AttributeData Basic = new AttributeData();// 基本属性,角色初始属性+装备属性+成长属性
        public AttributeData Buff = new AttributeData();
        public AttributeData Final = new AttributeData();// 最终属性,基本属性+Buff

        private int _level;

        public NAttributeDynamic Dynamic;// 动态属性,战斗中变化的属性

        public float HP
        {
            get
            {
                return Dynamic.Hp;
            }
            set
            {
                Dynamic.Hp = (int)value;
            }
        }
        public float MP { get; set; }

        // 最大生命
        public float MaxHp { get { return (int)Final.MaxHp; } }
        // 最大法力
        public float MaxMp { get { return (int)Final.MaxMp; } }
        // 力量
        public float STR { get { return (int)Final.STR; } }
        // 智力
        public float INT { get { return (int)Final.INT; } }
        // 敏捷
        public float DEX { get { return (int)Final.DEX; } }
        // 魔抗
        public float MDEF { get { return (int)Final.MDEF; } }
        // 物理攻击
        public float AD { get { return (int)Final.AD; } }
        // 法术攻击
        public float AP { get { return (int)Final.AP; } }
        // 物理防御
        public float DEF { get { return (int)Final.DEF; } }
        // 攻击速度
        public float SPD { get { return (int)Final.SPD; } }
        // 暴击
        public float CRI { get { return (int)Final.CRI; } }

        public void Init(CharacterDefine define, List<EquipDefine> equips, int level, NAttributeDynamic dynamic)
        {
            this.Dynamic = dynamic;
            this.LoadInitAttribute(_inital, define);
            this.LoadGrowthAttribute(_growth, define);
            this.LoadEquipAttribute(_equip, equips);
            this._level = level;
            this.InitBasicAttribute();
            this.InitSecondaryAttribute();
            this.InitFinalAttribute();

            if (this.Dynamic == null)
            {
                Dynamic = new NAttributeDynamic();
                this.HP = this.MaxHp;
                this.MP = this.MaxMp;
            }
            else
            {
                this.HP = dynamic.Hp;
                this.MP = dynamic.Mp;
            }

        }


        public void InitFinalAttribute()
        {
            for (int i = (int)AttributesType.MaxHp; i < (int)AttributesType.Max; ++i)
            {
                this.Final.Data[i] = this.Basic.Data[i] + Buff.Data[i];
            }
        }

        public void InitBasicAttribute()
        {
            for (int i = (int)AttributesType.MaxHp; i < (int)AttributesType.Max; ++i)
            {
                Basic.Data[i] = _inital.Data[i];
            }

            for (int i = (int)AttributesType.STR; i < (int)AttributesType.Max; ++i)
            {
                this.Basic.Data[i] = Basic.Data[i] + _growth.Data[i] * (this._level - 1);// 一级成长属性
                this.Basic.Data[i] += _equip.Data[i];// 装备一级属性加在计算成长属性之前
            }
        }

        /*
         *   2级属性	战士	            法师	        弓箭手
         *   HP	        力量*10+260	        力量*10+270	        力量*10+250
         *   MP	        智力*10+180	        智力*10+290	        智力*10+260		
         *   物理攻击	力量*5+20	        力量*5+25	        力量*5+25		
         *   魔法攻击	智力*5+25	        智力*5+20	        智力*5+30						
         *   物理防御	力量*2+敏捷*1+15	力量*2+敏捷*1+9	    力量*2+敏捷*1+9						
         *   魔法防御	智力*2+敏捷*1+9	    智力*2+敏捷*1+15	智力*2+敏捷*1+15						
         *   攻击速度	敏捷*0.2+59	        敏捷*0.2+69	        敏捷*0.2+78.6						
         *   暴击概率	敏捷*0.02%+2%	    敏捷*0.02%+2%	    敏捷*0.02%+5%						
         */
        public void InitSecondaryAttribute()
        {
            // 二级属性加成（包括装备）
            this.Basic.MaxHp = this.Basic.STR * 10 + this._inital.MaxHp + this._equip.MaxHp;
            this.Basic.MaxMp = this.Basic.INT * 10 + this._inital.MaxMp + this._equip.MaxMp;

            this.Basic.AD = this.Basic.STR * 5 + this._inital.AD + this._equip.AD;
            this.Basic.AP = this.Basic.INT * 5 + this._inital.AP + this._equip.AP;
            this.Basic.DEF = this.Basic.STR * 2 + this.Basic.DEX * 1 + this._inital.DEF + this._equip.DEF;
            this.Basic.MDEF = this.Basic.INT * 2 + this.Basic.DEX * 1 + this._inital.MDEF + this._equip.MDEF;

            this.Basic.SPD = this.Basic.DEX * 0.2f + this._inital.SPD + this._equip.SPD;
            this.Basic.CRI = this.Basic.DEX * 0.0002f + this._inital.CRI + this._equip.CRI;
        }

        private void LoadInitAttribute(AttributeData attr, CharacterDefine define)
        {
            attr.MaxMp = define.MaxMP;
            attr.MaxHp = define.MaxHP;

            attr.AD = define.AD;
            attr.DEF = define.DEF;
            attr.MDEF = define.MDEF;
            attr.SPD = define.SPD;
            attr.CRI = define.CRI;
            attr.AP = define.AP;
            attr.STR = define.STR;
            attr.INT = define.INT;
            attr.DEX = define.DEX;
        }

        private void LoadGrowthAttribute(AttributeData attr, CharacterDefine define)
        {
            attr.STR = define.GrowthSTR;
            attr.INT = define.GrowthINT;
            attr.DEX = define.GrowthDEX;
        }

        private void LoadEquipAttribute(AttributeData attr, List<EquipDefine> equips)
        {
            attr.Reset();
            if(equips == null) return;
            foreach (var equip in equips)
            {
                attr.MaxMp += equip.MaxMP;
                attr.MaxHp += equip.MaxHP;
                attr.AD = equip.AD;
                attr.DEF = equip.DEF;
                attr.MDEF = equip.MDEF;
                attr.SPD = equip.SPD;
                attr.CRI = equip.CRI;
                attr.AP = equip.AP;
                attr.STR = equip.STR;
                attr.INT = equip.INT;
                attr.DEX = equip.DEX;

            }
        }
    }
}
