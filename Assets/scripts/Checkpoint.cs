using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector] public Transform nextCheckpoint;

    private void OnTriggerEnter(Collider other)
    {
        ResetPlayer player = other.GetComponent<ResetPlayer>();
        if (player != null && nextCheckpoint != null)
        {
            player.SetCheckpoint(transform, nextCheckpoint);
        }
    }
}
