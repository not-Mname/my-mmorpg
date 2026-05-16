using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AssetBundleFramework
{
    /// <summary>
    /// 场景资源的异步抽象基类
    /// 继承自 AResourceAsync，提供异步场景加载的公共轮询逻辑、进度计算和方法隐藏
    /// </summary>
    internal abstract class ASceneResourceAsync : AResourceAsync
    {
        /// <summary>
        /// 场景加载模式：Single 替换所有场景，Additive 叠加到当前场景
        /// </summary>
        protected LoadSceneMode _mode;

        /// <summary>
        /// Unity 异步场景加载操作句柄
        /// 由 SceneManager.LoadSceneAsync 返回，用于跟踪加载进度
        /// </summary>
        protected AsyncOperation _sceneLoadOp;

        /// <summary>
        /// 总体加载进度 0-1
        /// Bundle 阶段占 0-0.2，场景加载阶段占 0.2-1.0
        /// </summary>
        public float Progress { get; protected set; }

        /// <summary>
        /// 场景加载完成回调
        /// 因 C# 泛型委托逆变规则，Action&lt;SceneResource&gt; 不能赋值给 Action&lt;AResource&gt;
        /// 故场景体系单独维护此回调，类型统一为 Action&lt;IResource&gt;
        /// </summary>
        internal Action<IResource> SceneFinishedCallback { get; set; }

        protected ASceneResourceAsync(LoadSceneMode mode)
        {
            _mode = mode;
        }

        #region 异步轮询逻辑

        /// <summary>
        /// 每帧更新异步加载状态
        /// 轮询顺序：依赖资源 → Bundle → 场景加载
        /// 每个阶段完成后自动推进到下一阶段
        /// </summary>
        /// <returns>true 表示加载完成，false 表示仍在加载中</returns>
        internal override bool Update()
        {
            if (done)
            {
                return true;
            }

            // 第一阶段：等待所有依赖资源加载完成
            if (dependencies != null)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    if (!dependencies[i].done)
                    {
                        return false;
                    }
                }
            }

            // 第二阶段：等待 Bundle 异步加载完成
            if (!Bundle.done)
            {
                var bundleAsync = Bundle as BundleAsync;
                if (bundleAsync != null)
                {
                    Progress = bundleAsync.progress * 0.2f;
                }
                return false;
            }

            // 确认加载的是场景 Bundle
            if (!Bundle.isStreamedSceneAssetBundle)
            {
                throw new Exception($"{nameof(ASceneResourceAsync)}.{nameof(Update)}() Bundle [{Bundle.url}] 不是场景 Bundle，无法使用 SceneManager 加载");
            }

            // 第三阶段：Bundle 完成后，启动场景异步加载
            if (_sceneLoadOp == null)
            {
                LoadAssetAsync();
            }

            // 第四阶段：等待场景加载完成
            if (!_sceneLoadOp.isDone)
            {
                Progress = 0.2f + _sceneLoadOp.progress * 0.8f;
                return false;
            }

            // 全部完成
            Progress = 1f;
            LoadAsset();
            return true;
        }

        /// <summary>
        /// 启动场景异步加载
        /// 调用 Unity SceneManager.LoadSceneAsync，返回 AsyncOperation 供 Update 轮询
        /// </summary>
        internal override void LoadAssetAsync()
        {
            if (Bundle == null)
            {
                throw new Exception($"{nameof(ASceneResourceAsync)}.{nameof(LoadAssetAsync)}() {nameof(Bundle)} 为空");
            }

            _sceneLoadOp = SceneManager.LoadSceneAsync(url, _mode);

            if (_sceneLoadOp == null)
            {
                throw new Exception($"{nameof(ASceneResourceAsync)}.{nameof(LoadAssetAsync)}() SceneManager.LoadSceneAsync 返回 null，场景 [{url}] 可能不存在或未正确打包");
            }
        }

        /// <summary>
        /// 标记加载完成并触发回调
        /// 子类可重写以添加额外逻辑（如编辑器模式下的特殊处理）
        /// </summary>
        internal override void LoadAsset()
        {
            done = true;

            if (SceneFinishedCallback != null)
            {
                var tempCallback = SceneFinishedCallback;
                SceneFinishedCallback = null;
                tempCallback.Invoke(this);
            }

            if (FinishedCallback != null)
            {
                var tempCallback = FinishedCallback;
                FinishedCallback = null;
                tempCallback.Invoke(this);
            }
        }

        #endregion

        #region 隐藏不适用的方法

        public override T GetAsset<T>()
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 GetAsset<T> 操作。场景通过 SceneManager 加载，没有对应的 UnityEngine.Object 对象");
        }

        public new Object GetAsset()
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 GetAsset 操作。场景通过 SceneManager 加载，没有对应的 UnityEngine.Object 对象");
        }

        public new GameObject Instantiate()
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 Instantiate 操作。场景本身不能作为预制体实例化，请使用 SceneManager 管理场景");
        }

        public new GameObject Instantiate(bool autoUnload)
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 Instantiate 操作。场景本身不能作为预制体实例化，请使用 SceneManager 管理场景");
        }

        public new GameObject Instantiate(Vector3 position, Quaternion rotation)
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 Instantiate 操作。场景本身不能作为预制体实例化，请使用 SceneManager 管理场景");
        }

        public new GameObject Instantiate(Vector3 position, Quaternion rotation, bool autoUnload)
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 Instantiate 操作。场景本身不能作为预制体实例化，请使用 SceneManager 管理场景");
        }

        public new GameObject Instantiate(Transform parent)
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 Instantiate 操作。场景本身不能作为预制体实例化，请使用 SceneManager 管理场景");
        }

        public new GameObject Instantiate(Transform parent, bool instantiateInWorldSpace)
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 Instantiate 操作。场景本身不能作为预制体实例化，请使用 SceneManager 管理场景");
        }

        public new GameObject Instantiate(Transform parent, bool instantiateInWorldSpace, bool autoUnload)
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 Instantiate 操作。场景本身不能作为预制体实例化，请使用 SceneManager 管理场景");
        }

        public new GameObject Instantiate(Transform parent, Quaternion rotation)
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 Instantiate 操作。场景本身不能作为预制体实例化，请使用 SceneManager 管理场景");
        }

        public new GameObject Instantiate(Transform parent, Quaternion rotation, bool autoUnload)
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 Instantiate 操作。场景本身不能作为预制体实例化，请使用 SceneManager 管理场景");
        }

        #endregion
    }
}
