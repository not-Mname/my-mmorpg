using System;
using UnityEngine.SceneManagement;

namespace AssetBundleFramework
{
    /// <summary>
    /// 同步场景资源加载类
    /// 继承自 ASceneResource，使用 SceneManager.LoadScene 从 AssetBundle 中同步加载场景
    /// 场景资源不需要返回具体的 Asset 对象，加载完成后直接切换或叠加场景
    /// </summary>
    internal class SceneResource : ASceneResource
    {
        public override bool keepWaiting => !done;

        /// <summary>
        /// 初始化同步场景资源
        /// </summary>
        /// <param name="mode">场景加载模式：Single 或 Additive</param>
        public SceneResource(LoadSceneMode mode) : base(mode)
        {
        }

        /// <summary>
        /// 加载场景资源
        /// 流程：查表找到场景所在的 Bundle → 同步加载 Bundle → 使用 SceneManager 加载场景
        /// 加载完成后直接标记 done = true，触发完成回调
        /// </summary>
        internal override void Load()
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException($"{nameof(SceneResource)}.{nameof(Load)}() {nameof(url)} 为空");
            }

            if (Bundle != null)
            {
                throw new Exception($"{nameof(SceneResource)}.{nameof(Load)}() {nameof(Bundle)} 不为空，场景已被加载过，url: {url}");
            }

            // 查找场景所在的 Bundle 路径
            if (!ResourceManager.Instance.ResourceBundleDict.TryGetValue(url, out string bundleUrl))
            {
                throw new Exception($"{nameof(SceneResource)}.{nameof(Load)}() 在 ResourceBundleDict 中找不到 {url} 对应的 Bundle 路径");
            }

            // 同步加载 Bundle（依赖的 Bundle 已在 ResourceManager.LoadSceneInternal 中递归加载完成）
            Bundle = BundleManager.Instance.Load(bundleUrl);

            // 确认加载的是场景 Bundle，防止配置错误
            if (!Bundle.isStreamedSceneAssetBundle)
            {
                throw new Exception($"{nameof(SceneResource)}.{nameof(Load)}() Bundle [{bundleUrl}] 不是场景 Bundle，请检查 BuildSetting.xml 中场景文件的打包规则是否正确");
            }

            // 使用 SceneManager 同步加载场景
            SceneManager.LoadScene(url, _mode);
            done = true;

            // 同步加载完成后立即触发回调
            if (SceneFinishedCallback != null)
            {
                var tempCallback = SceneFinishedCallback;
                SceneFinishedCallback = null;
                tempCallback.Invoke(this);
            }

            // 同时触发基类的 FinishedCallback（供兼容使用）
            if (FinishedCallback != null)
            {
                var tempCallback = FinishedCallback;
                FinishedCallback = null;
                tempCallback.Invoke(this);
            }
        }

        /// <summary>
        /// 卸载场景资源
        /// 流程：异步卸载场景 → 释放 Bundle 引用 → 清理回调
        /// </summary>
        internal override void Unload()
        {
            if (Bundle == null)
            {
                return;
            }

            // 异步卸载场景，避免主线程卡顿
            SceneManager.UnloadSceneAsync(url);

            // 释放 Bundle 引用（依赖的 Bundle 由 ResourceManager.LateUpdate 递归卸载）
            BundleManager.Instance.Unload(Bundle);
            Bundle = null;

            // 清理回调，防止内存泄漏
            FinishedCallback = null;
            SceneFinishedCallback = null;
            Awaiter = null;
        }

        /// <summary>
        /// 场景资源不需要从 Bundle 中加载 Asset
        /// 场景在 Load() 中已通过 SceneManager 完成加载，此方法为无操作
        /// </summary>
        internal override void LoadAsset()
        {
        }
    }
}
