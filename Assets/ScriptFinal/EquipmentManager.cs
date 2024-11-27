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

        itemDatabase.Add("Axe_Exe", new ItemData("옛 왕의 수호자", "종류 : 대형 둔기\r\n공격배수 : 1.55\r\n\r\n황실 수호대장의 전투 도끼이다.\r\n\"그 누구도 황제 폐하께 반기를 든다면 내 도끼가 그들의 머리를 가를 것이다. 설령 그게 악마 새끼일 지라도...\"\r\n- 황실 수호대장 렌스 -", axeSprite));
        itemDatabase.Add("Dagger_Red", new ItemData("그림자 단검", "종류 : 한손 단검\r\n공격배수 : 1.03\r\n\r\n흉악한 자의 피가 깃든 단검이다.\r\n\"칼리오스력 144년, 한 남자가 이웃 42명을 단검 한 자루로 살해한 뒤 붙잡혀 지하 감옥에 감금되었다. 그는 자신이 악마를 처단했다고 주장했다.\r\n- 칼리오스 경비대 일지 -", daggerSprite));
        itemDatabase.Add("Axe_Rot", new ItemData("파괴자", "종류 : 대형 둔기\r\n공격배수 : 1.55\r\n\r\n대악마 카몬의 무기이다.\r\n\"그의 무기가 지면과 맞붙는 소리는 마치 하늘의 우렛소리와도 같아 그 누구도 그의 앞에서 고개를 들 수 없으니...\"\r\n- 칼리오스 금서 일부 기록 -", axeRotSprite));
        itemDatabase.Add("Falchion_Cross", new ItemData("칼리오스 병사의 곡검", "종류 : 한손 장검\r\n공격배수 : 1.25\r\n\r\n칼리오스 병사들이 주로 사용하던 무기이다.", falchionSprite));
        itemDatabase.Add("Longsword_Elite", new ItemData("칼리오스 병사의 대검", "종류 : 한손 장검\r\n공격배수 : 1.50\r\n\r\n칼리오스 정예 병사의 대검이다.\r\n\"황제 폐하께서 너희에게 이 검을 친히 하사하셨다. 너희는 이제 칼리오스의 검이다. 두려움 없이 나아가 황제 폐하의 적을 처단하라...\"\r\n- 황실 수호대장 렌스 -", longswordSprite));
        itemDatabase.Add("Longsword_Blood", new ItemData("피의 어금니", "종류 : 한손 장검\r\n공격배수 : 1.50\r\n\r\n메이지 엘리아스가 간직하던 검이다.\r\n\"칼리오스의 첫번째 반역자 엘리아스는 황실 수호대장 렌스에 의해 국경에서 추방되었다. 언젠가 왕국을 향한 복수의 칼날을 품에 안고서...\"\r\n- 칼리오스 황실 기록서 -", bloodmolarSprite));

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