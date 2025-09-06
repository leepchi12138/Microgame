using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volumeSetting : MonoBehaviour
{
    Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        //初始化，先找到slider组件
        slider = transform.Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);//委托事件
    }
    void OnValueChanged(float value) { 
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
