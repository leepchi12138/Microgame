using System.Collections;
using UnityEngine;

public class movement : MonoBehaviour
{
    // ========= ��� =========
    public Animator animator;
    public Transform groundCheck;
    public LayerMask Ground;
    public bool isGrounded = true;
    private Rigidbody rb;

    // ========= ת�� =========
    public float rotationSpeed = 150f; // A/D��ת�ٶȣ���/��)

    // ========= Root Motion =========
    [Header("Root Motion")]
    public float rootMotionScale = 2.5f;   // ����λ�ƷŴ�ϵ��

    // ========= Boost ���� =========
    [Header("Boost")]
    public float boostAdditiveSpeed = 0f;  // ˮƽֱ�߼���
    public float boostFriction = 8f;       // ˥������

    private bool isBoosting = false;       // �Ƿ���Boost״̬
    private Coroutine boostCoroutine;      // BoostЭ�����ã��������

    // ========= ��Ծ =========
    [SerializeField] private int maxJumpCount = 2; // �����Ծ��������������
    private int currentJumpCount;
    [SerializeField] float baseJumpForce = 7f;     // һ��������
    [SerializeField] private float secondJumpHeightMultiplier = 0.5f; // ���������ȱ���
    private float _originalAnimSpeed;              // ��¼ԭʼ�����ٶȣ�������ԾʱҲ���٣�

    // ========= ���� =========
    public float fallGravityMultiplier = 2.0f;
    public AnimationCurve jumpGravityCurve = AnimationCurve.Linear(0, 1, 1, 1);

    // ========= ��Ƶ =========
    public AudioClip[] jumpClips;
    public AudioClip[] secondJumpClips;
    private int lastAudioIndex = -1;
    private AudioSource audioSource;

    // ========= ��ػ��� =========
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
            _originalAnimSpeed = animator.speed; // ��ʼ����¼ԭʼ�����ٶ�
        }

        currentJumpCount = maxJumpCount;
        animator.SetBool("Idle", false);
    }

    void Update()
    {
        // ---- �����⣨���⴩ģ��----
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, Ground);

        if (!wasGrounded && isGrounded)
        {
            ResetJumpCount();
            landingTimer = landingBufferTime;
            // ���ʱ�ָ������ٶȣ�����Boost״̬��������Ծ������
            if (animator != null) animator.speed = _originalAnimSpeed;
        }

        if (landingTimer > 0) landingTimer -= Time.deltaTime;

        // ---- ������ ----
        KeyBoardCheckOut();
    }

    void FixedUpdate()
    {
        ApplyCustomGravity();

        // Boost ˮƽ�ٶ�˥��
        if (boostAdditiveSpeed > 0f)
        {
            boostAdditiveSpeed = Mathf.Max(0f, boostAdditiveSpeed - boostFriction * Time.fixedDeltaTime);
        }
    }

    // ========= ���������� =========
    public virtual void KeyBoardCheckOut()
    {
        bool isWKeyDown = Input.GetKey(KeyCode.W) || landingTimer > 0f;
        bool isSKeyDown = Input.GetKey(KeyCode.S);
        bool isADown = Input.GetKey(KeyCode.A);
        bool isDDown = Input.GetKey(KeyCode.D);
        bool isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isSpaceKeyDown = Input.GetKeyDown(KeyCode.Space);

        // ---- ��Ծ ----
        if (isSpaceKeyDown && currentJumpCount > 0)
        {
            // ��Ծǰ�����������ٶ�Ϊԭʼ�ٶȣ���ֹBoostӰ����Ծ������
            if (animator != null) animator.speed = _originalAnimSpeed;
            Jump(currentJumpCount < maxJumpCount);
            currentJumpCount--;
            animator.SetBool("Idle", false);
            return;
        }

        // ---- ����״̬�µ��ƶ��߼� ----
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

                // ���ƽ������λ��
                float backSpeed = 3f; // �����ٶȣ����Ե�
                transform.position += -transform.forward * backSpeed * Time.deltaTime;
            }
            else
            {
                animator.SetBool("Sprit_Fwd", false);
                animator.SetBool("Run_Fwd", false);
                animator.SetBool("Idle", true);
            }
        }

        // ---- ����״̬ ----
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

    // ========= ��Ծ =========
    public virtual void Jump(bool isSecondJump)
    {
        // ��������
        if (isSecondJump) animator.SetTrigger("SecondJump");
        else animator.SetTrigger("Jump");

        // ������Ч
        PlayJumpSound(isSecondJump);

        // ������Ծ����֤�̳�ˮƽ�ٶ� + ��ֱ�ٶ����ã�
        if (rb != null)
        {
            Vector3 vel = rb.velocity;
            vel.y = 0f; // ������ֱ�ٶȣ������ε���
            vel.y = isSecondJump ? (baseJumpForce * secondJumpHeightMultiplier) : baseJumpForce;
            rb.velocity = vel;
        }
    }

    // ========= ��Ծ��Ч =========
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

    // ========= ������Ծ���� =========
    private void ResetJumpCount() => currentJumpCount = maxJumpCount;

    // ========= �Զ���������֧����Ծ���� & ������٣� =========
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

    // ========= Root Motion ������ɫ =========
    void OnAnimatorMove()
    {
        if (rb == null || animator == null) return;

        Vector3 rootMotion = animator.deltaPosition * rootMotionScale;

        // ����ֱ��Boostλ��
        Vector3 extra = Vector3.zero;
        if (boostAdditiveSpeed > 0f)
        {
            extra = transform.forward * boostAdditiveSpeed * Time.deltaTime;
        }

        rb.MovePosition(rb.position + rootMotion + extra);
        rb.MoveRotation(rb.rotation * animator.deltaRotation);
    }

    // ========= Boost���� =========
    public bool TryBoost(float animSpeedMultiplier, float duration, float additiveImpulse)
    {
        // �������BoostЭ�� �� ���������ǵ���
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

        // ���������ٶ�
        animator.speed = _originalAnimSpeed * animSpeedMultiplier;

        // ע��һ����ˮƽ�ٶ�
        boostAdditiveSpeed += Mathf.Max(0f, additiveImpulse);

        // �ȴ�Boost����ʱ��
        yield return new WaitForSeconds(duration);

        // �ָ������ٶ�
        animator.speed = _originalAnimSpeed;
        isBoosting = false;
        boostCoroutine = null;
    }
}
