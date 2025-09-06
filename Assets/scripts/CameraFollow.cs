using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    private Vector3 offset; // 存储相对偏移量
    public float smoothSpeed = 0.125f;

    [Header("相机视野调整")]
    public float heightOffset = 0f;   // 额外往上抬高
    public float backwardOffset = 1f;   // 额外往后拉远
    [Range(-30f, 45f)]
    public float tiltAngle = -20f;       // 相机往下看的角度（度数）

    void Start()
    {
        // 获取父对象(Player)的引用
        player = transform.parent;
        if (player == null)
        {
            Debug.LogError("相机必须是Player的子对象！");
            return;
        }

        // 记录初始相对偏移
        offset = transform.localPosition;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 基础偏移 + 额外的上下/后移偏移
        Vector3 adjustedOffset = offset
                               + Vector3.up * heightOffset
                               - Vector3.forward * backwardOffset;

        // 计算新位置（基于Player的位置和旋转保持相对偏移）
        Vector3 newPosition = player.TransformPoint(adjustedOffset);

        // 平滑移动到新位置
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed);

        // 应用俯视角度（保证角度可调）
        transform.rotation = Quaternion.LookRotation(player.position - transform.position, Vector3.up);
        transform.rotation *= Quaternion.Euler(tiltAngle, 0f, 0f);
    }
}
