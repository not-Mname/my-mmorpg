using Asset;
using AssetBundleFramework;
using UnityEngine;

public class InputBox
{
    static IResource _cacheRes = null;
    public static UIInputBox Show(string message, string title = "", string btnOK = "", string btnCancel = "", string emptyTips = "")
    {
        if(_cacheRes == null || _cacheRes.GetAsset() == null)
        {//如果没有缓存资源，或者缓存资源被卸载了，就重新加载
            _cacheRes = Resloader.Instance.LoadAssetSync("Assets/AssetBundle/Prefab/UI/UIInputBox.prefab");
        }

        GameObject go = _cacheRes.Instantiate();
        UIInputBox inputBox = go.GetComponent<UIInputBox>();
        inputBox.Init(title, message, btnOK, btnCancel, emptyTips);
        return inputBox;
    }

}
