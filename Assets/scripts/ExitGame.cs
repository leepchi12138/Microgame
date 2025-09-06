using UnityEngine;
using UnityEngine.UI;

public class ExitGame : MonoBehaviour
{
    [Tooltip("�˳�ȷ�ϵ�����UI���")]
    public GameObject confirmationPanel;

    [Tooltip("ȷ���˳���ť")]
    public Button confirmButton;

    [Tooltip("ȡ���˳���ť")]
    public Button cancelButton;

    private Button exitButton;

    void Start()
    {
        // ��ȡ��ǰ�����ϵ��˳���ť���
        exitButton = GetComponent<Button>();

        // ȷ��������ʼ�����ص�
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }

        // ���˳���ť����¼�
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ShowConfirmationPanel);
        }
        else
        {
            Debug.LogError("��ǰ������û��Button�����");
        }

        // ��ȷ�Ϻ�ȡ����ť�¼�
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ExitGameFunC);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(HideConfirmationPanel);
        }
    }

    // ��ʾȷ�ϵ���
    private void ShowConfirmationPanel()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("����Inspector��ָ��ȷ�ϵ�����壡");
        }
    }

    // ����ȷ�ϵ���
    private void HideConfirmationPanel()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    // ִ���˳���Ϸ����
    private void ExitGameFunC()
    {
        // �����ص���
        HideConfirmationPanel();

        // �ڱ༭��������ʱ�����־����ʵ���˳�
#if UNITY_EDITOR
        Debug.Log("��Ϸ�˳����༭��ģʽ��");
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // ��ʵ������ʱ�˳���Ϸ
            Debug.Log("��Ϸ�˳�");
            Application.Quit();
#endif
    }

    // ��ѡ�����ESC���˳�����
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �������û��ʾ����ESC��ʾ�����������������ʾ����ESCȡ���˳�
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
