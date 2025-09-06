using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 空气墙管理器：统一处理玩家与多个空气墙的阻挡逻辑
/// 功能：当玩家移动时检测是否会碰撞到空气墙，若碰撞则让玩家沿墙滑动而不是直接阻挡
/// </summary>
public class LimitZoneManagerversion1 : MonoBehaviour
{
    // 玩家的Transform组件，用于获取和设置玩家位置
    public Transform player;
    // 空气墙的层级掩码，用于射线检测时只检测指定层级的物体
    public LayerMask AirWall;
    // 射线检测的距离，用于判断玩家是否即将碰撞到空气墙
    public float checkDistance = 0.5f;
    // 玩家的动画组件，用于获取玩家的移动方向和距离
    private Animator _anim;

    /// <summary>
    /// 初始化函数：在脚本启动时调用
    /// 功能：获取玩家的动画组件
    /// </summary>
    void Start()
    {
        // 如果玩家引用存在，则获取其Animator组件
        if (player != null)
            _anim = player.GetComponent<Animator>();
    }

    /// <summary>
    /// 帧更新函数：每帧调用一次
    /// 功能：检测玩家移动并处理与空气墙的碰撞逻辑
    /// </summary>
    void Update()
    {
        // 如果玩家或动画组件不存在，则退出函数，避免空引用错误
        if (player == null || _anim == null)
            return;

        // 从动画组件获取玩家本帧的移动量（基于动画的位移）
        Vector3 move = _anim.deltaPosition;

        // 如果移动量极小（几乎不动），则无需处理碰撞，直接退出
        if (move.magnitude < 0.001f)
            return;

        // 计算玩家不考虑碰撞时的理论新位置
        Vector3 newPos = player.position + move;

        // 射线检测：从玩家当前位置向移动方向发射射线，检测是否会碰撞到空气墙
        // 参数：起点(玩家位置)、方向(移动方向归一化)、碰撞信息、检测距离、检测层级
        RaycastHit hit;
        if (Physics.Raycast(player.position, move.normalized, out hit, checkDistance, AirWall))
        {
            // 如果检测到空气墙，则计算沿墙滑动的方向
            // 原理：将移动向量投影到空气墙表面（垂直于墙的法线方向）
            Vector3 slideDir = Vector3.ProjectOnPlane(move, hit.normal);

            // 计算滑动后的新位置
            newPos = player.position + slideDir;

            // 稍微沿法线方向偏移一点，避免玩家卡在墙内
            newPos += hit.normal * 0.05f;
        }

        // 应用计算后的新位置到玩家
        player.position = newPos;
    }
}