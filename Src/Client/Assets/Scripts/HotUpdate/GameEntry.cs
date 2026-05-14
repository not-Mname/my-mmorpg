using Asset;
using AssetBundleFramework;
using GameInterFace;
using Managers;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace HotUpdate
{
    public static class GameEntry
    {
        private static UI.Common.ProgressBar _progressBar;
        private static TextMeshProUGUI _progressText;
        private static List<IInitializable> _initializables = new() {
            EventManager.Instance,
            MapService.Instance,
            ShopManager.Instance,
            StatusService.Instance,
            ChatService.Instance,
            TeamService.Instance,
            FriendService.Instance,
            ArenaService.Instance,
            GuildService.Instance,
        };

        /// <summary>
        /// LoadingManager 转交控制权后的统一入口
        /// 此方法由 Core 通过反射调用
        /// </summary>
        public static IEnumerator Run(LoadingManager loading, bool editor)
        {
            Debug.Log("GameEntry Run");
            _progressBar = loading.progressBar;
            _progressText = loading.progressText;
            Debug.Log($"_progressBar = {_progressBar == null} _progressText = {_progressText == null}");

            yield return LoadAllSystem();

            SoundManager.Instance.PlayMusic(SoundDefine.Music_Login);

            yield return LoadAllConfig(editor);

            yield return InitAllServices(loading);

            _progressText.text = "加载完成!";
            yield return new WaitForSeconds(0.5f);
            loading.ShowLogin();
        }

        private static IEnumerator LoadAllSystem()
        {
            Debug.Log("LoadAllSystem Run");
            _progressText.text = "正在加载系统...";
            while (!Resloader.Instance.isInit)
            {
                yield return null;
            }

            IResource res = Resloader.Instance.LoadAssetAsync("Assets/AssetBundle/Prefab/Level/System.prefab");

            yield return res;
            res.Instantiate();
        }

        private static IEnumerator LoadAllConfig(bool editor)
        {
            Debug.Log("LoadAllConfig Run");
            DataManager.Editor = editor;
            if (editor)
            { yield return DataManager.Instance.LoadDataEditor(_progressBar, _progressText); }
            else
            { yield return DataManager.Instance.Load(_progressBar, _progressText); }
        }

        private static IEnumerator InitAllServices(LoadingManager loading)
        {
            Debug.Log("InitAllServices Run");
            _progressText.text = "正在初始化系统...";
            _progressBar.SetData(100, 0, _initializables.Count);

            var wait = new WaitForSeconds(0.1f);
            float step = 100f / _initializables.Count;

            foreach (var init in _initializables)
            {
                try
                {
                    init.Init();
                }
                catch (Exception e)
                {
                    Debug.LogError($"{init.GetType()} {e}");
                }

                _progressBar.CurrentValue += step;
                yield return wait;
            }
            _progressBar.UpdateProgress();
        }
    }
}