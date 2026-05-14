using System;
using UnityEngine.SceneManagement;

namespace AssetBundleFramework
{
    internal interface IResourceManager
    {
        #region 普通资源加载
        public void LoadWithCallback(string url, bool async, Action<IResource> callback);
        public IResource Load(string url, bool async);
        public ResourceAwaiter LoadWithAwaiter(string url);
        public void Unload(IResource resource);
        #endregion

        #region 场景加载
        /// <summary>
        /// 同步加载场景
        /// 从 AssetBundle 中加载场景并立即切换/叠加
        /// </summary>
        /// <param name="url">场景文件的资源路径，如 "assets/assetbundle/levels/maincity.unity"</param>
        /// <param name="mode">场景加载模式：Single 替换当前场景，Additive 叠加场景</param>
        /// <param name="async">是否异步加载，默认 false 同步加载</param>
        /// <returns>场景资源对象，可用于后续卸载</returns>
        public IResource LoadScene(string url, LoadSceneMode mode = LoadSceneMode.Single, bool async = false);

        /// <summary>
        /// 异步加载场景（回调方式）
        /// 加载完成后通过回调通知，回调中可获取场景资源对象
        /// </summary>
        /// <param name="url">场景文件的资源路径</param>
        /// <param name="mode">场景加载模式</param>
        /// <param name="callback">加载完成回调，参数为已加载的场景资源</param>
        public void LoadSceneWithCallback(string url, LoadSceneMode mode, Action<IResource> callback);

        /// <summary>
        /// 异步加载场景（async/await 方式）
        /// 返回 ResourceAwaiter，支持 await 关键字异步等待场景加载完成
        /// 注意：await 返回的类型是 IResource，可强转为 SceneResource 使用
        /// </summary>
        /// <param name="url">场景文件的资源路径</param>
        /// <param name="mode">场景加载模式</param>
        /// <returns>可被 await 的异步等待对象</returns>
        public ResourceAwaiter LoadSceneWithAwaiter(string url, LoadSceneMode mode = LoadSceneMode.Single);

        /// <summary>
        /// 卸载场景并释放对应的 AssetBundle
        /// 引用计数归零后会在 LateUpdate 中统一执行卸载
        /// </summary>
        /// <param name="resource">由 LoadScene 或 LoadSceneWithCallback 返回的场景资源</param>
        public void UnloadScene(AResource resource);
        #endregion
    }
}