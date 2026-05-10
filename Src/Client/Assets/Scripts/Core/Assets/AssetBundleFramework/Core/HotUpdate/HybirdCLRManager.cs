using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommonUtility;
using UnityEngine;

namespace HotUpdate
{
    /// <summary>
    /// HybridCLR 热更新管理器
    /// </summary>
    public class HybirdCLRManager : MonoSingleton<HybirdCLRManager>
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static string platfrom = "Windows";
#elif UNITY_ANDROID
    public static string platfrom = "Android";
#elif UNITY_IPHONE
    public static string platfrom = "IOS";
#endif
        [HideInInspector]
        public bool _editor = false;
        /// <summary>
        /// 热更新 DLL 文件路径 (在 StreamingAssets 或 PersistentData 目录)
        /// </summary>
        private string _hotUpdateDllPath => Path.Combine(Application.persistentDataPath, $"AssetBundle/{platfrom}/HotUpdate");

        /// <summary>
        /// 加载的热更新程序集
        /// </summary>
        private Dictionary<string, Assembly> _hotUpdateAssemblies = new();

        /// <summary>
        /// 初始化并加载热更新 DLL
        /// </summary>
        public void Initialize()
        {
            if (_editor) { return; }
            Debug.Log("开始初始化 HybridCLR 热更新...");

            // 检查 DLL 文件是否存在
            if (!Directory.Exists(_hotUpdateDllPath))
            {
                Debug.LogError($"未找到热更新 DLL 文件夹：{_hotUpdateDllPath}");
                return;
            }

            try
            {
                DirectoryInfo info = new DirectoryInfo(_hotUpdateDllPath);
                foreach (FileInfo file in info.GetFiles("*.dll.bytes", SearchOption.AllDirectories))
                {
                    string assemblyName = Path.GetFileNameWithoutExtension(file.Name);
                    if (AssemblyExistsInAppDomain(assemblyName))
                    {
                        Debug.Log($"跳过已存在的程序集：{assemblyName}");
                        continue;
                    }
                    string pdbPath = file.FullName.Replace(".dll.bytes", ".pdb.bytes");

                    byte[] dllBytes = File.ReadAllBytes(file.FullName);
                    byte[] pdbBytes = File.ReadAllBytes(pdbPath);
                    Assembly dll = null;
                    if (File.Exists(pdbPath))
                    {
                         dll = Assembly.Load(dllBytes, pdbBytes);
                        Debug.Log($"成功加载热更新程序dll和pdb：{file.FullName}");
                    }
                    else
                    {
                         dll=Assembly.Load(dllBytes);
                        Debug.Log($"成功加载热更新程序dll，pdb加载失败：{file.FullName}");

                    }

                    _hotUpdateAssemblies.Add(dll.GetName().Name, dll);

                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"加载热更新 DLL 失败：{ex.Message}\n{ex.StackTrace}");
            }
        }

        private bool AssemblyExistsInAppDomain(string name)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name == name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取热更新程序集中的类型
        /// </summary>
        /// <param name="typeName">完整类型名 (包含命名空间)</param>
        /// <returns>类型对象</returns>
        public Type GetHotUpdateType(string p_assembly, string typeName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name == p_assembly)
                    return asm.GetType($"{p_assembly}.{typeName}");
            }

            if (_hotUpdateAssemblies.TryGetValue(p_assembly, out Assembly hotfixAsm))
                return hotfixAsm.GetType($"{p_assembly}.{typeName}");

            return null;
        }

        /// <summary>
        /// 调用热更新程序集的静态方法
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="typeName">类型全名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">方法参数</param>
        /// <returns>方法返回值</returns>
        public T InvokeStaticMethod<T>(string assembly, string typeName, string methodName, params object[] parameters)
        {
            Type type = GetHotUpdateType(assembly, typeName);
            if (type == null)
            {
                Debug.LogError($"未找到类型：{typeName}");
                return default;
            }

            MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            if (method == null)
            {
                Debug.LogError($"未找到方法：{methodName}");
                return default;
            }

            try
            {
                return (T)method.Invoke(null, parameters);
            }
            catch (TargetInvocationException ex)
            {
                Debug.LogError($"HotUpdate异常:\n{ex.InnerException}");
                throw;
            }
        }
    }
}