using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    [Tooltip("�淨˵�����UI")]
    public GameObject howToPlayPanel;

    [Tooltip("�ر��淨˵����ť")]
    public Button closeButton;

    private Button howToPlayButton;

    void Start()
    {
        // ��ȡ��ǰ�����ϵİ�ť���
        howToPlayButton = GetComponent<Button>();

        // ȷ������ʼ�����ص�
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

        // �󶨰�ť����¼�
        if (howToPlayButton != null)
        {
            howToPlayButton.onClick.AddListener(ShowHowToPlay);
        }
        else
        {
            Debug.LogError("��ǰ������û��Button�����");
        }

        // �󶨹رհ�ť�¼�
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideHowToPlay);
        }
    }

    // ��ʾ�淨˵��
    private void ShowHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("����Inspector��ָ���淨˵����壡");
        }
    }

    // �����淨˵��
    private void HideHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    // ESC ������
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
