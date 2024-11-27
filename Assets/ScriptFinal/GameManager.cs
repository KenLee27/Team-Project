using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SuperPlayerController playerController;
    public Slider hpBar;
    public Slider stBar;
    public Slider mnBar;
    public Slider loadingBar;
    public Text soulText;
    public Text loadingTip; // 로딩 팁 텍스트

    public GameObject hpSliderPrefab;     // HP 슬라이더 프리팹
    public GameObject stSliderPrefab;     // ST 슬라이더 프리팹
    public GameObject mnSliderPrefab;
    public GameObject crabHPSliderPrefab;     // HP 슬라이더 프리팹
    public GameObject aBossHPSliderPrefab;
    public GameObject bBossHPSliderPrefab;
    public GameObject cBossHPSliderPrefab;

    public GameObject gameOverTextPrefab;
    public GameObject mapNameTextPrefab;
    public GameObject demonSlainedTextPrefab;

    public Vector3 spawnPosition;          //플레이어 스폰 위치
    public string displayName;

    private AudioSource audioSource; // AudioSource 컴포넌트
    public AudioClip[] BackgroundMusic;
    public AudioClip kamonBossRoomMusic;
    public AudioClip mageBossRoomMusic;
    public AudioClip finalBossRoomMusic;
    public AudioClip clearSound;
    private int currentTrackIndex = 0; // 현재 재생 중인 트랙 인덱스
    public float backgourndMusicVolume = 0.6f;

    public void SavePosition(Vector3 position)      //플레이어 스폰 위치 저장
    {
        spawnPosition = position;
        SaveSpawnPosition(position);
    }

    void Awake()
    {
        spawnPosition = LoadPosition(); //플레이어 초기 위치 설정
        displayName = "어둠이 드리운 땅"; //플레이어 위치 이름 설정

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  //게임 오브젝트가 씬 로드 시 파괴되지 않도록 설정

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);  //이미 인스턴스가 존재하면 새로 생성된 객체를 파괴
        }

        InitializeMonsters(); //전체 몬스터 초기화
        InitializePlayerStat(); //전체 플레이어 스텟 초기화
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.0f;
        //0.12 Default
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (BackgroundMusic.Length == 0) return; // 음악이 없으면 종료

        int randomIndex = Random.Range(0, 5); // 랜덤 인덱스 선택
        audioSource.clip = BackgroundMusic[randomIndex]; // 랜덤으로 선택한 음악 설정
        audioSource.loop = false; // 루프 설정
        audioSource.Play(); // 음악 재생

        StartCoroutine(PlayNextMusic()); // 음악이 끝나면 다음 음악 재생
    }

    private System.Collections.IEnumerator PlayNextMusic()
    {
        while (audioSource.isPlaying)
        {
            yield return null; // 음악이 재생 중일 때 대기
        }
        PlayBackgroundMusic(); // 음악이 끝나면 다음 음악 재생
    }

    public void StopBackgroundMusic()
    {
        audioSource.loop = false;
        audioSource.Stop(); // 음악 멈추기
    }

    public void VolumeUp()
    {
        audioSource.volume = 0.0f;
        //0.25 Default
    }

    public void VolumeDown()
    {
        audioSource.volume = 0.0f;
        //0.12 Default
    }

    public void PlaySelectedBackgroundMusic(int index)
    {
        if (index < 0 || index >= BackgroundMusic.Length)
        {
            Debug.LogWarning("Index out of bounds for BackgroundMusic array."); // 인덱스 체크
            return;
        }

        StopBackgroundMusic(); // 현재 음악 멈추기
        audioSource.clip = BackgroundMusic[index]; // 선택한 음악 설정
        audioSource.loop = true; // 루프 설정
        audioSource.Play(); // 음악 재생
    }

    public void InitializePlayerStat()
    {
        if (!PlayerPrefs.HasKey("playerLevel")) //플레이어 레벨 PlayerPrefs
        {
            PlayerPrefs.SetFloat("playerLevel", 1);
        }

        if (!PlayerPrefs.HasKey("playerSoul")) //플레이어 경험치 PlayerPrefs
        {
            PlayerPrefs.SetFloat("playerSoul", 5500);
        }

        if (!PlayerPrefs.HasKey("playerLife")) //플레이어 생명력(HP) PlayerPrefs
        {
            PlayerPrefs.SetFloat("playerLife", 10);
        }

        if (!PlayerPrefs.HasKey("playerMental")) //플레이어 정신력(MN) PlayerPrefs
        {
            PlayerPrefs.SetFloat("playerMental", 10);
        }

        if (!PlayerPrefs.HasKey("playerEndurance"))
        {
            PlayerPrefs.SetFloat("playerEndurance", 10);
        }

        if (!PlayerPrefs.HasKey("playerForce"))
        {
            PlayerPrefs.SetFloat("playerForce", 10);
        }

        if (!PlayerPrefs.HasKey("toggleTeleporter"))
        {
            PlayerPrefs.SetInt("toggleTeleporter", 0);
        }

        if (!PlayerPrefs.HasKey("toggleFTeleporter"))
        {
            PlayerPrefs.SetInt("toggleFTeleporter", 0);
        }

        if (!PlayerPrefs.HasKey("maxHPposion"))
        {
            PlayerPrefs.SetFloat ("maxHPposion", 3);
        }

        if (!PlayerPrefs.HasKey("maxMPposion"))
        {
            PlayerPrefs.SetFloat("maxMPposion", 3);
        }

        PlayerPrefs.Save();
    }

    public void Update()
    {
        float currentPlayerHPLevel = PlayerPrefs.GetFloat("playerLife");
        float currentPlayerMNLevel = PlayerPrefs.GetFloat("playerMental");
        float currentPlayerSTLevel = PlayerPrefs.GetFloat("playerEndurance");
        float currentPlayerSTRLevel = PlayerPrefs.GetFloat("playerForce");

        if (currentPlayerHPLevel >= 40) { UIManager.Instance.canHPLevelUp = false; }
        if (currentPlayerMNLevel >= 40) { UIManager.Instance.canMNLevelUP = false; }
        if (currentPlayerSTLevel >= 40) { UIManager.Instance.canSTLevelUp = false; }
        if (currentPlayerSTRLevel >= 40) { UIManager.Instance.canSTRLevelUp = false; }
    }

    public void SaveSpawnPosition(Vector3 position)
    {
        PlayerPrefs.SetFloat("PlayerPosX", position.x);
        PlayerPrefs.SetFloat("PlayerPosY", position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", position.z);
        PlayerPrefs.Save();  // PlayerPrefs 값을 영구 저장
    }

    // 플레이어 위치 불러오기 메서드
    public Vector3 LoadPosition()
    {
        float x = PlayerPrefs.GetFloat("PlayerPosX", 169.007f);  // 저장된 값이 없을 때는 첫 시작 위치
        float y = PlayerPrefs.GetFloat("PlayerPosY", 83.23f);
        float z = PlayerPrefs.GetFloat("PlayerPosZ", 463.072f);
        return new Vector3(x, y, z);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LoadingScene" || scene.name == "MainScene")
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
            uiManager.UpdateStatusText();
        }

        FindAndSetSliders();
        InitializePlayerHP();
        InitializePlayerST();
        InitializePlayerMana();
        DisplayMapName(displayName);
        InitializePlayerController();
        InitializeCursor();
    }

    public void changeDisplayName(string changeName) 
    {
        displayName = changeName;
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

        PlayerPrefs.SetFloat("playerSoul", playerController.playerSoul);
        float currentSoul = PlayerPrefs.GetFloat("playerSoul");

        if (soulText != null)
        {
            soulText.text = currentSoul.ToString();
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
        PlayerPrefs.SetFloat("playerSoul", playerController.playerSoul);
        float currentSoul = PlayerPrefs.GetFloat("playerSoul");

        if (soulText != null)
        {
            soulText.text = currentSoul.ToString();
        }

        UIManager.Instance.UpdateStatusText();
    }

    public void UpdateUsePlayerSOUL(float necessitySoul)
    //해당 메서드 호출시 사용한 소울만큼 감소.
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController not assigned!");
            return;
        }

        playerController.playerSoul -= necessitySoul;
        PlayerPrefs.SetFloat("playerSoul", playerController.playerSoul);
        float currentSoul = PlayerPrefs.GetFloat("playerSoul");

        if (soulText != null)
        {
            soulText.text = currentSoul.ToString();
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


        hpBar = playerUI.Find("HPprogress")?.GetComponent<Slider>();
        if (hpBar == null) Debug.LogWarning("HPprogress not found!");

        stBar = playerUI.Find("STprogress")?.GetComponent<Slider>();
        if (stBar == null) Debug.LogWarning("STprogress not found!");

        mnBar = playerUI.Find("MNprogress")?.GetComponent<Slider>();
        if (mnBar == null) Debug.LogWarning("MNprogress not found!");

        soulText = playerUI.Find("SOULprogress")?.GetComponent<Text>();
        if (soulText == null) Debug.LogWarning("SOULprogress not found!");
    }

    private void DisplayMapName(string displayName)
    {
        GameObject mapNameInstance = Instantiate(mapNameTextPrefab, FindObjectOfType<Canvas>().transform);
        Text mapNameText = mapNameInstance.GetComponent<Text>();
        mapNameText.text = displayName;

        // 텍스트가 서서히 나타났다 사라지는 코루틴 시작
        StartCoroutine(FadeInAndOut(mapNameInstance.GetComponent<CanvasGroup>()));
    }

    public void DisplayDemonSlainedText()
    {
        GameObject slainedTextInstance = Instantiate(demonSlainedTextPrefab, FindObjectOfType<Canvas>().transform);
        StartCoroutine(FadeInAndOut(slainedTextInstance.GetComponent<CanvasGroup>()));
        audioSource.PlayOneShot(clearSound);
    }


    public void InitializeMonsters()
    {
        EnemyController2[] monsters = FindObjectsOfType<EnemyController2>(); // 모든 몬스터 탐색
        EnemyEliteController[] elites = FindObjectsOfType<EnemyEliteController>();
        EnemyControllerClabKing[] crabKing = FindObjectsOfType<EnemyControllerClabKing>();
        BowEnemyController[] bow_monsters = FindObjectsOfType<BowEnemyController>();
        BossAController[] aBoss = FindObjectsOfType<BossAController>();
        BossBController[] bBoss = FindObjectsOfType<BossBController>();
        BossCController[] cBoss = FindObjectsOfType<BossCController>();
        EnemySuperAIController[] aiEnemy = FindObjectsOfType<EnemySuperAIController>();
        EnemyMonsterController[] monEnemy = FindObjectsOfType<EnemyMonsterController>();
        //EnemyMagicEliteController[] magicEnemy = FindObjectsOfType<EnemyMagicEliteController>();

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
        foreach (var a in cBoss)
        {
            a.InitializeHPBar(cBossHPSliderPrefab);
        }
        foreach (var a in aiEnemy)
        {
            a.InitializeHPBar(hpSliderPrefab);
        }
        foreach (var mon in monEnemy)
        {
            mon.InitializeHPBar(hpSliderPrefab);
        }

        /*
        foreach (var a in magicEnemy)
        {
            a.InitializeHPBar(hpSliderPrefab);
        }
        */
    }

    public void UpdatePlayerHP(float currentHP)
    {
        if (hpBar != null)
        {
            float hpRatio = currentHP / playerController.PlayerMaxHP;
            hpBar.value = hpRatio;
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
            float initialHPRatio = playerController.PlayerHP / playerController.PlayerMaxHP;
            hpBar.value = initialHPRatio; // 체력을 직접 설정
        }
    }

    public void UpdatePlayerST(float currentST)
    {
        if (stBar != null)
        {
            float stRatio = currentST / playerController.PlayerMaxStamina;
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
        StopBackgroundMusic();
        StartCoroutine(LoadSceneAsync("Calios"));
    }

    public IEnumerator LoadSceneAsync(string nextSceneName)
    {
        LoadingManager.tips = tips; // 로딩 씬 실행 전 부가적인 데이터 설정
        LoadingManager.nextSceneName = nextSceneName;

        SceneManager.LoadScene("LoadingScene");
        yield return null; // 로딩 씬 로드 후 다음 프레임 대기
    }

    public void StartScene()
    {
        StopBackgroundMusic();
        StartCoroutine(LoadSceneAsync("Calios"));
    }

    private string[] tips = new string[]
    {
        "- TMI -\r\n사실 메이지 엘리아스는 칼을 거꾸로 들고 있는 바보입니다.",
        "- TMI -\r\n메이지 엘리아스는 칼리오스에서 추방당한 전사였습니다.\r\n지금은 마법사지만...",
        "- TMI -\r\n대악마 카몬은 사실 대악마가 아닙니다.\r\n악마중에서 제일 약한 자칭 대악마입니다.",
        "- TMI -\r\n주인공의 이름은 알레그로입니다.\r\n그는 이곳에서 옛 친구 레온의 발자취를 따라가고 있습니다.",
        "- TMI -\r\n레온은 주인공의 옛 친구이자\r\n칼리오스 왕국을 악마에게 팔아넘긴 역적입니다.",
        "- TMI -\r\n구울은 악마가 아닙니다.\r\n정확히는 악마가 키우는 애완동물입니다.",
        "- TIP -\r\n정확한 타이밍에 구르면 \r\n적의 공격에 맞지 않습니다.",
        "- TIP -\r\n때로는 한 발자국 물러나서\r\n적의 공격 패턴을 숙지하면 큰 도움이 됩니다.",
        "- TIP -\r\n다수의 적이 몰려있으면\r\n하나씩 각개격파 하는 것이 무조건 유리합니다.",
        "- TIP -\r\n구르기 버튼을 연타한다고\r\n적의 공격에서 달아날 수 없습니다.",
    };
}
