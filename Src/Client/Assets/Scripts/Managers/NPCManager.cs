using Common.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Managers
{
    public class NPCManager : Singleton<NPCManager>
    {
        public delegate bool NPCActionHandler(NPCDefine npc);

        Dictionary<NPCFunction, NPCActionHandler> eventMap = new Dictionary<NPCFunction, NPCActionHandler>();
        Dictionary<int, Vector3> npcPositions = new Dictionary<int, Vector3>();
        public NPCDefine GetNPCDefine(int npcId)
        {
            NPCDefine npcDefine = null;
            DataManager.Instance.NPCs.TryGetValue(npcId, out npcDefine);
            return npcDefine;
        }

        public void DegisterNPCEvent(NPCFunction function, NPCActionHandler handler)
        {
            if (eventMap.ContainsKey(function))
            {
                eventMap[function] += handler;
            }
            else
            {
                eventMap[function] = handler;
            }
        }

        public bool Interactive(int npcId)
        {
            if (DataManager.Instance.NPCs.ContainsKey(npcId))
            {
                var npc = DataManager.Instance.NPCs[npcId];
                return Interactive(npc);
            }
            return false;
        }

        private bool Interactive(NPCDefine npc)
        {
            if(DoTaskInteractive(npc))
            {                
                return true;
            }
            if(npc.Type == NpcType.Functional)
            {
                return DoFunctionalInteractive(npc);
            }
            return false;
        }

        private bool DoFunctionalInteractive(NPCDefine npc)
        {
            if (eventMap.ContainsKey(npc.Function) && eventMap[npc.Function] != null)
                eventMap[npc.Function](npc);
            return true;
        }

        private bool DoTaskInteractive(NPCDefine npc)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NPCQuestStatus.None)
                return false;

            return QuestManager.Instance.OpenQuest(npc.ID);
        }

        public void UpdateNPCPosition(int npcId, Vector3 position)
        {
            npcPositions[npcId] = position;
        }
        public Vector3 GetNPCPostion(int npc)
        {
            return npcPositions[npc];
        }
    }
}
