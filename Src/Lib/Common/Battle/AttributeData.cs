using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public class AttributeData
    {
        public float[] Data = new float[(int)AttributesType.Max];

        // 最大生命
        public float MaxHp { get { return Data[(int)AttributesType.MaxHp]; } set { Data[(int)AttributesType.MaxHp] = value; } }
        // 最大法力
        public float MaxMp { get { return Data[(int)AttributesType.MaxMp]; } set { Data[(int)AttributesType.MaxMp] = value; } }
        // 力量
        public float STR { get { return Data[(int)AttributesType.STR]; } set { Data[(int)AttributesType.STR] = value; } }
        // 智力
        public float INT { get { return Data[(int)AttributesType.INT]; } set { Data[(int)AttributesType.INT] = value; } }
        // 敏捷
        public float DEX { get { return Data[(int)AttributesType.DEX]; } set { Data[(int)AttributesType.DEX] = value; } }
        // 魔抗
        public float MDEF { get { return Data[(int)AttributesType.MDEF]; } set { Data[(int)AttributesType.MDEF] = value; } }
        // 物理攻击
        public float AD { get { return Data[(int)AttributesType.AD]; } set { Data[(int)AttributesType.AD] = value; } }
        // 法术攻击
        public float AP { get { return Data[(int)AttributesType.AP]; } set { Data[(int)AttributesType.AP] = value; } }
        // 物理防御
        public float DEF { get { return Data[(int)AttributesType.DEF]; } set { Data[(int)AttributesType.DEF] = value; } }
        // 攻击速度
        public float SPD { get { return Data[(int)AttributesType.SPD]; } set { Data[(int)AttributesType.SPD] = value; } }
        // 暴击
        public float CRI { get { return Data[(int)AttributesType.CRI]; } set { Data[(int)AttributesType.CRI] = value; } }

        public void Reset()
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = 0;
            }
        }
    }
}
