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
        //Input.mousePosition获取鼠标位置 eg拖动物品栏的道具
        //Input.GetMouseButton当某键持续被按下 0是左键/1是右键/2是中键
        //Input.touchCount手机版的 最后一帧多少根手指碰到了
        if (Input.GetMouseButton(0))
        {
            Debug.Log("左键被按下了");
        }
    }
}
