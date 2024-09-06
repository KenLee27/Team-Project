using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject axePrefab;
    public GameObject falchionPrefab;
    public GameObject daggerPrefab;

    public Image weaponImage; // 현재 무기 아이콘을 표시할 UI 이미지
    public Sprite axeSprite; // 각 무기의 스프라이트를 설정합니다
    public Sprite falchionSprite;
    public Sprite daggerSprite;
    public string weaponName = "";

    private GameObject[] weaponPrefabs;
    private Sprite[] weaponSprites; // 각 무기의 스프라이트를 저장하는 배열
    private int currentWeaponIndex = 0;
    public GameObject currentWeapon;

    void Start()
    {
        weaponPrefabs = new GameObject[] { axePrefab, falchionPrefab, daggerPrefab };
        weaponSprites = new Sprite[] { axeSprite, falchionSprite, daggerSprite };
        EquipCurrentWeapon();
        weaponName = currentWeapon.name;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchToNextWeapon();
            weaponName = currentWeapon.name;
        }
    }

    private void EquipCurrentWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(weaponPrefabs[currentWeaponIndex].transform.GetChild(0).gameObject);
        currentWeapon.name = weaponPrefabs[currentWeaponIndex].name + "_Instance";

        UpdateWeaponUI();

        SuperPlayerController playerController = FindObjectOfType<SuperPlayerController>();
        if (playerController != null)
        {
            playerController.SetCurrentWeaponType(weaponPrefabs[currentWeaponIndex].name);
        }

        WeaponAttachment attachScript = currentWeapon.GetComponent<WeaponAttachment>();
        if (attachScript == null)
        {
            Debug.LogWarning("WeaponAttachment script not found on " + currentWeapon.name);
        }
    }

    private void SwitchToNextWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponPrefabs.Length;
        EquipCurrentWeapon();
    }

    private void UpdateWeaponUI()
    {
        if (weaponImage != null)
        {
            weaponImage.sprite = weaponSprites[currentWeaponIndex];
        }
    }
}