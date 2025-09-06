using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(Collider))]
public class BoostZone : MonoBehaviour
{
    [Header("���ٲ���")]
    public float animSpeedMultiplier = 1.5f; // ����������ٱ��ʣ�Ӱ�� Root Motion��
    public float boostDuration = 1.2f;      // ����ʱ�䣨�룩
    public float additiveImpulse = 6f;      // ����ֱ���ٶȣ�m/s��������ʱһ����ע��

    [Header("��Ч����ѡ��")]
    public AudioClip boostSound;

    [Header("��Ƶ��Ч UI")]
    public GameObject boostEffectUI;     // UI (RawImage������)
    public VideoPlayer boostVideoPlayer; // VideoPlayer���

    private void Reset()
    {
        // ȷ���Ǵ�����
        Collider c = GetComponent<Collider>();
        if (c) c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<movement>();
        if (player == null) return;

        //  ���� Boost��movement �ڲ������Ƿ��Ѿ��ڼ����У�
        bool applied = player.TryBoost(animSpeedMultiplier, boostDuration, additiveImpulse);

        if (applied)
        {
            if (boostSound)
                AudioSource.PlayClipAtPoint(boostSound, transform.position);

            // ������ƵЧ��
            if (boostEffectUI != null && boostVideoPlayer != null)
            {
                boostEffectUI.SetActive(true);
                boostVideoPlayer.Play();

                // ���� UI ����Ƶ���Ž���ʱ
                boostVideoPlayer.loopPointReached += OnVideoFinished;
            }
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (boostEffectUI != null)
            boostEffectUI.SetActive(false);

        vp.loopPointReached -= OnVideoFinished; // �Ƴ��¼��������δ���
    }
}
