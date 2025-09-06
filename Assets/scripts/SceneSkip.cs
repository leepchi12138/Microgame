using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSkip : MonoBehaviour
{
    [Header("UI 组件")]
    public Button gameBtn;          // 主按钮，点击显示地图选择
    public Button buttonOlympic;    // Olympic Stadium 按钮
    public Button buttonSkyCity;    // SkyOfCity 按钮
    public GameObject loadingBar;   // 加载条或其他提示
    public GameObject uiToHide;     // 其他需要隐藏的UI

    void Start()
    {
        // 初始状态
        if (buttonOlympic != null) buttonOlympic.gameObject.SetActive(false);
        if (buttonSkyCity != null) buttonSkyCity.gameObject.SetActive(false);
        if (loadingBar != null) loadingBar.SetActive(false);

        // 主按钮绑定事件
        if (gameBtn != null)
            gameBtn.onClick.AddListener(OnGameBtnClicked);

        // 地图按钮绑定事件
        if (buttonOlympic != null)
            buttonOlympic.onClick.AddListener(() => OnMapButtonClicked("Olympic Stadium"));

        if (buttonSkyCity != null)
            buttonSkyCity.onClick.AddListener(() => OnMapButtonClicked("SkyOfCity"));
    }

    // 点击主按钮，显示地图选择
    private void OnGameBtnClicked()
    {
        if (buttonOlympic != null) buttonOlympic.gameObject.SetActive(true);
        if (buttonSkyCity != null) buttonSkyCity.gameObject.SetActive(true);

        if (uiToHide != null) uiToHide.SetActive(false); // 隐藏其他UI
    }

    // 点击地图按钮
    private void OnMapButtonClicked(string sceneName)
    {
        // 隐藏两个地图按钮
        if (buttonOlympic != null) buttonOlympic.gameObject.SetActive(false);
        if (buttonSkyCity != null) buttonSkyCity.gameObject.SetActive(false);

        // 显示加载条
        if (loadingBar != null) loadingBar.SetActive(true);

        // 开始加载场景
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            // 可以在这里更新加载条进度
            // float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            yield return null;
        }
    }
}
