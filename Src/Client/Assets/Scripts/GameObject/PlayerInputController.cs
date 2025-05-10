using Entities;
using Managers;
using Services;
using SkillBridge.Message;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInputController : MonoBehaviour
{
    private NavMeshAgent _agent;

    private bool _autoNav;

    public Rigidbody rb;

    SkillBridge.Message.CharacterState state;

    public Character character;

    public float rotateSpeed = 2.0f;

    public float turnAngle = 10;

    public int speed;

    public EntityController entityController;

    public bool onAir = false;

    void Start()
    {
        if (_agent == null)
        {
            _agent = this.GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = 0.3f;
        }
        _agent.enabled = false;
    }

    public void StartNav(Vector3 target)
    {
        _agent.enabled = true;
        StartCoroutine(BeginNav(target));
    }
    IEnumerator BeginNav(Vector3 target)
    {

        _agent.SetDestination(target);
        yield return null;
        _autoNav = true;
        if (state != CharacterState.Move)
        {
            state = CharacterState.Move;
            this.character.MoveForward();
            this.SendEntityEvent(EntityEvent.MoveFwd);
            _agent.speed = this.character.speed / 100f;
        }

    }
    public void StopNav()
    {
        _autoNav = false;
        _agent.ResetPath();
        if (state != CharacterState.Idle)
        {
            state = CharacterState.Idle;
            this.rb.velocity = Vector3.zero;
            this.character.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
        }
        NavPathRenderer.Instance.SetPath(null, Vector3.zero);
        _agent.enabled = false;
    }
    public void NavMove()
    {
        if (_agent.pathPending) return;
        if (_agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            StopNav();
            return;
        }
        if (_agent.pathStatus != NavMeshPathStatus.PathComplete) return;

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            StopNav();
            return;
        }
        NavPathRenderer.Instance.SetPath(_agent.path, _agent.destination);
        if (_agent.isStopped || _agent.remainingDistance < 2f)
        {
            StopNav();
            return;
        }
    }
    void FixedUpdate()
    {
        if (character == null)
            return;

        if (InputManager.Instance != null && InputManager.Instance.IsInputMode) return;

        if (_autoNav) return;

        float v = Input.GetAxis("Vertical");
        if (v > 0.01)
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else if (v < -0.01)
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else
        {
            if (state != SkillBridge.Message.CharacterState.Idle)
            {
                state = SkillBridge.Message.CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }

        float h = Input.GetAxis("Horizontal");
        if (h > 0.01 || h < -0.01)
        {
            this.transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            if (rot.eulerAngles.y > turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
            {
                character.direction = GameObjectTool.WorldToLogic(this.transform.forward);
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }
        }
    }

    Vector3 lastPos;
    float lastSync = 0;

    void LateUpdate()
    {
        if (character == null) return;

        if (_autoNav)
        {
            this.NavMove();
            return;
        }

        Vector3 offset = this.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        this.lastPos = this.rb.transform.position;
        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 50)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;
        Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir, this.transform.forward);

        if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
        {
            this.character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
            this.SendEntityEvent(EntityEvent.None);
        }
    }

    public void SendEntityEvent(EntityEvent entityEvent, int param = 0)
    {
        if (entityController != null)
            entityController.OnEntityEvent(entityEvent, param);
        MapService.Instance.SendMapEntitySync(entityEvent, this.character.EntityData, param);
    }
}
