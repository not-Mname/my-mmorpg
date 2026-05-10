using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    internal class UIPopupText : MonoBehaviour
    {
        public float FloatTime = 0.5f;
        public TextMeshProUGUI Text;
        public Transform player;

        public void Init(PopupType type, float number, bool isCritical, Transform player)
        {
            this.player = player;
            this.gameObject.SetActive(false);
            Text.text = number.ToString("0");
            if (number > 0)
            {//治疗
                Text.color = Color.green;
            }
            else if (isCritical)
            {//暴击
                Text.color = Color.red;
            }
            else
            {//普通伤害
                Text.color = Color.yellow;
            }

            float time = FloatTime + Random.Range(0, 0.5f);
            float height = Random.Range(0.5f, 1f);
            float x = Random.Range(-0.5f, 0.5f);
            float y = Random.Range(-0.5f, 0.5f);
            x += Mathf.Sign(x)*0.3f;
            y += Mathf.Sign(y) * 0.3f;
            this.gameObject.SetActive(true);
            transform.DOMove(new Vector3(transform.position.x + x, transform.position.y + height, transform.position.z + y), time).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }

        void Update()
        {
            transform.forward = player.forward;
        }
    }

    public enum PopupType
    {
        None,
        Damage,
        Heal,
    }
}
