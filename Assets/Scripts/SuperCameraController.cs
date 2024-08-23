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
    private List<Transform> enemies = new List<Transform>(); // 적들을 담을 리스트
    private bool isLockedOn = false;

    private GameObject currentMarker; // 현재 Lock On 있는 적의 마커
    private float lockOnCooldown = 1f; // Lock On 대기 시간
    private float lastLockOnChangeTime = 0f; // 마지막 Lock On 변경 시간

    public bool IsLockedOn => isLockedOn;
    public Transform LockedTarget => lockedTarget;

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isLockedOn = !isLockedOn; // Lock On 상태 토글
            if (isLockedOn)
            {
                FindClosestEnemy(); // 가장 가까운 적 찾기
            }
            else
            {
                lockedTarget = null; // Lock On 해제
                DestroyCurrentMarker(); // 이전 마커 삭제
            }
        }

        if (isLockedOn && lockedTarget != null)
        {
            float mouseMovement = Input.GetAxis("Mouse X"); // 마우스 X 이동 감지
            float sensitivityAdjustment = 0.5f; // 이동 감도 조정
            mouseMovement *= sensitivityAdjustment; // 감도 조정

            // Lock On 대기 시간이 지난 경우에만 Lock On 타겟 변경
            if (Time.time - lastLockOnChangeTime > lockOnCooldown)
            {
                // 마우스 이동이 일정 이상일 때만 Lock On 타겟 변경
                if (Mathf.Abs(mouseMovement) > 0.5f) // 감도 조정 및 최소 이동량 설정
                {
                    ChangeLockedTarget(mouseMovement);
                    lastLockOnChangeTime = Time.time; // Lock On 변경 시간 업데이트
                }
            }
            UpdateMarkerPosition(); // 마커 위치 업데이트
        }

        // 플레이어와 타겟 사이의 거리가 30f 이상일 경우 자동으로 Lock On 해제
        if (isLockedOn && lockedTarget != null)
        {
            float distanceToTarget = (player.position - lockedTarget.position).sqrMagnitude;
            if (distanceToTarget > 30f * 30f) // 거리 제곱으로 비교
            {
                isLockedOn = false;
                lockedTarget = null;
                DestroyCurrentMarker(); // 마커 삭제
            }
        }
    }

    void LateUpdate()
    {
        if (lockedTarget != null && isLockedOn)
        {
            // Lock On 상태에서의 카메라 각도 조정
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
            // 비고정 상태에서의 기본 카메라 조작
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

        enemies.Clear(); // 리스트 초기화
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                enemies.Add(hitCollider.transform); // 적 추가
                float currentDistance = (player.position - hitCollider.transform.position).sqrMagnitude;
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    lockedTarget = hitCollider.transform; // 가장 가까운 적을 Lock On
                }
            }
        }
    }

    private void ChangeLockedTarget(float mouseMovement)
    {
        int currentIndex = enemies.IndexOf(lockedTarget); // 현재 Lock On 타겟 인덱스
        if (currentIndex < 0) return; // 현재 Lock On 타겟이 리스트에 없으면 반환

        // 오른쪽 이동
        if (mouseMovement > 0)
        {
            currentIndex = (currentIndex + 1) % enemies.Count; // 오른쪽 적으로 변경
        }
        // 왼쪽 이동
        else if (mouseMovement < 0)
        {
            currentIndex = (currentIndex - 1 + enemies.Count) % enemies.Count; // 왼쪽 적으로 변경
        }

        // 새로운 Lock On 타겟 설정
        if (lockedTarget != enemies[currentIndex])
        {
            lockedTarget = enemies[currentIndex]; // 새로운 Lock On 타겟으로 변경
            UpdateMarkerPosition(); // 새로운 타겟으로 마커 위치 업데이트
        }
    }

    private void UpdateMarkerPosition()
    {
        if (currentMarker == null)
        {
            // 마커가 없으면 생성
            currentMarker = Instantiate(lockOnMarker, lockedTarget.position + Vector3.up * 1f, Quaternion.identity); // Y축으로 1 단위 위에 위치
        }
        else
        {
            // 마커의 위치 업데이트
            currentMarker.transform.position = lockedTarget.position + Vector3.up * 1f; // Y축으로 1 단위 위에 위치
        }
    }

    private void DestroyCurrentMarker()
    {
        if (currentMarker != null)
        {
            Destroy(currentMarker);
            currentMarker = null; // 마커 null로 초기화
        }
    }
}