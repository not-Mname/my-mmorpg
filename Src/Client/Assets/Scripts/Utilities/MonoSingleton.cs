using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool global = true;
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 尝试在场景中查找
                instance = FindFirstObjectByType<T>();

                // 如果场景中没有，则动态创建
                if (instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();

                    if (instance is MonoSingleton<T> singleton && singleton.global)
                    {
                        DontDestroyOnLoad(go);
                    }
                }
            }
            return instance;
        }
    }


    void Awake()
    {
        if (global)
        {
            if(instance != null && instance != this.gameObject.GetComponent<T>())
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject.GetComponent<T>();
        }
        this.OnStart();
    }

    protected virtual void OnStart()
    {

    }
}