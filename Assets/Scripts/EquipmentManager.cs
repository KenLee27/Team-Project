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


    void Start()
    {
        EquipmentMenu.SetActive(false); // ���� �� �޴��� ��Ȱ��ȭ
        Time.timeScale = 1; // ������ ���� �ӵ��� ����ǵ��� ����

        itemDatabase.Add("Axe_Exe", new ItemData("�� ���� ��ȣ��", "�� ���� ��ȣ�� ���� ���� �߰�"));
        itemDatabase.Add("Dagger_Red", new ItemData("�׸��� �ܰ�", "�׸��� �ܰ� ���� ���� �߰�"));
        itemDatabase.Add("Axe_Rot", new ItemData("�ı���", "�ı��� ���� ���� �߰�"));
        // itemDatabase.Add("Falchion", new WeaponData("Į������ ������ ���", "Į������ ������ ��� ���� ���� �߰�"));

    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        Debug.Log("E key pressed");
    //        Cursor.visible = true;  // ��ư Ŭ�� �� Ŀ�� ���̱�
    //        Cursor.lockState = CursorLockMode.None; // Ŀ�� ��� ����


    //        menuActivated = !menuActivated; 
    //        EquipmentMenu.SetActive(menuActivated); 

    //        if (menuActivated)
    //        {
    //            Time.timeScale = 0; // �޴��� Ȱ��ȭ�Ǹ� ���� �Ͻ� ����
    //        }
    //        else
    //        {
    //            Time.timeScale = 1; // �޴��� ��Ȱ��ȭ�Ǹ� ���� �簳
    //        }
    //    }
    //}

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