using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // 用于加载场景
using System.Linq;

public class FinishUI : MonoBehaviour
{
    [Header("UI 元素")]
    public GameObject resultPanel;    // 结果面板
    public TMP_Text resultText;       // 显示结果
    public Button retryButton;        // 重新挑战按钮（直接重载场景）
    public Button quitButton;         // 退出按钮
    public Button ChangeScences; //更改挑战场景

    private void Start()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (retryButton != null)
            retryButton.onClick.AddListener(RestartRace);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
        if (ChangeScences != null)
            ChangeScences.onClick.AddListener(OnChangeMapClicked);
    }

    /// <summary>
    /// 显示多圈比赛结果（RaceManager）
    /// </summary>
    public void ShowResults(List<RaceManager.PlayerData> ranking)
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);

        resultText.text = "Competition Result\n\n";

        for (int i = 0; i < ranking.Count; i++)
        {
            string pos = (i + 1) + ". ";
            string bestLap = ranking[i].bestLapTime < Mathf.Infinity ?
                FormatTime(ranking[i].bestLapTime) : "N/A";

            resultText.text += $"{pos}{ranking[i].playerName} - Total {FormatTime(ranking[i].raceTime)} | BestLap {bestLap}\n";
        }
    }

    /// <summary>
    /// 显示单圈比赛结果（OneLapRaceManager）
    /// </summary>
    public void ShowResults(List<OneLapRaceManager.PlayerData> ranking)
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);

        resultText.text = "Competition Result (One Lap)\n\n";

        var ordered = ranking.OrderBy(p => p.CurrentRaceTime).ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            string pos = (i + 1) + ". ";
            resultText.text += $"{pos}{ordered[i].playerName} - Time {FormatTime(ordered[i].CurrentRaceTime)}\n";
        }
    }

    /// <summary>
    /// 隐藏结果面板
    /// </summary>
    public void Hide()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    /// <summary>
    /// 点击重新挑战（直接重载当前场景）
    /// </summary>
    private void RestartRace()
    {
        Debug.Log("重新挑战，重载场景！");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnChangeMapClicked()
    {
        // 不做 SceneManager.LoadScene，留给按钮挂载的其他脚本处理
    }

    /// <summary>
    /// 点击退出
    /// </summary>
    private void OnQuitClicked()
    {
        Debug.Log("退出游戏！");
        Application.Quit();
    }

    private string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int hundredths = (int)((time * 100) % 100);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundredths);
    }
}
