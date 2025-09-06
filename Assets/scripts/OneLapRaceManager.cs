using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class OneLapRaceManager : MonoBehaviour
{
    public static OneLapRaceManager Instance;

    [Header("比赛状态")]
    public bool raceStarted = false;
    public bool raceFinished = false;

    [Header("UI 显示")]
    public Text timerText;
    public Text bestLapText;
    public Text lastLapText;
    public TMP_Text rankText;

    [Header("结算画面")]
    public FinishUI finishUI;

    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public float raceStartTime;
        public float finishTime;
        public bool finished = false;
        public float lastLapTime = 0f;

        public float CurrentRaceTime => finished ? finishTime - raceStartTime : Time.time - raceStartTime;

        public PlayerData(string name) { playerName = name; }
    }

    public List<PlayerData> players = new List<PlayerData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            players.Clear();
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (finishUI != null) finishUI.Hide();

        string playerName = LoginManager.CurrentPlayerName;
        if (!string.IsNullOrEmpty(playerName))
        {
            ResetPlayer localPlayer = FindObjectOfType<ResetPlayer>();
            if (localPlayer != null)
            {
                localPlayer.SetPlayerName(playerName);
                players.Add(new PlayerData(playerName));
            }
        }

        CountdownUI countdown = FindObjectOfType<CountdownUI>();
        if (countdown != null) countdown.StartCountdown();

        StartRace();
    }

    private void Update()
    {
        if (raceStarted && !raceFinished)
        {
            UpdateUI();
            UpdateRanking();
        }
    }

    public void StartRace()
    {
        raceStarted = true;
        raceFinished = false;
        foreach (var p in players)
        {
            p.raceStartTime = Time.time;
            p.finished = false;
            p.lastLapTime = 0f;
        }
    }

    public void FinishRace(string playerName, float lapTime)
    {
        if (raceFinished) return;

        var pData = players.Find(p => p.playerName == playerName);
        if (pData == null) return;

        pData.finished = true;
        pData.finishTime = Time.time;
        pData.lastLapTime = lapTime;

        Debug.Log($"{playerName} 到达终点！圈速: {lapTime:F2}");

        if (finishUI != null)
            finishUI.ShowResults(players);

        raceFinished = true;
        raceStarted = false;
    }

    private void UpdateUI()
    {
        var player = players.FirstOrDefault();
        if (player == null) return;

        if (timerText != null) timerText.text = "比赛时间: " + FormatTime(player.CurrentRaceTime);
        if (bestLapText != null) bestLapText.text = "最佳圈速: " + FormatTime(player.lastLapTime);
        if (lastLapText != null) lastLapText.text = "上一圈时间: " + FormatTime(player.lastLapTime);
    }

    private void UpdateRanking()
    {
        var ranking = players.OrderBy(p => p.CurrentRaceTime).ToList();
        if (rankText != null)
        {
            rankText.text = "";
            for (int i = 0; i < ranking.Count; i++)
                rankText.text += $"{GetOrdinal(i + 1)}: {ranking[i].playerName}\n";
        }
    }

    private string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int hundredths = (int)((time * 100) % 100);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundredths);
    }

    private string GetOrdinal(int number)
    {
        switch (number % 100)
        {
            case 11:
            case 12:
            case 13: return number + "th";
        }
        switch (number % 10)
        {
            case 1: return number + "st";
            case 2: return number + "nd";
            case 3: return number + "rd";
            default: return number + "th";
        }
    }
}
