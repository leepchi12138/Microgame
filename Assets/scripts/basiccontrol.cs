using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basiccontrol : MonoBehaviour
{
    private float moveSpeed = 30f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0, 0, moveSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, 0, -moveSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.Translate(new Vector3(0, -moveSpeed * Time.deltaTime, 0));
        }
        //Input.mousePosition��ȡ���λ�� eg�϶���Ʒ���ĵ���
        //Input.GetMouseButton��ĳ������������ 0�����/1���Ҽ�/2���м�
        //Input.touchCount�ֻ���� ���һ֡���ٸ���ָ������
        if (Input.GetMouseButton(0))
        {
            Debug.Log("�����������");
        }
    }
}
