using UnityEngine;

public class ChestController : MonoBehaviour
{
    public Transform player;  // �÷��̾��� Transform
    public GameObject chestTop;  // ������ ���κ� GameObject
    public GameObject pickup;  // ����Ʈ�� �ִ� �� ������Ʈ

    public GameObject weaponInChest; // ���� �ȿ� �ִ� ���� ������
    public InventoryManager inventoryManager; // �÷��̾��� �κ��丮 �Ŵ���

    public EquipmentManager equipmentManager; // ��� �޴� �Ŵ���

    public float openDistance = 2f;  // ���ڸ� �� �� �ִ� �ִ� �Ÿ�
    public float openingSpeed = 2f;  // ȸ�� �ӵ�

    private bool isOpen = false; // ���ڰ� ���ȴ��� ����
    private bool lightOff = false; // ���� �������� ����
    private Quaternion openRotation;  // ���� ������ Quaternion
    private Light pickupLight;

    void Start()
    {
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
    }

    void Update()
    {
        // �÷��̾�� ���� �� �Ÿ� ���
        float distance = Vector3.Distance(player.position, transform.position);

        // ���� ��ó�� �ְ� FŰ�� �����ٸ�, ���� ����
        if (distance <= openDistance && Input.GetKeyDown(KeyCode.F))
        {
            if (!isOpen)
            {
                // ���ڸ� ����
                isOpen = true;
            }
            else if (!lightOff && isOpen)
            {
                // ���� ���� ������ �ʾҴٸ� ����
                if (pickupLight != null)
                {
                    pickupLight.enabled = false;
                    lightOff = true;
                    // ActivateWeaponInInventory(); // ���� Ȱ��ȭ

                    DisplayWeaponInSlot(); // ���� ���â�� ǥ��
                }
            }
        }

        // ������ ���¿� ���� �ε巴�� ȸ��
        if (isOpen && chestTop.transform.localRotation != openRotation)
        {
            chestTop.transform.localRotation = Quaternion.Lerp(chestTop.transform.localRotation, openRotation, Time.deltaTime * openingSpeed);
        }
    }

    //private void ActivateWeaponInInventory()
    //{
    //    if (inventoryManager != null && weaponInChest != null)
    //    {
    //        string weaponName = weaponInChest.name;
    //        inventoryManager.AddWeaponToInventory(weaponName);
    //    }
    //}

    public void DisplayWeaponInSlot()
    {
        if (weaponInChest != null && equipmentManager != null)
        {
            Sprite itemSprite = weaponInChest.GetComponent<SpriteRenderer>().sprite; // weaponInChest���� �������� ��������Ʈ�� ������
            string itemName = weaponInChest.name;

            if (equipmentManager.itemDatabase.TryGetValue(itemName, out ItemData itemData)) // itemDatabase���� ���� �̸��� �ش��ϴ� ItemData�� ã��
            {
                equipmentManager.ShowWeaponInSlot(itemData.itemName, itemSprite, itemData.itemDescription); // ������ �����Ͱ� ������, ��� ���Կ� ���⸦ ǥ��
            }
            else
            {
                Debug.LogWarning("Weapon data not found for: " + itemName);
            }
        }
        else
        {
            Debug.LogWarning("Weapon in chest or EquipmentManager is not assigned.");
        }
    }
}