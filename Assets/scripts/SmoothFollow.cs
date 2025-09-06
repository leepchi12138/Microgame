// Smooth Follow from Standard Assets
// 支持上下视角调整的版本
using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    // 跟随的目标
    public Transform target;
    // 目标在xz平面上的距离
    public float distance = 2.0f;
    // 相机在目标上方的高度
    public float height = 2.5f;
    // 高度阻尼（平滑度）
    public float heightDamping = 2.0f;
    // 水平旋转阻尼（平滑度）
    public float rotationDamping = 3.0f;
    // 垂直旋转角度限制（向上看的最大角度）
    public float maxVerticalAngle = 60f;
    // 垂直旋转角度限制（向下看的最大角度）
    public float minVerticalAngle = -30f;
    // 当前垂直旋转角度
    private float currentVerticalAngle;
    // 垂直旋转灵敏度
    public float verticalSensitivity = 2f;

    [AddComponentMenu("Camera-Control/Smooth Follow")]

    void Start()
    {
        // 初始化垂直角度为相机当前的x轴旋转
        currentVerticalAngle = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        if (!target) return;

        // 处理垂直旋转输入（鼠标上下移动控制）
        currentVerticalAngle -= Input.GetAxis("Mouse Y") * verticalSensitivity;
        // 限制垂直旋转角度在合理范围内
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);

        // 水平旋转角度（跟随目标的y轴旋转）
        float wantedRotationAngle = target.eulerAngles.y;
        float currentRotationAngle = Mathf.LerpAngle(transform.eulerAngles.y, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // 高度平滑过渡
        float wantedHeight = target.position.y + height;
        float currentHeight = Mathf.Lerp(transform.position.y, wantedHeight, heightDamping * Time.deltaTime);

        // 创建旋转（包含水平和垂直旋转）
        Quaternion currentRotation = Quaternion.Euler(currentVerticalAngle, currentRotationAngle, 0);

        // 设置相机位置
        transform.position = target.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        // 确保高度正确
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        // 看向目标
        transform.LookAt(target);
    }
}
