using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateImage());//Я��
    }

    IEnumerator UpdateImage()
    {
        float value = 0;
        while (value <= 1)
        {
            yield return new WaitForSeconds(0.1f);
            value += 0.01f;
            image.fillAmount = value;
        }
        Debug.Log("Э�̽���������ֵ��" + value);
    }



    // Update is called once per frame
    void Update()
    {

    }
}
