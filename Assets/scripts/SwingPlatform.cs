using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    public enum MoveAxis { X, Y, Z }

    [Header("平台移动参数")]
    public MoveAxis moveAxis = MoveAxis.X; // 平台移动方向
    public float moveDistance = 3f;        // 最大摆动距离
    public float moveSpeed = 2f;           // 摆动速度

    private Vector3 startPos;
    private Vector3 lastPos;
    private Rigidbody rb;

    private Transform playerOnPlatform;

    void Start()
    {
        startPos = transform.position;
        lastPos = startPos;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // 平台自己移动
    }

    void FixedUpdate()
    {
        // 计算目标位置
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        Vector3 newPos = startPos;

        switch (moveAxis)
        {
            case MoveAxis.X: newPos.x += offset; break;
            case MoveAxis.Y: newPos.y += offset; break;
            case MoveAxis.Z: newPos.z += offset; break;
        }

        // 计算位移差
        Vector3 delta = newPos - lastPos;

        // 移动平台（物理方式）
        rb.MovePosition(newPos);

        // 如果玩家在平台上，且玩家是 Transform 控制的，手动加位移
        if (playerOnPlatform != null)
        {
            Rigidbody playerRb = playerOnPlatform.GetComponent<Rigidbody>();

            if (playerRb == null || playerRb.isKinematic)
            {
                // 非刚体角色 → 手动加位移
                playerOnPlatform.position += delta;
            }
            // 有刚体的角色，不需要额外处理，物理会自动推着走
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
