using System;
using System.Collections.Generic;
using UnityEngine;

public class SuperCameraController : MonoBehaviour
{
    public Transform player;
    public GameObject lockOnMarker;

    private float rotationSensitive = 3f;   //카메라 회전감도
    private float distance = 8f;            //카메라-플레이어 거리
    private float rotationMin = 5f;         //카메라 X축 회전 하한
    private float rotationMax = 80f;        //카메라 X축 회전 상한
    private float smoothTime = 0.12f;       //카메라 회전 지연 계수

    private float Xaxis = 0f;
    private float Yaxis = 0f;

    private Transform lockedTarget;                             //Lock On 대상 객체
    private List<Transform> enemies = new List<Transform>();    //Lock On 대상 리스트
    private bool isLockedOn = false;                            //Lock On 실행 여부

    private GameObject currentMarker;           // 현재 Lock On 대상
    private float lockOnCooldown = 1f;          // Lock On 재변경 쿨타임
    private float lastLockOnChangeTime = 0f;    // Lock On 변경 요청 시간

    public bool IsLockedOn => isLockedOn;
    public Transform LockedTarget => lockedTarget;

    void Update()
    {
        if (Input.GetMouseButtonDown(2))    //휠 버튼 누를시 Lock On 실행
        {
            isLockedOn = !isLockedOn;       //Lock On 상태 토글
            if (isLockedOn)
            {
                FindClosestEnemy();         //30f 이내의 모든 적 리스트에 담음
            }
            else
            {
                lockedTarget = null;        //Lock On 해제
                DestroyCurrentMarker();     //마커 삭제
            }
        }
            
        if (isLockedOn && lockedTarget != null)                     //Lock On 실행중 마우스 좌우 이동으로 타겟 변경
        {
            float mouseMovement = Input.GetAxis("Mouse X");         //마우스 X축 이동 감지
            float sensitivityAdjustment = 0.5f;                     //이동 감도 조정
            mouseMovement *= sensitivityAdjustment;                 //감도 조정
                
            if (Time.time - lastLockOnChangeTime > lockOnCooldown)  //Lock On 재변경 쿨타임 만족시 타겟 변경
            {
                if (Mathf.Abs(mouseMovement) > 0.5f)                //마우스 최소 이동량 설정
                {
                    ChangeLockedTarget(mouseMovement);              //Lock On 타겟 변경
                    lastLockOnChangeTime = Time.time;               //Lock On 변경 요청 시간 초기화
                }
            }
            UpdateMarkerPosition();                                 //마커 업데이트
        }

        if (isLockedOn && lockedTarget != null)                                                 //Lock On 실행중 인식 거리 이탈 시 자동 해제
        {
            float distanceToTarget = (player.position - lockedTarget.position).sqrMagnitude;    //Lock On 타겟과 플레이어 사이의 거리
            if (distanceToTarget > 30f * 30f)                                                   //거리 제곱으로 비교
            {
                isLockedOn = false;     //Lock On 해제
                lockedTarget = null;    //Lock On 타겟 Null
                DestroyCurrentMarker(); //마커 삭제
            }
        }
    }

    void LateUpdate()
    {
        if (lockedTarget != null && isLockedOn)     //Lock On 상태에서 카메라 조정
        {
            Xaxis -= Input.GetAxis("Mouse Y") * rotationSensitive;
            Xaxis = Mathf.Clamp(Xaxis, rotationMin, rotationMax);

            Vector3 direction = lockedTarget.position - player.position;                                //Lock On 타겟과의 방향벡터
            Quaternion rotationToFaceEnemy = Quaternion.LookRotation(direction);                        //Lock On 타겟으로 회전 계수 생성
            Quaternion targetRotation = Quaternion.Euler(Xaxis, rotationToFaceEnemy.eulerAngles.y, 0);  //Lock On 타겟으로 위치 계수 생성

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothTime);       //Lock On 타겟으로 부드럽게 회전
            transform.position = player.position - transform.forward * distance;                        //카메라 위치 설정
        }
        else                            //Lock On 미실시 상태에서 카메라 조정
        {
            Yaxis += Input.GetAxis("Mouse X") * rotationSensitive;
            Xaxis -= Input.GetAxis("Mouse Y") * rotationSensitive;
            Xaxis = Mathf.Clamp(Xaxis, rotationMin, rotationMax);

            Quaternion targetRotation = Quaternion.Euler(new Vector3(Xaxis, Yaxis));                //회전 계수 생성
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothTime);   //카메라 부드럽게 회전
            transform.position = player.position - transform.forward * distance;                    //카메라 위치 설정
        }
    }

    private void FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        Collider[] hitColliders = Physics.OverlapSphere(player.position, 30f);

        enemies.Clear();                                    //리스트 초기화
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                enemies.Add(hitCollider.transform);         //적 추가
                float currentDistance = (player.position - hitCollider.transform.position).sqrMagnitude;
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    lockedTarget = hitCollider.transform;   //가장 가까운 적을 Lock On
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