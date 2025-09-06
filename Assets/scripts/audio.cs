using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // ��Ƶ���
    private AudioSource audioSource;
    public AudioClip[] jumpClips;     // ������Ƶ
    private int lastAudioIndex = -1;  // ��¼��һ����������
    public bool isJumping = false;
    private bool hasPlayed = false;   // ����Ƿ��Ѳ��Ź���Ч

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
        // ����Ծ״̬��������δ������Чʱ�Ų���
        if (isJumping && !hasPlayed)
        {
            PlaySound();
            hasPlayed = true;  // ���Ϊ�Ѳ���
        }
        // ����Ծ״̬����ʱ���ñ��
        else if (!isJumping && hasPlayed)
        {
            hasPlayed = false;
        }
    }

    // ��Ƶ���ŷ���
    public void PlaySound()
    {
        // ��������Ƿ���Ч
        if (jumpClips == null || jumpClips.Length == 0)
        {
            Debug.LogWarning("JumpClips����δ��ֵ��Ϊ��");
            return;
        }

        // ֻ��һ����Ƶʱֱ�Ӳ���
        if (jumpClips.Length == 1)
        {
            audioSource.PlayOneShot(jumpClips[0]);
            return;
        }

        // ���ѡ�񲥷ţ������ظ���
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, jumpClips.Length);
        } while (randomIndex == lastAudioIndex);

        lastAudioIndex = randomIndex;
        audioSource.PlayOneShot(jumpClips[randomIndex]);
    }
}