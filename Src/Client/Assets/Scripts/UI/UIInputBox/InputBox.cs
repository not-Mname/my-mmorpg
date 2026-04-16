using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBox
{
    static Object _cacheObject = null;
    public static UIInputBox Show(string message, string title = "", string btnOK = "", string btnCancel = "", string emptyTips = "")
    {
        if(_cacheObject == null)
        {
            _cacheObject = Resloader.Load<Object>("Prefab/UI/UIInputBox");
        }

        GameObject go = (GameObject)GameObject.Instantiate(_cacheObject);
        UIInputBox inputBox = go.GetComponent<UIInputBox>();
        inputBox.Init(title, message, btnOK, btnCancel, emptyTips);
        return inputBox;
    }

}
