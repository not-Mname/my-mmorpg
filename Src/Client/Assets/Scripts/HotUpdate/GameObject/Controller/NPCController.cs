using Assets.Scripts.Models;
using Common.Data;
using Managers;
using Models;
using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public int NpcId;

    public Animator Animator;

    public bool IsInteractive = false;
    private NPCDefine _npcDefine;

    private SkinnedMeshRenderer _renderer;

    private Color _originalColor;

    private NPCQuestStatus _npcQuestStatus;

    void OnMouseDown()
    {
        if(Vector3.Distance(User.Instance.CurrentCharacterObject.transform.position, this.transform.position) > 2f)
        {
            User.Instance.CurrentCharacterObject.StartNav(this.transform.position);
            return;
        }
        Interactive();
    }

    //void OnMouseOver()
    //{
    //    Highlighte(true);
    //}
    //void OnMouseExit()
    //{
    //    Highlighte(false);
    //}

    //void OnMouseEnter()
    //{
    //    Highlighte(true);
    //}

    void Highlighte(bool highlight)
    {
        if (highlight)
        {
            if (_renderer.sharedMaterial.color != Color.white)
                _renderer.material.color = Color.white;
        }
        else
        {
            if (_renderer.sharedMaterial.color != _originalColor)
                _renderer.material.color = _originalColor;
        }
    }

    void Interactive()
    {
        if (!IsInteractive)
        {
            IsInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }

    IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();
        if (NPCManager.Instance.Interactive(NpcId))
        {
            Animator.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(2f);
        IsInteractive = false;

    }

    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        faceTo.y = 0;
        while (Mathf.Abs(Vector3.Angle(faceTo, this.gameObject.transform.forward)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    IEnumerator Actions()
    {
        if (!IsInteractive) yield return new WaitForSeconds(2f);
        else yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
        this.Relax();
    }

    void Relax()
    {
        this.Animator.SetTrigger("Relax");
    }


    void OnEnable()
    {
        if(Animator == null)
        {
            Animator = GetComponentInChildren<Animator>();


        }
        _npcDefine = NPCManager.Instance.GetNPCDefine(NpcId);
        _renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        _originalColor = _renderer.material.color;
        NPCManager.Instance.UpdateNPCPosition(NpcId, this.transform.position);
        this.StartCoroutine(Actions());
        RefreshNPCStatus();
        QuestManager.Instance.onQuestStatusChanged += OnNPCQuestStatusChanged;
    }
 

    void Update()
    {

    }

    void OnDisable()
    {
        QuestManager.Instance.onQuestStatusChanged -= OnNPCQuestStatusChanged;

        if (UIWouldElementManager.Instance != null)
        {
            UIWouldElementManager.Instance.RemoveStatusElement(this.transform);
        }
    }
    void OnNPCQuestStatusChanged(Quest quest)
    {
        this.RefreshNPCStatus();
    }
    void RefreshNPCStatus()
    {
        _npcQuestStatus = QuestManager.Instance.GetQuestStatusByNpc(NpcId);
        if (_npcQuestStatus == NPCQuestStatus.None) return;
        UIWouldElementManager.Instance.AddStatusElement(this.transform, _npcQuestStatus);
    }
}
