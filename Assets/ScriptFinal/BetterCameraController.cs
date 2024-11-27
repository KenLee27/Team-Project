using System;
using UnityEditor;
using UnityEngine;

public class BetterCameraController : MonoBehaviour
{
    public Transform player;
    private float rotationSensitive = 3f;
    private float distance = 8f;
    private float rotationMin = 5f;
    private float rotationMax = 80f;
    private float smoothTime = 0.12f;

    private float Xaxis = 0f;
    private float Yaxis = 0f;

    private Transform lockedTarget;

    private bool isLockedOn = false;

    public bool IsLockedOn => isLockedOn;
    public Transform LockedTarget => lockedTarget;

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isLockedOn = !isLockedOn; // Lock On ���� ���
            if (isLockedOn)
            {
                float closestDistance = Mathf.Infinity;
                Collider[] hitColliders = Physics.OverlapSphere(player.position, 30f);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Enemy"))
                    {
                        float currentDistance = (player.position - hitCollider.transform.position).sqrMagnitude;
                        if (currentDistance < closestDistance)
                        {
                            closestDistance = currentDistance;
                            lockedTarget = hitCollider.transform;
                        }
                    }
                }
            }
            else
            {
                lockedTarget = null; // Lock On ����
            }
        }

        // �÷��̾�� Ÿ�� ������ �Ÿ��� 30f �̻��� ��� �ڵ����� Lock On ����
        if (isLockedOn && lockedTarget != null)
        {
            float distanceToTarget = (player.position - lockedTarget.position).sqrMagnitude;
            if (distanceToTarget > 30f * 30f) // �Ÿ� �������� ��
            {
                isLockedOn = false;
                lockedTarget = null;
            }
        }
    }

    void LateUpdate()
    {
        if (lockedTarget != null && isLockedOn)
        {
            // Lock On ���¿��� ī�޶� ���� ����
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
}
