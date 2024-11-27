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

    public Image loadingImage;
    public Sprite[] loadingSprites;

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

        if (loadingImage != null && loadingSprites != null && loadingSprites.Length > 0)
        {
            loadingImage.sprite = loadingSprites[Random.Range(0, loadingSprites.Length)];
        }
        else
        {
            Debug.LogWarning("No loading images found.");
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

    private IEnumerator LoadNextSceneAsync() //���� ���� �񵿱������� �ε�
    {
        Debug.Log("Starting async load for: " + nextSceneName); //�ε� ���� �α�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName); //�񵿱� �� ��ȯ ��ü ����
        asyncLoad.allowSceneActivation = false; //�� �ڵ� ��ȯ ����

        while (!asyncLoad.isDone) //�� �ε��� �Ϸ�� ������ �ݺ�
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); //�� �ε� ���൵ 0�� 1���̷� Ŭ�����Ͽ� ���
            if (loadingBar != null)
            {
                loadingBar.value = progress;
            }

            if (asyncLoad.progress >= 0.9f) //�ε� ������� 90% �̻��� ���
            {
                loadingBar.value = 1f;  //�ε��� ��ġ�� 100%�� ����
                yield return new WaitForSeconds(2f); //2�� ���
                asyncLoad.allowSceneActivation = true; //�� Ȱ��ȭ ���
            }
            yield return null; //Couroutine �Լ� ����
        }
        GameManager.Instance.PlayBackgroundMusic(); //�� �ε��� �Ϸ�� �Ŀ� ������� ���
    }
}