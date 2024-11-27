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

    private IEnumerator LoadNextSceneAsync() //다음 씬을 비동기적으로 로딩
    {
        Debug.Log("Starting async load for: " + nextSceneName); //로딩 시작 로그
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName); //비동기 씬 전환 객체 생성
        asyncLoad.allowSceneActivation = false; //씬 자동 전환 차단

        while (!asyncLoad.isDone) //씬 로딩이 완료될 때까지 반복
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); //씬 로딩 진행도 0과 1사이로 클램프하여 계산
            if (loadingBar != null)
            {
                loadingBar.value = progress;
            }

            if (asyncLoad.progress >= 0.9f) //로딩 진행률이 90% 이상일 경우
            {
                loadingBar.value = 1f;  //로딩바 수치를 100%로 설정
                yield return new WaitForSeconds(2f); //2초 대기
                asyncLoad.allowSceneActivation = true; //씬 활성화 허용
            }
            yield return null; //Couroutine 함수 종료
        }
        GameManager.Instance.PlayBackgroundMusic(); //씬 로딩이 완료된 후에 배경음악 재생
    }
}