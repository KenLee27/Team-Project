using UnityEngine;

public class ChestController : MonoBehaviour
{
    public Transform player;  // 플레이어의 Transform
    public GameObject chestTop;  // 상자의 윗부분 GameObject
    public GameObject pickup;  // 라이트가 있는 빈 오브젝트

    public GameObject weaponInChest; // 상자 안에 있는 무기 프리팹
    public InventoryManager inventoryManager; // 플레이어의 인벤토리 매니저

    public float openDistance = 2f;  // 상자를 열 수 있는 최대 거리
    public float openingSpeed = 2f;  // 회전 속도

    private bool isOpen = false; // 상자가 열렸는지 여부
    private bool lightOff = false; // 불이 꺼졌는지 여부
    private Quaternion openRotation;  // 열린 상태의 Quaternion
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
    }

    void Update()
    {
        // 플레이어와 상자 간 거리 계산
        float distance = Vector3.Distance(player.position, transform.position);

        // 상자 근처에 있고 F키를 눌렀다면, 상태 변경
        if (distance <= openDistance && Input.GetKeyDown(KeyCode.F))
        {
            if (!isOpen)
            {
                // 상자를 열고
                isOpen = true;
            }
            else if (!lightOff && isOpen)
            {
                // 불이 아직 꺼지지 않았다면 끄기
                if (pickupLight != null)
                {
                    pickupLight.enabled = false;
                    lightOff = true;
                    ActivateWeaponInInventory(); // 무기 활성화
                }
            }
        }

        // 상자의 상태에 따라 부드럽게 회전
        if (isOpen && chestTop.transform.localRotation != openRotation)
        {
            chestTop.transform.localRotation = Quaternion.Lerp(chestTop.transform.localRotation, openRotation, Time.deltaTime * openingSpeed);
        }
    }

    private void ActivateWeaponInInventory()
    {
        if (inventoryManager != null && weaponInChest != null)
        {
            string weaponName = weaponInChest.name;
            inventoryManager.AddWeaponToInventory(weaponName);
        }
    }
}