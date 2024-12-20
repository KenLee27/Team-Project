using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject weaponUI;
    public GameObject stateUI;
    public GameObject statusUI;
    public GameObject equipmentUI;
    public GameObject teleportUI;

    private SuperPlayerController playerController;
    private EquipmentManager equipmentManager;

    public AudioClip buttonClickSound; // 버튼 클릭 효과음
    public AudioClip openMenuSound; // 메뉴 열기 효과음
    public AudioClip closeMenuSound; // 메뉴 닫기 효과음
    private AudioSource audioSource;

    private enum UIState { Game, Menu, Status, Equipment, Teleport }
    private UIState currentState = UIState.Game;


    private void Start()
    {
        AccessController();
        InitializeButtonClickEvents();
        SetInitialUIState();

    // AudioSource 컴포넌트 가져오기
    audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }


    public void InitializeButtonClickEvents()
    {
        Transform statusMenuTransform = stateUI.transform.Find("StatusMenu");
        Transform equipmentMenuTransform = stateUI.transform.Find("EquipmentMenu");

        if (statusMenuTransform != null)
        {
            Button[] statusButtons = statusMenuTransform.GetComponentsInChildren<Button>(); // StatusMenu의 모든 버튼 가져오기
            foreach (Button button in statusButtons)
            {
                //button.onClick.AddListener(ShowStatusUI); // ShowStatusUI 메서드 연결
                button.onClick.AddListener(() => { ShowStatusUI(); PlayButtonClickSound(); }); // ShowStatusUI 메서드 + 효과음 연결


            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (equipmentMenuTransform != null)
        {
            Button[] equipmentButtons = equipmentMenuTransform.GetComponentsInChildren<Button>(); // EquipmentMenu의 모든 버튼 가져오기
            foreach (Button button in equipmentButtons)
            {
                //button.onClick.AddListener(ShowEquipmentUI); // ShowEquipmentUI 메서드 연결
                button.onClick.AddListener(() => { ShowEquipmentUI(); PlayButtonClickSound(); }); // ShowEquipmentUI 메서드 + 효과음 연결


            }
        }
        else
        {
            Debug.LogWarning("EquipmentMenu not found in StateUI!");
        }
    }

    private void PlayButtonClickSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    public void AccessController()
    {
        playerController = FindObjectOfType<SuperPlayerController>();
        equipmentManager = FindObjectOfType<EquipmentManager>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene.");
        }
        if (equipmentManager == null)
        {
            Debug.LogError("EquipmentManager not found in the scene.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            GoBack();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            OpenTeleport();
        }
    }

    public void FindUIElements()
    {
        playerUI = GameObject.Find("PlayerUI");
        weaponUI = GameObject.Find("WeaponUI");
        stateUI = GameObject.Find("StateUI");
        statusUI = GameObject.Find("StatusUI");
        equipmentUI = GameObject.Find("EquipmentUI");
        teleportUI = GameObject.Find("TeleportUI");

        if (playerUI == null) Debug.LogWarning("PlayerUI not found!");
        if (weaponUI == null) Debug.LogWarning("WeaponUI not found!");
        if (stateUI == null) Debug.LogWarning("StateUI not found!");
        if (statusUI == null) Debug.LogWarning("StatusUI not found!");
        if (equipmentUI == null) Debug.LogWarning("EquipmentUI not found!");
        if (teleportUI == null) Debug.LogWarning("TeleportUI not found!");
    }

    public void SetInitialUIState()
    {
        if (playerUI != null) playerUI.SetActive(true);
        if (weaponUI != null) weaponUI.SetActive(true);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (teleportUI != null) teleportUI.SetActive(false);
    }

    private void ToggleMenu()
    {
        switch (currentState)
        {
            case UIState.Game:
                currentState = UIState.Menu;
                OpenMenu();
                break;
            case UIState.Menu:
            case UIState.Status:
            case UIState.Equipment:
            case UIState.Teleport:
                break;
        }
    }

    private void OpenTeleport()
    {
        currentState = UIState.Teleport;

        if (playerController != null) playerController.canAttack = false;

        if (playerUI != null) playerUI.SetActive(false);
        if (weaponUI != null) weaponUI.SetActive(false);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (equipmentUI != null) teleportUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OpenMenu() // E버튼 : 메뉴창 활성화, 나머지 전부 비활성화, 마우스 커서 활성화
    {
        if (playerController != null) playerController.canAttack = false;

        if (playerUI != null) playerUI.SetActive(false);
        if (weaponUI != null) weaponUI.SetActive(false);
        if (stateUI != null) stateUI.SetActive(true);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (equipmentUI != null) teleportUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlaySound(openMenuSound);

}

public void ShowStatusUI() // 스테이터스창 활성화, 메뉴창 비활성화
    {
        Debug.Log("스테이터스 열림");
        stateUI.SetActive(false);
        statusUI.SetActive(true);
        currentState = UIState.Status;
    }

    public void ShowEquipmentUI() // 장비창 활성화, 메뉴창 비활성화
    {
        Debug.Log("장비 열림");
        stateUI.SetActive(false);
        equipmentUI.SetActive(true);
        currentState = UIState.Equipment;

        if (equipmentManager != null)
        {
            equipmentManager.ToggleEquipmentMenu();
        }
    }

    private void GoBack() // Q버튼 : 뒤로가기
    {
        switch (currentState)
        {
            case UIState.Menu:
                currentState = UIState.Game;
                CloseMenu();
                break;
            case UIState.Status:
                currentState = UIState.Menu;
                OpenMenu();
                break;
            case UIState.Equipment:
                currentState = UIState.Menu;
                OpenMenu();
                break;
            case UIState.Teleport:
                currentState = UIState.Game;
                CloseMenu();
                break;
        }
    }

    private void CloseMenu()
    {
        if (playerController != null) playerController.canAttack = true;

        if (playerUI != null) playerUI.SetActive(true);
        if (weaponUI != null) weaponUI.SetActive(true);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (equipmentUI != null) teleportUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlaySound(closeMenuSound);

    }

    // 사운드 재생 함수 추가
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}