using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public Transform player;

    public GameObject doorRight;
    public GameObject doorLeft;

    public float openDistance = 3f;
    public float openingSpeed = 1.1f;

    private bool isOpen = false;
    private Quaternion openRotationRight;
    private Quaternion openRotationLeft;

    void Start()
    {
        openRotationRight = Quaternion.Euler(0, 180, 0);
        openRotationLeft = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // ���� ��ó�� �ְ� FŰ�� �����ٸ�, ���� ����
        if (distance <= openDistance && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("������");
            if (!isOpen)
            {
                isOpen = true;
            }
        }

        // ������ ���¿� ���� �ε巴�� ȸ��
        if (isOpen && doorRight.transform.localRotation != openRotationRight)
        {
            doorRight.transform.localRotation = Quaternion.Lerp(doorRight.transform.localRotation, openRotationRight, Time.deltaTime * openingSpeed);
            doorLeft.transform.localRotation = Quaternion.Lerp(doorLeft.transform.localRotation, openRotationLeft, Time.deltaTime * openingSpeed);
        }
    }
}