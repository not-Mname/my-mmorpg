using Asset;
using AssetBundleFramework;
using UnityEngine;

class MessageBox
{
    static IResource _cacheRes = null;

    public static UIMessageBox Show(string message, string title="", MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if (_cacheRes == null || _cacheRes.GetAsset() == null)
        {//如果没有缓存资源，或者缓存资源被卸载了，就重新加载
            _cacheRes = Resloader.Instance.LoadAssetSync("Assets/AssetBundle/Prefab/UI/UIMessageBox.prefab");
        }

        GameObject go = _cacheRes.Instantiate();
        UIMessageBox msgbox = go.GetComponent<UIMessageBox>();
        msgbox.Init(title, message, type, btnOK, btnCancel);
        return msgbox;
    }
}

public enum MessageBoxType
{
    /// <summary>
    /// Information Dialog with OK button
    /// </summary>
    Information = 1,

    /// <summary>
    /// Confirm Dialog whit OK and Cancel buttons
    /// </summary>
    Confirm = 2,

    /// <summary>
    /// Error Dialog with OK buttons
    /// </summary>
    Error = 3
}