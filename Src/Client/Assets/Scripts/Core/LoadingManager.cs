using Asset;
using GameInterFace;
using HotUpdate;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using Utilities;

public class LoadingManager : MonoBehaviour
{
    public GameObject UITips;
    public GameObject UILoading;
    public Transform UIRoot;

    public UI.Common.ProgressBar progressBar;
    public TextMeshProUGUI progressText;

    public bool Editor = false;

    private void Awake()
    {
        HybirdCLRManager.Instance._editor = Editor;
        Resloader.Instance._editor = Editor;
        LogInit();
    }


    private IEnumerator Start()
    {
        HotUpdateManager.Instance.OnHotUpdateStart += OnHotUpdateStart;
        HotUpdateManager.Instance.OnDownloadPackVersion += OnDownloadPackVersion;
        HotUpdateManager.Instance.OnComparePackVersion += OnComparePackVersion;
        HotUpdateManager.Instance.OnStartDownload += OnStartDownload;
        HotUpdateManager.Instance.OnOneFileDownload += OnOneFileDownload;
        HotUpdateManager.Instance.OnEndDownload += OnEndDownload;
        yield return StartCoroutine(ShowTips());
        if (Editor)
        {
            Debug.Log("UNITY_EDITOR 运行模式为编辑器模式，不进行热更新操作。");
            Resloader.Instance.OnStart();

            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .FirstOrDefault(t => t.FullName == "HotUpdate.GameEntry");

            if (type != null)
            {
                var method = type.GetMethod("Run", BindingFlags.Static | BindingFlags.Public);
                if (method != null)
                { StartCoroutine((IEnumerator)method.Invoke(null, new object[] { this })); }
            }
        }
        else
        {
            Debug.Log("非编辑器模式，进行热更新操作。");
            HotUpdateManager.Instance.StartHotUpdate();
        }
        
       
    }

    private IEnumerator ShowTips()
    {
        UITips.SetActive(true);
        UILoading.SetActive(false);
        yield return new WaitForSeconds(2f);
        UILoading.SetActive(true);
        yield return new WaitForSeconds(1f);
        UITips.SetActive(false);
    }

    private void LogInit()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");
    }

    /// <summary>
    /// 当热更新开始时
    /// </summary>
    public void OnHotUpdateStart()
    {
        progressText.text = "开始热更新...";
    }

    /// <summary>
    /// 当下载版本信息时
    /// </summary>
    public void OnDownloadPackVersion()
    {
        progressText.text = "下载版本信息...";
    }

    /// <summary>
    /// 当比较版本信息时
    /// </summary>
    public void OnComparePackVersion()
    {
        progressText.text = "比较版本信息...";
    }

    /// <summary>
    /// 当开始下载时，触发时会传入下载文件的总大小
    /// </summary>
    public void OnStartDownload(float size)
    {
        progressText.text = "开始下载...";
        // 没有文件需要下载时，跳过进度条初始化，避免 maxValue=0 导致除零
        if (size <= 0) return;
        this.progressBar.SetData(size, 0, 1);
    }

    /// <summary>
    /// 当单个文件下载完成，触发时会传入下载文件的大小
    /// </summary>
    public void OnOneFileDownload(float size)
    {
        progressText.text = "正在下载...";
        this.progressBar.CurrentValue += size;
    }

    /// <summary>
    /// 当下载完成时
    /// </summary>
    public void OnEndDownload()
    {
        progressBar.UpdateProgress();
        progressText.text = "下载完成!";
        HybirdCLRManager.Instance.Initialize();
        StartCoroutine(HybirdCLRManager.Instance.InvokeStaticMethod<IEnumerator>("HotUpdate", "GameEntry", "Run", this));
    }

    public void ShowLogin()
    {
        UILoading.SetActive(false);
        Resloader.Instance.LoadAssetWithCallback("Assets/AssetBundle/Prefab/UI/UILogin/UILogin.prefab", (res) =>
        {
            res.Instantiate(UIRoot);
        });
    }
}
