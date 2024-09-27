using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject weaponUI;
    public GameObject stateUI;
    public GameObject statusUI;
    public GameObject equipmentUI;

    private enum UIState { Game, Menu, Status, Equipment }
    private UIState currentState = UIState.Game;

    private EquipmentManager equipmentManager; // EquipmentManager�� ������ ����

    void Start()
    {
        SetInitialUIState();
        equipmentManager = FindObjectOfType<EquipmentManager>(); // EquipmentManager ã��
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

    private void SetInitialUIState()
    {
        playerUI.SetActive(true);
        weaponUI.SetActive(true);

        stateUI.SetActive(false);
        statusUI.SetActive(false);
        equipmentUI.SetActive(false);
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
        playerUI.SetActive(false);
        weaponUI.SetActive(false);
        stateUI.SetActive(true);
        statusUI.SetActive(false);
        equipmentUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowStatusUI() // �������ͽ�â Ȱ��ȭ, �޴�â ��Ȱ��ȭ
    {
        stateUI.SetActive(false);
        statusUI.SetActive(true);
        currentState = UIState.Status;
    }

    public void ShowEquipmentUI() // ���â Ȱ��ȭ, �޴�â ��Ȱ��ȭ
    {
        stateUI.SetActive(false);
        equipmentUI.SetActive(true);
        currentState = UIState.Equipment;

        // EquipmentManager�� EquipmentMenu�� Ȱ��ȭ
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
        playerUI.SetActive(true);
        weaponUI.SetActive(true);

        stateUI.SetActive(false);
        statusUI.SetActive(false);
        equipmentUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}