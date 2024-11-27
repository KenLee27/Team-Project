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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void OnClickNewGame()
    {
        PlayClickSound();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        StartCoroutine(StartNewGame()); // 새로운 게임 시작 코루틴
    }

    public void OnClickLoad()
    {
        PlayClickSound();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(StartLoadGame()); // 게임 불러오기 코루틴
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





    private IEnumerator StartNewGame()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        StartCoroutine(LoadSceneAsync("Calios"));
    }

    private IEnumerator StartLoadGame()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        StartCoroutine(LoadSceneAsync("Calios"));
    }



    

    public IEnumerator LoadSceneAsync(string nextSceneName)
    {
        LoadingManager.tips = tips; // 로딩 씬 실행 전 부가적인 데이터 설정
        LoadingManager.nextSceneName = nextSceneName;

        SceneManager.LoadScene("LoadingScene");
        yield return null; // 로딩 씬 로드 후 다음 프레임 대기
    }

    private string[] tips = new string[]
    {
        "정확한 타이밍에 맞춰 구르면 공격에 맞지 않습니다",
        "때로는 한 발자국 물러나서 적의 움직임을 살펴보는게 좋을 수도 있습니다",
        "메이지는 진짜로 칼을 거꾸로 들고 있습니다",
        "정확한 공격 타이밍을 알면 게임이 쉬워집니다",
        "무작정 구르기만 하면 스테미너 관리가 안되서 무너지기 쉽상입니다"
    };

    private IEnumerator LoadScene(string sceneName) //작성자 박지민
    {
        yield return new WaitForSeconds(1f); // 1초 대기 (로딩 효과)

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single); //씬 로드(?)

        while (!asyncLoad.isDone)
        {
            Debug.Log("로딩 중...");
            yield return null;
        }

        Debug.Log("씬 로드 완료");
    }






    private void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound); // 즉시 사운드 재생
        }
    }
}