using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static GameObject _singletonRoot = null;
    public bool global = true;
    public bool IsSence = true;
    bool IsInit { get; set; }
    private static T _instance;
    private static bool _init = false;
    private static bool ApplicationIsQuitting = false;

    public static T Instance
    {
        get
        {

            if (_instance == null && !ApplicationIsQuitting)
            {

                // 尝试在场景中查找
                string name = typeof(T).Name;
                var go = GameObject.Find(name);
                _instance = go != null ? go.GetComponent<T>() : null;
                InitParent();
                // 如果场景中没有
                if (_instance == null)
                {
                    //Debug.LogError($"Singleton {typeof(T)} 未找到对象");
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
        if (_instance == null)
        {
            var go = GameObject.Find(name);
            _instance = go != null ? go.GetComponent<T>() : null;
        }
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
        _init = true;
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