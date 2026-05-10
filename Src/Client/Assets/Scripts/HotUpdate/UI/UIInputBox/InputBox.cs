using Asset;
using AssetBundleFramework;
using UnityEngine;

public class InputBox
{
    static IResource _cacheRes = null;
    public static UIInputBox Show(string message, string title = "", string btnOK = "", string btnCancel = "", string emptyTips = "")
    {
        if(_cacheRes == null)
        {
            _cacheRes = Resloader.Instance.LoadAssetSync("Assets/AssetBundle/Prefab/UI/UIInputBox");
        }

        GameObject go = _cacheRes.Instantiate(true);
        UIInputBox inputBox = go.GetComponent<UIInputBox>();
        inputBox.Init(title, message, btnOK, btnCancel, emptyTips);
        return inputBox;
    }

}
