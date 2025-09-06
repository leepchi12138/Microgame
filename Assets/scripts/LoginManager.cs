using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("输入框")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    [Header("按钮")]
    public Button loginButton;
    public Button registerButton;

    [Header("弹窗反馈 UI")]
    public GameObject feedbackPopup;   // 整个弹窗Canvas
    public TMP_Text feedbackText;      // 弹窗里的提示文字

    [Header("UI 切换")]
    public GameObject loginUI;   // 登录界面（账号+密码+按钮）
    public GameObject gameUI;    // 比赛UI（计时器、排名等）

    // 模拟数据库
    private static Dictionary<string, string> userDatabase = new Dictionary<string, string>();

    // 当前玩家名
    public static string CurrentPlayerName;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLogin);
        registerButton.onClick.AddListener(OnRegister);

        if (feedbackPopup != null)
            feedbackPopup.SetActive(false); // 默认隐藏

        if (gameUI != null)
            gameUI.SetActive(false); // 游戏UI一开始隐藏

        //  不要在 Start() 里设置玩家名，这里登录还没发生
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

            // 登录成功后再设置玩家名和 RaceManager
            ResetPlayer localPlayer = FindObjectOfType<ResetPlayer>();
            if (localPlayer != null)
            {
                localPlayer.SetPlayerName(CurrentPlayerName);

                if (RaceManager.Instance != null)
                    RaceManager.Instance.RegisterLocalPlayer(localPlayer);
            }

            // 延迟切换 UI
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
