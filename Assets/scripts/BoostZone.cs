using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(Collider))]
public class BoostZone : MonoBehaviour
{
    [Header("加速参数")]
    public float animSpeedMultiplier = 1.5f; // 动画整体加速倍率（影响 Root Motion）
    public float boostDuration = 1.2f;      // 持续时间（秒）
    public float additiveImpulse = 6f;      // 额外直线速度（m/s），进入时一次性注入

    [Header("音效（可选）")]
    public AudioClip boostSound;

    [Header("视频特效 UI")]
    public GameObject boostEffectUI;     // UI (RawImage父物体)
    public VideoPlayer boostVideoPlayer; // VideoPlayer组件

    private void Reset()
    {
        // 确保是触发器
        Collider c = GetComponent<Collider>();
        if (c) c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<movement>();
        if (player == null) return;

        //  请求 Boost（movement 内部会检查是否已经在加速中）
        bool applied = player.TryBoost(animSpeedMultiplier, boostDuration, additiveImpulse);

        if (applied)
        {
            if (boostSound)
                AudioSource.PlayClipAtPoint(boostSound, transform.position);

            // 播放视频效果
            if (boostEffectUI != null && boostVideoPlayer != null)
            {
                boostEffectUI.SetActive(true);
                boostVideoPlayer.Play();

                // 隐藏 UI 在视频播放结束时
                boostVideoPlayer.loopPointReached += OnVideoFinished;
            }
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (boostEffectUI != null)
            boostEffectUI.SetActive(false);

        vp.loopPointReached -= OnVideoFinished; // 移除事件，避免多次触发
    }
}
