using UnityEngine;

public class lasting2021 : MonoBehaviour
{
    // �����������
    public Rigidbody Player;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask Ground;

    // �ٶȲ�������·���ܲ��ٶ���Ϊ0����������λ�ƣ�
    [SerializeField] public float walkSpeed = 0f;         // �����ٶȣ���������λ�ƣ�
    [SerializeField] public float runSpeed = 0f;          // �����ٶȣ���������λ�ƣ�
    [SerializeField] public float slowdownRate = 4f;      // ��������

    // ��������
    [SerializeField] public float dashRollDuration = 1f;  // ���ٷ���
    [SerializeField] public float evadeRollDuration = 0.4f; // ��ܷ�������ʱ��
    [SerializeField] public float invincibilityTime = 0.3f; // �޵�ʱ��

    // ԭ����Ծ����
    public float targetJumpHeight = 1.5f;                 // ԭ����Ծ�߶�
    public float jumpDuration = 0.5f;
    public AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // �ܲ���Ծ�߶ȣ���Ϊ0������������
    public float runningJumpHeight = 0f;

    // ״̬����
    private bool isJumping = false;          // ԭ����Ծ״̬
    private bool isDashRolling = false;      // ���ٷ���״̬����ͼ��
    private bool isEvadeRolling = false;     // ��ܷ���״̬���޵�֡��
    private bool isInvincible = false;       // �޵�״̬
    private float jumpStartTime;
    private float dashRollStartTime;
    private float evadeRollStartTime;
    private bool isRunningJump = false;      // ����Ƿ�Ϊ�ܲ�����Ծ

    // ����״̬
    private bool wasWKeyDown = false;        // ��һ֡W��״̬
    private bool wasShiftDown = false;       // ��һ֡Shift��״̬
    private bool wasCtrlDown = false;        // ��һ֡Ctrl��״̬
    private bool wasCKeyDown = false;        // ��һ֡C��״̬
    private bool isJumpKeyPressed = false;   // ��Ծ�����±��
    private bool isDashRollKeyPressed = false; // ���ٷ��������±��
    private bool isEvadeRollKeyPressed = false; // ��ܷ��������±��

    // �ƶ�״̬
    private bool isWalking = false;          // ����״̬
    private bool isRunning = false;          // ����״̬
    private bool wasMovingBeforeJump = false; // ��¼��Ծǰ�Ƿ����ƶ�

    //������
    public float mouseSensitivity = 100f;  // ���������
    public bool lockCursor = true;         // �Ƿ��������
    private float yRotation = 0f;          // ��ɫY����ת�Ƕ�

    // ��ǰ�ٶȣ������ڶ���״̬�жϣ�������λ�ƣ�
    protected float currentSpeed;
    protected bool isGrounded = true;
    protected bool wasGrounded = false;

    // ��������¼��Ծ��ʼλ��
    private Vector3 jumpStartPosition;

    public virtual void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!Player) Player = GetComponent<Rigidbody>();

        Player.freezeRotation = true;
        currentSpeed = 0f;

        // ���������������
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

        // �����޵�״̬
        UpdateInvincibility();

        // ��������
        HandleInput();

        // ������½�߼�
        if (!wasGrounded && isGrounded)
        {
            OnLand();
        }

        // ������Ծ�ͷ����߼�
        HandleJump();
        HandleDashRoll();
        HandleEvadeRoll();

        // ���¶���
        UpdateAnimation();
        // �����ƽ�ɫ��ת
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

        // ��ⰴ��״̬�仯
        bool wKeyJustPressed = isWKeyDown && !wasWKeyDown;
        bool shiftJustPressed = isShiftDown && !wasShiftDown;
        bool shiftJustReleased = !isShiftDown && wasShiftDown;
        bool wKeyJustReleased = !isWKeyDown && wasWKeyDown;
        bool ctrlJustPressed = isCtrlDown && !wasCtrlDown;
        bool cKeyJustPressed = isCKeyDown && !wasCKeyDown;

        // ���ڷǷ���״̬�´����ƶ����루������λ�ƣ�������״̬��
        if (!isDashRolling && !isEvadeRolling)
        {
            // ������Ӧ�����仯�������¶���״̬��������λ�ƣ�
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

        // ��¼��ǰ����״̬
        wasWKeyDown = isWKeyDown;
        wasShiftDown = isShiftDown;
        wasCtrlDown = isCtrlDown;
        wasCKeyDown = isCKeyDown;

        // ������Ծ����
        if (isJumpKeyDown && isGrounded && !isDashRolling && !isEvadeRolling)
        {
            isRunningJump = isRunning; // ����Ƿ�Ϊ�ܲ�����Ծ
            if (!isWalking && !isRunning)
            {
                // ԭ����Ծ
                isJumpKeyPressed = true;
            }
            else
            {
                // �ƶ�����Ծ
                wasMovingBeforeJump = true;
                animator.SetTrigger("Jump");
                currentSpeed = 0f;
            }
        }

        // ������ٷ���������Ctrl��������ͼ��
        if (ctrlJustPressed && isGrounded && isRunning && !isJumping && !isDashRolling && !isEvadeRolling)
        {
            isDashRollKeyPressed = true;
        }

        // �����ܷ���������C�������޵�֡��
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
            animator.SetTrigger("Roll"); // �������ٷ�������
        }

        // ����ʱ����Ʒ���״̬����ʱ��
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
            animator.SetTrigger("C"); // ������ܷ����������޵�֡��
        }

        // ����ʱ����Ʒ���״̬����ʱ��
        if (isEvadeRolling && (Time.time - evadeRollStartTime) >= evadeRollDuration)
        {
            EndEvadeRoll();
        }
    }

    public virtual void StartDashRoll()
    {
        isDashRolling = true;
        dashRollStartTime = Time.time;

        // ���������͸�����ƣ��ö�������λ��
        Player.useGravity = false;
        Player.velocity = Vector3.zero;
        Player.isKinematic = true;
    }

    public virtual void StartEvadeRoll()
    {
        isEvadeRolling = true;
        isInvincible = true;
        evadeRollStartTime = Time.time;

        // ���������͸�����ƣ��ö�������λ��
        Player.useGravity = false;
        Player.velocity = Vector3.zero;
        Player.isKinematic = true;
    }

    public virtual void EndDashRoll()
    {
        isDashRolling = false;

        // �ָ������͸������
        Player.useGravity = true;
        Player.isKinematic = false;

        // ʹ�õ�ǰW��״̬�ָ�����״̬
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

        // �ָ������͸������
        Player.useGravity = true;
        Player.isKinematic = false;

        // ʹ�õ�ǰW��״̬�ָ�����״̬
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

        // ��غ�ָ��ƶ�״̬
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
        // ��¼��Ծ��ʼλ��
        jumpStartPosition = transform.position;

        // ���������͸�����ƣ�������Ծ�������ø߶�
        Player.useGravity = false;
        Player.velocity = Vector3.zero;
        Player.isKinematic = true;
    }

    public virtual void ExecuteJump()
    {
        float elapsedTime = Time.time - jumpStartTime;
        float progress = Mathf.Clamp01(elapsedTime / jumpDuration);

        // �����Ƿ�Ϊԭ����Ծ���ò�ͬ�߶�
        float jumpHeight = isRunningJump ? runningJumpHeight : targetJumpHeight;
        float heightProgress = jumpCurve.Evaluate(progress);

        // �������λ�ã��ؼ��޸ĵ㣩
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
        isRunningJump = false; // ������Ծ���ͱ��

        // �ָ������͸������
        Player.useGravity = true;
        Player.isKinematic = false;
    }

    private void FixedUpdate()
    {
        // �Ƴ���·���ܲ���λ�ƿ��ƣ���ȫ��������λ��
        // ��������Ҫ��������£�������߼������ٶ�����Ϊ0��
        if (isGrounded && !isJumping && !isDashRolling && !isEvadeRolling)
        {
            // �����������߼������ٶ�����Ϊ0��ʵ����Ч����
            if (!isWalking && !isRunning && currentSpeed > 0)
            {
                currentSpeed = Mathf.Max(0, currentSpeed - slowdownRate * Time.fixedDeltaTime);
            }
        }
    }

    // �޵�״̬��⣨�ɱ��ⲿ���ã�
    public bool IsInvincible()
    {
        return isInvincible;
    }
}