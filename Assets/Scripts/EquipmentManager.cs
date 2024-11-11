using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public GameObject EquipmentMenu;
    public bool menuActivated = false; // �޴��� Ȱ��ȭ�Ǿ����� ���θ� ��Ÿ���� ����

    public ItemSlot[] itemSlot; // ������ ���� �迭

    public Dictionary<string, ItemData> itemDatabase = new Dictionary<string, ItemData>();


    public AudioSource audioSource; // AudioSource ������Ʈ
    public AudioClip menuCloseSound; // �޴� �ݱ� ȿ����

    public GameObject mainMenuCanvas; // ���� �޴� ĵ����


    void Start()
    {

        if (mainMenuCanvas != null && !mainMenuCanvas.activeSelf)
        {
            EquipmentMenu.SetActive(false); // ���� �� �޴��� ��Ȱ��ȭ
            Time.timeScale = 1; // ������ ���� �ӵ��� ����ǵ��� ����


        Sprite axeSprite = Resources.Load<Sprite>("Assets/Images/Axe");
        Sprite daggerSprite = Resources.Load<Sprite>("Assets/Images/Dagger");
        Sprite axeRotSprite = Resources.Load<Sprite>("Assets/Images/hammer");
        Sprite falchionSprite = Resources.Load<Sprite>("Assets/Images/Falchion");

        itemDatabase.Add("Axe_Exe", new ItemData("�� ���� ��ȣ��", "�� ���� ��ȣ�� ���� ���� �߰�", axeSprite));
        itemDatabase.Add("Dagger_Red", new ItemData("�׸��� �ܰ�", "�׸��� �ܰ� ���� ���� �߰�", daggerSprite));
        itemDatabase.Add("Axe_Rot", new ItemData("�ı���", "�ı��� ���� ���� �߰�", axeRotSprite));
        itemDatabase.Add("Falchion", new ItemData("Į������ ������ ���", "Į������ ������ ��� ���� ���� �߰�", falchionSprite));

        LoadItems(); // ������ �ε�
        }

        else
        {
            Time.timeScale = 0; // mainMenuCanvas�� Ȱ��ȭ�Ǿ� ���� �� ���� �Ͻ� ����
        }

    }

    void Update()
    {
        if (menuActivated && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleEquipmentMenu(); // Q Ű�� �޴� �ݱ�
        }
    }

    public void ToggleEquipmentMenu()
    {
        menuActivated = !menuActivated;
        EquipmentMenu.SetActive(menuActivated);

        if (menuActivated)
        {
           Time.timeScale = 0; // �޴��� Ȱ��ȭ�Ǹ� ���� �Ͻ� ����
        }
        else
        {
            Time.timeScale = 1; // �޴��� ��Ȱ��ȭ�Ǹ� ���� �簳
            PlayMenuCloseSound(); // �޴� �ݱ� ȿ���� ���
        }
    }

    private void PlayMenuCloseSound()
    {
        if (menuCloseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(menuCloseSound);
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
                itemSlot[i].LoadItem(itemData); // ���Կ� ������ �ε�
            }
        }
    }

    public void AddItem(string itemName, Sprite itemSprite, string itemDescription)
    {
        // ��� ������ ��ȸ�ϸ鼭 ��� �ִ� ���Կ� �������� �߰�
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

    // �������� ���Կ� ǥ���ϴ� �޼���
    public void ShowWeaponInSlot(string itemName, Sprite itemSprite, string itemDescription)
    {
        // ��� ������ ��ȸ�ϸ鼭 ��� �ִ� ���Կ� �������� ǥ��
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

    // ��� ������ ���� ���¸� �����ϴ� �޼���
    public void DeselectAllSlots()
    {
        // ��� ������ ��ȸ�ϸ鼭 ���� ���¸� ����
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false); // ���õ� ������ ���̴� ��Ȱ��ȭ
            itemSlot[i].thisItemSelected = false; // ������ ���� ���¸� false�� ����
        }
    }
}