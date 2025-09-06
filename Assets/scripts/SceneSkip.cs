using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSkip : MonoBehaviour
{
    [Header("UI ���")]
    public Button gameBtn;          // ����ť�������ʾ��ͼѡ��
    public Button buttonOlympic;    // Olympic Stadium ��ť
    public Button buttonSkyCity;    // SkyOfCity ��ť
    public GameObject loadingBar;   // ��������������ʾ
    public GameObject uiToHide;     // ������Ҫ���ص�UI

    void Start()
    {
        // ��ʼ״̬
        if (buttonOlympic != null) buttonOlympic.gameObject.SetActive(false);
        if (buttonSkyCity != null) buttonSkyCity.gameObject.SetActive(false);
        if (loadingBar != null) loadingBar.SetActive(false);

        // ����ť���¼�
        if (gameBtn != null)
            gameBtn.onClick.AddListener(OnGameBtnClicked);

        // ��ͼ��ť���¼�
        if (buttonOlympic != null)
            buttonOlympic.onClick.AddListener(() => OnMapButtonClicked("Olympic Stadium"));

        if (buttonSkyCity != null)
            buttonSkyCity.onClick.AddListener(() => OnMapButtonClicked("SkyOfCity"));
    }

    // �������ť����ʾ��ͼѡ��
    private void OnGameBtnClicked()
    {
        if (buttonOlympic != null) buttonOlympic.gameObject.SetActive(true);
        if (buttonSkyCity != null) buttonSkyCity.gameObject.SetActive(true);

        if (uiToHide != null) uiToHide.SetActive(false); // ��������UI
    }

    // �����ͼ��ť
    private void OnMapButtonClicked(string sceneName)
    {
        // ����������ͼ��ť
        if (buttonOlympic != null) buttonOlympic.gameObject.SetActive(false);
        if (buttonSkyCity != null) buttonSkyCity.gameObject.SetActive(false);

        // ��ʾ������
        if (loadingBar != null) loadingBar.SetActive(true);

        // ��ʼ���س���
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            // ������������¼���������
            // float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            yield return null;
        }
    }
}
