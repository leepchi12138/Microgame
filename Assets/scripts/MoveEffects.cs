using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ��Ч������
/// ���ܣ�
/// 1. ���ݲ�ͬ���ܶ����л�����Ч�ٶ�
/// 2. ��ɫ�����ܻ�ʱ���š�������Ч��
/// 3. �벻ͬ������ײʱ���Ų�ͬ�ġ���ײ��Ч��
/// 4. ���Ե�����ײ
/// </summary>
public class MoveEffects : MonoBehaviour
{
    [Header("����Ч����")]
    public GameObject windEffect;  // ����Ч������/������
    [Tooltip("����״̬�� �� ��Ӧ����")]
    public List<RunEffectMapping> runEffectMappings = new List<RunEffectMapping>();

    [Header("��ɫ�����ܻ���Ч")]
    [Tooltip("��ɫ���ϲ��ŵ���Ч����ѣ�Σ�")]
    public GameObject selfHitEffect;
    public float selfHitDuration = 1.5f;

    [Header("��ײ��Ч����")]
    [Tooltip("��ͬLayer��Ӧ��ͬ��Ч")]
    public List<LayerEffectMapping> collisionEffects = new List<LayerEffectMapping>();
    public float collisionEffectDuration = 0.5f;

    [Header("���Ե���ײ�㣨����棩")]
    public LayerMask groundLayer;

    // �ڲ�����
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
    /// ���ݶ���״̬�л�����Ч�ٶ�
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
    /// ��ײ���
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        int collisionLayer = collision.gameObject.layer;

        // ���Ե�����ײ
        if ((groundLayer.value & (1 << collisionLayer)) != 0)
            return;

        // 1. ���Ž�ɫ�����ܻ���Ч
        if (selfHitEffect != null)
        {
            if (selfHitCoroutine != null) StopCoroutine(selfHitCoroutine);
            selfHitCoroutine = StartCoroutine(PlaySelfHitEffect());
        }

        // 2. ���Ŷ�ӦLayer����ײ��Ч
        GameObject effect = GetEffectByLayer(collisionLayer);
        if (effect != null)
        {
            effect.transform.position = collision.contacts[0].point; // ������ײ��
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
/// ����״̬�ͷ���Ч�ٶ�ӳ��
/// </summary>
[System.Serializable]
public class RunEffectMapping
{
    public string animStateName;
    public float windSpeed = 1f;
}

/// <summary>
/// Layer����Ч��ӳ���ϵ
/// </summary>
[System.Serializable]
public class LayerEffectMapping
{
    public LayerMask targetLayer;
    public GameObject effectPrefab;
}
