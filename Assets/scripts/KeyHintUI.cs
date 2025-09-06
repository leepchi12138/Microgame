using UnityEngine;
using UnityEngine.UI;

public class KeyImageFeedback : MonoBehaviour
{
    [Header("键位设置")]
    public KeyCode key = KeyCode.W; // 当前按键

    [Header("UI 元素")]
    public Image keyImage; // 父类白底图像 (WImage)

    [Header("颜色设置")]
    public Color normalColor = Color.white;                  // 默认颜色
    public Color pressedColor = new Color(0.4f, 0.7f, 1f);   // 按下颜色 (浅蓝)

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
