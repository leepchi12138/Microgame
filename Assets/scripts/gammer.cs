using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gammer : MonoBehaviour
{
    //friction Ħ���� bounciness����
    // Start is called before the first frame update
    //Box Collider 2D 2D��ײ�� �˺������õ�   rigidbody 2D���� ��������  ��ײҪ2����ײ��һ�����и���
    Rigidbody rig;
    Rigidbody rig1;
    
    void Start()
    {
        rig = transform.GetComponent<Rigidbody>();
        rig1 = GameObject.Find("player").GetComponent<Rigidbody>();
        rig.Sleep();
        rig.WakeUp();
        rig.AddForce(new Vector3(0,0,100f));
        //AddForceAtPosition ָ��λ�����������������
        //�������������   AddForce�����������磩��AddRelative�����(�ֲ�����);AddTorqueŤ����;AddrelativeTorque���Ť����;AddExplosionForce��ը��

        //�������� Debug.DrawLine(transform.position��ʼλ�ã�position+transform.forward�յ�λ��,(Color),(float����ʱ��),(�Ƿ񱻿�������������ڵ�bool))
        Collider[] coliders =  Physics.OverlapSphere(transform.position,10f);//�������ߣ��������η�Χ�����������������Ϣ��
    
    }
    private void OnCollisionEnter(Collision collision)//��ײ����Ҫ�и�����������ܹ�ѡΪ������
    {
        //collision��������ײ�������Ϣ
        //���巢����ײʱ�Զ����÷���
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        
    }
}
