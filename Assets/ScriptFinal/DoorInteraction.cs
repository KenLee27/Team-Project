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

        // 상자 근처에 있고 F키를 눌렀다면, 상태 변경
        if (distance <= openDistance && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("문열어");
            if (!isOpen)
            {
                isOpen = true;
            }
        }

        // 상자의 상태에 따라 부드럽게 회전
        if (isOpen && doorRight.transform.localRotation != openRotationRight)
        {
            doorRight.transform.localRotation = Quaternion.Lerp(doorRight.transform.localRotation, openRotationRight, Time.deltaTime * openingSpeed);
            doorLeft.transform.localRotation = Quaternion.Lerp(doorLeft.transform.localRotation, openRotationLeft, Time.deltaTime * openingSpeed);
        }
    }
}