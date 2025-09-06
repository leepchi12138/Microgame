using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 比赛管理器核心类（单例模式）
/// 职责：统筹竞速比赛全流程（初始化/计时/圈数管理/排名计算/结果触发）
/// 与外部模块关联：LoginManager（获取玩家名）、ResetPlayer（玩家控制）、CountdownUI（倒计时）、FinishUI（结算）
/// </summary>
public class RaceManager : MonoBehaviour
{
    // 单例实例：全局唯一入口，确保其他脚本可直接调用（如ResetPlayer触发圈数结算）
    public static RaceManager Instance;

    [Header("竞速参数")]
    public int totalLaps = 2;               // 比赛总圈数（可在Inspector面板动态调整）
    public bool raceStarted = false;        // 比赛状态标记：防止Update中重复计算
    public bool raceFinished = false;       // 比赛结束标记：控制UI更新和逻辑执行开关

    [Header("UI显示")]
    public Text timerText;                  // 总比赛时间显示（关联场景中的Text组件）
    public Text lapText;                    // 当前圈数/总圈数显示
    public Text bestLapText;                // 玩家历史最佳圈速显示
    public Text lastLapText;                // 玩家上一圈完成时间显示
    public TMP_Text rankText;               // 实时排名显示（支持多玩家排序）

    [Header("结算画面")]
    public FinishUI finishUI;               // 比赛结束后调用的结算界面脚本引用

    /// <summary>
    /// 玩家数据封装类（序列化可在Inspector查看）
    /// 职责：存储单个玩家的所有比赛数据，避免零散变量管理
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        public string playerName;           // 玩家唯一标识（与LoginManager同步）
        public int currentLap = 0;          // 当前已完成圈数（初始0：未完成任何圈）
        public float raceTime = 0f;         // 累计总比赛时间（实时计算）
        public float bestLapTime = Mathf.Infinity;  // 最佳圈速（初始无穷大：未产生有效圈速）
        public List<float> lapTimes = new List<float>();  // 每圈时间历史记录（用于统计和显示）
        public float lapStartTime = 0f;     // 当前圈开始计时点（Time.time值，用于计算单圈耗时）
        public float raceStartTime;   //比赛开始的固定时间
        public float TotalRaceTime => Time.time - raceStartTime; //  比赛总时间
        /// <summary>
        /// 构造函数：创建玩家数据时必须传入名称（确保唯一性）
        /// </summary>
        /// <param name="name">玩家名称（从LoginManager获取）</param>
        public PlayerData(string name)
        {
            playerName = name;
        }
    }

    // 所有玩家数据集合（支持多玩家，本地玩家+AI/联机玩家均存于此）
    public List<PlayerData> players = new List<PlayerData>();

    /// <summary>
    /// 单例初始化（Awake比Start早执行，确保场景加载时先创建实例）
    /// 逻辑：防止重复创建实例，清空残留数据（避免场景切换后玩家数据污染）
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            players.Clear();  // 关键：场景重新加载时清空旧玩家列表，避免重复添加
        }
        else
        {
            // 若已有实例，销毁当前对象（保证单例唯一性，防止逻辑冲突）
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// 游戏启动初始化（仅执行一次）
    /// 流程：1.获取登录玩家名 2.绑定玩家控制脚本 3.启动倒计时 4.初始化比赛
    /// </summary>
    void Start()
    {
        if (finishUI != null)
            finishUI.gameObject.SetActive(false);
        // 1.从登录管理器获取当前玩家名称（前提：LoginManager已正确设置CurrentPlayerName）
        string playerName = LoginManager.CurrentPlayerName;
        if (!string.IsNullOrEmpty(playerName))
        {
            // 2.找到场景中本地玩家的控制脚本（ResetPlayer：负责玩家位置重置等逻辑）
            ResetPlayer localPlayer = FindObjectOfType<ResetPlayer>();
            if (localPlayer != null)
            {
                localPlayer.SetPlayerName(playerName);  // 给玩家控制脚本赋值名称（用于后续匹配数据）
                players.Add(new PlayerData(playerName));  // 将本地玩家添加到数据列表
                Debug.Log("Player: " + playerName + " has joined the race!");  // 调试日志：确认玩家注册成功
            }
        }

        // 3.启动比赛倒计时（若场景中存在CountdownUI脚本）
        CountdownUI countdown = FindObjectOfType<CountdownUI>();
        if (countdown != null)
            countdown.StartCountdown();  // 注：CountdownUI需实现倒计时逻辑，结束后才应真正开始比赛（当前代码此处可优化：倒计时结束后调用StartRace）

        // 4.初始化比赛数据（当前代码：倒计时与比赛启动同步，实际项目可调整为倒计时结束后调用）
        StartRace();
    }

    /// <summary>
    /// 每帧更新（实时执行）
    /// 触发条件：仅在比赛已开始且未结束时执行（避免无效计算）
    /// 核心逻辑：1.更新所有玩家总时间 2.刷新UI 3.重新计算排名
    /// </summary>
    void Update()
    {
        if (raceStarted && !raceFinished)
        {
            // 1.更新所有玩家的实时总时间（公式：当前圈已用时间 + 历史圈时间总和）
            foreach (var p in players)
            {
                p.raceTime = p.TotalRaceTime;   // 用 raceStartTime 计算，不会被重置
            }

            // 2.实时刷新UI显示（时间、圈数等）
            UpdateUI();
            // 3.实时重新计算排名（支持多玩家动态排序）
            UpdateRanking();
        }
    }

    /// <summary>
    /// 比赛启动/重置方法（可被外部调用：如重新挑战按钮）
    /// 职责：重置所有玩家的比赛数据，恢复初始状态
    /// </summary>
    public void StartRace()
    {
        raceStarted = true;    // 标记比赛开始
        raceFinished = false;  // 标记比赛未结束

        // 遍历所有玩家，初始化单局比赛数据
        foreach (var p in players)
        {
            p.currentLap = 0;
            p.lapTimes.Clear();
            p.bestLapTime = Mathf.Infinity;
            p.raceTime = 0f;
            p.raceStartTime = Time.time;   // 固定一次，比赛总时间从这里开始计
            p.lapStartTime = Time.time;    // 这个只负责圈速
        }

        Debug.Log("比赛开始！");
        UpdateUI();  // 立即刷新UI，避免初始状态显示异常
    }

    /// <summary>
    /// 圈数完成触发方法（由外部调用：如玩家穿过终点线触发器）
    /// 流程：1.匹配玩家数据 2.计算单圈时间 3.更新最佳圈速 4.判断是否完成比赛
    /// </summary>
    /// <param name="playerName">完成圈的玩家名称（用于匹配对应PlayerData）</param>
    //public void FinishLap(string playerName)
    //{
    //    if (raceFinished) return;

    //    var pData = players.Find(p => p.playerName == playerName);
    //    if (pData == null) return;

    //    float lapTime = Time.time - pData.lapStartTime;

    //    if (pData.currentLap == 0)
    //    {
    //        Debug.Log($"{pData.playerName} 起跑完成，开始第1圈计时");
    //    }
    //    else
    //    {

    //        pData.lapTimes.Add(lapTime);
    //        pData.bestLapTime = Mathf.Min(pData.bestLapTime, lapTime);

    //        Debug.Log($"{pData.playerName} 完成第 {pData.currentLap} 圈, 本圈用时: {lapTime:F2}");
    //    }

    //    pData.currentLap++;
    //    pData.lapStartTime = Time.time;


    //    if (pData.currentLap > totalLaps)
    //    {
    //        Debug.Log($"{pData.playerName} 完成比赛! 总用时: {pData.TotalRaceTime:F2}");
    //        FinishRace();
    //    }
    //}
    /// <summary>
    /// 圈数完成触发方法（由外部调用：如玩家穿过终点线触发器）
    /// 流程：1.匹配玩家数据 2.计算单圈时间 3.更新最佳圈速 4.判断是否完成比赛
    /// </summary>
    /// <param name="playerName">完成圈的玩家名称（用于匹配对应PlayerData）</param>
    public void FinishLap(string playerName)
    {
        // 1.根据玩家名称找到对应的数据
        var pData = players.Find(p => p.playerName == playerName);
        if (pData == null)
            return;  // 未找到玩家数据，直接返回

        // ---------------------------
        // 2. 判断是否是起跑圈
        // ---------------------------
        if (pData.currentLap == 0)
        {
            Debug.Log($"{pData.playerName} 起跑完成，开始第1圈计时");
            pData.currentLap = 1;           // 起跑完成后进入第1圈
            pData.lapStartTime = Time.time; // 记录第1圈开始时间
            return;                         // 起跑圈不记录圈速，直接返回
        }

        // ---------------------------
        // 3. 计算当前圈耗时并更新数据
        // ---------------------------
        float lapTime = Time.time - pData.lapStartTime;
        pData.lapTimes.Add(lapTime);                  // 添加历史圈时间
        pData.bestLapTime = Mathf.Min(pData.bestLapTime, lapTime); // 更新最佳圈速

        Debug.Log($"{pData.playerName} 完成第 {pData.currentLap} 圈, 本圈用时: {lapTime:F2}");

        // ---------------------------
        // 4. 检查是否完成总圈数
        // ---------------------------
        if (pData.currentLap >= totalLaps)
        {
            Debug.Log($"{pData.playerName} 完成比赛! 总用时: {pData.TotalRaceTime:F2}");
            FinishRace();  // 触发比赛结束逻辑，显示 FinishUI
        }
        else
        {
            // ---------------------------
            // 5. 未完成总圈数，准备下一圈
            // ---------------------------
            pData.currentLap++;          // 圈数 +1
            pData.lapStartTime = Time.time; // 下一圈开始计时
        }
    }




    /// <summary>
    /// 实时排名计算与显示
    /// 排序规则：1.优先按已完成圈数降序（圈多的在前） 2.圈数相同则按总时间升序（时间短的在前）
    /// </summary>
    private void UpdateRanking()
    {
        // LINQ排序：按规则对玩家列表进行排序（生成新列表，不修改原列表）
        var ranking = players
            .OrderByDescending(p => p.currentLap)  // 第一优先级：圈数多的排前面
            .ThenBy(p => p.raceTime)               // 第二优先级：圈数相同则时间短的排前面
            .ToList();

        // 更新排名文本（若UI组件已赋值）
        if (rankText != null)
        {
            rankText.text = "";  // 清空旧排名（避免文本叠加）
            for (int i = 0; i < ranking.Count; i++)
            {
                string pos = GetOrdinal(i + 1);  // 将排名数字转为序数词（1→1st，2→2nd）
                rankText.text += $"{pos}: {ranking[i].playerName}\n";  // 拼接排名文本（换行显示每个玩家）
            }
        }
    }

    /// <summary>
    /// 比赛结束逻辑（触发结算）
    /// 流程：1.更新比赛状态 2.计算最终排名 3.显示结算界面
    /// </summary>
    private void FinishRace()
    {
        raceFinished = true;  // 标记比赛结束（停止Update中的实时计算）
        raceStarted = false;  // 标记比赛未启动
        Debug.Log("比赛结束！");

        var ranking = players
            .OrderByDescending(p => p.currentLap)    // 先比圈数
            .ThenBy(p => p.TotalRaceTime)            // 圈数相同再比比赛总时间
            .ToList();
        for (int i = 0; i < ranking.Count; i++)
        {
            Debug.Log($"{ranking[i].playerName} - 总时间: {ranking[i].TotalRaceTime:F2}, 最佳圈速: {ranking[i].bestLapTime:F2}");
        }

        if (finishUI != null)
        {
            finishUI.gameObject.SetActive(true);
            finishUI.ShowResults(ranking);
        }
    }

    /// <summary>
    /// UI实时刷新方法（仅更新本地玩家信息，多玩家需扩展）
    /// 逻辑：获取第一个玩家（默认本地玩家）数据，更新对应UI文本
    /// </summary>
    private void UpdateUI()
    {
        var player = players.FirstOrDefault();
        if (player == null) return;

        if (timerText != null)
            timerText.text = "比赛总时间: " + FormatTime(player.raceTime);

        if (lapText != null)
            lapText.text = "圈数: " + player.currentLap + " / " + totalLaps;

        // 仅当完成至少 1 圈后才显示最佳圈速
        if (bestLapText != null)
            bestLapText.text = "最佳圈速: " +
                (player.currentLap > 0 && player.lapTimes.Count > 0
                    ? FormatTime(player.bestLapTime)
                    : "00:00:00");

        // 仅当完成至少 1 圈后才显示上一圈时间
        if (lastLapText != null && player.currentLap > 0 && player.lapTimes.Count > 0)
            lastLapText.text = "上一圈时间: " + FormatTime(player.lapTimes.Last());
    }

    /// <summary>
    /// 时间格式化工具方法（统一时间显示格式）
    /// 输入：秒级浮点数（如123.456秒）
    /// 输出：02:03:45（分:秒:毫秒，两位数补零）
    /// </summary>
    /// <param name="time">需要格式化的时间（单位：秒）</param>
    /// <returns>格式化后的时间字符串</returns>
    private string FormatTime(float time)
    {
        int minutes = (int)(time / 60);          // 计算分钟（1分钟=60秒）
        int seconds = (int)(time % 60);          // 计算剩余秒数（取余60）
        int hundredths = (int)((time * 100) % 100);  // 计算毫秒（保留两位小数，取余100）

        // 格式化字符串：确保每位都是两位数（不足补零，如1→01）
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundredths);
    }

    /// <summary>
    /// 序数词转换工具方法（优化排名显示体验）
    /// 规则：1→1st，2→2nd，3→3rd，4→4th，11→11th，12→12th，13→13th
    /// </summary>
    /// <param name="number">排名数字（正整数）</param>
    /// <returns>带序数词后缀的字符串</returns>
    private string GetOrdinal(int number)
    {
        if (number <= 0)
            return number.ToString();  // 异常处理：非正整数直接返回原数字

        // 特殊规则：11、12、13无论个位数，均用th后缀
        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                return number + "th";
        }

        // 普通规则：按个位数判断后缀
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
    /// 本地玩家注册补充方法（支持外部动态注册，如延迟加载玩家时）
    /// 作用：避免重复添加同一玩家，确保玩家数据唯一性
    /// </summary>
    /// <param name="localPlayer">本地玩家的ResetPlayer组件（含玩家名称）</param>
    public void RegisterLocalPlayer(ResetPlayer localPlayer)
    {
        if (localPlayer == null)
            return;  // 空引用防护

        // 检查玩家是否已注册（按名称匹配，避免重复添加）
        var existing = players.Find(p => p.playerName == localPlayer.playerName);
        if (existing == null)
        {
            players.Add(new PlayerData(localPlayer.playerName));
            Debug.Log($"本地玩家 {localPlayer.playerName} 已注册到比赛");
        }
    }

    /// <summary>
    /// 重新开始比赛
    /// </summary>
    public void RestartRace()
    {
        Debug.Log("重新挑战开始！");

        raceStarted = false;
        raceFinished = false;
        //重置所有玩家数据（但保留名字）
        foreach (var p in players)
        {
            p.currentLap = 0;   
            p.lapTimes.Clear();
            p.bestLapTime = Mathf.Infinity;
            p.raceTime = 0f;
            p.raceStartTime = Time.time;
            p.lapStartTime = Time.time;
        }

        // 隐藏结算 UI
        if (finishUI != null)
            finishUI.Hide();

        // 倒计时逻辑（可选）
        CountdownUI countdown = FindObjectOfType<CountdownUI>();
        if (countdown != null)
            countdown.StartCountdown();
        else
            StartRace(); // 没有倒计时就直接开跑

    }

}