using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public GameObject EquipmentMenu;
    public bool menuActivated = false; // 메뉴가 활성화되었는지 여부를 나타내는 변수

    public ItemSlot[] itemSlot; // 아이템 슬롯 배열

    public Dictionary<string, ItemData> itemDatabase = new Dictionary<string, ItemData>();


    void Start()
    {
        EquipmentMenu.SetActive(false); // 시작 시 메뉴를 비활성화
        Time.timeScale = 1; // 게임이 정상 속도로 실행되도록 설정

        Sprite axeSprite = Resources.Load<Sprite>("Assets/Images/Axe");
        Sprite daggerSprite = Resources.Load<Sprite>("Assets/Images/Dagger");
        Sprite axeRotSprite = Resources.Load<Sprite>("Assets/Images/hammer");
        Sprite falchionSprite = Resources.Load<Sprite>("Assets/Images/Falchion");
        Sprite longswordSprite = Resources.Load<Sprite>("Assets/Images/Longsword");
        Sprite bloodmolarSprite = Resources.Load<Sprite>("Assets/Images/bloodmolar");

        itemDatabase.Add("Axe_Exe", new ItemData("옛 왕의 수호자", "옛 왕의 수호자 무기 설명 추가", axeSprite));
        itemDatabase.Add("Dagger_Red", new ItemData("그림자 단검", "그림자 단검 무기 설명 추가", daggerSprite));
        itemDatabase.Add("Axe_Rot", new ItemData("파괴자", "파괴자 무기 설명 추가", axeRotSprite));
        itemDatabase.Add("Falchion", new ItemData("칼리오스 병사의 곡검", "칼리오스 병사의 곡검 무기 설명 추가", falchionSprite));
        itemDatabase.Add("Longsword_Elite", new ItemData("칼리오스 병사의 대검", "칼리오스 병사의 대검 무기 설명 추가", longswordSprite));
        itemDatabase.Add("Longsword_Blood", new ItemData("피의 어금니", "칼리오스 병사의 대검 무기 설명 추가", bloodmolarSprite));

        LoadItems(); // 아이템 로드

    }

    void Update()
    {
        if (menuActivated && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleEquipmentMenu(); // Q 키로 메뉴 닫기
        }
    }

    public void ToggleEquipmentMenu()
    {
        menuActivated = !menuActivated;
        EquipmentMenu.SetActive(menuActivated);

        if (menuActivated)
        {
            Time.timeScale = 0; // 메뉴가 활성화되면 게임 일시 정지
        }
        else
        {
            Time.timeScale = 1; // 메뉴가 비활성화되면 게임 재개
        }
    }

    public void SaveItems()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull)
            {
                ItemData itemData = itemSlot[i].GetItemData();
                PlayerPrefs.SetString("Item_" + i, JsonUtility.ToJson(itemData));
            }
            else
            {
                PlayerPrefs.DeleteKey("Item_" + i);
            }
        }
        PlayerPrefs.Save();
    }

    public void LoadItems()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            string jsonData = PlayerPrefs.GetString("Item_" + i, null);
            if (!string.IsNullOrEmpty(jsonData))
            {
                ItemData itemData = JsonUtility.FromJson<ItemData>(jsonData);
                itemSlot[i].LoadItem(itemData); // 슬롯에 아이템 로드
            }
        }
    }

    public void AddItem(string itemName, Sprite itemSprite, string itemDescription)
    {
        // 모든 슬롯을 순회하면서 비어 있는 슬롯에 아이템을 추가
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (!itemSlot[i].isFull)
            {
                itemSlot[i].AddItem(itemName, itemSprite, itemDescription);
                SaveItems();
                return;
            }
        }

        Debug.LogWarning("No empty slot available to display the weapon.");
    }

    // 아이템을 슬롯에 표시하는 메서드
    public void ShowWeaponInSlot(string itemName, Sprite itemSprite, string itemDescription)
    {
        // 모든 슬롯을 순회하면서 비어 있는 슬롯에 아이템을 표시
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (!itemSlot[i].isFull)
            {
                itemSlot[i].AddItem(itemName, itemSprite, itemDescription);
                SaveItems();
                return;
            }
        }

        Debug.LogWarning("No empty slot available to display the weapon.");
    }

    // 모든 슬롯의 선택 상태를 해제하는 메서드
    public void DeselectAllSlots()
    {
        // 모든 슬롯을 순회하면서 선택 상태를 해제
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false); // 선택된 슬롯의 쉐이더 비활성화
            itemSlot[i].thisItemSelected = false; // 아이템 선택 상태를 false로 설정
        }
    }
}