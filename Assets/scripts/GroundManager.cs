using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [Header("玩家对象（直接拖进来）")]
    public GameObject player;   // 拖玩家进来

    private List<List<GameObject>> groundGroups = new List<List<GameObject>>();
    private int currentGroupIndex = 0;

    void Start()
    {
        // 初始化：每3个子物体为一组
        int count = 0;
        List<GameObject> group = new List<GameObject>();

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false); // 初始隐藏
            group.Add(child.gameObject);
            count++;

            if (count == 3)
            {
                groundGroups.Add(group);
                group = new List<GameObject>();
                count = 0;
            }
        }

        // 激活第一组中的一个
        ActivateRandomFromGroup(0);
    }

    private void ActivateRandomFromGroup(int index)
    {
        if (index >= groundGroups.Count) return;

        List<GameObject> group = groundGroups[index];
        int randomIndex = Random.Range(0, group.Count);

        GameObject chosenGround = group[randomIndex];
        chosenGround.SetActive(true);

        // 确保有触发组件
        GroundTrigger trigger = chosenGround.GetComponent<GroundTrigger>();
        if (trigger == null) trigger = chosenGround.AddComponent<GroundTrigger>();
        trigger.manager = this;
        trigger.player = player;   // 传递玩家对象
    }

    // 被 GroundTrigger 调用：显示下一组
    public void OnGroundStepped()
    {
        currentGroupIndex++;
        ActivateRandomFromGroup(currentGroupIndex);
    }

    // 内部触发检测组件
    private class GroundTrigger : MonoBehaviour
    {
        [HideInInspector] public GroundManager manager;
        [HideInInspector] public GameObject player;
        private bool triggered = false;

        private void OnCollisionEnter(Collision other)
        {
            if (triggered) return;

            if (other.gameObject == player)   // 直接比对GameObject
            {
                triggered = true;
                manager.OnGroundStepped();
            }
        }
    }
}
