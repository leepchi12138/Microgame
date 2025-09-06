using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    [Tooltip("玩法说明面板UI")]
    public GameObject howToPlayPanel;

    [Tooltip("关闭玩法说明按钮")]
    public Button closeButton;

    private Button howToPlayButton;

    void Start()
    {
        // 获取当前对象上的按钮组件
        howToPlayButton = GetComponent<Button>();

        // 确保面板初始是隐藏的
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

        // 绑定按钮点击事件
        if (howToPlayButton != null)
        {
            howToPlayButton.onClick.AddListener(ShowHowToPlay);
        }
        else
        {
            Debug.LogError("当前对象上没有Button组件！");
        }

        // 绑定关闭按钮事件
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideHowToPlay);
        }
    }

    // 显示玩法说明
    private void ShowHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("请在Inspector中指定玩法说明面板！");
        }
    }

    // 隐藏玩法说明
    private void HideHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    // ESC 键控制
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (howToPlayPanel != null && howToPlayPanel.activeSelf)
            {
                HideHowToPlay();
            }
        }
    }
}
