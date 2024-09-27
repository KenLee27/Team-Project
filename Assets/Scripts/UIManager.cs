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

    private EquipmentManager equipmentManager; // EquipmentManager를 참조할 변수

    void Start()
    {
        SetInitialUIState();
        equipmentManager = FindObjectOfType<EquipmentManager>(); // EquipmentManager 찾기
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

    private void OpenMenu() // E버튼 : 메뉴창 활성화, 나머지 전부 비활성화, 마우스 커서 활성화
    {
        playerUI.SetActive(false);
        weaponUI.SetActive(false);
        stateUI.SetActive(true);
        statusUI.SetActive(false);
        equipmentUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowStatusUI() // 스테이터스창 활성화, 메뉴창 비활성화
    {
        stateUI.SetActive(false);
        statusUI.SetActive(true);
        currentState = UIState.Status;
    }

    public void ShowEquipmentUI() // 장비창 활성화, 메뉴창 비활성화
    {
        stateUI.SetActive(false);
        equipmentUI.SetActive(true);
        currentState = UIState.Equipment;

        // EquipmentManager의 EquipmentMenu를 활성화
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