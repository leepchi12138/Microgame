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
            Debug.Log($"��� {player.GetPlayerName()} �����յ㣬��ʱ: {lapTime:F2}s");
        }
    }
}
