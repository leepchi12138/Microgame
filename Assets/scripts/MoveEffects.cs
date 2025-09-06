using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色特效控制器
/// 功能：
/// 1. 根据不同奔跑动画切换风特效速度
/// 2. 角色自身受击时播放“自身特效”
/// 3. 与不同物体碰撞时播放不同的“碰撞特效”
/// 4. 忽略地面碰撞
/// </summary>
public class MoveEffects : MonoBehaviour
{
    [Header("风特效设置")]
    public GameObject windEffect;  // 风特效（粒子/动画）
    [Tooltip("动画状态名 → 对应风速")]
    public List<RunEffectMapping> runEffectMappings = new List<RunEffectMapping>();

    [Header("角色自身受击特效")]
    [Tooltip("角色身上播放的特效（如眩晕）")]
    public GameObject selfHitEffect;
    public float selfHitDuration = 1.5f;

    [Header("碰撞特效设置")]
    [Tooltip("不同Layer对应不同特效")]
    public List<LayerEffectMapping> collisionEffects = new List<LayerEffectMapping>();
    public float collisionEffectDuration = 0.5f;

    [Header("忽略的碰撞层（如地面）")]
    public LayerMask groundLayer;

    // 内部引用
    private Animator playerAnimator;
    private Coroutine selfHitCoroutine;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();

        if (windEffect != null)
            windEffect.SetActive(false);

        if (selfHitEffect != null)
            selfHitEffect.SetActive(false);

        HideAllCollisionEffects();
    }

    private void Update()
    {
        UpdateRunWindEffect();
    }

    /// <summary>
    /// 根据动画状态切换风特效速度
    /// </summary>
    private void UpdateRunWindEffect()
    {
        if (playerAnimator == null || windEffect == null) return;

        bool matched = false;
        foreach (var mapping in runEffectMappings)
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(mapping.animStateName))
            {
                if (!windEffect.activeSelf) windEffect.SetActive(true);
                SetWindEffectSpeed(mapping.windSpeed);
                matched = true;
                break;
            }
        }

        if (!matched && windEffect.activeSelf)
        {
            windEffect.SetActive(false);
        }
    }

    private void SetWindEffectSpeed(float speed)
    {
        ParticleSystem ps = windEffect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.simulationSpeed = speed;
        }
        else
        {
            Animator anim = windEffect.GetComponent<Animator>();
            if (anim != null) anim.speed = speed;
        }
    }

    /// <summary>
    /// 碰撞检测
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        int collisionLayer = collision.gameObject.layer;

        // 忽略地面碰撞
        if ((groundLayer.value & (1 << collisionLayer)) != 0)
            return;

        // 1. 播放角色自身受击特效
        if (selfHitEffect != null)
        {
            if (selfHitCoroutine != null) StopCoroutine(selfHitCoroutine);
            selfHitCoroutine = StartCoroutine(PlaySelfHitEffect());
        }

        // 2. 播放对应Layer的碰撞特效
        GameObject effect = GetEffectByLayer(collisionLayer);
        if (effect != null)
        {
            effect.transform.position = collision.contacts[0].point; // 放在碰撞点
            effect.SetActive(true);
            StartCoroutine(HideEffectAfterDelay(effect, collisionEffectDuration));
        }
    }

    private IEnumerator PlaySelfHitEffect()
    {
        selfHitEffect.SetActive(true);
        yield return new WaitForSeconds(selfHitDuration);
        selfHitEffect.SetActive(false);
    }

    private GameObject GetEffectByLayer(int layer)
    {
        foreach (var mapping in collisionEffects)
        {
            if ((mapping.targetLayer & (1 << layer)) != 0)
            {
                return mapping.effectPrefab;
            }
        }
        return null;
    }

    private IEnumerator HideEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effect != null)
            effect.SetActive(false);
    }


    private void HideAllCollisionEffects()
    {
        foreach (var mapping in collisionEffects)
        {
            if (mapping.effectPrefab != null)
                mapping.effectPrefab.SetActive(false);
        }
    }
}

/// <summary>
/// 动画状态和风特效速度映射
/// </summary>
[System.Serializable]
public class RunEffectMapping
{
    public string animStateName;
    public float windSpeed = 1f;
}

/// <summary>
/// Layer与特效的映射关系
/// </summary>
[System.Serializable]
public class LayerEffectMapping
{
    public LayerMask targetLayer;
    public GameObject effectPrefab;
}
