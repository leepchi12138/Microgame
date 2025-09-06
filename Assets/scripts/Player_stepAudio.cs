using UnityEngine;

public class Player_stepAudio : lasting2021
{
    // ��Ƶ���
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepClips;
    public AudioSource jumpAudioSource;
    public AudioSource landAudioSource;

    protected int lastPlayedIndex = -1;
    // �Ų����������
    protected float footstepTimer = 0f;
    [SerializeField] protected float walkInterval = 0.8f;    // ��·���
    [SerializeField] protected float runInterval = 0.4f;     // �ܲ����
    protected float currentInterval;

    public override void Start()
    {
        // ?? ��һ���պϲ������������������Ϊ null ʱ�������Ҳ�����������򷵻�����������
        //?.��δ�ҵ��򷵻�null,��ֹ������
        if (!footstepAudioSource) footstepAudioSource = GetComponent<AudioSource>() ?? GetComponentInParent<AudioSource>();
        if (!jumpAudioSource) jumpAudioSource = transform.Find("JumpAudio").GetComponent<AudioSource>();
        if (!landAudioSource) landAudioSource = transform.Find("LandAudio").GetComponent<AudioSource>();
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log(jumpAudioSource);
        // �����ƶ������ٶ�ʱ����϶���״̬���ŽŲ�
        if (isGrounded && currentSpeed > 0.5f)
        {
            footstepTimer += Time.deltaTime;

            // �����ٶ�ѡ��Ų������
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
            //if (jumpAudioSource) jumpAudioSource.Play();������Ϊnull������Ϊfalse������Ϊtrue��
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

    public void PlayFootstepSound()//�ؼ�֡����
    {
        PlayRandomAudio();
    }
}