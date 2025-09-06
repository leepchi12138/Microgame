using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIpanel : MonoBehaviour
{
    // Start is called before the first frame update

    public InputField usernameInput;
    public InputField pwInput;
    Toggle toggle;
    Button loginBtn;
    void Start()
    {
        toggle = transform.Find("Toggle").GetComponent<Toggle>();
        //修改图片的精灵 transform.Find("background").GetComponent<Image>().sprite = 变量;
        //修改文本 transform.Find("Text").GetComponent<Text>().sprite = 变量;
        usernameInput = transform.Find("UserInputField").GetComponent<InputField>();
        pwInput = transform.Find("UserInputField").GetComponent<InputField>();

        loginBtn = transform.Find("LoginBtn").GetComponent<Button>();

        loginBtn.onClick.AddListener(LonginBtnOnClick);//点击事件是个委托，绑定的事件就是点击后执行的内容
    }
    void WebsiteBtnOnClick() {
        Application.OpenURL("网址跳转");
    }
    void LonginBtnOnClick() {
        if (toggle.isOn == true)
        {
            string account = usernameInput.text;
            string pwd = pwInput.text;
        }
        else {
            Debug.Log("需要同意隐私协议");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
