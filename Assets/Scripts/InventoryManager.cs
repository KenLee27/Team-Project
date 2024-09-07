using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject axePrefab;
    public GameObject falchionPrefab;
    public GameObject daggerPrefab;

    public Image weaponImage; // ���� ���� �������� ǥ���� UI �̹���
    public Image weaponImageLeft; // ���� ���⸦ ǥ���� UI �̹���
    public Image weaponImageRight; // ���� ���⸦ ǥ���� UI �̹���
    public Text weaponNameText; // ���� ���� �̸��� ǥ���� UI �ؽ�Ʈ

    public Sprite axeSprite;
    public Sprite falchionSprite;
    public Sprite daggerSprite;

    private GameObject[] weaponPrefabs;
    private Sprite[] weaponSprites;
    private string[] weaponNames; // �ѱ� ���� �̸� �迭
    private int currentWeaponIndex = 0;
    public GameObject currentWeapon;

    private SkillController skillController;    // SkillController ����


    void Start()
    {
        weaponPrefabs = new GameObject[] { axePrefab, falchionPrefab, daggerPrefab };
        weaponSprites = new Sprite[] { axeSprite, falchionSprite, daggerSprite };
        weaponNames = new string[] { "�� ���� ��ȣ��", "Į������ ������ ���", "�׸��� �ܰ�" };
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
                Debug.LogWarning("skill_start_position�� ã�� �� �����ϴ�!");
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
            weaponNameText.text = weaponNames[currentWeaponIndex]; // �̸� ������Ʈ
        }
    }
}