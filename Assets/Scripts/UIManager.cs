using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject weaponUI;
    public GameObject stateUI;
    public GameObject statusUI;
    public GameObject equipmentUI;

    private SuperPlayerController playerController;
    private EquipmentManager equipmentManager;

    private enum UIState { Game, Menu, Status, Equipment }
    private UIState currentState = UIState.Game;

    private void Start()
    {
        AccessController();
        InitializeButtonClickEvents();
    }

    public void InitializeButtonClickEvents()
    {
        Transform statusMenuTransform = stateUI.transform.Find("StatusMenu");
        Transform equipmentMenuTransform = stateUI.transform.Find("EquipmentMenu");

        if (statusMenuTransform != null)
        {
            Button[] statusButtons = statusMenuTransform.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.AddListener(ShowStatusUI); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (equipmentMenuTransform != null)
        {
            Button[] equipmentButtons = equipmentMenuTransform.GetComponentsInChildren<Button>(); // EquipmentMenu�� ��� ��ư ��������
            foreach (Button button in equipmentButtons)
            {
                button.onClick.AddListener(ShowEquipmentUI); // ShowEquipmentUI �޼��� ����
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
    }

    public void FindUIElements()
    {
        playerUI = GameObject.Find("PlayerUI");
        weaponUI = GameObject.Find("WeaponUI");
        stateUI = GameObject.Find("StateUI");
        statusUI = GameObject.Find("StatusUI");
        equipmentUI = GameObject.Find("EquipmentUI");

        if (playerUI == null) Debug.LogWarning("PlayerUI not found!");
        if (weaponUI == null) Debug.LogWarning("WeaponUI not found!");
        if (stateUI == null) Debug.LogWarning("StateUI not found!");
        if (statusUI == null) Debug.LogWarning("StatusUI not found!");
        if (equipmentUI == null) Debug.LogWarning("EquipmentUI not found!");
    }

    public void SetInitialUIState()
    {
        if (playerUI != null) playerUI.SetActive(true);
        if (weaponUI != null) weaponUI.SetActive(true);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
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
                break;
        }
    }

    private void OpenMenu() // E��ư : �޴�â Ȱ��ȭ, ������ ���� ��Ȱ��ȭ, ���콺 Ŀ�� Ȱ��ȭ
    {
        if (playerController != null) playerController.canAttack = false;

        if (playerUI != null) playerUI.SetActive(false);
        if (weaponUI != null) weaponUI.SetActive(false);
        if (stateUI != null) stateUI.SetActive(true);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowStatusUI() // �������ͽ�â Ȱ��ȭ, �޴�â ��Ȱ��ȭ
    {
        Debug.Log("�������ͽ� ����");
        stateUI.SetActive(false);
        statusUI.SetActive(true);
        currentState = UIState.Status;
    }

    public void ShowEquipmentUI() // ���â Ȱ��ȭ, �޴�â ��Ȱ��ȭ
    {
        Debug.Log("��� ����");
        stateUI.SetActive(false);
        equipmentUI.SetActive(true);
        currentState = UIState.Equipment;

        if (equipmentManager != null)
        {
            equipmentManager.ToggleEquipmentMenu();
        }
    }

    private void GoBack() // Q��ư : �ڷΰ���
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}