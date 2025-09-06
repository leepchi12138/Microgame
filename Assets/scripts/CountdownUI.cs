using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownUI : MonoBehaviour
{
    [Header("UI元素")]
    public GameObject countdownPanel;
    public TMP_Text text3;
    public TMP_Text text2;
    public TMP_Text text1;
    public TMP_Text textGo;

    [Header("音效")]
    public AudioSource audioSource;
    public AudioClip countClip;
    public AudioClip goClip;

    [Header("倒计时时间")]
    public float interval = 1f;

    private RaceManager raceManager;

    void Awake()
    {
        // 找到 RaceManager（场景里必须有）
        raceManager = FindObjectOfType<RaceManager>();
    }

    // 外部调用
    public void StartCountdown()
    {
        Time.timeScale = 0f;
        countdownPanel.SetActive(true);
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        // 先全部隐藏
        text3.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);
        text1.gameObject.SetActive(false);
        textGo.gameObject.SetActive(false);

        // 显示 3
        text3.gameObject.SetActive(true);
        PlaySound(countClip);
        yield return WaitForRealSeconds(interval);
        text3.gameObject.SetActive(false);

        // 显示 2
        text2.gameObject.SetActive(true);
        PlaySound(countClip);
        yield return WaitForRealSeconds(interval);
        text2.gameObject.SetActive(false);

        // 显示 1
        text1.gameObject.SetActive(true);
        PlaySound(countClip);
        yield return WaitForRealSeconds(interval);
        text1.gameObject.SetActive(false);

        // 显示 GO!
        textGo.gameObject.SetActive(true);
        PlaySound(goClip);
        yield return WaitForRealSeconds(interval);
        textGo.gameObject.SetActive(false);

        // 倒计时结束
        countdownPanel.SetActive(false);
        Time.timeScale = 1f;

        // 通知 RaceManager 开始比赛
        if (raceManager != null)
            raceManager.StartRace();
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    // 不受 Time.timeScale 影响的等待
    IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }
}
