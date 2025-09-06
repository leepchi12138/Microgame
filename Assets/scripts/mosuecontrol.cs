using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mosuecontrol : MonoBehaviour
{
    // 鼠标灵敏度
    public float mouseSensitivity = 300f;

    // 垂直旋转角度限制
    public float minVerticalAngle = -180f;
    public float maxVerticalAngle = 180f;

    // 摄像机Transform
    public Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        // 锁定并隐藏鼠标光标
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 水平旋转Player（左右转）
        transform.Rotate(Vector3.up * mouseX);
    }
}