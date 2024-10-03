using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 아이템 슬롯 동작 관리 스크립트
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // 슬롯에 저장할 아이템의 이름, 이미지, 설명 및 상태
    public string itemName;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;

    [SerializeField]
    private Image itemImage; // // 슬롯에서 아이템 이미지를 표시할 UI 이미지

    public Image itemDescriptionImage; // 아이템 설명을 표시할 UI 이미지
    public Text ItemDescriptionNameText; // 아이템 이름을 표시할 UI 텍스트
    public Text ItemDescriptionText;  // 아이템 설명을 표시할 UI 텍스트

    public GameObject selectedShader; // 슬롯이 선택될 때 사용되는 시각적 효과
    public bool thisItemSelected; // 현재 슬롯이 선택되었는지 여부

    private EquipmentManager equipmentManager;
    private InventoryManager inventoryManager; // InventoryManager 참조


    public GameObject defaultWeaponPrefab; // 기본 고정 무기 프리팹

    public AudioSource audioSource; // 오디오 소스 컴포넌트
    public AudioClip clickSound; // 클릭할 때 재생할 사운드


    private void Start()
    {
        equipmentManager = GameObject.Find("Canvas").GetComponent<EquipmentManager>();

        var equipmentCanvas = GameObject.Find("Canvas");
        if (equipmentCanvas != null)
        {
            equipmentManager = equipmentCanvas.GetComponent<EquipmentManager>();
            if (equipmentManager == null)
            {
                Debug.LogError("EquipmentManager component not found on EquipmentCanvas.");
            }
        }
        else
        {
            Debug.LogError("EquipmentCanvas not found in the scene.");
        }

        inventoryManager = FindObjectOfType<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in the scene.");
        }


        if (!isFull && defaultWeaponPrefab != null)
        {
            Sprite itemSprite = defaultWeaponPrefab.GetComponent<SpriteRenderer>().sprite;
            string itemName = "칼리오스 병사의 곡검";
            string itemDescription = "칼리오스 병사의 곡검 무기 설명 추가"; // 해당 무기 설명으로 변경
            AddItem(itemName, itemSprite, itemDescription);  // 슬롯에 기본 아이템 추가
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Update()
    {
        // 선택된 아이템이 있을 때 A키를 누르면 인벤토리에 추가
        if (thisItemSelected && Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A key pressed, toggling weapon in inventory.");
            ToggleWeaponInInventory(); // 무기를 인벤토리에 추가하거나 해제
        }
    }

    private void ToggleWeaponInInventory()
    {
        if (inventoryManager != null && !string.IsNullOrEmpty(itemName))
        {
            // 무기가 이미 인벤토리에 있는지 확인
            if (inventoryManager.IsWeaponInInventory(itemName))
            {
                Debug.Log(itemName + " is already in the inventory. Removing it...");
                inventoryManager.RemoveWeaponFromInventory(itemName); // 인벤토리에서 해제
                Debug.Log(itemName + " has been removed from the inventory.");
            }
            else
            {
                Debug.Log("Trying to add: " + itemName);
                inventoryManager.AddWeaponToInventory(itemName); // 인벤토리에 추가
                Debug.Log(itemName + " has been added to the inventory.");
            }
        }
        else
        {
            Debug.LogWarning("No item selected or InventoryManager is not assigned.");
        }
    }

    public void AddItem(string itemName, Sprite itemSprite, string itemDescription)
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        isFull = true;

        Debug.Log("Item added to slot: " + itemName); // 슬롯에 추가된 아이템 이름 로그

        if (itemImage != null)
        {
            itemImage.sprite = itemSprite;
        }
        else
        {
            Debug.LogWarning("Item image component not assigned!");
        }
        Debug.Log("Added item: " + itemName);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Slot clicked: " + itemName); // 클릭된 슬롯의 아이템 이름 확인

            // 클릭 사운드 재생
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound); // 사운드 재생
            }
            else
            {
                Debug.LogWarning("AudioSource or ClickSound is not assigned.");
            }

            OnLeftClick(); // 왼쪽 클릭 시 OnLeftClick 메서드 호출
        }
    }


    public void OnLeftClick()
    {
        equipmentManager.DeselectAllSlots(); // 다른 슬롯의 선택 해제
        selectedShader.SetActive(true); // 현재 슬롯을 선택된 상태로 표시
        thisItemSelected = true; // 슬롯이 선택되었음을 표시

        ItemDescriptionNameText.text = itemName; // 아이템 이름 업데이트
        ItemDescriptionText.text = itemDescription; // 아이템 설명 업데이트
        itemDescriptionImage.sprite = itemSprite; // 아이템 이미지 업데이트

        Debug.Log("Item selected: " + itemName + ", " + itemDescription);

    }


    //private void ActivateWeaponInInventory()
    //{
    //    if (inventoryManager != null && !string.IsNullOrEmpty(itemName))
    //    {
    //        inventoryManager.AddWeaponToInventory(itemName); // 무기 이름을 전달하여 인벤토리에 추가
    //        Debug.Log(itemName + " has been added to the inventory.");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No item selected or InventoryManager is not assigned.");
    //    }
    //}

    private void ActivateWeaponInInventory()
    {
        if (inventoryManager != null && !string.IsNullOrEmpty(itemName))
        {
            Debug.Log("Trying to add: " + itemName);
            inventoryManager.AddWeaponToInventory(itemName); // 무기 이름을 전달하여 인벤토리에 추가
            Debug.Log(itemName + " has been added to the inventory.");
        }
        else
        {
            Debug.LogWarning("No item selected or InventoryManager is not assigned.");
        }
    }

    public ItemData GetItemData()
    {
        return new ItemData(itemName, itemDescription, itemSprite);
    }

    // 아이템을 추가할 때 호출하는 메서드 (PlayerPrefs에서 로드 시 사용)
    public void LoadItem(ItemData itemData)
    {
        AddItem(itemData.itemName, itemData.Base64ToSprite(), itemData.itemDescription);
    }

}