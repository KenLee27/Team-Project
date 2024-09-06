using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject axePrefab;
    public GameObject falchionPrefab;
    public GameObject daggerPrefab;

    private GameObject[] weaponPrefabs;
    private int currentWeaponIndex = 0;
    private GameObject currentWeapon;

    void Start()
    {
        weaponPrefabs = new GameObject[] { axePrefab, falchionPrefab, daggerPrefab };
        EquipCurrentWeapon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchToNextWeapon();
        }
    }

    private void EquipCurrentWeapon()
    {
        if (currentWeapon != null)
        {
            Debug.Log("Destroying: " + currentWeapon.name);
            Destroy(currentWeapon);
        }

        // 올바른 자식 오브젝트 인스턴스화(기본적으로 prefab 계층 구조에서 첫번째 자식 사용)
        GameObject weaponToInstantiate = weaponPrefabs[currentWeaponIndex].transform.GetChild(0).gameObject;

        currentWeapon = Instantiate(weaponToInstantiate);

        currentWeapon.name = weaponPrefabs[currentWeaponIndex].name + "_Instance";

        WeaponAttachment attachScript = currentWeapon.GetComponent<WeaponAttachment>();
        if (attachScript == null)
        {
            Debug.LogWarning("WeaponAttachment script not found on " + currentWeapon.name);
        }
        else
        {
            Debug.Log("WeaponAttachment script successfully attached to " + currentWeapon.name);
        }
    }

    private void SwitchToNextWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponPrefabs.Length;
        EquipCurrentWeapon();
    }
}