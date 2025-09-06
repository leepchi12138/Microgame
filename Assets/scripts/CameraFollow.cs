using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    private Vector3 offset; // �洢���ƫ����
    public float smoothSpeed = 0.125f;

    [Header("�����Ұ����")]
    public float heightOffset = 0f;   // ��������̧��
    public float backwardOffset = 1f;   // ����������Զ
    [Range(-30f, 45f)]
    public float tiltAngle = -20f;       // ������¿��ĽǶȣ�������

    void Start()
    {
        // ��ȡ������(Player)������
        player = transform.parent;
        if (player == null)
        {
            Debug.LogError("���������Player���Ӷ���");
            return;
        }

        // ��¼��ʼ���ƫ��
        offset = transform.localPosition;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // ����ƫ�� + ���������/����ƫ��
        Vector3 adjustedOffset = offset
                               + Vector3.up * heightOffset
                               - Vector3.forward * backwardOffset;

        // ������λ�ã�����Player��λ�ú���ת�������ƫ�ƣ�
        Vector3 newPosition = player.TransformPoint(adjustedOffset);

        // ƽ���ƶ�����λ��
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed);

        // Ӧ�ø��ӽǶȣ���֤�Ƕȿɵ���
        transform.rotation = Quaternion.LookRotation(player.position - transform.position, Vector3.up);
        transform.rotation *= Quaternion.Euler(tiltAngle, 0f, 0f);
    }
}
