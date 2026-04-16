using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Battle;
using SkillBridge.Message;

namespace Common.Data
{
    public class BuffDefine
    {
        /// <summary>
        /// buff唯一标识
        /// </summary>
        public int ID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 目标类型
        /// </summary>
        public TargetType Target { get; set; }
        public string Resource { get; set; }
        /// <summary>
        /// 图标路径
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        public BuffEffect Effect { get; set; }
        /// <summary>
        /// 触发时机
        /// </summary>
        public TriggerType Trigger { get; set; }
        public float CD { get; set; }
        /// <summary>
        /// 持续时间
        /// </summary>
        public float Duration { get; set; }
        /// <summary>
        /// 间隔时间
        /// </summary>
        public float Interval { get; set; }
        /// <summary>
        /// 物理攻击加成
        /// </summary>
        public float AD { get; set; }
        /// <summary>
        /// 法术攻击
        /// </summary>
        public float AP { get; set; }
        /// <summary>
        /// 攻击加成
        /// </summary>
        public float ADFator { get; set; }
        /// <summary>
        /// 法术加成
        /// </summary>
        public float APFator { get; set; }
        /// <summary>
        /// 防御加成
        /// </summary>
        public float DEFRatio { get; set; }
    }
}
