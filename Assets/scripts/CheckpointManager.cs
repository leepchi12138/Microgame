using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public Checkpoint[] checkpoints;  // 自动获取所有子物体的 Checkpoint

    void Awake()
    {
        // 获取所有 Checkpoint（必须是这个 Manager 的子物体）
        checkpoints = GetComponentsInChildren<Checkpoint>();

        for (int i = 0; i < checkpoints.Length; i++)
        {
            int nextIndex = (i + 1) % checkpoints.Length; // 最后一个连回第一个
            checkpoints[i].nextCheckpoint = checkpoints[nextIndex].transform;
        }
    }
}
