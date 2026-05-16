using System;
using UnityEngine.SceneManagement;

namespace AssetBundleFramework
{
    /// <summary>
    /// 异步场景资源加载类
    /// 继承自 ASceneResourceAsync，提供异步场景加载的具体实现
    /// 基类已处理轮询逻辑、进度计算和方法隐藏，此类仅实现 Load/Unload 的具体流程
    /// </summary>
    internal class SceneResourceAsync : ASceneResourceAsync
    {
        public override bool keepWaiting => !done;

        /// <summary>
        /// 初始化异步场景资源
        /// </summary>
        /// <param name="mode">场景加载模式：Single 或 Additive</param>
        public SceneResourceAsync(LoadSceneMode mode) : base(mode)
        {
        }

        /// <summary>
        /// 启动异步场景加载
        /// 异步加载场景所在的 Bundle（依赖的 Bundle 已在 ResourceManager.LoadSceneInternal 中递归加载）
        /// 后续的 Bundle 轮询和场景异步加载由基类 Update() 和 LoadAssetAsync() 处理
        /// </summary>
        internal override void Load()
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception($"{nameof(SceneResourceAsync)}.{nameof(Load)}() {nameof(url)} 为空");
            }

            if (Bundle != null)
            {
                throw new Exception($"{nameof(SceneResourceAsync)}.{nameof(Load)}() {nameof(Bundle)} 不为空，场景已被加载过，url: {url}");
            }

            // 查找场景所在的 Bundle 路径
            if (!ResourceManager.Instance.ResourceBundleDict.TryGetValue(url, out string bundleUrl))
            {
                throw new Exception($"{nameof(SceneResourceAsync)}.{nameof(Load)}() 在 ResourceBundleDict 中找不到 {url} 对应的 Bundle 路径");
            }

            // 异步加载 Bundle，后续在基类 Update() 中轮询完成
            Bundle = BundleManager.Instance.LoadAsync(bundleUrl);
        }

        /// <summary>
        /// 卸载场景并释放资源
        /// 先清理正在进行的异步操作，再异步卸载场景，释放 Bundle 引用
        /// </summary>
        internal override void Unload()
        {
            if (Bundle == null)
            {
                return;
            }

            // 如果有正在进行的场景加载，取消跟踪
            if (_sceneLoadOp != null)
            {
                _sceneLoadOp = null;
            }

            // 异步卸载场景，避免主线程卡顿
            SceneManager.UnloadSceneAsync(url);

            // 释放 Bundle 引用
            BundleManager.Instance.Unload(Bundle);
            Bundle = null;

            // 清理回调和请求，防止内存泄漏
            FinishedCallback = null;
            SceneFinishedCallback = null;
            Awaiter = null;
        }
    }
}
