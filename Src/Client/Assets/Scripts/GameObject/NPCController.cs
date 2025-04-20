using Assets.Scripts.Models;
using Common.Data;
using Managers;
using Models;
using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public int npcId;

    Animator animator;

    NPCDefine npcDefine;

    public bool isInteractive = false;

    SkinnedMeshRenderer renderer;

    Color originalColor;

    NPCQuestStatus npcQuestStatus;

    void OnMouseDown()
    {
        if(Vector3.Distance(User.Instance.CurrentCharacterObject.transform.position, this.transform.position) > 2f)
        {
            User.Instance.CurrentCharacterObject.StartNav(this.transform.position);
            return;
        }
        Interactive();
    }

    void OnMouseOver()
    {
        Highlighte(true);
    }
    void OnMouseExit()
    {
        Highlighte(false);
    }

    void OnMouseEnter()
    {
        Highlighte(true);
    }

    void Highlighte(bool highlight)
    {
        if (highlight)
        {
            if (renderer.sharedMaterial.color != Color.white)
                renderer.material.color = Color.white;
        }
        else
        {
            if (renderer.sharedMaterial.color != originalColor)
                renderer.material.color = originalColor;
        }
    }

    void Interactive()
    {
        if (!isInteractive)
        {
            isInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }

    IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();
        if (NPCManager.Instance.Interactive(npcId))
        {
            animator.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(2f);
        isInteractive = false;

    }

    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(faceTo, this.gameObject.transform.forward)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    IEnumerator Actions()
    {
        if (!isInteractive) yield return new WaitForSeconds(2f);
        else yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
        this.Relax();
    }

    void Relax()
    {
        this.animator.SetTrigger("Relax");
    }


    void Start()
    {
        animator = GetComponent<Animator>();
        npcDefine = NPCManager.Instance.GetNPCDefine(npcId);
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        originalColor = renderer.material.color;
        NPCManager.Instance.UpdateNPCPosition(npcId, this.transform.position);
        this.StartCoroutine(Actions());
        RefreshNPCStatus();
        QuestManager.Instance.onQuestStatusChanged += OnNPCQuestStatusChanged;
    }
    void OnNPCQuestStatusChanged(Quest quest)
    {
        this.RefreshNPCStatus();
    }
    void RefreshNPCStatus()
    {
        npcQuestStatus = QuestManager.Instance.GetQuestStatusByNpc(npcId);
        if (npcQuestStatus == NPCQuestStatus.None) return;
            UIWouldElementManager.Instance.AddStatusElement(this.transform, npcQuestStatus);
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        QuestManager.Instance.onQuestStatusChanged -= OnNPCQuestStatusChanged;

        if (UIWouldElementManager.Instance != null)
        {
            UIWouldElementManager.Instance.RemoveStatusElement(this.transform);
        }
    }
}
