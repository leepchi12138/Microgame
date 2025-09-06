using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//里面的类都是场景相关的类
using UnityEngine.UI;

public class MyFirstScript : MonoBehaviour
{
    private Transform monoTransform;
    [Range(1f,100f)]
    private float moveSpeed = 10f;
    //private float eulerX;

    private float sceneTime = 0;
    private float loadSceneDuration = 30f;//加载场景的时间间隔

    [HideInInspector]//希望对象是共有的不是私有的
    public GameObject directionalLight;//找其他对象
    [SerializeField]//希望对象是私有的不是共有的，同样拥有拖拽的功能
    private Transform lightTransform;
    void Awake()
    {
        //this.gameObject //挂在谁的身上返回的就是谁
        Debug.LogError("Awake:" + gameObject.name);
        monoTransform = gameObject.GetComponent<Transform>();//找自己的组件
        //monoTransform = this.transform;

        directionalLight = GameObject.Find("Directional Light");//找其他对象方法,直接找名字
        lightTransform = directionalLight.GetComponent<Transform>();//找其他对象的组件

        //作用某种情况下关闭当前组件(组件)脚本本身,若希望关闭对象的所有组件 this.gameObject.SetActive(false);
        directionalLight.GetComponent<Light>().enabled = true;//transform是没有enable属性的,其他组件都有

        Object fLight = GameObject.FindObjectOfType(typeof(Light));//查找场景里含有Light组件的对象
        //找到组件所属的对象
        Debug.LogWarning((fLight as Light).gameObject);
        //GameObject.SetActive(bool);对象是否隐藏

        //eulerX = lightTransform.eulerAngles.x;
    }
    void OnEnable()
    {
        //this.gameObject //挂在谁的身上返回的就是谁
        Debug.LogError("OnEnable:" + gameObject.name);
    }
    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject //挂在谁的身上返回的就是谁
        Debug.LogError("Start:" + gameObject.name);

        //transform.rotation = Quaternion.Euler(new Vector3(0,45,0));
        transform.eulerAngles = new Vector3(0,45,0);
        // Quaternion quaternion = transform.rotation;//四元素角，可以解决万向锁问题(欧拉角有一些无法转到的方向)，而欧拉角euler方便理解，
        StartCoroutine(Load());//开启携程的api
        //GameObject.Instantiate("变量");克隆物体
        //Destroy(延迟删除，5s);
    }

    //异步加载 用到携程
    private IEnumerator Load() {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("1");
        GameObject player = GameObject.Find("Player");
        DontDestroyOnLoad(player);//加载新场景后物体仍不会消失,子物体同样也不会消失
        asyncOperation.allowSceneActivation = false;//场景不能立马跳转
        while (asyncOperation.progress < 0.9f) {
            Debug.Log("loading" + asyncOperation.progress);
            yield return null;//不用时刻获取进度，null指的是一帧，让步一帧获取进度//可能出现的bug场景过下加载过快
        }
        asyncOperation.allowSceneActivation = true;

    }
    private void Update()
    {
       Debug.LogWarning("Update:" + gameObject.name);
        monoTransform.position = monoTransform.position + new Vector3(0, 0, moveSpeed * Time.deltaTime);//类里面定义所以是堆
        Debug.Log(Time.deltaTime);//基于游戏帧的时长
        //monoTransform.Translate(0,0, moveSpeed* Time.deltaTime);
        

        lightTransform.Rotate(new Vector3(10*Time.deltaTime, 0,0));


        //附上时间加载场景
        sceneTime += Time.deltaTime;
        Debug.LogError(sceneTime);
        if (sceneTime>= loadSceneDuration)
        {
            SceneManager.LoadScene("Scene2",LoadSceneMode.Additive);
            //            SceneManager.LoadScene("1"，LoadSceneMode.Additive);//索引，场景1、2同时加载
        }

    }
    void OnDisable()
    {
        //this.gameObject //挂在谁的身上返回的就是谁
        Debug.LogError("OnDisable:" + gameObject.name);
    }
}
//ctrl+j 回退
//在脚本里面修改游戏对象的位置：1.获取身上的Transform组件
//Vector3 结构体 核心是xyz