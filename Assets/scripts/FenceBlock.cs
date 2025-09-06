using UnityEngine;

public class FenceBlock : MonoBehaviour
{
    private Rigidbody rb;
    private bool isHit = false;

    [Header("���²���")]
    public float fallForce = 500f;      // ��������
    public float torqueForce = 200f;  // Ťת��������ƽ������

    [Header("������")]
    public LayerMask groundLayer;     // �� Inspector ��ָ������ Layer

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // ��ʼ����
    }

    /// <summary>
    /// ײ��ʱ��������
    /// </summary>
    public void Fall(Vector3 hitDirection)
    {
        if (isHit) return; // ��ֹ�ظ�����
        isHit = true;

        rb.isKinematic = false; // ��������
        rb.AddForce(hitDirection * fallForce, ForceMode.Impulse);
        rb.AddTorque(Vector3.forward * torqueForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ֻ���ڻ��е�����ʱ�򣬲��� Fence ֹͣ����
        if (isHit && ((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // �ŵغ�ֹͣģ�⣬���⿨�ڽ�ɫ��
        }
    }
}
