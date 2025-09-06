using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // 进度条组件
    public Slider progressBar;
    // 可选：显示进度百分比的文本
    public Text progressText;
    // 要切换到的场景名称
    public string targetSceneName;

    private AsyncOperation asyncOperation;

    void Start()
    {
        // 开始异步加载场景
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // 异步加载目标场景
        asyncOperation = SceneManager.LoadSceneAsync(targetSceneName);
        // 阻止场景在加载完成后立即激活，以便我们手动控制
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // 获取加载进度（范围在0到1之间）
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            // 更新进度条
            progressBar.value = progress;
            // 更新进度文本（如果有）
            if (progressText != null)
            {
                progressText.text = "加载进度: " + (int)(progress * 100) + "%";
            }

            // 当加载进度达到90%以上时，允许场景激活（因为asyncOperation.progress在接近完成时会跳到0.9，然后isDone变为true）
            if (asyncOperation.progress >= 0.9f)
            {
                progressBar.value = 1f;
                if (progressText != null)
                {
                    progressText.text = "加载完成，即将进入场景";
                }
                // 这里可以添加一个延迟，让玩家看到加载完成的状态，然后再激活场景
                yield return new WaitForSeconds(1f);
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    // 也可以提供一个方法，用于外部触发场景加载（比如按钮点击）
    public void LoadTargetScene()
    {
        StartCoroutine(LoadSceneAsync());
    }
}