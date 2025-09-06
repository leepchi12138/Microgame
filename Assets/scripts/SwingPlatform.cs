using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    public enum MoveAxis { X, Y, Z }

    [Header("ƽ̨�ƶ�����")]
    public MoveAxis moveAxis = MoveAxis.X; // ƽ̨�ƶ�����
    public float moveDistance = 3f;        // ���ڶ�����
    public float moveSpeed = 2f;           // �ڶ��ٶ�

    private Vector3 startPos;
    private Vector3 lastPos;
    private Rigidbody rb;

    private Transform playerOnPlatform;

    void Start()
    {
        startPos = transform.position;
        lastPos = startPos;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // ƽ̨�Լ��ƶ�
    }

    void FixedUpdate()
    {
        // ����Ŀ��λ��
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        Vector3 newPos = startPos;

        switch (moveAxis)
        {
            case MoveAxis.X: newPos.x += offset; break;
            case MoveAxis.Y: newPos.y += offset; break;
            case MoveAxis.Z: newPos.z += offset; break;
        }

        // ����λ�Ʋ�
        Vector3 delta = newPos - lastPos;

        // �ƶ�ƽ̨������ʽ��
        rb.MovePosition(newPos);

        // ��������ƽ̨�ϣ�������� Transform ���Ƶģ��ֶ���λ��
        if (playerOnPlatform != null)
        {
            Rigidbody playerRb = playerOnPlatform.GetComponent<Rigidbody>();

            if (playerRb == null || playerRb.isKinematic)
            {
                // �Ǹ����ɫ �� �ֶ���λ��
                playerOnPlatform.position += delta;
            }
            // �и���Ľ�ɫ������Ҫ���⴦��������Զ�������
        }

        lastPos = newPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = collision.transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = null;
        }
    }
}
