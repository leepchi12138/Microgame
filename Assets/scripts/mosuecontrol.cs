using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mosuecontrol : MonoBehaviour
{
    // ���������
    public float mouseSensitivity = 300f;

    // ��ֱ��ת�Ƕ�����
    public float minVerticalAngle = -180f;
    public float maxVerticalAngle = 180f;

    // �����Transform
    public Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        // ���������������
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // ��ȡ�������
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ˮƽ��תPlayer������ת��
        transform.Rotate(Vector3.up * mouseX);
    }
}