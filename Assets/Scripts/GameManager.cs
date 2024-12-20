using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SuperPlayerController playerController;
    //public ImgsFillDynamic hpBar;
    public Slider hpBar;  // hpBar를 Slider로 변경
    public Slider stBar;
    public Slider mnBar;
    public Slider loadingBar;
    public Text soulText;
    public Text loadingTip; // 로딩 팁 텍스트

    public MainMenu mainMenu; // MainMenu 스크립트를 참조

    public GameObject hpSliderPrefab;     // HP 슬라이더 프리팹
    public GameObject stSliderPrefab;     // ST 슬라이더 프리팹
    public GameObject mnSliderPrefab;
    public GameObject crabHPSliderPrefab;     // HP 슬라이더 프리팹
    public GameObject aBossHPSliderPrefab;
    public GameObject bBossHPSliderPrefab;

    public GameObject gameOverTextPrefab;
    public GameObject mapNameTextPrefab;

    public Vector3 spawnPosition;          //플레이어 스폰 위치

    public AudioSource audioSource;  // AudioSource 컴포넌트
    public AudioClip escapeSound;    // ESC키를 눌렀을 때 재생할 사운드

    public void SavePosition(Vector3 position)      //플레이어 스폰 위치 저장
    {
        spawnPosition = position;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (escapeSound == null)
            {
                Debug.LogError("Escape sound clip is not assigned!");
            }

            if (mainMenu == null)
            {
                mainMenu = FindObjectOfType<MainMenu>();
                if (mainMenu == null)
                {
                    Debug.LogError("MainMenu is not found in the scene.");
                }
                else
                {
                    Debug.Log("MainMenu found and assigned.");
                }
            }
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate GameManager destroyed.");
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainMenu != null)
            {
                mainMenu.ToggleMenu(); // 메인 메뉴의 토글 기능을 호출
            }
            else
            {
                Debug.LogWarning("mainMenu가 null입니다.");
            }
            if (escapeSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(escapeSound); // escapeSound 클립을 한 번만 재생
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string displayName = GetDisplayNameForScene(scene.name);

        if (scene.name == "LoadingScene")
        {
            return;
        }

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        }

        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.FindUIElements();
            uiManager.SetInitialUIState();
            uiManager.AccessController();
            uiManager.InitializeButtonClickEvents();
        }

        FindAndSetSliders();
        InitializePlayerHP();
        InitializePlayerST();
        InitializePlayerMana();
        DisplayMapName(displayName);
        InitializePlayerController();
        InitializeCursor();
    }

    private void InitializeCursor()
    {
        // 마우스 cursor 설정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // 기본적으로는 비활성화
    }

    private void InitializePlayerController()
    {
        playerController = FindObjectOfType<SuperPlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene.");
        }

        if (soulText != null)
        {
            soulText.text = playerController.playerSoul.ToString();
        }
    }

    public void UpdatePlayerSOUL(float soulGain)
    //해당 메서드 호출시 몬스터의 소울이 플레이어의 소울로 전환
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController not assigned!");
            return;
        }

        playerController.playerSoul += soulGain;

        if (soulText != null)
        {
            soulText.text = playerController.playerSoul.ToString();
        }
    }


    private string GetDisplayNameForScene(string sceneName)
    {
        // 씬 이름과 표시될 텍스트를 매핑
        switch (sceneName)
        {
            case "Old_Dock":
                return "버려진 부둣가";
            default:
                return sceneName; // 기본적으로는 씬 이름을 사용
        }
    }

    private void FindAndSetSliders()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("Canvas not found!");
            return;
        }

        Transform playerUI = canvas.transform.Find("PlayerUI");
        if (playerUI == null)
        {
            Debug.LogWarning("PlayerUI not found!");
            return;
        }


        //hpBar = playerUI.Find("HPprogress")?.GetComponent<ImgsFillDynamic>();
        //if (hpBar == null) Debug.LogWarning("HPprogress not found!");

        hpBar = playerUI.Find("HPprogress")?.GetComponent<Slider>();  // Slider로 변경
        if (hpBar == null) Debug.LogWarning("HPprogress Slider not found!");

        stBar = playerUI.Find("STprogress")?.GetComponent<Slider>();
        if (stBar == null) Debug.LogWarning("STprogress not found!");

        mnBar = playerUI.Find("MNprogress")?.GetComponent<Slider>();
        if (mnBar == null) Debug.LogWarning("MNprogress not found!");

        soulText = playerUI.Find("SOULprogress")?.GetComponent<Text>();
        if (soulText == null) Debug.LogWarning("SOULprogress not found!");
    }

    /*private void DisplayMapName(string displayName)
    {
        GameObject mapNameInstance = Instantiate(mapNameTextPrefab, FindObjectOfType<Canvas>().transform);
        Text mapNameText = mapNameInstance.GetComponent<Text>();
        mapNameText.text = displayName;

        // 텍스트가 서서히 나타났다 사라지는 코루틴 시작
        StartCoroutine(FadeInAndOut(mapNameInstance.GetComponent<CanvasGroup>()));
    }*/

    private void DisplayMapName(string displayName)
    {
        StartCoroutine(ShowMapNameAfterDelay(displayName, 2f)); // 2초 지연 후 텍스트 표시
    }

    private IEnumerator ShowMapNameAfterDelay(string displayName, float delay)
    {
        yield return new WaitForSeconds(delay); // 지연 시간만큼 대기

        GameObject mapNameInstance = Instantiate(mapNameTextPrefab, FindObjectOfType<Canvas>().transform);
        Text mapNameText = mapNameInstance.GetComponent<Text>();
        mapNameText.text = displayName;

        // 텍스트가 서서히 나타났다 사라지는 코루틴 시작
        StartCoroutine(FadeInAndOut(mapNameInstance.GetComponent<CanvasGroup>()));
    }


    public void InitializeMonsters()
    {
        EnemyController2[] monsters = FindObjectsOfType<EnemyController2>(); // 모든 몬스터 탐색
        EnemyEliteController[] elites = FindObjectsOfType<EnemyEliteController>();
        EnemyControllerClabKing[] crabKing = FindObjectsOfType<EnemyControllerClabKing>();
        BowEnemyController[] bow_monsters = FindObjectsOfType<BowEnemyController>();
        BossAController[] aBoss = FindObjectsOfType<BossAController>();
        BossBController[] bBoss = FindObjectsOfType<BossBController>();

        foreach (var monster in monsters)
        {
            monster.InitializeHPBar(hpSliderPrefab); // 각 몬스터에 슬라이더 초기
        }
        foreach (var bow_monster in bow_monsters)
        {
            bow_monster.InitializeHPBar(hpSliderPrefab); // 각 몬스터에 슬라이더 초기
        }
        foreach (var elite in elites)
        {
            elite.InitializeHPBar(hpSliderPrefab); // 각 몬스터에 슬라이더 초기
        }
        foreach (var king in crabKing)
        {
            king.InitializeHPBar(crabHPSliderPrefab); // 각 보스에 슬라이더 초기화
        }
        foreach (var a in aBoss)
        {
            a.InitializeHPBar(aBossHPSliderPrefab);
        }
        foreach (var a in bBoss)
        {
            a.InitializeHPBar(bBossHPSliderPrefab);
        }
    }

    public void UpdatePlayerHP(float currentHP)
    {
        if (hpBar != null)
        {
            float hpRatio = currentHP / 100f;
           // hpBar.SetValue(hpRatio);
            hpBar.value = hpRatio;  // Slider value 업데이트

        }
        if (currentHP <= 0)
        {
            DisplayGameOver();
        }
    }

    private void InitializePlayerHP()
    {
        if (playerController != null && hpBar != null)
        {
            float initialHPRatio = playerController.PlayerHP / 100f;
            //hpBar.SetValue(initialHPRatio, true); // 체력을 직접 설정
            hpBar.value = initialHPRatio; // 초기 체력 비율을 Slider에 설정

        }
    }

    public void UpdatePlayerST(float currentST)
    {
        if (stBar != null)
        {
            float stRatio = currentST / 100f;
            stBar.value = stRatio;
        }
    }

    private void InitializePlayerST()
    {
        if (playerController != null && stBar != null)
        {
            float initialSTRatio = playerController.PlayerStamina / playerController.PlayerMaxStamina;
            stBar.value = initialSTRatio;
        }
    }
    public void UpdatePlayerMana(float currentMana)
    {
        {
            float mnRatio = currentMana / playerController.PlayerMaxMana;
            mnBar.value = mnRatio;
        }
    }

    private void InitializePlayerMana()
    {
        if (playerController != null && mnBar != null)
        {
            float initialMNRacio = playerController.PlayerMana / playerController.PlayerMaxMana;
            mnBar.value = initialMNRacio;
        }
    }

    public void DisplayGameOver()
    {
        GameObject gameOverTextInstance = Instantiate(gameOverTextPrefab, FindObjectOfType<Canvas>().transform);
        StartCoroutine(FadeInAndLoadScene(gameOverTextInstance.GetComponent<CanvasGroup>()));
    }

    private IEnumerator FadeInAndOut(CanvasGroup canvasGroup)
    {
        float fadeDuration = 1f;
        float displayDuration = 3f;
        float elapsedTime = 0f;

        // 서서히 나타남
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        // 일정 시간 보여줌
        yield return new WaitForSeconds(displayDuration);

        elapsedTime = 0f;

        // 서서히 사라짐
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            yield return null;
        }

        if (canvasGroup != null)
        {
            Destroy(canvasGroup.gameObject); // CanvasGroup 삭제
        }
    }

    private IEnumerator FadeInAndLoadScene(CanvasGroup canvasGroup)
    {
        float duration = 1f; // 서서히 나타나는 데 걸리는 시간
        float elapsedTime = 0f;

        // 서서히 Alpha 값을 0에서 1로 증가
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }

        // 5초 후 씬 로드
        yield return new WaitForSeconds(3f);
        StartCoroutine(LoadSceneAsync("Old_Dock"));
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
}
