using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static GameObject _singletonRoot = null;
    public bool global = true;
    public bool IsSence = true;
    private bool _isInit = false;
    private static T _instance;
    private static bool ApplicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 尝试在场景中查找
                string name = typeof(T).Name;
                _instance = GameObject.Find(name) as T;
                InitParent();
                // 如果场景中没有，则动态创建
                if (_instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    go.name = typeof(T).Name;
                    go.transform.SetParent(_singletonRoot.transform);

                    _instance = go.AddComponent<T>();

                    if (_instance is MonoSingleton<T> singleton && singleton.global)
                    {
                        DontDestroyOnLoad(go);
                    }
                }
            }
            return _instance;
        }
    }

    private static void InitParent()
    {
        _singletonRoot = GameObject.Find("SingletonRoot");
        if (_singletonRoot == null)
        {
            _singletonRoot = new GameObject("SingletonRoot");
        }
        if (_instance != null && _instance.gameObject.transform.parent != _singletonRoot.transform)
        {
            _instance.gameObject.transform.SetParent(_singletonRoot.transform);
        }
        DontDestroyOnLoad(_singletonRoot);
    }

    void Awake()
    {
        string name = typeof(T).Name;
        _instance = GameObject.Find(name) as T;
        if (_instance != null && _instance != gameObject.GetComponent<T>())
        {
            Destroy(gameObject);
            return;
        }
        if (ApplicationIsQuitting) { return; }
        _instance = gameObject.GetComponent<T>();
        if (global)
        {
            DontDestroyOnLoad(gameObject);
        }
        InitParent();
        OnAwake();
    }

    protected virtual void OnAwake()
    {
    }

    protected virtual void OnApplicationQuit()
    {
        ApplicationIsQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        //string name = typeof(T).Name;
        //Debug.Log($"{name} Destroyed");
    }
}