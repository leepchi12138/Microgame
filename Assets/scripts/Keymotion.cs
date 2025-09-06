using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keymotion : Player_stepAudio
{

    // Update is called once per frame
    public override void Update()
    {
        //base.Update();
        //if (isGrounded && currentSpeed > 0.5f)
        //{
        //    footstepTimer += Time.deltaTime;

        //    // 根据速度选择脚步声间隔
        //    currentInterval = currentSpeed > 3f ? runInterval : walkInterval;

        //    if (footstepTimer >= currentInterval)
        //    {
        //        // PlayRandomAudio();
        //        footstepTimer = 0f;
        //    }}

        
    }
    //public void PlayRandomAudio()
    //{
    //    if (footstepAudioSource == null || footstepClips == null || footstepClips.Length == 0) return;

    //    int randomIndex;
    //    do
    //    {
    //        randomIndex = Random.Range(0, footstepClips.Length);
    //    } while (footstepClips.Length > 1 && randomIndex == lastPlayedIndex);

    //    footstepAudioSource.PlayOneShot(footstepClips[randomIndex]);
    //    lastPlayedIndex = randomIndex;
    //}
    //public void PlayFootstepSound()//关键帧调用
    //{
    //    PlayRandomAudio();
    //}
}
