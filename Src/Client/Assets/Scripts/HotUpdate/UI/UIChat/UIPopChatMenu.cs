using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopChatMenu : UIWindow, IDeselectHandler
{
    public int TargetId;
    public string TargetName;

    public void OnDeselect(BaseEventData eventData)
    {
        var ed = eventData as PointerEventData;
        if (ed.hovered.Contains(this.gameObject))//如果点击的对象是自己，则不关闭弹窗
            return;
        this.Close(WindowResult.None);
    }
    public void OnEnable()
    {
        this.GetComponent<Selectable>().Select();//强行设置为已选择上面的逻辑才能正常执行
        this.Root.transform.position = Input.mousePosition + new Vector3(80, 0, 0);
    }

    public void OnClickChat()
    {
        ChatManager.Instance.StartPrivateChat(TargetId, TargetName);
        this.Close(WindowResult.No);
    }
    public void OnClickFriend()
    {
        this.Close(WindowResult.No);
    }

    public void OnClickTeam()
    {
        this.Close(WindowResult.No);
    }
}
