using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class UIManager : Singleton<UIManager>
    {
        class UIElement
        {
            public string resources;
            public bool cache;
            public GameObject instance;
        }

        private Dictionary<Type, UIElement> uiResources = new Dictionary<Type, UIElement>();

        public UIManager()
        {
            this.uiResources.Add(typeof(UIBag),new UIElement() { resources = "UI/UIBag", cache = false });
            this.uiResources.Add(typeof(UIShop), new UIElement() { resources = "UI/UIShop", cache = false });
            this.uiResources.Add(typeof(UIEquip), new UIElement() { resources = "UI/UICharEquip", cache = false });
            this.uiResources.Add(typeof(UIQuest), new UIElement() { resources = "UI/UIQuestSystem", cache = false });
            this.uiResources.Add(typeof(UIQuestDialog), new UIElement() { resources = "UI/UIQuestDialog", cache = false });
            this.uiResources.Add(typeof(UIFriends), new UIElement() { resources = "UI/UIFriends", cache = false });
            this.uiResources.Add(typeof(UIGuild), new UIElement() { resources = "UI/Guild/UIGuild", cache = false });
            this.uiResources.Add(typeof(UIGuildList), new UIElement() { resources = "UI/Guild/UIGuildList", cache = false });
            this.uiResources.Add(typeof(UIGuildPopNoGuild), new UIElement() { resources = "UI/Guild/UIGuildPopNoGuild", cache = false });
            this.uiResources.Add(typeof(UIGuildPopCreate), new UIElement() { resources = "UI/Guild/UIGuildPopCreate", cache = false });
            this.uiResources.Add(typeof(UIGuildApplyList), new UIElement() { resources = "UI/Guild/UIGuildApplyList", cache = false });
            this.uiResources.Add(typeof(UISetting), new UIElement() { resources = "UI/UISetting", cache = false });
            this.uiResources.Add(typeof(UIPopChatMenu), new UIElement() { resources = "UI/UIPopChatMenu", cache = false });
            this.uiResources.Add(typeof(UIRide), new UIElement() { resources = "UI/UIRide", cache = false });
            this.uiResources.Add(typeof(UISystemConfig), new UIElement() { resources = "UI/UISystemConfig", cache = false });
        
        }

        ~UIManager()
        {

        }

        public T Show<T>()
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);
            Type type = typeof(T);//通过反射获取类型
            if (uiResources.ContainsKey(type))
            {
                UIElement uiElement = uiResources[type];
                if(uiElement.instance == null)
                {

                    UnityEngine.Object perfad = Resources.Load(uiElement.resources);
                    if(perfad == null) return default(T);
                    uiElement.instance = (GameObject)GameObject.Instantiate(perfad, UIMain.Instance.transform);
                    return uiElement.instance.GetComponent<T>();
                }
                else
                {
                    uiElement.instance.SetActive(true);
                }
            }
            return default(T);
        }

        public void Close(Type type)
        {
            if (uiResources.ContainsKey(type))
            {
                UIElement uiElement = uiResources[type];
                if (uiElement.cache)
                {
                    uiElement.instance.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(uiElement.instance);
                    uiElement.instance = null;
                }
            }
        }
        public void Close<T>()
        {
            Close(typeof(T));
        }
    }
}
