using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ���������������ࣨ����ģʽ��
/// ְ��ͳ�ﾺ�ٱ���ȫ���̣���ʼ��/��ʱ/Ȧ������/��������/���������
/// ���ⲿģ�������LoginManager����ȡ���������ResetPlayer����ҿ��ƣ���CountdownUI������ʱ����FinishUI�����㣩
/// </summary>
public class RaceManager : MonoBehaviour
{
    // ����ʵ����ȫ��Ψһ��ڣ�ȷ�������ű���ֱ�ӵ��ã���ResetPlayer����Ȧ�����㣩
    public static RaceManager Instance;

    [Header("���ٲ���")]
    public int totalLaps = 2;               // ������Ȧ��������Inspector��嶯̬������
    public bool raceStarted = false;        // ����״̬��ǣ���ֹUpdate���ظ�����
    public bool raceFinished = false;       // ����������ǣ�����UI���º��߼�ִ�п���

    [Header("UI��ʾ")]
    public Text timerText;                  // �ܱ���ʱ����ʾ�����������е�Text�����
    public Text lapText;                    // ��ǰȦ��/��Ȧ����ʾ
    public Text bestLapText;                // �����ʷ���Ȧ����ʾ
    public Text lastLapText;                // �����һȦ���ʱ����ʾ
    public TMP_Text rankText;               // ʵʱ������ʾ��֧�ֶ��������

    [Header("���㻭��")]
    public FinishUI finishUI;               // ������������õĽ������ű�����

    /// <summary>
    /// ������ݷ�װ�ࣨ���л�����Inspector�鿴��
    /// ְ�𣺴洢������ҵ����б������ݣ�������ɢ��������
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        public string playerName;           // ���Ψһ��ʶ����LoginManagerͬ����
        public int currentLap = 0;          // ��ǰ�����Ȧ������ʼ0��δ����κ�Ȧ��
        public float raceTime = 0f;         // �ۼ��ܱ���ʱ�䣨ʵʱ���㣩
        public float bestLapTime = Mathf.Infinity;  // ���Ȧ�٣���ʼ�����δ������ЧȦ�٣�
        public List<float> lapTimes = new List<float>();  // ÿȦʱ����ʷ��¼������ͳ�ƺ���ʾ��
        public float lapStartTime = 0f;     // ��ǰȦ��ʼ��ʱ�㣨Time.timeֵ�����ڼ��㵥Ȧ��ʱ��
        public float raceStartTime;   //������ʼ�Ĺ̶�ʱ��
        public float TotalRaceTime => Time.time - raceStartTime; //  ������ʱ��
        /// <summary>
        /// ���캯���������������ʱ���봫�����ƣ�ȷ��Ψһ�ԣ�
        /// </summary>
        /// <param name="name">������ƣ���LoginManager��ȡ��</param>
        public PlayerData(string name)
        {
            playerName = name;
        }
    }

    // ����������ݼ��ϣ�֧�ֶ���ң��������+AI/������Ҿ����ڴˣ�
    public List<PlayerData> players = new List<PlayerData>();

    /// <summary>
    /// ������ʼ����Awake��Start��ִ�У�ȷ����������ʱ�ȴ���ʵ����
    /// �߼�����ֹ�ظ�����ʵ������ղ������ݣ����ⳡ���л������������Ⱦ��
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            players.Clear();  // �ؼ����������¼���ʱ��վ�����б������ظ����
        }
        else
        {
            // ������ʵ�������ٵ�ǰ���󣨱�֤����Ψһ�ԣ���ֹ�߼���ͻ��
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// ��Ϸ������ʼ������ִ��һ�Σ�
    /// ���̣�1.��ȡ��¼����� 2.����ҿ��ƽű� 3.��������ʱ 4.��ʼ������
    /// </summary>
    void Start()
    {
        if (finishUI != null)
            finishUI.gameObject.SetActive(false);
        // 1.�ӵ�¼��������ȡ��ǰ������ƣ�ǰ�᣺LoginManager����ȷ����CurrentPlayerName��
        string playerName = LoginManager.CurrentPlayerName;
        if (!string.IsNullOrEmpty(playerName))
        {
            // 2.�ҵ������б�����ҵĿ��ƽű���ResetPlayer���������λ�����õ��߼���
            ResetPlayer localPlayer = FindObjectOfType<ResetPlayer>();
            if (localPlayer != null)
            {
                localPlayer.SetPlayerName(playerName);  // ����ҿ��ƽű���ֵ���ƣ����ں���ƥ�����ݣ�
                players.Add(new PlayerData(playerName));  // �����������ӵ������б�
                Debug.Log("Player: " + playerName + " has joined the race!");  // ������־��ȷ�����ע��ɹ�
            }
        }

        // 3.������������ʱ���������д���CountdownUI�ű���
        CountdownUI countdown = FindObjectOfType<CountdownUI>();
        if (countdown != null)
            countdown.StartCountdown();  // ע��CountdownUI��ʵ�ֵ���ʱ�߼����������Ӧ������ʼ��������ǰ����˴����Ż�������ʱ���������StartRace��

        // 4.��ʼ���������ݣ���ǰ���룺����ʱ���������ͬ����ʵ����Ŀ�ɵ���Ϊ����ʱ��������ã�
        StartRace();
    }

    /// <summary>
    /// ÿ֡���£�ʵʱִ�У�
    /// �������������ڱ����ѿ�ʼ��δ����ʱִ�У�������Ч���㣩
    /// �����߼���1.�������������ʱ�� 2.ˢ��UI 3.���¼�������
    /// </summary>
    void Update()
    {
        if (raceStarted && !raceFinished)
        {
            // 1.����������ҵ�ʵʱ��ʱ�䣨��ʽ����ǰȦ����ʱ�� + ��ʷȦʱ���ܺͣ�
            foreach (var p in players)
            {
                p.raceTime = p.TotalRaceTime;   // �� raceStartTime ���㣬���ᱻ����
            }

            // 2.ʵʱˢ��UI��ʾ��ʱ�䡢Ȧ���ȣ�
            UpdateUI();
            // 3.ʵʱ���¼���������֧�ֶ���Ҷ�̬����
            UpdateRanking();
        }
    }

    /// <summary>
    /// ��������/���÷������ɱ��ⲿ���ã���������ս��ť��
    /// ְ������������ҵı������ݣ��ָ���ʼ״̬
    /// </summary>
    public void StartRace()
    {
        raceStarted = true;    // ��Ǳ�����ʼ
        raceFinished = false;  // ��Ǳ���δ����

        // ����������ң���ʼ�����ֱ�������
        foreach (var p in players)
        {
            p.currentLap = 0;
            p.lapTimes.Clear();
            p.bestLapTime = Mathf.Infinity;
            p.raceTime = 0f;
            p.raceStartTime = Time.time;   // �̶�һ�Σ�������ʱ������￪ʼ��
            p.lapStartTime = Time.time;    // ���ֻ����Ȧ��
        }

        Debug.Log("������ʼ��");
        UpdateUI();  // ����ˢ��UI�������ʼ״̬��ʾ�쳣
    }

    /// <summary>
    /// Ȧ����ɴ������������ⲿ���ã�����Ҵ����յ��ߴ�������
    /// ���̣�1.ƥ��������� 2.���㵥Ȧʱ�� 3.�������Ȧ�� 4.�ж��Ƿ���ɱ���
    /// </summary>
    /// <param name="playerName">���Ȧ��������ƣ�����ƥ���ӦPlayerData��</param>
    //public void FinishLap(string playerName)
    //{
    //    if (raceFinished) return;

    //    var pData = players.Find(p => p.playerName == playerName);
    //    if (pData == null) return;

    //    float lapTime = Time.time - pData.lapStartTime;

    //    if (pData.currentLap == 0)
    //    {
    //        Debug.Log($"{pData.playerName} ������ɣ���ʼ��1Ȧ��ʱ");
    //    }
    //    else
    //    {

    //        pData.lapTimes.Add(lapTime);
    //        pData.bestLapTime = Mathf.Min(pData.bestLapTime, lapTime);

    //        Debug.Log($"{pData.playerName} ��ɵ� {pData.currentLap} Ȧ, ��Ȧ��ʱ: {lapTime:F2}");
    //    }

    //    pData.currentLap++;
    //    pData.lapStartTime = Time.time;


    //    if (pData.currentLap > totalLaps)
    //    {
    //        Debug.Log($"{pData.playerName} ��ɱ���! ����ʱ: {pData.TotalRaceTime:F2}");
    //        FinishRace();
    //    }
    //}
    /// <summary>
    /// Ȧ����ɴ������������ⲿ���ã�����Ҵ����յ��ߴ�������
    /// ���̣�1.ƥ��������� 2.���㵥Ȧʱ�� 3.�������Ȧ�� 4.�ж��Ƿ���ɱ���
    /// </summary>
    /// <param name="playerName">���Ȧ��������ƣ�����ƥ���ӦPlayerData��</param>
    public void FinishLap(string playerName)
    {
        // 1.������������ҵ���Ӧ������
        var pData = players.Find(p => p.playerName == playerName);
        if (pData == null)
            return;  // δ�ҵ�������ݣ�ֱ�ӷ���

        // ---------------------------
        // 2. �ж��Ƿ�������Ȧ
        // ---------------------------
        if (pData.currentLap == 0)
        {
            Debug.Log($"{pData.playerName} ������ɣ���ʼ��1Ȧ��ʱ");
            pData.currentLap = 1;           // ������ɺ�����1Ȧ
            pData.lapStartTime = Time.time; // ��¼��1Ȧ��ʼʱ��
            return;                         // ����Ȧ����¼Ȧ�٣�ֱ�ӷ���
        }

        // ---------------------------
        // 3. ���㵱ǰȦ��ʱ����������
        // ---------------------------
        float lapTime = Time.time - pData.lapStartTime;
        pData.lapTimes.Add(lapTime);                  // �����ʷȦʱ��
        pData.bestLapTime = Mathf.Min(pData.bestLapTime, lapTime); // �������Ȧ��

        Debug.Log($"{pData.playerName} ��ɵ� {pData.currentLap} Ȧ, ��Ȧ��ʱ: {lapTime:F2}");

        // ---------------------------
        // 4. ����Ƿ������Ȧ��
        // ---------------------------
        if (pData.currentLap >= totalLaps)
        {
            Debug.Log($"{pData.playerName} ��ɱ���! ����ʱ: {pData.TotalRaceTime:F2}");
            FinishRace();  // �������������߼�����ʾ FinishUI
        }
        else
        {
            // ---------------------------
            // 5. δ�����Ȧ����׼����һȦ
            // ---------------------------
            pData.currentLap++;          // Ȧ�� +1
            pData.lapStartTime = Time.time; // ��һȦ��ʼ��ʱ
        }
    }




    /// <summary>
    /// ʵʱ������������ʾ
    /// �������1.���Ȱ������Ȧ������Ȧ�����ǰ�� 2.Ȧ����ͬ����ʱ������ʱ��̵���ǰ��
    /// </summary>
    private void UpdateRanking()
    {
        // LINQ���򣺰����������б���������������б����޸�ԭ�б�
        var ranking = players
            .OrderByDescending(p => p.currentLap)  // ��һ���ȼ���Ȧ�������ǰ��
            .ThenBy(p => p.raceTime)               // �ڶ����ȼ���Ȧ����ͬ��ʱ��̵���ǰ��
            .ToList();

        // ���������ı�����UI����Ѹ�ֵ��
        if (rankText != null)
        {
            rankText.text = "";  // ��վ������������ı����ӣ�
            for (int i = 0; i < ranking.Count; i++)
            {
                string pos = GetOrdinal(i + 1);  // ����������תΪ�����ʣ�1��1st��2��2nd��
                rankText.text += $"{pos}: {ranking[i].playerName}\n";  // ƴ�������ı���������ʾÿ����ң�
            }
        }
    }

    /// <summary>
    /// ���������߼����������㣩
    /// ���̣�1.���±���״̬ 2.������������ 3.��ʾ�������
    /// </summary>
    private void FinishRace()
    {
        raceFinished = true;  // ��Ǳ���������ֹͣUpdate�е�ʵʱ���㣩
        raceStarted = false;  // ��Ǳ���δ����
        Debug.Log("����������");

        var ranking = players
            .OrderByDescending(p => p.currentLap)    // �ȱ�Ȧ��
            .ThenBy(p => p.TotalRaceTime)            // Ȧ����ͬ�ٱȱ�����ʱ��
            .ToList();
        for (int i = 0; i < ranking.Count; i++)
        {
            Debug.Log($"{ranking[i].playerName} - ��ʱ��: {ranking[i].TotalRaceTime:F2}, ���Ȧ��: {ranking[i].bestLapTime:F2}");
        }

        if (finishUI != null)
        {
            finishUI.gameObject.SetActive(true);
            finishUI.ShowResults(ranking);
        }
    }

    /// <summary>
    /// UIʵʱˢ�·����������±��������Ϣ�����������չ��
    /// �߼�����ȡ��һ����ң�Ĭ�ϱ�����ң����ݣ����¶�ӦUI�ı�
    /// </summary>
    private void UpdateUI()
    {
        var player = players.FirstOrDefault();
        if (player == null) return;

        if (timerText != null)
            timerText.text = "������ʱ��: " + FormatTime(player.raceTime);

        if (lapText != null)
            lapText.text = "Ȧ��: " + player.currentLap + " / " + totalLaps;

        // ����������� 1 Ȧ�����ʾ���Ȧ��
        if (bestLapText != null)
            bestLapText.text = "���Ȧ��: " +
                (player.currentLap > 0 && player.lapTimes.Count > 0
                    ? FormatTime(player.bestLapTime)
                    : "00:00:00");

        // ����������� 1 Ȧ�����ʾ��һȦʱ��
        if (lastLapText != null && player.currentLap > 0 && player.lapTimes.Count > 0)
            lastLapText.text = "��һȦʱ��: " + FormatTime(player.lapTimes.Last());
    }

    /// <summary>
    /// ʱ���ʽ�����߷�����ͳһʱ����ʾ��ʽ��
    /// ���룺�뼶����������123.456�룩
    /// �����02:03:45����:��:���룬��λ�����㣩
    /// </summary>
    /// <param name="time">��Ҫ��ʽ����ʱ�䣨��λ���룩</param>
    /// <returns>��ʽ�����ʱ���ַ���</returns>
    private string FormatTime(float time)
    {
        int minutes = (int)(time / 60);          // ������ӣ�1����=60�룩
        int seconds = (int)(time % 60);          // ����ʣ��������ȡ��60��
        int hundredths = (int)((time * 100) % 100);  // ������루������λС����ȡ��100��

        // ��ʽ���ַ�����ȷ��ÿλ������λ�������㲹�㣬��1��01��
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundredths);
    }

    /// <summary>
    /// ������ת�����߷������Ż�������ʾ���飩
    /// ����1��1st��2��2nd��3��3rd��4��4th��11��11th��12��12th��13��13th
    /// </summary>
    /// <param name="number">�������֣���������</param>
    /// <returns>�������ʺ�׺���ַ���</returns>
    private string GetOrdinal(int number)
    {
        if (number <= 0)
            return number.ToString();  // �쳣������������ֱ�ӷ���ԭ����

        // �������11��12��13���۸�λ��������th��׺
        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                return number + "th";
        }

        // ��ͨ���򣺰���λ���жϺ�׺
        switch (number % 10)
        {
            case 1:
                return number + "st";
            case 2:
                return number + "nd";
            case 3:
                return number + "rd";
            default:
                return number + "th";
        }
    }

    /// <summary>
    /// �������ע�Ჹ�䷽����֧���ⲿ��̬ע�ᣬ���ӳټ������ʱ��
    /// ���ã������ظ����ͬһ��ң�ȷ���������Ψһ��
    /// </summary>
    /// <param name="localPlayer">������ҵ�ResetPlayer�������������ƣ�</param>
    public void RegisterLocalPlayer(ResetPlayer localPlayer)
    {
        if (localPlayer == null)
            return;  // �����÷���

        // �������Ƿ���ע�ᣨ������ƥ�䣬�����ظ���ӣ�
        var existing = players.Find(p => p.playerName == localPlayer.playerName);
        if (existing == null)
        {
            players.Add(new PlayerData(localPlayer.playerName));
            Debug.Log($"������� {localPlayer.playerName} ��ע�ᵽ����");
        }
    }

    /// <summary>
    /// ���¿�ʼ����
    /// </summary>
    public void RestartRace()
    {
        Debug.Log("������ս��ʼ��");

        raceStarted = false;
        raceFinished = false;
        //��������������ݣ����������֣�
        foreach (var p in players)
        {
            p.currentLap = 0;   
            p.lapTimes.Clear();
            p.bestLapTime = Mathf.Infinity;
            p.raceTime = 0f;
            p.raceStartTime = Time.time;
            p.lapStartTime = Time.time;
        }

        // ���ؽ��� UI
        if (finishUI != null)
            finishUI.Hide();

        // ����ʱ�߼�����ѡ��
        CountdownUI countdown = FindObjectOfType<CountdownUI>();
        if (countdown != null)
            countdown.StartCountdown();
        else
            StartRace(); // û�е���ʱ��ֱ�ӿ���

    }

}