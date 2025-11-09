using Const;
using Entities;
using Managers;
using Services;
using SkillBridge.Message;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;

    private bool _autoNav;

    public CharacterController characterController;

    public PlayerEnemyCheck Checker;

    CharacterState state;

    public BattleUnit character;

    public float JumpHeight;// 跳跃高度

    public float Gravity;// 重力

    public int Speed;// 速度

    public int RunSpeed;// 奔跑速度

    public EntityController entityController;

    private Vector3 velocity;

    private float v;

    private float h;

    private float cameraDirY;

    public float turnAngle;

    Vector3 lastPos;
    float lastSync = 0;

    #region 回调
    void LateUpdate()
    {
        if (character == null) return;

        if (_autoNav)
        {
            this.NavMove();
            return;
        }

        // 位置同步逻辑
        Vector3 worldPosition = this.characterController.transform.position;
        Vector3Int logicPosition = GameObjectTool.WorldToLogic(worldPosition);

        // 检查位置差异是否超过阈值再同步
        if (Vector3Int.Distance(logicPosition, this.character.Position) > 50)
        {
            this.character.SetPosition(logicPosition);
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = worldPosition;

        // 方向同步逻辑
        Vector3 logicDir = GameObjectTool.LogicToWorld(character.Direction);
        float angleDiff = Vector3.SignedAngle(logicDir, this.transform.forward, Vector3.up);

        if (Mathf.Abs(angleDiff) > turnAngle)
        {
            // 只有当角度偏差超过阈值时，才更新逻辑方向
            this.character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
            this.SendEntityEvent(EntityEvent.None);
        }
    }

    void OnEnable()
    {
        Init();
    }

    void OnDisable()
    {
        EVENT.Unsubscribe(EventId.on_player_lock_target);
    }

    void Update()
    {
        if (!CheckCanMove())
        {
            return;
        }
        UpdateInput();
        Move();
    }
    private void OnLockEnemy(GameObject enemy)
    {
        if (enemy == null || enemy == MainPlayerCamera.Instance.Target)
        {
            MainPlayerCamera.Instance.Target = null;
            return;
        }

        EntityController enemyController = enemy.GetComponent<EntityController>();
        BattleManager.Instance.CurrentTarget = enemyController ? enemyController.entity as BattleUnit : null;
        MainPlayerCamera.Instance.Target = enemy;
    }
    #endregion

    #region 寻路相关
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
    #endregion

    #region 私有函数
    private void Init()
    {
        JumpHeight = 1.5f;
        Gravity = -9.81f;
        Speed = 5;
        RunSpeed = 10;
        turnAngle = 15f;
        if (_agent == null)
        {
            _agent = this.GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = 0.3f;
        }
        _agent.enabled = false;

        if (Checker == null)
        {
            Checker = this.GetComponent<PlayerEnemyCheck>();
        }

        EVENT.Subscribe<GameObject>(EventId.on_player_lock_target, OnLockEnemy);
    }

    private bool CheckCanMove()
    {
        if (character == null || _autoNav || !InputManager.Instance || InputManager.Instance.IsInputMode)
        {
            return false;
        }

        //if (MainPlayerCamera.Instance.isCursorVisible)
        //{
        //    return false;
        //}

        return true;
    }

    private void Move()
    {
        var cameraDir = this.transform.position;
        Debug.Log("position:" + this.transform.position);
        // 计算相对于摄像机方向的移动向量
        // 使用原始代码的输入映射 (v -> x, h -> z) 并将其旋转到摄像机Y轴方向
        Vector3 rawDir = new Vector3(h, 0f, v); // h → X（左右），v → Z（前后）
        Vector3 forwardDir = Quaternion.Euler(0f, cameraDirY, 0f) * rawDir;
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
        this.characterController.Move(forwardDir * Speed * Time.deltaTime);

        if (IsGrounded() && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (InputManager.Instance.KeyValueJump && this.IsGrounded())
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        velocity.y += Gravity * Time.deltaTime;
        this.characterController.Move(velocity * Time.deltaTime); // 应用垂直位移
    }
    private bool IsGrounded()
    {
        float rayLength = 0.3f; // 固定长度，略大于预期缝隙（如台阶、小坡）
        return Physics.CheckSphere(this.transform.position, rayLength, LayerMask.GetMask("Terrain"));
    }

    private void UpdateInput()
    {
        v = InputManager.Instance.KeyValueVertical;
        h = InputManager.Instance.KeyValueHorizontal;
        cameraDirY = MainPlayerCamera.Instance.transform.eulerAngles.y;
        if (InputManager.Instance.KeyValueLockEnemy)
        {
            Checker.Check();
        }
    }

    #endregion

    public void SendEntityEvent(EntityEvent entityEvent, int param = 0)
    {
        if (entityController != null)
            entityController.OnEntityEvent(entityEvent, param);
        MapService.Instance.SendMapEntitySync(entityEvent, this.character.EntityData, param);
    }


}