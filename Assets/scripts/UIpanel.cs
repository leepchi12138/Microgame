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
        //�޸�ͼƬ�ľ��� transform.Find("background").GetComponent<Image>().sprite = ����;
        //�޸��ı� transform.Find("Text").GetComponent<Text>().sprite = ����;
        usernameInput = transform.Find("UserInputField").GetComponent<InputField>();
        pwInput = transform.Find("UserInputField").GetComponent<InputField>();

        loginBtn = transform.Find("LoginBtn").GetComponent<Button>();

        loginBtn.onClick.AddListener(LonginBtnOnClick);//����¼��Ǹ�ί�У��󶨵��¼����ǵ����ִ�е�����
    }
    void WebsiteBtnOnClick() {
        Application.OpenURL("��ַ��ת");
    }
    void LonginBtnOnClick() {
        if (toggle.isOn == true)
        {
            string account = usernameInput.text;
            string pwd = pwInput.text;
        }
        else {
            Debug.Log("��Ҫͬ����˽Э��");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
