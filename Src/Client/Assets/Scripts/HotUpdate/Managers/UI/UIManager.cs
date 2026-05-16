using Asset;
using AssetBundleFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.MainUI;
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
            this.uiResources.Add(typeof(UIBag), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/UIBag.prefab", cache = false });
            this.uiResources.Add(typeof(UIShop), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/UIShop.prefab", cache = false });
            this.uiResources.Add(typeof(UIEquip), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/UICharEquip.prefab", cache = false });
            this.uiResources.Add(typeof(UIQuest), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/UIQuestSystem.prefab", cache = false });
            this.uiResources.Add(typeof(UIQuestDialog), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/UIQuestDialog.prefab", cache = false });
            this.uiResources.Add(typeof(UIFriend), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/UIFriend/UIFriend.prefab", cache = false });
            this.uiResources.Add(typeof(UIGuild), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/Guild/UIGuild.prefab", cache = false });
            this.uiResources.Add(typeof(UIGuildList), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/Guild/UIGuildList.prefab", cache = false });
            this.uiResources.Add(typeof(UIGuildPopNoGuild), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/Guild/UIGuildPopNoGuild.prefab", cache = false });
            this.uiResources.Add(typeof(UIGuildPopCreate), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/Guild/UIGuildPopCreate.prefab", cache = false });
            this.uiResources.Add(typeof(UIGuildApplyList), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/Guild/UIGuildApplyList.prefab", cache = false });
            this.uiResources.Add(typeof(UISetting), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/UISetting.prefab", cache = false });
            this.uiResources.Add(typeof(UIPopChatMenu), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/UIPopChatMenu.prefab", cache = false });
            this.uiResources.Add(typeof(UIRide), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/UIRide.prefab", cache = false });
            this.uiResources.Add(typeof(UISystemConfig), new UIElement() { resources = "Assets/AssetBundle/Prefab/UI/UISystemConfig.prefab", cache = false });

        }

        public T Show<T>()
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);
            Type type = typeof(T);//通过反射获取类型
            if (uiResources.ContainsKey(type))
            {
                UIElement uiElement = uiResources[type];
                if (uiElement.instance == null)
                {
                    IResource res = Resloader.Instance.LoadAssetSync(uiElement.resources);
                    GameObject perfad = res.Instantiate(UIMain.Instance.transform, false, true);
                    if (perfad == null) { return default(T); }
                    uiElement.instance = perfad;
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
