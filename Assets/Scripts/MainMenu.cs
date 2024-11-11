using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuCanvas;  // 메인 메뉴 캔버스
    private bool isMenuActive = true;  // 메뉴 활성화 상태 초기화 (시작 시 활성화)
    public AudioClip buttonClickSound; 
    private AudioSource audioSource; 

    void Start()
    {
        Debug.Log("MainMenu Start 실행됨"); // Start 실행 확인
        mainMenuCanvas.SetActive(true); // 메인 메뉴 활성화
        Time.timeScale = 0f; // 게임 일시 정지
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void OnClickNewGame()
    {
        PlayClickSound(); // 클릭 사운드 재생
        Debug.Log("새 게임 시작");
        PlayerPrefs.DeleteAll(); // PlayerPrefs 초기화
        StartCoroutine(StartNewGame()); // 새로운 게임 시작 코루틴
    }

    public void OnClickLoad()
    {
        PlayClickSound(); // 클릭 사운드 재생
        Debug.Log("게임 불러오기");
        StartCoroutine(StartLoadGame()); // 게임 불러오기 코루틴
    }

    private IEnumerator StartNewGame()
    {
        yield return new WaitForSecondsRealtime(0.2f); // 사운드 재생 후 잠시 대기
        StartCoroutine(LoadScene("Old_Dock"));
    }

    private IEnumerator StartLoadGame()
    {
        yield return new WaitForSecondsRealtime(0.2f); // 사운드 재생 후 잠시 대기
        StartCoroutine(LoadScene("Old_Dock"));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        Debug.Log($"씬 '{sceneName}' 로드 시작");
        mainMenuCanvas.SetActive(false); // 메인 메뉴 비활성화
        Time.timeScale = 1f; // 게임 재개
        yield return new WaitForSeconds(1f); // 1초 대기 (로딩 효과)

        // 씬 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // 로드가 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            Debug.Log("로딩 중..."); // 로딩 중 로그
            yield return null; // 다음 프레임까지 대기
        }

        Debug.Log("씬 로드 완료"); // 로드 완료 로그
    }

    public void OnClickQuit()
    {
        PlayClickSound(); // 클릭 사운드 재생
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 버튼 클릭 사운드 재생 메서드
    private void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound); // 즉시 사운드 재생
        }
    }

    public void ToggleMenu()
    {
        isMenuActive = !isMenuActive;
        mainMenuCanvas.SetActive(isMenuActive);
        Time.timeScale = isMenuActive ? 0f : 1f;
        Cursor.visible = isMenuActive;
        Cursor.lockState = isMenuActive ? CursorLockMode.None : CursorLockMode.Locked;
    }

}