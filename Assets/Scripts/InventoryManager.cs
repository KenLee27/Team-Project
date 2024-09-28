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
        weaponPrefabs = new List<GameObject> { falchionPrefab }; // ó������ Falchion�� ����
        weaponSprites = new List<Sprite> { falchionSprite };
        weaponNames = new List<string> { "Į������ ������ ���" };

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
            Debug.Log("skill_start_position�� ã�� �� �����ϴ�!");
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

        UpdateWeaponUI();
        EquipCurrentWeapon();
    }
}