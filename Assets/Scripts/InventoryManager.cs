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
        weaponPrefabs = new List<GameObject>();
        weaponSprites = new List<Sprite>();
        weaponNames = new List<string>();

        skillController = GetComponent<SkillController>();

        // 게임 처음 실행 시만 초기 무기 추가
        if (!PlayerPrefs.HasKey("IsInitialWeaponAdded"))
        {
            AddInitialWeapon(); // 초기 무기 추가
            PlayerPrefs.SetInt("IsInitialWeaponAdded", 1); // 1을 저장하여 초기 무기가 추가되었음을 표시
            PlayerPrefs.Save();
            SaveInventory();
        }

        // 저장된 인벤토리 데이터 불러오기
        LoadInventory();

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

    private void AddInitialWeapon()
    {
        // 초기 무기를 인벤토리에 추가 (곡검)
        weaponPrefabs.Add(falchionPrefab);
        weaponSprites.Add(falchionSprite);
        weaponNames.Add("칼리오스 병사의 곡검");

        Debug.Log("Initial weapon '칼리오스 병사의 곡검' has been added to the inventory.");
    }


    private void SaveInventory()
    {
        // 무기 이름 목록을 문자열로 저장 (쉼표로 구분)
        string weaponList = string.Join(",", weaponNames);
        PlayerPrefs.SetString("WeaponList", weaponList);

        // 현재 선택된 무기 인덱스 저장
        PlayerPrefs.SetInt("CurrentWeaponIndex", currentWeaponIndex);

        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        // 무기 이름 목록을 불러옵니다.
        if (PlayerPrefs.HasKey("WeaponList"))
        {
            string weaponList = PlayerPrefs.GetString("WeaponList");
            string[] loadedWeaponNames = weaponList.Split(',');

            // 불러온 무기 이름을 기준으로 무기 추가
            foreach (string weaponName in loadedWeaponNames)
            {
                AddWeaponToInventory(weaponName);
            }
        }

        // 현재 무기 인덱스를 불러옵니다.
        if (PlayerPrefs.HasKey("CurrentWeaponIndex"))
        {
            currentWeaponIndex = PlayerPrefs.GetInt("CurrentWeaponIndex");
        }

        EquipCurrentWeapon();
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
        else if(weaponName == "칼리오스 병사의 곡검")
        {
            weaponName = "Falchion";
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
        else if(weaponName == "Falchion" && !weaponPrefabs.Contains(falchionPrefab))
        {
            weaponPrefabs.Add(falchionPrefab); // 처음에는 Falchion만 포함
            weaponSprites.Add(falchionSprite);
            weaponNames.Add("칼리오스 병사의 곡검");
        }

        SaveInventory();
        UpdateWeaponUI();
        EquipCurrentWeapon();
    }

    public void RemoveWeaponFromInventory(string weaponName)
    {
        if (weaponNames.Count <= 1)
        {
            Debug.LogWarning("Cannot remove weapon. At least one weapon must remain in the inventory.");
            return; // 무기 해제 방지
        }

        int index = weaponNames.IndexOf(weaponName);

        if (index >= 0)
        {
            weaponPrefabs.RemoveAt(index);
            weaponSprites.RemoveAt(index);
            weaponNames.RemoveAt(index);

            // 무기 인덱스를 조정하고 UI 업데이트
            currentWeaponIndex = Mathf.Clamp(currentWeaponIndex, 0, weaponPrefabs.Count - 1);

            SaveInventory();
            UpdateWeaponUI();
            EquipCurrentWeapon();
        }
    }

    public bool IsWeaponInInventory(string weaponName)
    {
        return weaponNames.Contains(weaponName);
    }

}