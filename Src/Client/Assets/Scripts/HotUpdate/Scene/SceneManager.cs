using Asset;
using AssetBundleFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utilities;

namespace MMO
{
    public class SceneManager : MonoSingleton<SceneManager>
    {
        public UnityAction OnSenceLoadDone = null;
        public string CurrentScene;
        public IResource CurrentSceneResource;
        private string _loadingScene;
        public UnityAction<float> OnProgress = null;
        private bool _editor;

        private void Start()
        {
            _editor = Resloader.Instance._editor;
        }

        public void LoadScene(string name)
        {
            StartCoroutine(LoadLevel(name));
        }

        IEnumerator LoadLevel(string name)
        {
            this._loadingScene = name;

            // 构建模式下，先用 Resloader 加载场景 AB 包
            if (!_editor)
            {
                if(CurrentSceneResource != null)
                {// 卸载上一个场景
                    Resloader.Instance.UnloadScene(CurrentSceneResource);
                    CurrentSceneResource = null;
                }
                string scenePath = $"Assets/AssetBundle/Levels/{name}/{name}.unity";
                IResource sceneResource = Resloader.Instance.LoadSceneAsync(scenePath);
                CurrentSceneResource = sceneResource;
                yield return sceneResource;
            }
            
            // 编辑器用全路径，构建模式 AB 已加载后按名查找 
            string loadPath = _editor
                ? $"Assets/AssetBundle/Levels/{name}/{name}.unity"
                : name;

            AsyncOperation async = null;
            if (_editor)
            {
#if UNITY_EDITOR
                async = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(loadPath, new LoadSceneParameters(LoadSceneMode.Single));
#endif
            }
            else
            {
                 async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(loadPath);

            }


            async.allowSceneActivation = true;
            async.completed += LevelLoadCompleted;
            while (!async.isDone)
            {
                OnProgress?.Invoke(async.progress);
                yield return null;
            }
            OnSenceLoadDone?.Invoke();
        }

        private void LevelLoadCompleted(AsyncOperation obj)
        {
            OnProgress?.Invoke(1f);
            this.CurrentScene = this._loadingScene;
            EVENT.Fire(Const.EventId.on_map_change, this.CurrentScene);
        }
    }
}
