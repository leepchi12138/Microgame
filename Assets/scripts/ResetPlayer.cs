using UnityEngine;
using UnityEngine.UI;

public class ResetPlayer : MonoBehaviour
{
    [Header("�����Ϣ")]
    public string playerName = "Player";
    public Animator animator;

    [Header("��������")]
    public KeyCode resetKey = KeyCode.R;
    private Transform defaultSpawnPoint;
    private Transform lastCheckpoint;
    private Transform nextCheckpoint;
    private Rigidbody rb;

    [Header("���м��")]
    public float wrongWayAngle = 120f;             // �Ƕ���ֵ
    public float wrongWayDuration = 2f;            // ���г���ʱ��
    public float wrongWayDistanceThreshold = 5f;  // ���д�������С����
    private float wrongWayTimer = 0f;
    private bool isWrongWay = false;
    private bool onMovingPlatform = false;
    private Vector3 wrongWayStartPos;              // ������ʼλ��

    [Header("UI ��ʾ")]
    public Text warningText;
    public string warningMessage = "�����У�";

    [Header("�������")]
    public GameObject[] voidObjects;

    // ��Ȧ����ʱ
    private float lapStartTime = 0f;
    private float lastLapTime = 0f;

    public string GetPlayerName() => playerName;
    public float GetLastLapTime() => lastLapTime;
    public float GetCurrentLapTime() => Time.time - lapStartTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!animator) animator = GetComponent<Animator>();

        if (defaultSpawnPoint == null)
        {
            GameObject spawn = new GameObject("DefaultSpawnPoint");
            spawn.transform.position = transform.position;
            spawn.transform.rotation = transform.rotation;
            defaultSpawnPoint = spawn.transform;
        }

        lastCheckpoint = defaultSpawnPoint;
        lapStartTime = Time.time;

        if (warningText != null)
            warningText.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(resetKey))
            ResetPosition();

        if (onMovingPlatform) return;

        if (rb.velocity.magnitude > 0.1f && lastCheckpoint != null && nextCheckpoint != null)
        {
            Vector3 forward = transform.forward;
            Vector3 trackDir = (nextCheckpoint.position - lastCheckpoint.position).normalized;
            float angle = Vector3.Angle(forward, trackDir);

            if (angle > wrongWayAngle)
            {
                if (!isWrongWay)
                {
                    // ��һ�ν�������״̬����¼���
                    isWrongWay = true;
                    wrongWayTimer = 0f;
                    wrongWayStartPos = transform.position;

                    if (warningText != null)
                    {
                        warningText.enabled = true;
                        warningText.text = warningMessage;
                    }
                }

                // ���������ۼƾ���
                float wrongWayDistance = Vector3.Distance(transform.position, wrongWayStartPos);

                if (wrongWayDistance >= wrongWayDistanceThreshold)
                {
                    wrongWayTimer += Time.deltaTime;
                    if (wrongWayTimer >= wrongWayDuration)
                    {
                        ResetPosition();
                        isWrongWay = false;
                        if (warningText != null) warningText.enabled = false;
                    }
                }
            }
            else
            {
                isWrongWay = false;
                wrongWayTimer = 0f;
                if (warningText != null) warningText.enabled = false;
            }
        }
    }

    public void SetCheckpoint(Transform checkpoint, Transform next)
    {
        lastLapTime = Time.time - lapStartTime; // ������һȦʱ��
        lapStartTime = Time.time;

        lastCheckpoint = checkpoint;
        nextCheckpoint = next;
    }

    public void ResetPosition()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = lastCheckpoint.position;
        transform.rotation = lastCheckpoint.rotation;

        lapStartTime = Time.time;

        if (animator != null)
        {
            animator.SetBool("Sprit_Fwd", false);
            animator.SetBool("Run_Fwd", false);
            animator.SetBool("SecondJump", false);
            animator.SetBool("Idle", true);
        }
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsVoidObject(collision.gameObject))
            ResetPosition();

        if (collision.gameObject.CompareTag("MovingPlatform"))
            onMovingPlatform = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
            onMovingPlatform = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsVoidObject(other.gameObject))
            ResetPosition();
    }

    private bool IsVoidObject(GameObject obj)
    {
        foreach (GameObject voidObj in voidObjects)
        {
            if (obj == voidObj || obj.transform.IsChildOf(voidObj.transform))
                return true;
        }
        return false;
    }

    // --- ��Ȧ�յ㴥���� ---
    public void FinishLap()
    {
        lastLapTime = Time.time - lapStartTime;

        // ֪ͨ OneLapRaceManager
        if (OneLapRaceManager.Instance != null)
            OneLapRaceManager.Instance.FinishRace(playerName, lastLapTime);
    }
}
