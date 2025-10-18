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

    public CharacterController characterController;

    CharacterState state;

    public BattleUnit character;

    public float rotateSpeed = 2.0f;

    public float turnAngle = 10;

    public float jumpHeight = 1.5f;

    public float gravity = -9.81f;

    public int speed;

    public EntityController entityController;

    private Vector3 velocity;

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
            _agent.speed = this.character.Speed / 100f;
        }

    }

    public void StopNav()
    {
        _autoNav = false;
        _agent.ResetPath();
        if (state != CharacterState.Idle)
        {
            state = CharacterState.Idle;
            this.characterController.Move(Vector3.zero);
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

    void Update()
    {
        if (character == null || _autoNav|| !InputManager.Instance || InputManager.Instance.IsInputMode)
            return;

        float v = InputManager.Instance.KeyValueVertical;
        float h = -InputManager.Instance.KeyValueHorizontal;
        float cameraDir = MainPlayerCamera.Instance.transform.eulerAngles.z;
        Vector3 forwardDir = new Vector3(v, 0, h);
        forwardDir = forwardDir.normalized;

        if (v != 0 || h != 0)
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            this.transform.forward = forwardDir;
        }
        else
        {
            if (state != CharacterState.Idle)
            {
                state = CharacterState.Idle;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }
        this.characterController.Move(forwardDir * speed);

        if (this.characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 接地修正
        }

        if (Input.GetButtonDown("Jump") && this.characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        this.characterController.Move(velocity * Time.deltaTime); // 应用垂直位移
    }

    Vector3 lastPos;
    float lastSync = 0;

    void LateUpdate()
    {
        //if (character == null) return;

        //if (_autoNav)
        //{
        //    this.NavMove();
        //    return;
        //}

        //Vector3 offset = this.transform.position - lastPos;
        //this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        //this.lastPos = this.characterController.transform.position;
        //if ((GameObjectTool.WorldToLogic(this.characterController.transform.position) - this.character.Position).magnitude > 50)
        //{
        //    this.character.SetPosition(GameObjectTool.WorldToLogic(this.characterController.transform.position));
        //    this.SendEntityEvent(EntityEvent.None);
        //}
        //this.transform.position = this.characterController.transform.position;
        //Vector3 dir = GameObjectTool.LogicToWorld(character.Direction);
        //Quaternion rot = new Quaternion();
        //rot.SetFromToRotation(dir, this.transform.forward);

        //if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
        //{
        //    this.character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
        //    this.SendEntityEvent(EntityEvent.None);
        //}
    }

    public void SendEntityEvent(EntityEvent entityEvent, int param = 0)
    {
        if (entityController != null)
            entityController.OnEntityEvent(entityEvent, param);
        MapService.Instance.SendMapEntitySync(entityEvent, this.character.EntityData, param);
    }
}
