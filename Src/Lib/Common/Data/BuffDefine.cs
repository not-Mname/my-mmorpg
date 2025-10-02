using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{
    public class BuffDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        //public TargetType Target { get; set; }
        public string Resource { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        //public EffectType Effect { get; set; }
        public float CD { get; set; }
        public float Duration { get; set; }
        public float Interval { get; set; }
        public float AD { get; set; }
        public float AP { get; set; }
        public float ADFator { get; set; }
        public float APFator { get; set; }
        public float DEFRatio { get; set; }
    }
}
