using AssetBundleFramework;
using HotUpdate;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asset
{
    public class Resloader : MonoSingleton<Resloader>
    {
        private ResourceManager _resourceManager = ResourceManager.Instance;
        [HideInInspector]
        public bool _editor = false;

        public bool isInit = false;

        /// <summary>
        /// AssetBundle 文件路径前缀
        /// 用于构建完整的资源访问路径
        /// </summary>
        private string _prefixPath { get; set; }

        /// <summary>
        /// 当前运行平台标识
        /// 根据 Unity 编辑器或不同构建目标自动确定
        /// </summary>
        private string _platForm { get; set; }

        private Action OnUpdate;

        private Action OnLateUpdate;

        #region 资源加载

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IResource LoadAssetSync(string path)
        {
            return _resourceManager.Load(path);
        }

        /// <summary>
        /// 异步加载资源，协程专用
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IResource LoadAssetAsync(string path)
        {
            return _resourceManager.Load(path, true);
        }

        /// <summary>
        /// 异步加载资源，回调专用
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <param name="async"></param>
        public void LoadAssetWithCallback(string path, Action<IResource> callback, bool async = true)
        {
            _resourceManager.LoadWithCallback(path, async, callback);
        }

        /// <summary>
        /// 异步加载资源，async专用
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ResourceAwaiter LoadAssetWithAwaiter(string path)
        {
            return _resourceManager.LoadWithAwaiter(path);
        }
        #endregion

        #region 场景加载
        /// <summary>
        /// 同步加载场景
        /// 从 AssetBundle 中同步加载场景并切换/叠加
        /// 注意：同步加载会阻塞主线程，大场景建议使用异步方法
        /// </summary>
        /// <param name="path">场景的资源路径，如 "assets/assetbundle/levels/maincity.unity"</param>
        /// <param name="mode">场景加载模式：Single 替换所有场景，Additive 叠加到当前场景</param>
        /// <returns>场景资源对象，可用于后续卸载场景</returns>
        public IResource LoadSceneSync(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return _resourceManager.LoadScene(path, mode);
        }

        /// <summary>
        /// 异步加载场景，协程专用
        /// 返回的 SceneResource（实际为 SceneResourceAsync）继承自 CustomYieldInstruction，可在协程中 yield return
        /// </summary>
        /// <param name="path">场景的资源路径</param>
        /// <param name="mode">场景加载模式</param>
        /// <returns>场景资源对象，可 yield return 等待加载完成</returns>
        public IResource LoadSceneAsync(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return _resourceManager.LoadScene(path, mode, async: true);
        }

        /// <summary>
        /// 异步加载场景，回调方式
        /// 加载完成后自动触发回调，不阻塞主线程，适合做加载进度界面
        /// </summary>
        /// <param name="path">场景的资源路径</param>
        /// <param name="mode">场景加载模式</param>
        /// <param name="callback">加载完成回调，参数为已加载完成的场景资源</param>
        public void LoadSceneWithCallback(string path, LoadSceneMode mode, Action<IResource> callback)
        {
            _resourceManager.LoadSceneWithCallback(path, mode, callback);
        }

        /// <summary>
        /// 异步加载场景，async/await 方式
        /// 返回 ResourceAwaiter，支持使用 await 关键字等待场景加载完成
        /// await 返回 IResource 类型，可强转为 SceneResource 使用
        /// </summary>
        /// <param name="path">场景的资源路径</param>
        /// <param name="mode">场景加载模式</param>
        /// <returns>可被 await 的异步等待对象</returns>
        public ResourceAwaiter LoadSceneWithAwaiter(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return _resourceManager.LoadSceneWithAwaiter(path, mode);
        }

        /// <summary>
        /// 卸载场景
        /// 自动卸载场景并释放对应的 AssetBundle 资源
        /// 引用计数归零后延迟到 LateUpdate 统一执行卸载
        /// </summary>
        /// <param name="resource">由加载方法返回的场景资源对象</param>
        public void UnloadScene(IResource resource)
        {
            _resourceManager.UnloadScene(resource as AResource);
        }
        #endregion

        #region 生命周期
        protected override void OnAwake()
        {
            base.OnAwake();
            isInit = false;
            _platForm = GetPlatform();
            // 构建 AssetBundle 根目录路径
            _prefixPath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "AssetBundle")).Replace("\\", "/");
            _prefixPath += $"/{_platForm}";
        }

        private void Start()
        {
            HotUpdateManager.Instance.OnEndDownload += OnStart;
        }

        public void OnStart()
        {
            _resourceManager.Initialize(GetPlatform(), GetFileUrl, _editor, 0);
            OnUpdate += _resourceManager.Update;
            OnLateUpdate += _resourceManager.LateUpdate;
            isInit = true;
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }
        #endregion

        #region 工具
        /// <summary>
        /// 获取资源的完整文件路径
        /// 将相对路径转换为绝对路径供 ResourceManager 使用
        /// </summary>
        /// <param name="url">资源的相对路径</param>
        /// <returns>资源的绝对路径</returns>
        private string GetFileUrl(string url)
        {
            return Path.Combine(_prefixPath, url).Replace("\\", "/");
        }

        /// <summary>
        /// 根据当前运行环境获取平台标识符
        /// 支持 Windows、Android、iOS 等主流平台
        /// </summary>
        /// <returns>平台名称字符串</returns>
        /// <exception cref="System.Exception">遇到未支持的平台时抛出异常</exception>
        public string GetPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                default:
                    throw new System.Exception($"未支持的平台:{Application.platform}");
            }
        }
        #endregion
    }
}

