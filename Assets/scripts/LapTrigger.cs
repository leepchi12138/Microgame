using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    // ∑¿÷π÷ÿ∏¥¥•∑¢
    private bool hasTriggered = false;
    public float resetDelay = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        ResetPlayer player = other.GetComponent<ResetPlayer>();
        if (player != null && !string.IsNullOrEmpty(player.playerName))
        {
            if (RaceManager.Instance != null)
            {
                RaceManager.Instance.FinishLap(player.playerName);
                hasTriggered = true;
                Invoke(nameof(ResetTrigger), resetDelay);
            }
        }
    }

    private void ResetTrigger()
    {
        hasTriggered = false;
    }
}
