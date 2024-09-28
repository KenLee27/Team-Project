using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public Slider loadingBar;
    public Text loadingTip;

    public static string[] tips;
    public static string nextSceneName;

    void Start()
    {
        if (loadingTip != null && tips != null && tips.Length > 0)
        {
            loadingTip.text = tips[Random.Range(0, tips.Length)];
        }
        else
        {
            Debug.LogWarning("No loading tips found.");
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(LoadNextSceneAsync());
        }
        else
        {
            Debug.LogError("Next scene name is not specified.");
        }
    }

    private IEnumerator LoadNextSceneAsync()
    {
        Debug.Log("Starting async load for: " + nextSceneName);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            if (loadingBar != null)
            {
                loadingBar.value = progress;
            }

            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2f);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}