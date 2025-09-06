using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public Checkpoint[] checkpoints;  // �Զ���ȡ����������� Checkpoint

    void Awake()
    {
        // ��ȡ���� Checkpoint����������� Manager �������壩
        checkpoints = GetComponentsInChildren<Checkpoint>();

        for (int i = 0; i < checkpoints.Length; i++)
        {
            int nextIndex = (i + 1) % checkpoints.Length; // ���һ�����ص�һ��
            checkpoints[i].nextCheckpoint = checkpoints[nextIndex].transform;
        }
    }
}
