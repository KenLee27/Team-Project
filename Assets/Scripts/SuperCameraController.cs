using System;
using System.Collections.Generic;
using UnityEngine;

public class SuperCameraController : MonoBehaviour
{
    public Transform player;
    public GameObject lockOnMarker;

    private float rotationSensitive = 3f;   //ī�޶� ȸ������
    private float distance = 8f;            //ī�޶�-�÷��̾� �Ÿ�
    private float rotationMin = 5f;         //ī�޶� X�� ȸ�� ����
    private float rotationMax = 80f;        //ī�޶� X�� ȸ�� ����
    private float smoothTime = 0.12f;       //ī�޶� ȸ�� ���� ���

    private float Xaxis = 0f;
    private float Yaxis = 0f;

    private Transform lockedTarget;                             //Lock On ��� ��ü
    private List<Transform> enemies = new List<Transform>();    //Lock On ��� ����Ʈ
    private bool isLockedOn = false;                            //Lock On ���� ����

    private GameObject currentMarker;           // ���� Lock On ���
    private float lockOnCooldown = 1f;          // Lock On �纯�� ��Ÿ��
    private float lastLockOnChangeTime = 0f;    // Lock On ���� ��û �ð�

    public bool IsLockedOn => isLockedOn;
    public Transform LockedTarget => lockedTarget;

    void Update()
    {
        if (Input.GetMouseButtonDown(2))    //�� ��ư ������ Lock On ����
        {
            isLockedOn = !isLockedOn;       //Lock On ���� ���
            if (isLockedOn)
            {
                FindClosestEnemy();         //30f �̳��� ��� �� ����Ʈ�� ����
            }
            else
            {
                lockedTarget = null;        //Lock On ����
                DestroyCurrentMarker();     //��Ŀ ����
            }
        }
            
        if (isLockedOn && lockedTarget != null)                     //Lock On ������ ���콺 �¿� �̵����� Ÿ�� ����
        {
            float mouseMovement = Input.GetAxis("Mouse X");         //���콺 X�� �̵� ����
            float sensitivityAdjustment = 0.5f;                     //�̵� ���� ����
            mouseMovement *= sensitivityAdjustment;                 //���� ����
                
            if (Time.time - lastLockOnChangeTime > lockOnCooldown)  //Lock On �纯�� ��Ÿ�� ������ Ÿ�� ����
            {
                if (Mathf.Abs(mouseMovement) > 0.5f)                //���콺 �ּ� �̵��� ����
                {
                    ChangeLockedTarget(mouseMovement);              //Lock On Ÿ�� ����
                    lastLockOnChangeTime = Time.time;               //Lock On ���� ��û �ð� �ʱ�ȭ
                }
            }
            UpdateMarkerPosition();                                 //��Ŀ ������Ʈ
        }

        if (isLockedOn && lockedTarget != null)                                                 //Lock On ������ �ν� �Ÿ� ��Ż �� �ڵ� ����
        {
            float distanceToTarget = (player.position - lockedTarget.position).sqrMagnitude;    //Lock On Ÿ�ٰ� �÷��̾� ������ �Ÿ�
            if (distanceToTarget > 30f * 30f)                                                   //�Ÿ� �������� ��
            {
                isLockedOn = false;     //Lock On ����
                lockedTarget = null;    //Lock On Ÿ�� Null
                DestroyCurrentMarker(); //��Ŀ ����
            }
        }
    }

    void LateUpdate()
    {
        if (lockedTarget != null && isLockedOn)     //Lock On ���¿��� ī�޶� ����
        {
            Xaxis -= Input.GetAxis("Mouse Y") * rotationSensitive;
            Xaxis = Mathf.Clamp(Xaxis, rotationMin, rotationMax);

            Vector3 direction = lockedTarget.position - player.position;                                //Lock On Ÿ�ٰ��� ���⺤��
            Quaternion rotationToFaceEnemy = Quaternion.LookRotation(direction);                        //Lock On Ÿ������ ȸ�� ��� ����
            Quaternion targetRotation = Quaternion.Euler(Xaxis, rotationToFaceEnemy.eulerAngles.y, 0);  //Lock On Ÿ������ ��ġ ��� ����

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothTime);       //Lock On Ÿ������ �ε巴�� ȸ��
            transform.position = player.position - transform.forward * distance;                        //ī�޶� ��ġ ����
        }
        else                            //Lock On �̽ǽ� ���¿��� ī�޶� ����
        {
            Yaxis += Input.GetAxis("Mouse X") * rotationSensitive;
            Xaxis -= Input.GetAxis("Mouse Y") * rotationSensitive;
            Xaxis = Mathf.Clamp(Xaxis, rotationMin, rotationMax);

            Quaternion targetRotation = Quaternion.Euler(new Vector3(Xaxis, Yaxis));                //ȸ�� ��� ����
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothTime);   //ī�޶� �ε巴�� ȸ��
            transform.position = player.position - transform.forward * distance;                    //ī�޶� ��ġ ����
        }
    }

    private void FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        Collider[] hitColliders = Physics.OverlapSphere(player.position, 30f);

        enemies.Clear();                                    //����Ʈ �ʱ�ȭ
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                enemies.Add(hitCollider.transform);         //�� �߰�
                float currentDistance = (player.position - hitCollider.transform.position).sqrMagnitude;
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    lockedTarget = hitCollider.transform;   //���� ����� ���� Lock On
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