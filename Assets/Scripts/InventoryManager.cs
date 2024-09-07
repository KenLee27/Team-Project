using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject axePrefab;
    public GameObject falchionPrefab;
    public GameObject daggerPrefab;

    public Image weaponImage; // 현재 무기 아이콘을 표시할 UI 이미지
    public Image weaponImageLeft; // 이전 무기를 표시할 UI 이미지
    public Image weaponImageRight; // 다음 무기를 표시할 UI 이미지
    public Text weaponNameText; // 현재 무기 이름을 표시할 UI 텍스트

    public Sprite axeSprite;
    public Sprite falchionSprite;
    public Sprite daggerSprite;

    private GameObject[] weaponPrefabs;
    private Sprite[] weaponSprites;
    private string[] weaponNames; // 한글 무기 이름 배열
    private int currentWeaponIndex = 0;
    public GameObject currentWeapon;

    private SkillController skillController;    // SkillController 참조


    void Start()
    {
        weaponPrefabs = new GameObject[] { axePrefab, falchionPrefab, daggerPrefab };
        weaponSprites = new Sprite[] { axeSprite, falchionSprite, daggerSprite };
        weaponNames = new string[] { "옛 왕의 수호자", "칼리오스 병사의 곡검", "그림자 단검" };
        skillController = GetComponent<SkillController>();

        EquipCurrentWeapon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchToNextWeapon();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchToPreviousWeapon();
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

        if(currentWeaponIndex == 2)
        {
            Transform firePoint = currentWeapon.transform.Find("skill_start_position");
            if (firePoint != null)
            {
                skillController.SetFirePoint(firePoint);
            }
            else
            {
                Debug.LogWarning("skill_start_position을 찾을 수 없습니다!");
            }
        }

    }

    private void SwitchToNextWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponPrefabs.Length;
        EquipCurrentWeapon();
    }

    private void SwitchToPreviousWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex - 1 + weaponPrefabs.Length) % weaponPrefabs.Length;
        EquipCurrentWeapon();
    }

    private void UpdateWeaponUI()
    {
        if (weaponImage != null)
        {
            weaponImage.sprite = weaponSprites[currentWeaponIndex];
        }

        if (weaponImageLeft != null)
        {
            int leftIndex = (currentWeaponIndex - 1 + weaponSprites.Length) % weaponSprites.Length;
            weaponImageLeft.sprite = weaponSprites[leftIndex];
        }

        if (weaponImageRight != null)
        {
            int rightIndex = (currentWeaponIndex + 1) % weaponSprites.Length;
            weaponImageRight.sprite = weaponSprites[rightIndex];
        }

        if (weaponNameText != null)
        {
            weaponNameText.text = weaponNames[currentWeaponIndex]; // 이름 업데이트
        }
    }
}