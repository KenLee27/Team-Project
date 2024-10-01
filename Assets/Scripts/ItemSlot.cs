using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ������ ���� ���� ���� ��ũ��Ʈ
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // ���Կ� ������ �������� �̸�, �̹���, ���� �� ����
    public string itemName;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;

    [SerializeField]
    private Image itemImage; // // ���Կ��� ������ �̹����� ǥ���� UI �̹���

    public Image itemDescriptionImage; // ������ ������ ǥ���� UI �̹���
    public Text ItemDescriptionNameText; // ������ �̸��� ǥ���� UI �ؽ�Ʈ
    public Text ItemDescriptionText;  // ������ ������ ǥ���� UI �ؽ�Ʈ

    public GameObject selectedShader; // ������ ���õ� �� ���Ǵ� �ð��� ȿ��
    public bool thisItemSelected; // ���� ������ ���õǾ����� ����

    private EquipmentManager equipmentManager;
    private InventoryManager inventoryManager; // InventoryManager ����


    public GameObject defaultWeaponPrefab; // �⺻ ���� ���� ������

    public AudioSource audioSource; // ����� �ҽ� ������Ʈ
    public AudioClip clickSound; // Ŭ���� �� ����� ����


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
            string itemName = "Į������ ������ ���";
            string itemDescription = "Į������ ������ ��� ���� ���� �߰�"; // �ش� ���� �������� ����
            AddItem(itemName, itemSprite, itemDescription);  // ���Կ� �⺻ ������ �߰�
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Update()
    {
        // ���õ� �������� ���� �� AŰ�� ������ �κ��丮�� �߰�
        if (thisItemSelected && Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A key pressed, toggling weapon in inventory.");
            ToggleWeaponInInventory(); // ���⸦ �κ��丮�� �߰��ϰų� ����
        }
    }

    private void ToggleWeaponInInventory()
    {
        if (inventoryManager != null && !string.IsNullOrEmpty(itemName))
        {
            // ���Ⱑ �̹� �κ��丮�� �ִ��� Ȯ��
            if (inventoryManager.IsWeaponInInventory(itemName))
            {
                Debug.Log(itemName + " is already in the inventory. Removing it...");
                inventoryManager.RemoveWeaponFromInventory(itemName); // �κ��丮���� ����
                Debug.Log(itemName + " has been removed from the inventory.");
            }
            else
            {
                Debug.Log("Trying to add: " + itemName);
                inventoryManager.AddWeaponToInventory(itemName); // �κ��丮�� �߰�
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

        Debug.Log("Item added to slot: " + itemName); // ���Կ� �߰��� ������ �̸� �α�

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
            Debug.Log("Slot clicked: " + itemName); // Ŭ���� ������ ������ �̸� Ȯ��

            // Ŭ�� ���� ���
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound); // ���� ���
            }
            else
            {
                Debug.LogWarning("AudioSource or ClickSound is not assigned.");
            }

            OnLeftClick(); // ���� Ŭ�� �� OnLeftClick �޼��� ȣ��
        }
    }


    public void OnLeftClick()
    {
        equipmentManager.DeselectAllSlots(); // �ٸ� ������ ���� ����
        selectedShader.SetActive(true); // ���� ������ ���õ� ���·� ǥ��
        thisItemSelected = true; // ������ ���õǾ����� ǥ��

        ItemDescriptionNameText.text = itemName; // ������ �̸� ������Ʈ
        ItemDescriptionText.text = itemDescription; // ������ ���� ������Ʈ
        itemDescriptionImage.sprite = itemSprite; // ������ �̹��� ������Ʈ

        Debug.Log("Item selected: " + itemName + ", " + itemDescription);

    }


    //private void ActivateWeaponInInventory()
    //{
    //    if (inventoryManager != null && !string.IsNullOrEmpty(itemName))
    //    {
    //        inventoryManager.AddWeaponToInventory(itemName); // ���� �̸��� �����Ͽ� �κ��丮�� �߰�
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
            inventoryManager.AddWeaponToInventory(itemName); // ���� �̸��� �����Ͽ� �κ��丮�� �߰�
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

    // �������� �߰��� �� ȣ���ϴ� �޼��� (PlayerPrefs���� �ε� �� ���)
    public void LoadItem(ItemData itemData)
    {
        AddItem(itemData.itemName, itemData.Base64ToSprite(), itemData.itemDescription);
    }

}