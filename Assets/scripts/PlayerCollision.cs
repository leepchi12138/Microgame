using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public Animator animator;

    // Layer ����
    private int blockLayer;

    [Header("��ײ��ȴʱ��")]
    public float invincibleTime = 1f; // Ĭ��1���޵�
    private float lastCollisionTime = -999f; // ��һ�δ���ʱ��

    void Start()
    {
        blockLayer = LayerMask.NameToLayer("blocks"); // ��ȡ blocks ��
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �ж��Ƿ�Ϊ blocks ��
        if (collision.gameObject.layer == blockLayer)
        {
            // �ж���ȴ�Ƿ����
            if (Time.time - lastCollisionTime < invincibleTime)
                return; // �����޵�ʱ�䣬����

            lastCollisionTime = Time.time; // ��¼���δ���ʱ��

            // ����դ������
            FenceBlock block = collision.gameObject.GetComponent<FenceBlock>();
            if (block != null)
            {
                Vector3 hitDir = transform.forward; // ײ������
                block.Fall(hitDir);
            }

            // �������ˤ������
            if (animator != null)
            {
                animator.SetTrigger("Fall");
            }
        }
    }
}
