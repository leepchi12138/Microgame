using UnityEngine;

[DisallowMultipleComponent]
public class FallRollLanding : MonoBehaviour
{
    [Header("���")]
    public Animator animator;
    public Rigidbody rb;
    public Transform groundCheck;         // ͨ�����ڽ�ɫ�ŵ�
    public LayerMask groundLayer;

    [Header("����")]
    public float detectDistance = 4f;     // ���������ж��ĸ߶�
    public float baseFallSpeed = 8f;      // ��ͨ�����ٶ�
    public float rollFallSpeed = 15f;     // ����ʱ�������ٶ�
    public float sprintForce = 8f;        // �ɹ���غ�������
    public float failRecoverTime = 1f;    // ʧ�ָܻ�ʱ��

    // ״̬
    private bool isFalling;
    private bool hasLanded;
    private bool wantToRoll;              // �Ƿ���������ط�������
    private bool isAirRolling;            // �Ƿ��ڿ��з�����

    // ��������
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

        // �����⣺���з�����������Ctrl������
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

        // �������ж��õĻ������� (C��)
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

        // �����ٶȸ����Ƿ��ڿ��з���������
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

        animator.SetBool(airRollBool, false); // ��غ�رտ��з���

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
