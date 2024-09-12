using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperCameraController : MonoBehaviour
{
    public Transform player;                       // �÷��̾��� Transform
    public GameObject lockOnMarker;

    private float rotationSensitive = 3f;          // ī�޶� ȸ�� ����
    private float distance = 5f;                   // ī�޶�-�÷��̾� �Ÿ�
    private float minDistance = 0.8f;                // �ּ� �Ÿ�
    private float rotationMin = -45f;                // ī�޶� X�� ȸ�� ����
    private float rotationMax = 80f;               // ī�޶� X�� ȸ�� ����
    private float smoothTime = 0.12f;              // ī�޶� ȸ�� ���� ���
    //private float smoothMove = 0.2f;

    private float Xaxis = 0f;                      // ī�޶� X�� ȸ�� ����
    private float Yaxis = 0f;                      // ī�޶� Y�� ȸ�� ����

    private float previousYaxis;
    private float previousXaxis;

    private Transform lockedTarget;                 // Lock On ���
    private List<Transform> enemies = new List<Transform>(); // Lock On�� �� ����Ʈ
    private bool isLockedOn = false;                // Lock On ���� ����
    private bool wasLockedOn = false;

    private GameObject currentMarker;               // ���� Lock On ��Ŀ ��ü
    private float lockOnCooldown = 1f;              // Lock On �纯�� ��Ÿ��
    private float lastLockOnChangeTime = 0f;       // ������ Lock On ���� ��û �ð�

    //private float timeSinceHit = 0f;

    public bool IsLockedOn => isLockedOn;
    public Transform LockedTarget => lockedTarget;
    private Vector3 velocity = Vector3.zero;


    public float minSmoothMove = 0f; // �ּ� smoothMove ��
    public float maxSmoothMove = 0.2f; // �ִ� smoothMove ��
    public float smoothChangeTime = 0.1f; // smoothMove ���� ��ȭ�� �ҿ�Ǵ� �ð�
    private float currentSmoothMove; // ���� smoothMove ��



    void Update()
    {
        if (Input.GetMouseButtonDown(2))    // �� ��ư Ŭ�� �� Lock On ����
        {
            isLockedOn = !isLockedOn;       // Lock On ���� ���
            if (isLockedOn)
            {
                FindClosestEnemy();         // 30f �̳��� ��� �� ����Ʈ�� ����
            }
            else
            {
                lockedTarget = null;        // Lock On ����
                DestroyCurrentMarker();     // ��Ŀ ����
            }
        }

        if (isLockedOn && lockedTarget != null)                     // Lock On ���� �� ���콺 �¿� �̵����� Ÿ�� ����
        {
            float mouseMovement = Input.GetAxis("Mouse X");         // ���콺 X�� �̵� ����
            float sensitivityAdjustment = 0.5f;                     // �̵� ���� ����
            mouseMovement *= sensitivityAdjustment;                 // ���� ����

            if (Time.time - lastLockOnChangeTime > lockOnCooldown)  // Lock On �纯�� ��Ÿ�� ���� �� Ÿ�� ����
            {
                if (Mathf.Abs(mouseMovement) > 0.5f)                // ���콺 �ּ� �̵��� ����
                {
                    ChangeLockedTarget(mouseMovement);              // Lock On Ÿ�� ����
                    lastLockOnChangeTime = Time.time;               // Lock On ���� ��û �ð� �ʱ�ȭ
                }
            }
            UpdateMarkerPosition();                                 // ��Ŀ ������Ʈ
        }

        if (isLockedOn && lockedTarget != null)                                                 // Lock On ���� �� �ν� �Ÿ� ��Ż �� �ڵ� ����
        {
            float distanceToTarget = (player.position - lockedTarget.position).sqrMagnitude;    // Lock On Ÿ�ٰ� �÷��̾� ������ �Ÿ�
            if (distanceToTarget > 30f * 30f)                                                   // �Ÿ� �������� ��
            {
                isLockedOn = false;     // Lock On ����
                lockedTarget = null;    // Lock On Ÿ�� Null
                DestroyCurrentMarker(); // ��Ŀ ����
            }
        }


        if (lockedTarget == null)                                                 // Lock On ���� �� �ν� �Ÿ� ��Ż �� �ڵ� ����
        {
            DestroyCurrentMarker(); // ��Ŀ ����
        }


        // �浹 �˻� �� ī�޶� ��ġ ����
        //HandleCameraCollision(); // ��ֹ� ���� �� �Ÿ� ����
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position - transform.forward * distance; // �⺻ ī�޶� ��ġ ����
        RaycastHit hit;
        //Vector3 targetPosition;
        Quaternion targetRotation;

        // Raycast�� �̿��Ͽ� Ground�� �浹 �˻�
        if (Physics.Raycast(player.position, -transform.forward, out hit, 5f)) // �÷��̾� ��ġ���� �Ʒ��� Raycast
        {
            if (hit.collider.CompareTag("Ground"))
            {
                // �浹 �� distance ���� 2f�� ���������� �ٿ�����
                distance = Mathf.Lerp(distance, Mathf.Max(Vector3.Distance(player.position, hit.point), minDistance), Time.deltaTime * 5f); // �Ÿ� ����
            }
        }
        else
        {
            distance = Mathf.Lerp(distance, 5f, Time.deltaTime * 5f); // �Ÿ� ����
        }


        if (lockedTarget != null && isLockedOn) // Lock On ���¿��� ī�޶� ����
        {
            Vector3 direction = lockedTarget.position - player.position; // Lock On Ÿ�ٰ��� ���⺤��
            Quaternion rotationToFaceEnemy = Quaternion.LookRotation(direction); // Lock On Ÿ������ ȸ�� ��� ����
            targetRotation = Quaternion.Euler(Xaxis, rotationToFaceEnemy.eulerAngles.y, 0); // Lock On Ÿ������ ��ġ ��� ����


            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothTime); // Lock On Ÿ������ �ε巴�� ȸ��
            //targetPosition = player.position - transform.forward * distance + Vector3.up * 1.3f;
            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime); //�ε巯�� ī�޶� �̵�
            transform.position = player.position - transform.forward * distance + Vector3.up * 1.3f; //�ﰢ���� ī�޶� �̵�

            previousYaxis = rotationToFaceEnemy.eulerAngles.y;
            previousXaxis = Xaxis;

            wasLockedOn = true; // ���� �������� ���

        }
        else // Lock On �̽ǽ� ���¿��� ī�޶� ����
        {
            if (wasLockedOn) // ���� ���¿��� �����Ǿ��� ��
            {
                // ����� Yaxis�� Xaxis ���� ����Ͽ� �ε巴�� ��ȯ
                Yaxis = previousYaxis;
                Xaxis = previousXaxis;
                wasLockedOn = false; // ���� ��ȯ ���
            }
            else // �⺻ ���¿��� �������� ���콺 �̵��� ���� ����
            {
                Yaxis += Input.GetAxis("Mouse X") * rotationSensitive;
                Xaxis -= Input.GetAxis("Mouse Y") * rotationSensitive;
                Xaxis = Mathf.Clamp(Xaxis, rotationMin, rotationMax);
            }

            targetRotation = Quaternion.Euler(new Vector3(Xaxis, Yaxis));
            transform.rotation = targetRotation;
            transform.position = player.position - transform.forward * distance + Vector3.up * 1.3f;
        }
    }

    private void HandleCameraCollision()
    {
        Vector3 desiredPosition = player.position - transform.forward * distance; // �⺻ ī�޶� ��ġ ����
        RaycastHit hit;

        // Raycast�� �̿��Ͽ� Ground�� �浹 �˻�
        if (Physics.Raycast(player.position, -transform.forward, out hit, distance)) // �÷��̾� ��ġ���� �Ʒ��� Raycast
        {
            if (hit.collider.CompareTag("Ground"))
            {
                // �浹 �� distance ���� 2f�� ���������� �ٿ�����
                distance = Mathf.Lerp(distance, minDistance, Time.deltaTime * 5f); // �Ÿ� ����
            }
        }
        else
        {
            distance = Mathf.Lerp(distance, 7f, Time.deltaTime * 5f); // �Ÿ� ����
        }

        // ���� ī�޶� ��ġ ����
        transform.position = player.position - transform.forward * distance; // �÷��̾� �������� ī�޶� ��ġ ����
    }

    private void RotatePlayerTowardsLockedTarget()
    {
        if (lockedTarget != null)
        {
            Vector3 direction = lockedTarget.position - player.position;
            direction.y = 0; // Y�� ���� ����
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction); // �� �������� ȸ��
                player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * rotationSensitive); // �ε巯�� ȸ��
            }
        }
    }

    private void FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity; // ���� ����� ������ �Ÿ� �ʱ�ȭ
        Collider[] hitColliders = Physics.OverlapSphere(player.position, 30f); // �÷��̾� �ֺ��� �� Ž��

        enemies.Clear(); // ����Ʈ �ʱ�ȭ
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy")) // �� �±� Ȯ��
            {
                enemies.Add(hitCollider.transform); // �� �߰�
                float currentDistance = (player.position - hitCollider.transform.position).sqrMagnitude; // ���� �Ÿ� ���
                if (currentDistance < closestDistance) // ���� ����� �� ã��
                {
                    closestDistance = currentDistance;
                    lockedTarget = hitCollider.transform; // Lock On�� �� ����
                }
            }
        }
    }

    private void ChangeLockedTarget(float mouseMovement)
    {
        int currentIndex = enemies.IndexOf(lockedTarget); // ���� Lock On Ÿ�� �ε���
        if (currentIndex < 0) return; // ���� Lock On Ÿ���� ������ ��ȯ

        // ������ �̵�
        if (mouseMovement > 0)
        {
            currentIndex = (currentIndex + 1) % enemies.Count; // ������ ������ ����
        }
        // ���� �̵�
        else if (mouseMovement < 0)
        {
            currentIndex = (currentIndex - 1 + enemies.Count) % enemies.Count; // ���� ������ ����
        }

        // ���ο� Lock On Ÿ�� ����
        if (lockedTarget != enemies[currentIndex])
        {
            lockedTarget = enemies[currentIndex]; // ���ο� Lock On Ÿ������ ����
            UpdateMarkerPosition(); // ���ο� Ÿ������ ��Ŀ ��ġ ������Ʈ
        }
    }

    private void UpdateMarkerPosition()
    {
        if (currentMarker == null)
        {
            currentMarker = Instantiate(lockOnMarker, GameObject.Find("Canvas").transform); // Canvas�� �θ�� ����

            currentMarker.gameObject.SetActive(true); // ������ ��Ŀ�� Ȱ��ȭ
        }

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(lockedTarget.position);
        currentMarker.transform.position = screenPosition; // ��Ŀ ��ġ ������Ʈ
    }

    private void DestroyCurrentMarker()
    {
        if (currentMarker != null)
        {
            //Destroy(currentMarker);
            Destroy(currentMarker.gameObject);
            currentMarker = null; // ��Ŀ �ʱ�ȭ
        }
    }
}