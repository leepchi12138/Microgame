using UnityEngine;
using System.Collections.Generic;

public class LimitZoneManager : MonoBehaviour
{
    public Transform player;
    public LayerMask AirWall;
    public float checkDistance = 0.5f;

    private Animator _anim;

    // 角色体积参数
    public float capsuleRadius = 0.3f;
    public float capsuleHeight = 1.8f;

    // 渐隐渐显控制
    public float fadeSpeed = 3f;   // 透明度变化速度
    private Dictionary<Renderer, float> wallAlpha = new Dictionary<Renderer, float>();

    void Start()
    {
        if (player != null)
            _anim = player.GetComponent<Animator>();

        // 找到场景中所有空气墙并初始化为透明
        Collider[] walls = FindObjectsOfType<Collider>();
        foreach (var wall in walls)
        {
            if (((1 << wall.gameObject.layer) & AirWall) != 0)
            {
                Renderer r = wall.GetComponent<Renderer>();
                if (r != null)
                {
                    wallAlpha[r] = 0f;
                    SetAlpha(r, 0f);
                    r.material.renderQueue = 3000; // 强制进入透明队列
                }
            }
        }
    }

    void Update()
    {
        if (player == null || _anim == null) return;

        Vector3 move = _anim.deltaPosition;
        if (move.magnitude < 0.001f) return;

        Vector3 newPos = player.position + move;

        // 胶囊体两个端点
        Vector3 point1 = player.position + Vector3.up * 0.1f;
        Vector3 point2 = player.position + Vector3.up * (capsuleHeight - 0.1f);

        RaycastHit hit;
        if (Physics.CapsuleCast(point1, point2, capsuleRadius, move.normalized, out hit, checkDistance, AirWall))
        {
            // 碰到空气墙 → 渐显
            Renderer r = hit.collider.GetComponent<Renderer>();
            if (r != null)
            {
                if (!wallAlpha.ContainsKey(r)) wallAlpha[r] = 0f;
                wallAlpha[r] = Mathf.MoveTowards(wallAlpha[r], 1f, fadeSpeed * Time.deltaTime);
                SetAlpha(r, wallAlpha[r]);
            }

            // 沿着墙滑动
            Vector3 slideDir = Vector3.ProjectOnPlane(move, hit.normal);
            newPos = player.position + slideDir;
            newPos += hit.normal * 0.05f;
        }

        player.position = newPos;

        // 没有碰到的墙 → 渐隐
        List<Renderer> keys = new List<Renderer>(wallAlpha.Keys);
        foreach (var r in keys)
        {
            if (r == null) continue;
            if (r != hit.collider?.GetComponent<Renderer>())
            {
                wallAlpha[r] = Mathf.MoveTowards(wallAlpha[r], 0f, fadeSpeed * Time.deltaTime);
                SetAlpha(r, wallAlpha[r]);
            }
        }
    }

    void SetAlpha(Renderer r, float a)
    {
        if (r == null) return;
        foreach (var mat in r.materials)
        {
            Color c = mat.color;
            c.a = a;
            mat.color = c;
            mat.renderQueue = 3000; // 确保透明渲染
        }
    }
}
