using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 音频组件
    private AudioSource audioSource;
    public AudioClip[] jumpClips;     // 跳叫声频
    private int lastAudioIndex = -1;  // 记录上一个声音索引
    public bool isJumping = false;
    private bool hasPlayed = false;   // 标记是否已播放过音效

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // 当跳跃状态激活且尚未播放音效时才播放
        if (isJumping && !hasPlayed)
        {
            PlaySound();
            hasPlayed = true;  // 标记为已播放
        }
        // 当跳跃状态结束时重置标记
        else if (!isJumping && hasPlayed)
        {
            hasPlayed = false;
        }
    }

    // 音频播放方法
    public void PlaySound()
    {
        // 检查数组是否有效
        if (jumpClips == null || jumpClips.Length == 0)
        {
            Debug.LogWarning("JumpClips数组未赋值或为空");
            return;
        }

        // 只有一个音频时直接播放
        if (jumpClips.Length == 1)
        {
            audioSource.PlayOneShot(jumpClips[0]);
            return;
        }

        // 随机选择播放（避免重复）
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, jumpClips.Length);
        } while (randomIndex == lastAudioIndex);

        lastAudioIndex = randomIndex;
        audioSource.PlayOneShot(jumpClips[randomIndex]);
    }
}