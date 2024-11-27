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

    // PlayerPrefs�� ����Ͽ� ������ ���¸� ������ Ű
    private string chestKey;

    void Start()
    {
        // ���ڿ� ���� ���� Ű ���� (��ġ ���)
        chestKey = "Chest_" + transform.position.ToString();

        // ���ڰ� �̹� ���ȴ��� Ȯ��
        if (PlayerPrefs.GetInt(chestKey, 0) == 1)
        {
            isOpen = true; // ���ڰ� ������ ������
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
                // ���ڸ� ���ϴ�
                isOpen = true;
            }
            else if (!lightOff && isOpen)
            {
                // ���� ���ϴ�
                if (pickupLight != null)
                {
                    lightOff = true;
                    pickupLight.enabled = false;

                    DisplayWeaponInSlot(); // ��� ���Կ� ���⸦ ǥ���մϴ�


                    PlayerPrefs.SetInt(chestKey, 1); // ���� ����
                    PlayerPrefs.Save(); // ������ ������ ����

                }
            }
        }

        // �ε巴�� ȸ���մϴ�
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
                Debug.LogWarning("���� �����Ͱ� �����ϴ�: " + itemName);
            }
        }
        else
        {
            Debug.LogWarning("���� ���� ���� �Ǵ� EquipmentManager�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }
}