using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{
    public class NPCDefine
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Descript { get; set; }

        public NVector3 position { get; set; }
        public NpcType Type { get; set; }

        public NPCFunction Function { get; set; }

        public int param { get; set; }
    }

    public enum NpcType
    {
        None = 0,
        Functional,
        Task,

    }

    public enum NPCFunction
    {
        None = 0,
        InvokeShop,
        InvokeInsrance,
    }
}
