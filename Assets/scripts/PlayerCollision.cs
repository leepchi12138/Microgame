using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public Animator animator;

    // Layer 名称
    private int blockLayer;

    [Header("碰撞冷却时间")]
    public float invincibleTime = 1f; // 默认1秒无敌
    private float lastCollisionTime = -999f; // 上一次触发时间

    void Start()
    {
        blockLayer = LayerMask.NameToLayer("blocks"); // 获取 blocks 层
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 判断是否为 blocks 层
        if (collision.gameObject.layer == blockLayer)
        {
            // 判断冷却是否结束
            if (Time.time - lastCollisionTime < invincibleTime)
                return; // 仍在无敌时间，忽略

            lastCollisionTime = Time.time; // 记录本次触发时间

            // 触发栅栏倒下
            FenceBlock block = collision.gameObject.GetComponent<FenceBlock>();
            if (block != null)
            {
                Vector3 hitDir = transform.forward; // 撞击方向
                block.Fall(hitDir);
            }

            // 播放玩家摔倒动画
            if (animator != null)
            {
                animator.SetTrigger("Fall");
            }
        }
    }
}
