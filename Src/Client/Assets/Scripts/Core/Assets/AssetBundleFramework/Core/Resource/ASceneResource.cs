using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AssetBundleFramework
{
    /// <summary>
    /// 场景资源的同步抽象基类
    /// 继承自 AResource，隐藏所有对场景无意义的 Instantiate/GetAsset 方法
    /// </summary>
    internal abstract class ASceneResource : AResource
    {
        /// <summary>
        /// 场景加载模式：Single 替换所有场景，Additive 叠加到当前场景
        /// </summary>
        protected LoadSceneMode _mode;

        /// <summary>
        /// 场景加载完成回调
        /// 因 C# 泛型委托逆变规则，Action&lt;SceneResource&gt; 不能赋值给 Action&lt;AResource&gt;
        /// 故场景体系单独维护此回调
        /// </summary>
        internal Action<IResource> SceneFinishedCallback { get; set; }

        protected ASceneResource(LoadSceneMode mode)
        {
            _mode = mode;
        }

        #region 隐藏不适用的方法

        /// <summary>
        /// 场景资源不支持 GetAsset 操作，场景通过 SceneManager 管理，没有对应的 Asset 对象
        /// </summary>
        public override T GetAsset<T>()
        {
            throw new InvalidOperationException(
                $"场景资源 [{url}] 不支持 GetAsset<T> 操作。场景通过 SceneManager 加载，没有对应的 UnityEngine.Object 对象");
        }

        /// <summary>
        /// 场景资源不支持 GetAsset 操作（非泛型版本）
        /// </summary>
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
