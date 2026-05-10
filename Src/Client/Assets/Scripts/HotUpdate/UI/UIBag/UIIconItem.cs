using Asset;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    public Image main;
    public Text mainText;
    
    public void SetMainIcon(string iconName, string text)
    {
        main.overrideSprite = Resloader.Instance.LoadAssetSync(iconName).GetAsset<Sprite>();
        mainText.text = text;
    }

   
}
