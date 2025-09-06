using UnityEngine;
using UnityEngine.UI;

public class ExitGame : MonoBehaviour
{
    [Tooltip("退出确认弹窗的UI面板")]
    public GameObject confirmationPanel;

    [Tooltip("确认退出按钮")]
    public Button confirmButton;

    [Tooltip("取消退出按钮")]
    public Button cancelButton;

    private Button exitButton;

    void Start()
    {
        // 获取当前对象上的退出按钮组件
        exitButton = GetComponent<Button>();

        // 确保弹窗初始是隐藏的
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }

        // 绑定退出按钮点击事件
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ShowConfirmationPanel);
        }
        else
        {
            Debug.LogError("当前对象上没有Button组件！");
        }

        // 绑定确认和取消按钮事件
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ExitGameFunC);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(HideConfirmationPanel);
        }
    }

    // 显示确认弹窗
    private void ShowConfirmationPanel()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("请在Inspector中指定确认弹窗面板！");
        }
    }

    // 隐藏确认弹窗
    private void HideConfirmationPanel()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    // 执行退出游戏操作
    private void ExitGameFunC()
    {
        // 先隐藏弹窗
        HideConfirmationPanel();

        // 在编辑器中运行时输出日志，不实际退出
#if UNITY_EDITOR
        Debug.Log("游戏退出（编辑器模式）");
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // 在实际运行时退出游戏
            Debug.Log("游戏退出");
            Application.Quit();
#endif
    }

    // 可选：添加ESC键退出功能
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 如果弹窗没显示，按ESC显示弹窗；如果弹窗已显示，按ESC取消退出
            if (confirmationPanel != null && confirmationPanel.activeSelf)
            {
                HideConfirmationPanel();
            }
            else
            {
                ShowConfirmationPanel();
            }
        }
    }
}
