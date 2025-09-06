using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // ���������
    public Slider progressBar;
    // ��ѡ����ʾ���Ȱٷֱȵ��ı�
    public Text progressText;
    // Ҫ�л����ĳ�������
    public string targetSceneName;

    private AsyncOperation asyncOperation;

    void Start()
    {
        // ��ʼ�첽���س���
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // �첽����Ŀ�곡��
        asyncOperation = SceneManager.LoadSceneAsync(targetSceneName);
        // ��ֹ�����ڼ�����ɺ���������Ա������ֶ�����
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // ��ȡ���ؽ��ȣ���Χ��0��1֮�䣩
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            // ���½�����
            progressBar.value = progress;
            // ���½����ı�������У�
            if (progressText != null)
            {
                progressText.text = "���ؽ���: " + (int)(progress * 100) + "%";
            }

            // �����ؽ��ȴﵽ90%����ʱ�������������ΪasyncOperation.progress�ڽӽ����ʱ������0.9��Ȼ��isDone��Ϊtrue��
            if (asyncOperation.progress >= 0.9f)
            {
                progressBar.value = 1f;
                if (progressText != null)
                {
                    progressText.text = "������ɣ��������볡��";
                }
                // ����������һ���ӳ٣�����ҿ���������ɵ�״̬��Ȼ���ټ����
                yield return new WaitForSeconds(1f);
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    // Ҳ�����ṩһ�������������ⲿ�����������أ����簴ť�����
    public void LoadTargetScene()
    {
        StartCoroutine(LoadSceneAsync());
    }
}