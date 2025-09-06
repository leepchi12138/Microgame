using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownUI : MonoBehaviour
{
    [Header("UIԪ��")]
    public GameObject countdownPanel;
    public TMP_Text text3;
    public TMP_Text text2;
    public TMP_Text text1;
    public TMP_Text textGo;

    [Header("��Ч")]
    public AudioSource audioSource;
    public AudioClip countClip;
    public AudioClip goClip;

    [Header("����ʱʱ��")]
    public float interval = 1f;

    private RaceManager raceManager;

    void Awake()
    {
        // �ҵ� RaceManager������������У�
        raceManager = FindObjectOfType<RaceManager>();
    }

    // �ⲿ����
    public void StartCountdown()
    {
        Time.timeScale = 0f;
        countdownPanel.SetActive(true);
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        // ��ȫ������
        text3.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);
        text1.gameObject.SetActive(false);
        textGo.gameObject.SetActive(false);

        // ��ʾ 3
        text3.gameObject.SetActive(true);
        PlaySound(countClip);
        yield return WaitForRealSeconds(interval);
        text3.gameObject.SetActive(false);

        // ��ʾ 2
        text2.gameObject.SetActive(true);
        PlaySound(countClip);
        yield return WaitForRealSeconds(interval);
        text2.gameObject.SetActive(false);

        // ��ʾ 1
        text1.gameObject.SetActive(true);
        PlaySound(countClip);
        yield return WaitForRealSeconds(interval);
        text1.gameObject.SetActive(false);

        // ��ʾ GO!
        textGo.gameObject.SetActive(true);
        PlaySound(goClip);
        yield return WaitForRealSeconds(interval);
        textGo.gameObject.SetActive(false);

        // ����ʱ����
        countdownPanel.SetActive(false);
        Time.timeScale = 1f;

        // ֪ͨ RaceManager ��ʼ����
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

    // ���� Time.timeScale Ӱ��ĵȴ�
    IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }
}
