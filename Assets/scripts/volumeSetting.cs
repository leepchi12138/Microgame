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
        //��ʼ�������ҵ�slider���
        slider = transform.Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);//ί���¼�
    }
    void OnValueChanged(float value) { 
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
