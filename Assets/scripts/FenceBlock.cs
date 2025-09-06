using UnityEngine;

public class FenceBlock : MonoBehaviour
{
    private Rigidbody rb;
    private bool isHit = false;

    [Header("倒下参数")]
    public float fallForce = 500f;      // 倒下力度
    public float torqueForce = 200f;  // 扭转力，让它平滑倒下

    [Header("地面检测")]
    public LayerMask groundLayer;     // 在 Inspector 里指定地面 Layer

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // 初始不动
    }

    /// <summary>
    /// 撞击时触发倒下
    /// </summary>
    public void Fall(Vector3 hitDirection)
    {
        if (isHit) return; // 防止重复触发
        isHit = true;

        rb.isKinematic = false; // 开启物理
        rb.AddForce(hitDirection * fallForce, ForceMode.Impulse);
        rb.AddTorque(Vector3.forward * torqueForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 只有在击中地面层的时候，才让 Fence 停止下来
        if (isHit && ((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // 着地后停止模拟，避免卡在角色上
        }
    }
}
