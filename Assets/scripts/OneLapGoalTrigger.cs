using UnityEngine;

public class OneLapGoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ResetPlayer player = other.GetComponent<ResetPlayer>();
        if (player != null)
        {
            float lapTime = player.GetCurrentLapTime();
            player.FinishLap();
            Debug.Log($"玩家 {player.GetPlayerName()} 到达终点，用时: {lapTime:F2}s");
        }
    }
}
