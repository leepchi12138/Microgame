using System.Collections;
using UnityEngine;

public class movement : MonoBehaviour
{
    // ========= 组件 =========
    public Animator animator;
    public Transform groundCheck;
    public LayerMask Ground;
    public bool isGrounded = true;
    private Rigidbody rb;

    // ========= 转向 =========
    public float rotationSpeed = 150f; // A/D旋转速度（度/秒)

    // ========= Root Motion =========
    [Header("Root Motion")]
    public float rootMotionScale = 2.5f;   // 动画位移放大系数

    // ========= Boost 加速 =========
    [Header("Boost")]
    public float boostAdditiveSpeed = 0f;  // 水平直线加速
    public float boostFriction = 8f;       // 衰减阻力

    private bool isBoosting = false;       // 是否在Boost状态
    private Coroutine boostCoroutine;      // Boost协程引用，避免叠加

    // ========= 跳跃 =========
    [SerializeField] private int maxJumpCount = 2; // 最大跳跃次数（二段跳）
    private int currentJumpCount;
    [SerializeField] float baseJumpForce = 7f;     // 一段跳力度
    [SerializeField] private float secondJumpHeightMultiplier = 0.5f; // 二段跳力度倍数
    private float _originalAnimSpeed;              // 记录原始动画速度（避免跳跃时也加速）

    // ========= 重力 =========
    public float fallGravityMultiplier = 2.0f;
    public AnimationCurve jumpGravityCurve = AnimationCurve.Linear(0, 1, 1, 1);

    // ========= 音频 =========
    public AudioClip[] jumpClips;
    public AudioClip[] secondJumpClips;
    private int lastAudioIndex = -1;
    private AudioSource audioSource;

    // ========= 落地缓冲 =========
    [SerializeField] private float landingBufferTime = 0.1f;
    private float landingTimer = 0f;

    void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        if (animator != null)
        {
            animator.applyRootMotion = true;
            _originalAnimSpeed = animator.speed; // 初始化记录原始动画速度
        }

        currentJumpCount = maxJumpCount;
        animator.SetBool("Idle", false);
    }

    void Update()
    {
        // ---- 地面检测（避免穿模）----
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, Ground);

        if (!wasGrounded && isGrounded)
        {
            ResetJumpCount();
            landingTimer = landingBufferTime;
            // 落地时恢复动画速度（避免Boost状态残留到跳跃动画）
            if (animator != null) animator.speed = _originalAnimSpeed;
        }

        if (landingTimer > 0) landingTimer -= Time.deltaTime;

        // ---- 输入检测 ----
        KeyBoardCheckOut();
    }

    void FixedUpdate()
    {
        ApplyCustomGravity();

        // Boost 水平速度衰减
        if (boostAdditiveSpeed > 0f)
        {
            boostAdditiveSpeed = Mathf.Max(0f, boostAdditiveSpeed - boostFriction * Time.fixedDeltaTime);
        }
    }

    // ========= 键盘输入检测 =========
    public virtual void KeyBoardCheckOut()
    {
        bool isWKeyDown = Input.GetKey(KeyCode.W) || landingTimer > 0f;
        bool isSKeyDown = Input.GetKey(KeyCode.S);
        bool isADown = Input.GetKey(KeyCode.A);
        bool isDDown = Input.GetKey(KeyCode.D);
        bool isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isSpaceKeyDown = Input.GetKeyDown(KeyCode.Space);

        // ---- 跳跃 ----
        if (isSpaceKeyDown && currentJumpCount > 0)
        {
            // 跳跃前：锁定动画速度为原始速度（禁止Boost影响跳跃动画）
            if (animator != null) animator.speed = _originalAnimSpeed;
            Jump(currentJumpCount < maxJumpCount);
            currentJumpCount--;
            animator.SetBool("Idle", false);
            return;
        }

        // ---- 地面状态下的移动逻辑 ----
        if (isGrounded)
        {
            if (isADown)
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            else if (isDDown)
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            if (isWKeyDown && !isShiftDown)
            {
                animator.SetBool("Sprit_Fwd", false);
                animator.SetBool("Run_Fwd", true);
                animator.SetBool("Idle", false);
            }
            else if (isWKeyDown && isShiftDown)
            {
                animator.SetBool("Run_Fwd", false);
                animator.SetBool("Sprit_Fwd", true);
                animator.SetBool("Idle", false);
            }
            else if (isSKeyDown)
            {
                animator.SetBool("Sprit_Fwd", false);
                animator.SetBool("Run_Fwd", false);
                animator.SetTrigger("SlowDown");
                animator.SetBool("Idle", true);

                // 添加平滑后退位移
                float backSpeed = 3f; // 后退速度，可以调
                transform.position += -transform.forward * backSpeed * Time.deltaTime;
            }
            else
            {
                animator.SetBool("Sprit_Fwd", false);
                animator.SetBool("Run_Fwd", false);
                animator.SetBool("Idle", true);
            }
        }

        // ---- 空中状态 ----
        else
        {
            if (isADown) transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime * 0.7f);
            else if (isDDown) transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * 0.7f);

            animator.SetBool("Sprit_Fwd", false);
            animator.SetBool("Run_Fwd", false);
            animator.SetBool("Idle", false);
            animator.ResetTrigger("SlowDown");
        }
    }

    // ========= 跳跃 =========
    public virtual void Jump(bool isSecondJump)
    {
        // 动画触发
        if (isSecondJump) animator.SetTrigger("SecondJump");
        else animator.SetTrigger("Jump");

        // 播放音效
        PlayJumpSound(isSecondJump);

        // 物理跳跃（保证继承水平速度 + 竖直速度重置）
        if (rb != null)
        {
            Vector3 vel = rb.velocity;
            vel.y = 0f; // 清零竖直速度，避免多次叠加
            vel.y = isSecondJump ? (baseJumpForce * secondJumpHeightMultiplier) : baseJumpForce;
            rb.velocity = vel;
        }
    }

    // ========= 跳跃音效 =========
    private void PlayJumpSound(bool isSecondJump)
    {
        AudioClip[] targetClips = isSecondJump ? secondJumpClips : jumpClips;
        if (targetClips == null || targetClips.Length == 0) return;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, targetClips.Length);
        } while (randomIndex == lastAudioIndex && targetClips.Length > 1);

        lastAudioIndex = randomIndex;
        if (targetClips[randomIndex] != null && audioSource != null)
        {
            audioSource.PlayOneShot(targetClips[randomIndex]);
        }
    }

    // ========= 重置跳跃次数 =========
    private void ResetJumpCount() => currentJumpCount = maxJumpCount;

    // ========= 自定义重力（支持跳跃曲线 & 下落加速） =========
    void ApplyCustomGravity()
    {
        if (isGrounded) return;

        if (rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1) * Time.fixedDeltaTime;
        else
        {
            float normalizedYVel = Mathf.InverseLerp(baseJumpForce, 0, rb.velocity.y);
            float gravityScale = jumpGravityCurve.Evaluate(normalizedYVel);
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityScale - 1) * Time.fixedDeltaTime;
        }
    }

    // ========= Root Motion 驱动角色 =========
    void OnAnimatorMove()
    {
        if (rb == null || animator == null) return;

        Vector3 rootMotion = animator.deltaPosition * rootMotionScale;

        // 额外直线Boost位移
        Vector3 extra = Vector3.zero;
        if (boostAdditiveSpeed > 0f)
        {
            extra = transform.forward * boostAdditiveSpeed * Time.deltaTime;
        }

        rb.MovePosition(rb.position + rootMotion + extra);
        rb.MoveRotation(rb.rotation * animator.deltaRotation);
    }

    // ========= Boost管理 =========
    public bool TryBoost(float animSpeedMultiplier, float duration, float additiveImpulse)
    {
        // 如果已有Boost协程 → 重启而不是叠加
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
        }
        boostCoroutine = StartCoroutine(BoostRoutine(animSpeedMultiplier, duration, additiveImpulse));
        return true;
    }

    private IEnumerator BoostRoutine(float animSpeedMultiplier, float duration, float additiveImpulse)
    {
        isBoosting = true;

        // 提升动画速度
        animator.speed = _originalAnimSpeed * animSpeedMultiplier;

        // 注入一次性水平速度
        boostAdditiveSpeed += Mathf.Max(0f, additiveImpulse);

        // 等待Boost持续时间
        yield return new WaitForSeconds(duration);

        // 恢复动画速度
        animator.speed = _originalAnimSpeed;
        isBoosting = false;
        boostCoroutine = null;
    }
}
