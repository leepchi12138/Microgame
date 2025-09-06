using UnityEngine;

public class lasting2021 : MonoBehaviour
{
    // 基础组件引用
    public Rigidbody Player;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask Ground;

    // 速度参数（走路和跑步速度设为0，依赖动画位移）
    [SerializeField] public float walkSpeed = 0f;         // 行走速度（依赖动画位移）
    [SerializeField] public float runSpeed = 0f;          // 奔跑速度（依赖动画位移）
    [SerializeField] public float slowdownRate = 4f;      // 减速速率

    // 翻滚参数
    [SerializeField] public float dashRollDuration = 1f;  // 加速翻滚
    [SerializeField] public float evadeRollDuration = 0.4f; // 躲避翻滚持续时间
    [SerializeField] public float invincibilityTime = 0.3f; // 无敌时间

    // 原地跳跃参数
    public float targetJumpHeight = 1.5f;                 // 原地跳跃高度
    public float jumpDuration = 0.5f;
    public AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // 跑步跳跃高度（设为0，依赖动画）
    public float runningJumpHeight = 0f;

    // 状态变量
    private bool isJumping = false;          // 原地跳跃状态
    private bool isDashRolling = false;      // 加速翻滚状态（跑图）
    private bool isEvadeRolling = false;     // 躲避翻滚状态（无敌帧）
    private bool isInvincible = false;       // 无敌状态
    private float jumpStartTime;
    private float dashRollStartTime;
    private float evadeRollStartTime;
    private bool isRunningJump = false;      // 标记是否为跑步中跳跃

    // 输入状态
    private bool wasWKeyDown = false;        // 上一帧W键状态
    private bool wasShiftDown = false;       // 上一帧Shift键状态
    private bool wasCtrlDown = false;        // 上一帧Ctrl键状态
    private bool wasCKeyDown = false;        // 上一帧C键状态
    private bool isJumpKeyPressed = false;   // 跳跃键按下标记
    private bool isDashRollKeyPressed = false; // 加速翻滚键按下标记
    private bool isEvadeRollKeyPressed = false; // 躲避翻滚键按下标记

    // 移动状态
    private bool isWalking = false;          // 行走状态
    private bool isRunning = false;          // 奔跑状态
    private bool wasMovingBeforeJump = false; // 记录跳跃前是否在移动

    //鼠标相关
    public float mouseSensitivity = 100f;  // 鼠标灵敏度
    public bool lockCursor = true;         // 是否锁定鼠标
    private float yRotation = 0f;          // 角色Y轴旋转角度

    // 当前速度（仅用于动画状态判断，不控制位移）
    protected float currentSpeed;
    protected bool isGrounded = true;
    protected bool wasGrounded = false;

    // 新增：记录跳跃起始位置
    private Vector3 jumpStartPosition;

    public virtual void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!Player) Player = GetComponent<Rigidbody>();

        Player.freezeRotation = true;
        currentSpeed = 0f;

        // 锁定并隐藏鼠标光标
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public virtual void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, Ground);

        // 更新无敌状态
        UpdateInvincibility();

        // 处理输入
        HandleInput();

        // 处理着陆逻辑
        if (!wasGrounded && isGrounded)
        {
            OnLand();
        }

        // 处理跳跃和翻滚逻辑
        HandleJump();
        HandleDashRoll();
        HandleEvadeRoll();

        // 更新动画
        UpdateAnimation();
        // 鼠标控制角色旋转
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void UpdateInvincibility()
    {
        if (isInvincible && (Time.time - evadeRollStartTime) >= invincibilityTime)
        {
            isInvincible = false;
        }
    }

    private void HandleInput()
    {
        bool isWKeyDown = Input.GetKey(KeyCode.W);
        bool isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isCtrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool isCKeyDown = Input.GetKey(KeyCode.C);
        bool isJumpKeyDown = Input.GetKeyDown(KeyCode.Space);

        // 检测按键状态变化
        bool wKeyJustPressed = isWKeyDown && !wasWKeyDown;
        bool shiftJustPressed = isShiftDown && !wasShiftDown;
        bool shiftJustReleased = !isShiftDown && wasShiftDown;
        bool wKeyJustReleased = !isWKeyDown && wasWKeyDown;
        bool ctrlJustPressed = isCtrlDown && !wasCtrlDown;
        bool cKeyJustPressed = isCKeyDown && !wasCKeyDown;

        // 仅在非翻滚状态下处理移动输入（不控制位移，仅更新状态）
        if (!isDashRolling && !isEvadeRolling)
        {
            // 立即响应按键变化（仅更新动画状态，不控制位移）
            if (wKeyJustPressed)
            {
                isWalking = true;
                isRunning = false;
                currentSpeed = walkSpeed;
            }
            else if (shiftJustPressed && isWKeyDown)
            {
                isWalking = false;
                isRunning = true;
                currentSpeed = runSpeed;
            }
            else if (shiftJustReleased && isRunning)
            {
                isRunning = false;
                isWalking = true;
                currentSpeed = walkSpeed;
            }
            else if (wKeyJustReleased)
            {
                isWalking = false;
                isRunning = false;
                currentSpeed = 0f;
            }
        }

        // 记录当前按键状态
        wasWKeyDown = isWKeyDown;
        wasShiftDown = isShiftDown;
        wasCtrlDown = isCtrlDown;
        wasCKeyDown = isCKeyDown;

        // 处理跳跃按键
        if (isJumpKeyDown && isGrounded && !isDashRolling && !isEvadeRolling)
        {
            isRunningJump = isRunning; // 标记是否为跑步中跳跃
            if (!isWalking && !isRunning)
            {
                // 原地跳跃
                isJumpKeyPressed = true;
            }
            else
            {
                // 移动中跳跃
                wasMovingBeforeJump = true;
                animator.SetTrigger("Jump");
                currentSpeed = 0f;
            }
        }

        // 处理加速翻滚按键（Ctrl，用于跑图）
        if (ctrlJustPressed && isGrounded && isRunning && !isJumping && !isDashRolling && !isEvadeRolling)
        {
            isDashRollKeyPressed = true;
        }

        // 处理躲避翻滚按键（C，用于无敌帧）
        if (cKeyJustPressed && isGrounded && isRunning && !isJumping && !isDashRolling && !isEvadeRolling)
        {
            isEvadeRollKeyPressed = true;
        }
    }

    public virtual void HandleJump()
    {
        if (isJumpKeyPressed && isGrounded && !isWalking && !isRunning && !isDashRolling && !isEvadeRolling)
        {
            isJumpKeyPressed = false;
            StartJump();
            animator.SetTrigger("Jump");
        }

        if (isJumping)
        {
            ExecuteJump();
        }
    }

    public virtual void HandleDashRoll()
    {
        if (isDashRollKeyPressed && isGrounded && isRunning && !isJumping)
        {
            isDashRollKeyPressed = false;
            StartDashRoll();
            animator.SetTrigger("Roll"); // 触发加速翻滚动画
        }

        // 基于时间控制翻滚状态持续时间
        if (isDashRolling && (Time.time - dashRollStartTime) >= dashRollDuration)
        {
            EndDashRoll();
        }
    }

    public virtual void HandleEvadeRoll()
    {
        if (isEvadeRollKeyPressed && isGrounded && isRunning && !isJumping)
        {
            isEvadeRollKeyPressed = false;
            StartEvadeRoll();
            animator.SetTrigger("C"); // 触发躲避翻滚动画（无敌帧）
        }

        // 基于时间控制翻滚状态持续时间
        if (isEvadeRolling && (Time.time - evadeRollStartTime) >= evadeRollDuration)
        {
            EndEvadeRoll();
        }
    }

    public virtual void StartDashRoll()
    {
        isDashRolling = true;
        dashRollStartTime = Time.time;

        // 禁用重力和刚体控制，让动画控制位移
        Player.useGravity = false;
        Player.velocity = Vector3.zero;
        Player.isKinematic = true;
    }

    public virtual void StartEvadeRoll()
    {
        isEvadeRolling = true;
        isInvincible = true;
        evadeRollStartTime = Time.time;

        // 禁用重力和刚体控制，让动画控制位移
        Player.useGravity = false;
        Player.velocity = Vector3.zero;
        Player.isKinematic = true;
    }

    public virtual void EndDashRoll()
    {
        isDashRolling = false;

        // 恢复重力和刚体控制
        Player.useGravity = true;
        Player.isKinematic = false;

        // 使用当前W键状态恢复动画状态
        if (Input.GetKey(KeyCode.W))
        {
            isRunning = true;
            currentSpeed = runSpeed;
        }
        else
        {
            isRunning = false;
            isWalking = false;
            currentSpeed = 0f;
        }
    }

    public virtual void EndEvadeRoll()
    {
        isEvadeRolling = false;

        // 恢复重力和刚体控制
        Player.useGravity = true;
        Player.isKinematic = false;

        // 使用当前W键状态恢复动画状态
        if (Input.GetKey(KeyCode.W))
        {
            isRunning = true;
            currentSpeed = runSpeed;
        }
        else
        {
            isRunning = false;
            isWalking = false;
            currentSpeed = 0f;
        }
    }

    public virtual void OnLand()
    {
        isJumping = false;
        animator.SetTrigger("Land");

        // 落地后恢复移动状态
        if (wasMovingBeforeJump && Input.GetKey(KeyCode.W))
        {
            isWalking = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            currentSpeed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runSpeed : walkSpeed;
            wasMovingBeforeJump = false;
        }
        else
        {
            isRunning = false;
            isWalking = false;
            currentSpeed = 0f;
        }
    }

    public virtual void UpdateAnimation()
    {
        animator.SetBool("Idle", !isWalking && !isRunning && !isDashRolling && !isEvadeRolling);
        animator.SetBool("Walk", isWalking && !isDashRolling && !isEvadeRolling);
        animator.SetBool("Run", isRunning && !isDashRolling && !isEvadeRolling);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("Height", Player.velocity.y);
    }

    public virtual void StartJump()
    {
        isJumping = true;
        jumpStartTime = Time.time;
        // 记录跳跃起始位置
        jumpStartPosition = transform.position;

        // 禁用重力和刚体控制，根据跳跃类型设置高度
        Player.useGravity = false;
        Player.velocity = Vector3.zero;
        Player.isKinematic = true;
    }

    public virtual void ExecuteJump()
    {
        float elapsedTime = Time.time - jumpStartTime;
        float progress = Mathf.Clamp01(elapsedTime / jumpDuration);

        // 根据是否为原地跳跃设置不同高度
        float jumpHeight = isRunningJump ? runningJumpHeight : targetJumpHeight;
        float heightProgress = jumpCurve.Evaluate(progress);

        // 计算绝对位置（关键修改点）
        Vector3 newPosition = new Vector3(
            jumpStartPosition.x,
            jumpStartPosition.y + jumpHeight * heightProgress,
            jumpStartPosition.z
        );

        Player.MovePosition(newPosition);

        if (progress >= 1.0f)
        {
            EndJump();
        }
    }

    public virtual void EndJump()
    {
        isJumping = false;
        isRunningJump = false; // 重置跳跃类型标记

        // 恢复重力和刚体控制
        Player.useGravity = true;
        Player.isKinematic = false;
    }

    private void FixedUpdate()
    {
        // 移除走路和跑步的位移控制，完全依赖动画位移
        // 仅保留必要的物理更新（如减速逻辑，但速度已设为0）
        if (isGrounded && !isJumping && !isDashRolling && !isEvadeRolling)
        {
            // 仅保留减速逻辑（但速度已设为0，实际无效果）
            if (!isWalking && !isRunning && currentSpeed > 0)
            {
                currentSpeed = Mathf.Max(0, currentSpeed - slowdownRate * Time.fixedDeltaTime);
            }
        }
    }

    // 无敌状态检测（可被外部调用）
    public bool IsInvincible()
    {
        return isInvincible;
    }
}