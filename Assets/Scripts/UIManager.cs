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

    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UIManager>();
            return _instance;
        }
    }

    private SuperPlayerController playerController;
    private EquipmentManager equipmentManager;

    public enum UIState { Game, Menu, Status, Equipment, Teleport }
    public UIState currentState = UIState.Game;

    private Vector3 burn = new Vector3(-2.83f, 4.548f, 25.77f);     //다른 세이브 포인트 위치 변경시 같이 변경.
    private Vector3 cal_b = new Vector3(-38.515f, 9.509007f, 44.15f);     //다른 세이브 포인트 위치 변경시 같이 변경.
    private Vector3 outer = new Vector3(43.73f, 4.548f, 20.12f);     //다른 세이브 포인트 위치 변경시 같이 변경.


    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 싱글톤 인스턴스 설정
        _instance = this;

        AccessController();
        InitializeButtonClickEvents();
        SetInitialUIState();
        LoadSaveButtonState();
    }

    private void teleportBurn()
    {
        GameManager.Instance.SavePosition(burn);
        GoBack();
        GameManager.Instance.StartScene();
    }
    private void teleportCal_b()
    {
        GameManager.Instance.SavePosition(cal_b);
        GoBack();
        GameManager.Instance.StartScene();
    }
    private void teleportOuter()
    {
        GameManager.Instance.SavePosition(outer);
        GoBack();
        GameManager.Instance.StartScene();
    }

    public void ActivateSaveButton(string savePointName)
    {
        Transform buttonTransform = teleportUI.transform.Find(savePointName);

        if (buttonTransform != null)
        {
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                button.gameObject.SetActive(true); // 버튼 활성화
            }
            else
            {
                Debug.LogWarning($"Button component not found on {savePointName}!");
            }
        }
        else
        {
            Debug.LogWarning($"Button for save point {savePointName} not found in teleportUI!");
        }
    }

    // 게임 시작 시 버튼 활성화 상태 복원 함수
    public void LoadSaveButtonState()
    {
        foreach (Transform buttonTransform in teleportUI.transform)
        {
            if (buttonTransform.TryGetComponent(out Button button))
            {
                string savePointName = buttonTransform.name;
                if (PlayerPrefs.GetInt(savePointName, 0) == 1)  // 1이면 활성화된 상태
                {
                    button.gameObject.SetActive(true);
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

    public void InitializeButtonClickEvents()
    {
        Transform statusMenuTransform = stateUI.transform.Find("StatusMenu");
        Transform equipmentMenuTransform = stateUI.transform.Find("EquipmentMenu");

        Transform teleportburn = teleportUI.transform.Find("BurningGround");
        Transform teleportcal_b = teleportUI.transform.Find("CaliosBridge");
        Transform teleportouter = teleportUI.transform.Find("OuterBattleField");

        if (teleportburn != null)
        {
            Button[] statusButtons = teleportburn.GetComponentsInChildren<Button>(); // StatusMenu의 모든 버튼 가져오기
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(teleportBurn); // ShowStatusUI 메서드 연결
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (teleportcal_b != null)
        {
            Button[] statusButtons = teleportcal_b.GetComponentsInChildren<Button>(); // StatusMenu의 모든 버튼 가져오기
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(teleportCal_b); // ShowStatusUI 메서드 연결
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (teleportouter != null)
        {
            Button[] statusButtons = teleportouter.GetComponentsInChildren<Button>(); // StatusMenu의 모든 버튼 가져오기
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(teleportOuter); // ShowStatusUI 메서드 연결
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (statusMenuTransform != null)
        {
            Button[] statusButtons = statusMenuTransform.GetComponentsInChildren<Button>(); // StatusMenu의 모든 버튼 가져오기
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ShowStatusUI); // ShowStatusUI 메서드 연결
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
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ShowEquipmentUI); // ShowEquipmentUI 메서드 연결
                Debug.Log($"Button {button.name} 이벤트 등록됨.");
            }
        }
        else
        {
            Debug.LogWarning("EquipmentMenu not found in StateUI!");
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
            LoadSaveButtonState();
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

    public void OpenTeleport()
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
    }
}