using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public enum PlayMode { ListLoop, Shuffle }
    [Header("播放模式")]
    public PlayMode playMode = PlayMode.ListLoop;

    [System.Serializable]
    public class SongData
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [System.Serializable]
    public class MusicGroup
    {
        public string groupName;
        public List<SongData> songs = new List<SongData>();
    }

    [Header("音乐组设置")]
    public List<MusicGroup> musicGroups = new List<MusicGroup>();

    [Header("音量设置 (全局)")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource musicSource;
    private string currentGroupName;
    private int currentSongIndex = 0;
    private List<int> shuffleQueue = new List<int>(); // 随机播放队列

    [Header("场景音乐映射")]
    public List<string> sceneNames;
    public List<string> groupNames;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = false; // 改为单曲不循环，由我们自己控制
        musicSource.playOnAwake = false;
    }

    void Update()
    {
        UpdateVolume();

        // 如果一首歌播放结束 -> 播放下一首
        if (!musicSource.isPlaying && musicSource.clip != null)
        {
            PlayNextSong();
        }
    }

    private void UpdateVolume()
    {
        if (musicSource.clip == null) return;

        MusicGroup group = musicGroups.Find(g => g.groupName == currentGroupName);
        if (group == null || currentSongIndex >= group.songs.Count) return;

        float songVolume = group.songs[currentSongIndex].volume;
        musicSource.volume = masterVolume * musicVolume * songVolume;
    }

    public void PlayGroup(string groupName, int songIndex = 0)
    {
        MusicGroup group = musicGroups.Find(g => g.groupName == groupName);
        if (group != null && group.songs.Count > 0)
        {
            currentGroupName = groupName;

            if (playMode == PlayMode.Shuffle)
            {
                ResetShuffleQueue(group.songs.Count);
                currentSongIndex = GetNextShuffleIndex();
            }
            else
            {
                currentSongIndex = Mathf.Clamp(songIndex, 0, group.songs.Count - 1);
            }

            musicSource.clip = group.songs[currentSongIndex].clip;
            musicSource.Play();
            UpdateVolume();
        }
        else
        {
            Debug.LogWarning($"找不到音乐组: {groupName}");
        }
    }

    public void PlayNextSong()
    {
        MusicGroup group = musicGroups.Find(g => g.groupName == currentGroupName);
        if (group != null && group.songs.Count > 0)
        {
            if (playMode == PlayMode.Shuffle)
            {
                currentSongIndex = GetNextShuffleIndex();
            }
            else // 列表循环
            {
                currentSongIndex = (currentSongIndex + 1) % group.songs.Count;
            }

            musicSource.clip = group.songs[currentSongIndex].clip;
            musicSource.Play();
            UpdateVolume();
        }
    }

    private void ResetShuffleQueue(int count)
    {
        shuffleQueue.Clear();
        List<int> indices = new List<int>();
        for (int i = 0; i < count; i++) indices.Add(i);

        // 打乱顺序
        while (indices.Count > 0)
        {
            int randomIndex = Random.Range(0, indices.Count);
            shuffleQueue.Add(indices[randomIndex]);
            indices.RemoveAt(randomIndex);
        }
    }

    private int GetNextShuffleIndex()
    {
        if (shuffleQueue.Count == 0)
        {
            MusicGroup group = musicGroups.Find(g => g.groupName == currentGroupName);
            if (group != null) ResetShuffleQueue(group.songs.Count);
        }

        int nextIndex = shuffleQueue[0];
        shuffleQueue.RemoveAt(0);
        return nextIndex;
    }

    public void SetSongVolume(string groupName, int songIndex, float value)
    {
        MusicGroup group = musicGroups.Find(g => g.groupName == groupName);
        if (group != null && songIndex >= 0 && songIndex < group.songs.Count)
        {
            group.songs[songIndex].volume = Mathf.Clamp01(value);
            if (groupName == currentGroupName && songIndex == currentSongIndex)
            {
                UpdateVolume();
            }
        }
    }

    public void SetMasterVolume(float value) => masterVolume = Mathf.Clamp01(value);
    public void SetMusicVolume(float value) => musicVolume = Mathf.Clamp01(value);
    public void SetSFXVolume(float value) => sfxVolume = Mathf.Clamp01(value);

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        for (int i = 0; i < sceneNames.Count; i++)
        {
            if (scene.name == sceneNames[i])
            {
                string targetGroup = groupNames[i];

                if (currentGroupName == targetGroup && musicSource.isPlaying)
                {
                    Debug.Log($"保持当前音乐组: {targetGroup}");
                    return;
                }

                PlayGroup(targetGroup, 0);
                break;
            }
        }
    }
}
