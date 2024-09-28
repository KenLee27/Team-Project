using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject axePrefab;
    public GameObject falchionPrefab;
    public GameObject daggerPrefab;
    public GameObject hammerPrefab;

    public Image weaponImage; // 현재 무기 아이콘을 표시할 UI 이미지
    public Image weaponImageLeft; // 이전 무기를 표시할 UI 이미지
    public Image weaponImageRight; // 다음 무기를 표시할 UI 이미지
    public Text weaponNameText; // 현재 무기 이름을 표시할 UI 텍스트

    public Sprite axeSprite;
    public Sprite falchionSprite;
    public Sprite daggerSprite;
    public Sprite hammerSprite;

    private List<GameObject> weaponPrefabs; // List로 변경하여 유연하게 관리
    private List<Sprite> weaponSprites;
    private List<string> weaponNames;

    private int currentWeaponIndex = 0;
    public GameObject currentWeapon;

    private SkillController skillController;    // SkillController 참조


    void Start()
    {
        weaponPrefabs = new List<GameObject> { falchionPrefab }; // 처음에는 Falchion만 포함
        weaponSprites = new List<Sprite> { falchionSprite };
        weaponNames = new List<string> { "칼리오스 병사의 곡검" };

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

        Transform firePoint = currentWeapon.transform.Find("skill_start_position");
        if (firePoint != null)
        {
            skillController.SetFirePoint(firePoint);
        }
        else
        {
            Debug.Log("skill_start_position을 찾을 수 없습니다!");
        }


    }

    private void SwitchToNextWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponPrefabs.Count;
        EquipCurrentWeapon();
    }

    private void SwitchToPreviousWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex - 1 + weaponPrefabs.Count) % weaponPrefabs.Count;
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
            int leftIndex = (currentWeaponIndex - 1 + weaponSprites.Count) % weaponSprites.Count;
            weaponImageLeft.sprite = weaponSprites[leftIndex];
        }

        if (weaponImageRight != null)
        {
            int rightIndex = (currentWeaponIndex + 1) % weaponSprites.Count;
            weaponImageRight.sprite = weaponSprites[rightIndex];
        }

        if (weaponNameText != null)
        {
            weaponNameText.text = weaponNames[currentWeaponIndex]; // 이름 업데이트
        }
    }

    public void AddWeaponToInventory(string weaponName)
    {
        // 한글 이름을 영어 이름으로 매핑
        if (weaponName == "옛 왕의 수호자")
        {
            weaponName = "Axe";
        }
        else if (weaponName == "그림자 단검")
        {
            weaponName = "Dagger";
        }
        else if (weaponName == "파괴자")
        {
            weaponName = "Hammer";
        }


        if (weaponName == "Axe" && !weaponPrefabs.Contains(axePrefab))
        {
            weaponPrefabs.Add(axePrefab);
            weaponSprites.Add(axeSprite);
            weaponNames.Add("옛 왕의 수호자");
        }
        else if (weaponName == "Dagger" && !weaponPrefabs.Contains(daggerPrefab))
        {
            weaponPrefabs.Add(daggerPrefab);
            weaponSprites.Add(daggerSprite);
            weaponNames.Add("그림자 단검");
        }
        else if (weaponName == "Hammer" && !weaponPrefabs.Contains(hammerPrefab))
        {
            weaponPrefabs.Add(hammerPrefab);
            weaponSprites.Add(hammerSprite);
            weaponNames.Add("파괴자");
        }

        UpdateWeaponUI();
        EquipCurrentWeapon();
    }
}