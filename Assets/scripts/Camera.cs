using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform player;
    private Vector3 direction;
    private float smoothSpeed = 0.125f;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        direction = transform.position - player.position;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Vector3 destinationPosition = player.position + player.forward * direction.z + Vector3.up * direction.y;
        //transform.position = destinationPosition;
        //transform.LookAt(player);


        // ʹ��Player����ת��ȷ�������λ��
        Vector3 newPosition = player.position + player.forward * direction.z + Vector3.up * direction.y + player.rotation * direction;
        //transform.position = newPosition;
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed);//��ǰλ�ã�Ŀ��λ�ã�ƽ������
        //ƽ�����ƶ�����ֵ�ƶ�

        // �����������Playerһ��
       transform.rotation = player.rotation;
    }
}
