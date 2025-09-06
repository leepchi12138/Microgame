using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [Header("��Ҷ���ֱ���Ͻ�����")]
    public GameObject player;   // ����ҽ���

    private List<List<GameObject>> groundGroups = new List<List<GameObject>>();
    private int currentGroupIndex = 0;

    void Start()
    {
        // ��ʼ����ÿ3��������Ϊһ��
        int count = 0;
        List<GameObject> group = new List<GameObject>();

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false); // ��ʼ����
            group.Add(child.gameObject);
            count++;

            if (count == 3)
            {
                groundGroups.Add(group);
                group = new List<GameObject>();
                count = 0;
            }
        }

        // �����һ���е�һ��
        ActivateRandomFromGroup(0);
    }

    private void ActivateRandomFromGroup(int index)
    {
        if (index >= groundGroups.Count) return;

        List<GameObject> group = groundGroups[index];
        int randomIndex = Random.Range(0, group.Count);

        GameObject chosenGround = group[randomIndex];
        chosenGround.SetActive(true);

        // ȷ���д������
        GroundTrigger trigger = chosenGround.GetComponent<GroundTrigger>();
        if (trigger == null) trigger = chosenGround.AddComponent<GroundTrigger>();
        trigger.manager = this;
        trigger.player = player;   // ������Ҷ���
    }

    // �� GroundTrigger ���ã���ʾ��һ��
    public void OnGroundStepped()
    {
        currentGroupIndex++;
        ActivateRandomFromGroup(currentGroupIndex);
    }

    // �ڲ�����������
    private class GroundTrigger : MonoBehaviour
    {
        [HideInInspector] public GroundManager manager;
        [HideInInspector] public GameObject player;
        private bool triggered = false;

        private void OnCollisionEnter(Collision other)
        {
            if (triggered) return;

            if (other.gameObject == player)   // ֱ�ӱȶ�GameObject
            {
                triggered = true;
                manager.OnGroundStepped();
            }
        }
    }
}
