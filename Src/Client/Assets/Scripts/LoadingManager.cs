using GameInterFace;
using Managers;
using Services;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TMPro;
using UI.Common;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class LoadingManager : MonoBehaviour
{
    public GameObject UITips;
    public GameObject UILoading;
    public GameObject UILogin;

    public ProgressBar progressBar;
    public TextMeshProUGUI progressText;

    // Use this for initialization
    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        UITips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        UILoading.SetActive(true);
        yield return new WaitForSeconds(1f);
        UITips.SetActive(false);
        progressBar.OnValueChanged += OnLoadFinish;
        progressBar.SetData(100f, 0f, 2f);
        progressText.text = "正在加载配置数据...";
        yield return DataManager.Instance.LoadData();
        progressBar.CurrentValue += 10;
        progressText.text = "正在初始化系统...";
        //Init basic services
        var wait = new WaitForSeconds(0.2f);

        var types = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t =>
            t.IsClass &&
            !t.IsAbstract &&
            typeof(IInitializable).IsAssignableFrom(t) // 非泛型接口判断
        )
        .ToList();
        foreach (var type in types)
        {
            // 递归查找 Instance 属性，包括父类
            PropertyInfo instanceProperty = null;
            var currentType = type;
            // 向上查找直到找到 Instance 属性或到达 Object 类为止
            while (currentType != null && instanceProperty == null)
            {
                instanceProperty = currentType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                currentType = currentType.BaseType;
            }

            if (instanceProperty != null)
            {

                var instance = instanceProperty.GetValue(null);
                if (instance is IInitializable initializable) // 直接转换为接口
                {
                    initializable.Init(); // 直接调用方法
                    progressBar.CurrentValue += (progressBar.MaxValue + 10) / types.Count;
                    yield return wait;
                }
            }
            else
            {
                LogHelper.LogError($"No Instance property found for {type.Name}");
            }

            SoundManager.Instance.PlayMusic(SoundDefine.Music_Login);
            progressBar.CurrentValue = progressBar.MaxValue;
            yield return wait;
        }
    }

    private void OnLoadFinish(float value)
    {
        if (value >= 0.98f)
        {
            UILoading.SetActive(false);
            UILogin.SetActive(true);
            progressBar.OnValueChanged -= OnLoadFinish;
            this.progressText.text = "加载完成!";
        }
    }
}
