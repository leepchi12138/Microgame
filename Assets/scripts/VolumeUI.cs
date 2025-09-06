using UnityEngine;
using UnityEngine.UI;

public class VolumeUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // 初始化滑块位置
        masterSlider.value = MusicManager.Instance.masterVolume;
        musicSlider.value = MusicManager.Instance.musicVolume;
        sfxSlider.value = MusicManager.Instance.sfxVolume;

        // 绑定事件
        masterSlider.onValueChanged.AddListener((v) => MusicManager.Instance.SetMasterVolume(v));
        musicSlider.onValueChanged.AddListener((v) => MusicManager.Instance.SetMusicVolume(v));
        sfxSlider.onValueChanged.AddListener((v) => MusicManager.Instance.SetSFXVolume(v));
    }
}
