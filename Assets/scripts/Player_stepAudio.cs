using UnityEngine;

public class Player_stepAudio : lasting2021
{
    // 音频组件
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepClips;
    public AudioSource jumpAudioSource;
    public AudioSource landAudioSource;

    protected int lastPlayedIndex = -1;
    // 脚步声间隔控制
    protected float footstepTimer = 0f;
    [SerializeField] protected float walkInterval = 0.8f;    // 走路间隔
    [SerializeField] protected float runInterval = 0.4f;     // 跑步间隔
    protected float currentInterval;

    public override void Start()
    {
        // ?? 是一个空合并运算符，当左侧操作数为 null 时，返回右侧操作数；否则返回左侧操作数。
        //?.若未找到则返回null,防止空引用
        if (!footstepAudioSource) footstepAudioSource = GetComponent<AudioSource>() ?? GetComponentInParent<AudioSource>();
        if (!jumpAudioSource) jumpAudioSource = transform.Find("JumpAudio").GetComponent<AudioSource>();
        if (!landAudioSource) landAudioSource = transform.Find("LandAudio").GetComponent<AudioSource>();
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log(jumpAudioSource);
        // 地面移动且有速度时，结合动画状态播放脚步
        if (isGrounded && currentSpeed > 0.5f)
        {
            footstepTimer += Time.deltaTime;

            // 根据速度选择脚步声间隔
            currentInterval = currentSpeed > 3f ? runInterval : walkInterval;

            if (footstepTimer >= currentInterval)
            {
                PlayRandomAudio();
                footstepTimer = 0f;
            }
            if (!wasGrounded && isGrounded)
            {
                OnLand();
            }

        }
    }

    public override void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded)
        {
            //if (jumpAudioSource) jumpAudioSource.Play();若变量为null则条件为false，否则为true。
            jumpAudioSource.Play();
        }
    }

    public override void OnLand()
    {
        landAudioSource.Play();
    }

    public void PlayRandomAudio()
    {
        if (footstepAudioSource == null || footstepClips == null || footstepClips.Length == 0) return;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, footstepClips.Length);
        } while (footstepClips.Length > 1 && randomIndex == lastPlayedIndex);

        footstepAudioSource.PlayOneShot(footstepClips[randomIndex]);
        lastPlayedIndex = randomIndex;
    }

    public void PlayFootstepSound()//关键帧调用
    {
        PlayRandomAudio();
    }
}