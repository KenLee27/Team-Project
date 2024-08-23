using System;
using System.Collections.Generic;
using UnityEngine;

public class SuperCameraController : MonoBehaviour
{
    public Transform player;
    public GameObject lockOnMarker; // Lock On Marker Prefab
    private float rotationSensitive = 3f;
    private float distance = 8f;
    private float rotationMin = 5f;
    private float rotationMax = 80f;
    private float smoothTime = 0.12f;

    private float Xaxis = 0f;
    private float Yaxis = 0f;

    private Transform lockedTarget;
    private List<Transform> enemies = new List<Transform>(); // ������ ���� ����Ʈ
    private bool isLockedOn = false;

    private GameObject currentMarker; // ���� Lock On �ִ� ���� ��Ŀ
    private float lockOnCooldown = 1f; // Lock On ��� �ð�
    private float lastLockOnChangeTime = 0f; // ������ Lock On ���� �ð�

    public bool IsLockedOn => isLockedOn;
    public Transform LockedTarget => lockedTarget;

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isLockedOn = !isLockedOn; // Lock On ���� ���
            if (isLockedOn)
            {
                FindClosestEnemy(); // ���� ����� �� ã��
            }
            else
            {
                lockedTarget = null; // Lock On ����
                DestroyCurrentMarker(); // ���� ��Ŀ ����
            }
        }

        if (isLockedOn && lockedTarget != null)
        {
            float mouseMovement = Input.GetAxis("Mouse X"); // ���콺 X �̵� ����
            float sensitivityAdjustment = 0.5f; // �̵� ���� ����
            mouseMovement *= sensitivityAdjustment; // ���� ����

            // Lock On ��� �ð��� ���� ��쿡�� Lock On Ÿ�� ����
            if (Time.time - lastLockOnChangeTime > lockOnCooldown)
            {
                // ���콺 �̵��� ���� �̻��� ���� Lock On Ÿ�� ����
                if (Mathf.Abs(mouseMovement) > 0.5f) // ���� ���� �� �ּ� �̵��� ����
                {
                    ChangeLockedTarget(mouseMovement);
                    lastLockOnChangeTime = Time.time; // Lock On ���� �ð� ������Ʈ
                }
            }
            UpdateMarkerPosition(); // ��Ŀ ��ġ ������Ʈ
        }

        // �÷��̾�� Ÿ�� ������ �Ÿ��� 30f �̻��� ��� �ڵ����� Lock On ����
        if (isLockedOn && lockedTarget != null)
        {
            float distanceToTarget = (player.position - lockedTarget.position).sqrMagnitude;
            if (distanceToTarget > 30f * 30f) // �Ÿ� �������� ��
            {
                isLockedOn = false;
                lockedTarget = null;
                DestroyCurrentMarker(); // ��Ŀ ����
            }
        }
    }

    void LateUpdate()
    {
        if (lockedTarget != null && isLockedOn)
        {
            // Lock On ���¿����� ī�޶� ���� ����
            Xaxis -= Input.GetAxis("Mouse Y") * rotationSensitive;
            Xaxis = Mathf.Clamp(Xaxis, rotationMin, rotationMax);

            Vector3 direction = lockedTarget.position - player.position;
            Quaternion rotationToFaceEnemy = Quaternion.LookRotation(direction);
            Quaternion targetRotation = Quaternion.Euler(Xaxis, rotationToFaceEnemy.eulerAngles.y, 0);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothTime);
            transform.position = player.position - transform.forward * distance;
        }
        else
        {
            // ����� ���¿����� �⺻ ī�޶� ����
            Yaxis += Input.GetAxis("Mouse X") * rotationSensitive;
            Xaxis -= Input.GetAxis("Mouse Y") * rotationSensitive;
            Xaxis = Mathf.Clamp(Xaxis, rotationMin, rotationMax);

            Quaternion targetRotation = Quaternion.Euler(new Vector3(Xaxis, Yaxis));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothTime);
            transform.position = player.position - transform.forward * distance;
        }
    }

    private void FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        Collider[] hitColliders = Physics.OverlapSphere(player.position, 30f);

        enemies.Clear(); // ����Ʈ �ʱ�ȭ
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                enemies.Add(hitCollider.transform); // �� �߰�
                float currentDistance = (player.position - hitCollider.transform.position).sqrMagnitude;
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    lockedTarget = hitCollider.transform; // ���� ����� ���� Lock On
                }
            }
        }
    }

    private void ChangeLockedTarget(float mouseMovement)
    {
        int currentIndex = enemies.IndexOf(lockedTarget); // ���� Lock On Ÿ�� �ε���
        if (currentIndex < 0) return; // ���� Lock On Ÿ���� ����Ʈ�� ������ ��ȯ

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
            // ��Ŀ�� ������ ����
            currentMarker = Instantiate(lockOnMarker, lockedTarget.position + Vector3.up * 1f, Quaternion.identity); // Y������ 1 ���� ���� ��ġ
        }
        else
        {
            // ��Ŀ�� ��ġ ������Ʈ
            currentMarker.transform.position = lockedTarget.position + Vector3.up * 1f; // Y������ 1 ���� ���� ��ġ
        }
    }

    private void DestroyCurrentMarker()
    {
        if (currentMarker != null)
        {
            Destroy(currentMarker);
            currentMarker = null; // ��Ŀ null�� �ʱ�ȭ
        }
    }
}