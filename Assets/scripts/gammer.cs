using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gammer : MonoBehaviour
{
    //friction 摩檫力 bounciness弹力
    // Start is called before the first frame update
    //Box Collider 2D 2D碰撞器 人和物体用的   rigidbody 2D刚体 物质属性  碰撞要2个碰撞器一个带有刚体
    Rigidbody rig;
    Rigidbody rig1;
    
    void Start()
    {
        rig = transform.GetComponent<Rigidbody>();
        rig1 = GameObject.Find("player").GetComponent<Rigidbody>();
        rig.Sleep();
        rig.WakeUp();
        rig.AddForce(new Vector3(0,0,100f));
        //AddForceAtPosition 指定位置上添加力（外力）
        //在重心上添加力   AddForce绝对力（世界）；AddRelative相对力(局部坐标);AddTorque扭矩力;AddrelativeTorque相对扭矩力;AddExplosionForce爆炸力

        //绘制射线 Debug.DrawLine(transform.position起始位置，position+transform.forward终点位置,(Color),(float持续时间),(是否被靠近的相机物体遮挡bool))
        Collider[] coliders =  Physics.OverlapSphere(transform.position,10f);//球型射线，返回球形范围内射线命中物体的信息数
    
    }
    private void OnCollisionEnter(Collision collision)//碰撞方法要有刚体组件，不能勾选为触发器
    {
        //collision保存了碰撞物体的信息
        //物体发生碰撞时自动调用方法
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        
    }
}
