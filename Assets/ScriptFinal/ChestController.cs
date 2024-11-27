using UnityEngine;

public class ChestController : MonoBehaviour
{
    public Transform player;
    public GameObject chestTop;
    public GameObject pickup;

    public GameObject weaponInChest;
    public InventoryManager inventoryManager;
    public EquipmentManager equipmentManager;

    public float openDistance = 2f;
    public float openingSpeed = 2f;

    private bool isOpen = false;
    private bool lightOff = false;
    private Quaternion openRotation;
    private Light pickupLight;

    // PlayerPrefs를 사용하여 상자의 상태를 저장할 키
    private string chestKey;

    void Start()
    {
        // 상자에 대한 고유 키 생성 (위치 기반)
        chestKey = "Chest_" + transform.position.ToString();

        // 상자가 이미 열렸는지 확인
        if (PlayerPrefs.GetInt(chestKey, 0) == 1)
        {
            isOpen = true; // 상자가 이전에 열렸음
            lightOff = true;
        }

        openRotation = Quaternion.Euler(0, 0, -90);

        if (pickup != null)
        {
            pickupLight = pickup.GetComponent<Light>();
        }
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
        if (equipmentManager == null)
        {
            equipmentManager = FindObjectOfType<EquipmentManager>();
        }
        if (isOpen)
        {
            pickupLight.enabled = false;
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= openDistance && Input.GetKeyDown(KeyCode.F))
        {
            if (!isOpen)
            {
                // 상자를 엽니다
                isOpen = true;
            }
            else if (!lightOff && isOpen)
            {
                // 불을 끕니다
                if (pickupLight != null)
                {
                    lightOff = true;
                    pickupLight.enabled = false;

                    DisplayWeaponInSlot(); // 장비 슬롯에 무기를 표시합니다


                    PlayerPrefs.SetInt(chestKey, 1); // 상태 저장
                    PlayerPrefs.Save(); // 데이터 저장을 보장

                }
            }
        }

        // 부드럽게 회전합니다
        if (isOpen && chestTop.transform.localRotation != openRotation)
        {
            chestTop.transform.localRotation = Quaternion.Lerp(chestTop.transform.localRotation, openRotation, Time.deltaTime * openingSpeed);
        }
    }

    public void DisplayWeaponInSlot()
    {
        if (weaponInChest != null && equipmentManager != null)
        {
            Sprite itemSprite = weaponInChest.GetComponent<SpriteRenderer>().sprite;
            string itemName = weaponInChest.name;

            if (equipmentManager.itemDatabase.TryGetValue(itemName, out ItemData itemData))
            {
                equipmentManager.ShowWeaponInSlot(itemData.itemName, itemSprite, itemData.itemDescription);
            }
            else
            {
                Debug.LogWarning("무기 데이터가 없습니다: " + itemName);
            }
        }
        else
        {
            Debug.LogWarning("상자 안의 무기 또는 EquipmentManager가 할당되지 않았습니다.");
        }
    }
}