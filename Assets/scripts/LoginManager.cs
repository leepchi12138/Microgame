using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("�����")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    [Header("��ť")]
    public Button loginButton;
    public Button registerButton;

    [Header("�������� UI")]
    public GameObject feedbackPopup;   // ��������Canvas
    public TMP_Text feedbackText;      // ���������ʾ����

    [Header("UI �л�")]
    public GameObject loginUI;   // ��¼���棨�˺�+����+��ť��
    public GameObject gameUI;    // ����UI����ʱ���������ȣ�

    // ģ�����ݿ�
    private static Dictionary<string, string> userDatabase = new Dictionary<string, string>();

    // ��ǰ�����
    public static string CurrentPlayerName;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLogin);
        registerButton.onClick.AddListener(OnRegister);

        if (feedbackPopup != null)
            feedbackPopup.SetActive(false); // Ĭ������

        if (gameUI != null)
            gameUI.SetActive(false); // ��ϷUIһ��ʼ����

        //  ��Ҫ�� Start() ������������������¼��û����
    }

    private void ShowFeedback(string message)
    {
        if (feedbackPopup != null && feedbackText != null)
        {
            feedbackText.text = message;
            feedbackPopup.SetActive(true);

            CancelInvoke(nameof(HideFeedback));
            Invoke(nameof(HideFeedback), 2f);
        }
    }

    private void HideFeedback()
    {
        if (feedbackPopup != null)
            feedbackPopup.SetActive(false);
    }

    private void OnRegister()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowFeedback("The account or password cannot be empty!");
            return;
        }

        if (userDatabase.ContainsKey(username))
        {
            ShowFeedback("This username already exists!");
        }
        else
        {
            userDatabase.Add(username, password);
            ShowFeedback("Registration successful. Please log in!");
        }
    }

    private void OnLogin()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowFeedback("Please enter your account and password!");
            return;
        }

        if (userDatabase.ContainsKey(username) && userDatabase[username] == password)
        {
            CurrentPlayerName = username;
            ShowFeedback("Login successful! Welcome " + username);

            // ��¼�ɹ���������������� RaceManager
            ResetPlayer localPlayer = FindObjectOfType<ResetPlayer>();
            if (localPlayer != null)
            {
                localPlayer.SetPlayerName(CurrentPlayerName);

                if (RaceManager.Instance != null)
                    RaceManager.Instance.RegisterLocalPlayer(localPlayer);
            }

            // �ӳ��л� UI
            Invoke(nameof(SwitchToGameUI), 1.5f);
        }
        else
        {
            ShowFeedback("Account or password error!");
        }
    }

    private void SwitchToGameUI()
    {
        if (loginUI != null)
            loginUI.SetActive(false);

        if (gameUI != null)
            gameUI.SetActive(true);
    }
}
