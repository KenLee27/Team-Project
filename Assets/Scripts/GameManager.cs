using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SuperPlayerController playerController;
    public ImgsFillDynamic hpBar;
    public Slider stBar;
    public Slider mnBar;
    public Text soulText;

    public GameObject hpSliderPrefab;     // HP 슬라이더 프리팹
    public GameObject stSliderPrefab;     // ST 슬라이더 프리팹
    public GameObject mnSliderPrefab;
    public GameObject crabHPSliderPrefab;     // HP 슬라이더 프리팹

    public GameObject gameOverTextPrefab;
    public GameObject mapNameTextPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 게임 오브젝트가 씬 로드 시 파괴되지 않도록 설정
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);  // 이미 인스턴스가 존재하면 새로 생성된 객체를 파괴
        }

        InitializeMonsters();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAndSetSliders();

        InitializePlayerHP();
        InitializePlayerST();
        InitializePlayerMana();

        string displayName = GetDisplayNameForScene(scene.name);
        DisplayMapName(displayName);

        InitializePlayerController();                                           //고은서가 추가함 : 플레이어컨트롤러 missing 방지
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


        hpBar = playerUI.Find("HPprogress")?.GetComponent<ImgsFillDynamic>();
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


    public void InitializeMonsters()
    {
        EnemyController2[] monsters = FindObjectsOfType<EnemyController2>(); // 모든 몬스터 탐색
        EnemyControllerClabKing[] crabKing = FindObjectsOfType<EnemyControllerClabKing>();

        foreach (var monster in monsters)
        {
            monster.InitializeHPBar(hpSliderPrefab); // 각 몬스터에 슬라이더 초기
        }
        foreach (var king in crabKing)
        {
            king.InitializeHPBar(crabHPSliderPrefab); // 각 보스에 슬라이더 초기화
        }
    }

    public void UpdatePlayerHP(float currentHP)
    {
        if (hpBar != null)
        {
            float hpRatio = currentHP / 100f;
            hpBar.SetValue(hpRatio);
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
            hpBar.SetValue(initialHPRatio, true); // 체력을 직접 설정
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

        Destroy(canvasGroup.gameObject); // 텍스트 오브젝트 삭제
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
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Old_Dock");
    }
}
