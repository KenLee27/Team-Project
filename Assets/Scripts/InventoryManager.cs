using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject axePrefab;
    public GameObject falchionPrefab;
    public GameObject daggerPrefab;
    public GameObject hammerPrefab;

    public Image weaponImage; // ���� ���� �������� ǥ���� UI �̹���
    public Image weaponImageLeft; // ���� ���⸦ ǥ���� UI �̹���
    public Image weaponImageRight; // ���� ���⸦ ǥ���� UI �̹���
    public Text weaponNameText; // ���� ���� �̸��� ǥ���� UI �ؽ�Ʈ

    public Sprite axeSprite;
    public Sprite falchionSprite;
    public Sprite daggerSprite;
    public Sprite hammerSprite;

    private List<GameObject> weaponPrefabs; // List�� �����Ͽ� �����ϰ� ����
    private List<Sprite> weaponSprites;
    private List<string> weaponNames;

    private int currentWeaponIndex = 0;
    public GameObject currentWeapon;

    private SkillController skillController;    // SkillController ����


    void Start()
    {
        weaponPrefabs = new List<GameObject>();
        weaponSprites = new List<Sprite>();
        weaponNames = new List<string>();

        skillController = GetComponent<SkillController>();

        // ���� ó�� ���� �ø� �ʱ� ���� �߰�
        if (!PlayerPrefs.HasKey("IsInitialWeaponAdded"))
        {
            AddInitialWeapon(); // �ʱ� ���� �߰�
            PlayerPrefs.SetInt("IsInitialWeaponAdded", 1); // 1�� �����Ͽ� �ʱ� ���Ⱑ �߰��Ǿ����� ǥ��
            PlayerPrefs.Save();
            SaveInventory();
        }

        // ����� �κ��丮 ������ �ҷ�����
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
        // �ʱ� ���⸦ �κ��丮�� �߰� (���)
        weaponPrefabs.Add(falchionPrefab);
        weaponSprites.Add(falchionSprite);
        weaponNames.Add("Į������ ������ ���");

        Debug.Log("Initial weapon 'Į������ ������ ���' has been added to the inventory.");
    }


    private void SaveInventory()
    {
        // ���� �̸� ����� ���ڿ��� ���� (��ǥ�� ����)
        string weaponList = string.Join(",", weaponNames);
        PlayerPrefs.SetString("WeaponList", weaponList);

        // ���� ���õ� ���� �ε��� ����
        PlayerPrefs.SetInt("CurrentWeaponIndex", currentWeaponIndex);

        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        // ���� �̸� ����� �ҷ��ɴϴ�.
        if (PlayerPrefs.HasKey("WeaponList"))
        {
            string weaponList = PlayerPrefs.GetString("WeaponList");
            string[] loadedWeaponNames = weaponList.Split(',');

            // �ҷ��� ���� �̸��� �������� ���� �߰�
            foreach (string weaponName in loadedWeaponNames)
            {
                AddWeaponToInventory(weaponName);
            }
        }

        // ���� ���� �ε����� �ҷ��ɴϴ�.
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
            weaponNameText.text = weaponNames[currentWeaponIndex]; // �̸� ������Ʈ
        }
    }

    public void AddWeaponToInventory(string weaponName)
    {
        // �ѱ� �̸��� ���� �̸����� ����
        if (weaponName == "�� ���� ��ȣ��")
        {
            weaponName = "Axe";
        }
        else if (weaponName == "�׸��� �ܰ�")
        {
            weaponName = "Dagger";
        }
        else if (weaponName == "�ı���")
        {
            weaponName = "Hammer";
        }
        else if(weaponName == "Į������ ������ ���")
        {
            weaponName = "Falchion";
        }


        if (weaponName == "Axe" && !weaponPrefabs.Contains(axePrefab))
        {
            weaponPrefabs.Add(axePrefab);
            weaponSprites.Add(axeSprite);
            weaponNames.Add("�� ���� ��ȣ��");
        }
        else if (weaponName == "Dagger" && !weaponPrefabs.Contains(daggerPrefab))
        {
            weaponPrefabs.Add(daggerPrefab);
            weaponSprites.Add(daggerSprite);
            weaponNames.Add("�׸��� �ܰ�");
        }
        else if (weaponName == "Hammer" && !weaponPrefabs.Contains(hammerPrefab))
        {
            weaponPrefabs.Add(hammerPrefab);
            weaponSprites.Add(hammerSprite);
            weaponNames.Add("�ı���");
        }
        else if(weaponName == "Falchion" && !weaponPrefabs.Contains(falchionPrefab))
        {
            weaponPrefabs.Add(falchionPrefab); // ó������ Falchion�� ����
            weaponSprites.Add(falchionSprite);
            weaponNames.Add("Į������ ������ ���");
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
            return; // ���� ���� ����
        }

        int index = weaponNames.IndexOf(weaponName);

        if (index >= 0)
        {
            weaponPrefabs.RemoveAt(index);
            weaponSprites.RemoveAt(index);
            weaponNames.RemoveAt(index);

            // ���� �ε����� �����ϰ� UI ������Ʈ
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