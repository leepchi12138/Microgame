using UnityEngine;

[DisallowMultipleComponent]
public class FallRollLanding : MonoBehaviour
{
    [Header("组件")]
    public Animator animator;
    public Rigidbody rb;
    public Transform groundCheck;         // 通常放在角色脚底
    public LayerMask groundLayer;

    [Header("参数")]
    public float detectDistance = 4f;     // 触发翻滚判定的高度
    public float baseFallSpeed = 8f;      // 普通下落速度
    public float rollFallSpeed = 15f;     // 翻滚时的下落速度
    public float sprintForce = 8f;        // 成功落地后冲刺力度
    public float failRecoverTime = 1f;    // 失败恢复时间

    // 状态
    private bool isFalling;
    private bool hasLanded;
    private bool wantToRoll;              // 是否已满足落地翻滚条件
    private bool isAirRolling;            // 是否在空中翻滚中

    // 动画参数
    private int rollTrigger;
    private int failTrigger;
    private int airRollBool;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!animator) animator = GetComponent<Animator>();

        rollTrigger = Animator.StringToHash("RollTrigger");
        failTrigger = Animator.StringToHash("FallTrigger");
        airRollBool = Animator.StringToHash("AirRoll");

        rb.useGravity = false;
    }

    void OnEnable()
    {
        isFalling = true;
        hasLanded = false;
        wantToRoll = false;
        isAirRolling = false;
        rb.useGravity = false;
        Debug.Log("[FallRollLanding] Enter Falling.");
    }

    void Update()
    {
        if (!isFalling || hasLanded) return;

        // 输入检测：空中翻滚（持续按Ctrl触发）
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (!isAirRolling)
            {
                isAirRolling = true;
                animator.SetBool(airRollBool, true);
                Debug.Log("[FallRollLanding] Start Air Roll!");
            }
        }
        else
        {
            if (isAirRolling)
            {
                isAirRolling = false;
                animator.SetBool(airRollBool, false);
                Debug.Log("[FallRollLanding] Stop Air Roll.");
            }
        }

        // 检测落地判定用的缓冲输入 (C键)
        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, 100f, groundLayer))
        {
            float distance = hit.distance;
            if (distance <= detectDistance && Input.GetKeyDown(KeyCode.C))
            {
                wantToRoll = true;
                Debug.Log("[FallRollLanding] Buffered Roll Input (distance=" + distance + ")");
            }
        }
    }

    void FixedUpdate()
    {
        if (!isFalling || hasLanded) return;

        // 下落速度根据是否在空中翻滚来决定
        float currentFallSpeed = isAirRolling ? rollFallSpeed : baseFallSpeed;
        rb.velocity = new Vector3(0, -currentFallSpeed, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isFalling || hasLanded) return;
        if (((1 << collision.gameObject.layer) & groundLayer) == 0) return;

        DoLand();
    }

    void DoLand()
    {
        hasLanded = true;
        isFalling = false;
        rb.velocity = Vector3.zero;
        rb.useGravity = true;

        animator.SetBool(airRollBool, false); // 落地后关闭空中翻滚

        if (wantToRoll)
        {
            Debug.Log("[FallRollLanding] SUCCESS: Roll!");
            animator.SetTrigger(rollTrigger);
            rb.AddForce(transform.forward * sprintForce, ForceMode.Impulse);
        }
        else
        {
            Debug.Log("[FallRollLanding] FAIL: Fall.");
            animator.SetTrigger(failTrigger);
            Invoke(nameof(RecoverFromFail), failRecoverTime);
        }
    }

    void RecoverFromFail()
    {
        Debug.Log("[FallRollLanding] Recover from failing land.");
        animator.ResetTrigger(failTrigger);
    }
}
