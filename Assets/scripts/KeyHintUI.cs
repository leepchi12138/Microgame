using UnityEngine;
using UnityEngine.UI;

public class KeyImageFeedback : MonoBehaviour
{
    [Header("��λ����")]
    public KeyCode key = KeyCode.W; // ��ǰ����

    [Header("UI Ԫ��")]
    public Image keyImage; // ����׵�ͼ�� (WImage)

    [Header("��ɫ����")]
    public Color normalColor = Color.white;                  // Ĭ����ɫ
    public Color pressedColor = new Color(0.4f, 0.7f, 1f);   // ������ɫ (ǳ��)

    private void Update()
    {
        if (Input.GetKey(key))
        {
            keyImage.color = pressedColor;
        }
        else
        {
            keyImage.color = normalColor;
        }
    }
}
