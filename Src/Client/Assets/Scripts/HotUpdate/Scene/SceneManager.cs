using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace MMO
{
    public class SceneManager : MonoSingleton<SceneManager>
    {
        public UnityAction OnSenceLoadDone = null;
        public string CurrentScene;
        private string _loadingScene;
        public UnityAction<float> OnProgress = null;


        public void LoadScene(string name)
        {
            StartCoroutine(LoadLevel(name));
        }

        IEnumerator LoadLevel(string name)
        {
            this._loadingScene = name;
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            async.allowSceneActivation = true;
            async.completed += LevelLoadCompleted;
            while (!async.isDone)
            {
                if (OnProgress != null)
                    OnProgress?.Invoke(async.progress);
                yield return null;
            }
            OnSenceLoadDone?.Invoke();
        }

        private void LevelLoadCompleted(AsyncOperation obj)
        {
            if (OnProgress != null)
                OnProgress(1f);
            this.CurrentScene = this._loadingScene;
            EVENT.Fire(Const.EventId.on_map_change, this.CurrentScene);
        }
    }

}
