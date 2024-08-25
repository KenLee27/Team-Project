using System.Collections.Generic;
using UnityEngine;

public class SuperCameraController : MonoBehaviour
{
    public Transform player;                       // 플레이어의 Transform
    public GameObject lockOnMarker;                // Lock On 마커 프리팹w

    private float rotationSensitive = 3f;          // 카메라 회전 감도
    private float distance = 7f;                   // 카메라-플레이어 거리
    private float minDistance = 1f;                // 최소 거리
    private float rotationMin = -20f;                // 카메라 X축 회전 하한
    private float rotationMax = 80f;               // 카메라 X축 회전 상한
    private float smoothTime = 0.12f;              // 카메라 회전 지연 계수
    private float smoothMove = 0.2f;

    private float Xaxis = 0f;                      // 카메라 X축 회전 각도
    private float Yaxis = 0f;                      // 카메라 Y축 회전 각도

    private Transform lockedTarget;                 // Lock On 대상
    private List<Transform> enemies = new List<Transform>(); // Lock On할 적 리스트
    private bool isLockedOn = false;                // Lock On 상태 여부

    private GameObject currentMarker;               // 현재 Lock On 마커 객체
    private float lockOnCooldown = 1f;              // Lock On 재변경 쿨타임
    private float lastLockOnChangeTime = 0f;       // 마지막 Lock On 변경 요청 시간

    private float timeSinceHit = 0f;

    public bool IsLockedOn => isLockedOn;
    public Transform LockedTarget => lockedTarget;
    private Vector3 velocity = Vector3.zero;


    public float minSmoothMove = 0f; // 최소 smoothMove 값
    public float maxSmoothMove = 0.2f; // 최대 smoothMove 값
    public float smoothChangeTime = 0.1f; // smoothMove 값의 변화에 소요되는 시간
    private float currentSmoothMove; // 현재 smoothMove 값



    void Update()
    {
        if (Input.GetMouseButtonDown(2))    // 휠 버튼 클릭 시 Lock On 실행
        {
            isLockedOn = !isLockedOn;       // Lock On 상태 토글
            if (isLockedOn)
            {
                FindClosestEnemy();         // 30f 이내의 모든 적 리스트에 담음
            }
            else
            {
                lockedTarget = null;        // Lock On 해제
                DestroyCurrentMarker();     // 마커 삭제
            }
        }

        if (isLockedOn && lockedTarget != null)                     // Lock On 실행 중 마우스 좌우 이동으로 타겟 변경
        {
            float mouseMovement = Input.GetAxis("Mouse X");         // 마우스 X축 이동 감지
            float sensitivityAdjustment = 0.5f;                     // 이동 감도 조정
            mouseMovement *= sensitivityAdjustment;                 // 감도 조정

            if (Time.time - lastLockOnChangeTime > lockOnCooldown)  // Lock On 재변경 쿨타임 만족 시 타겟 변경
            {
                if (Mathf.Abs(mouseMovement) > 0.5f)                // 마우스 최소 이동량 설정
                {
                    ChangeLockedTarget(mouseMovement);              // Lock On 타겟 변경
                    lastLockOnChangeTime = Time.time;               // Lock On 변경 요청 시간 초기화
                }
            }
            UpdateMarkerPosition();                                 // 마커 업데이트
        }

        if (isLockedOn && lockedTarget != null)                                                 // Lock On 실행 중 인식 거리 이탈 시 자동 해제
        {
            float distanceToTarget = (player.position - lockedTarget.position).sqrMagnitude;    // Lock On 타겟과 플레이어 사이의 거리
            if (distanceToTarget > 30f * 30f)                                                   // 거리 제곱으로 비교
            {
                isLockedOn = false;     // Lock On 해제
                lockedTarget = null;    // Lock On 타겟 Null
                DestroyCurrentMarker(); // 마커 삭제
            }
        }

        // 충돌 검사 및 카메라 위치 조정
        //HandleCameraCollision(); // 장애물 감지 및 거리 조정
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position - transform.forward * distance; // 기본 카메라 위치 설정
        RaycastHit hit;

        // Raycast를 이용하여 Ground와 충돌 검사
        if (Physics.Raycast(player.position, -transform.forward, out hit, distance)) // 플레이어 위치에서 아래로 Raycast
        {
            if (hit.collider.CompareTag("Ground"))
            {
                // 충돌 시 distance 값을 2f로 점진적으로 줄여나감
                distance = Mathf.Lerp(distance, minDistance, Time.deltaTime * 5f); // 거리 감소
            }
        }
        else
        {
            distance = Mathf.Lerp(distance, 7f, Time.deltaTime * 5f); // 거리 복원
        }

        if (lockedTarget != null && isLockedOn) // Lock On 상태에서 카메라 조정
        {
            Vector3 direction = lockedTarget.position - player.position; // Lock On 타겟과의 방향벡터
            Quaternion rotationToFaceEnemy = Quaternion.LookRotation(direction); // Lock On 타겟으로 회전 계수 생성
            Quaternion targetRotation = Quaternion.Euler(Xaxis, rotationToFaceEnemy.eulerAngles.y, 0); // Lock On 타겟으로 위치 계수 생성

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothTime); // Lock On 타겟으로 부드럽게 회전
            Vector3 targetPosition = player.position - transform.forward * distance + Vector3.up * 1.3f;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.2f);
        }
        else // Lock On 미실시 상태에서 카메라 조정
        {
            // 마우스 입력에 따라 smoothMove 값을 조정
            float mouseInput = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));

            

            // 마우스 이동으로 인한 회전 감지 및 적용
            Yaxis += Input.GetAxis("Mouse X") * rotationSensitive;
            Xaxis -= Input.GetAxis("Mouse Y") * rotationSensitive;
            Xaxis = Mathf.Clamp(Xaxis, rotationMin, rotationMax);

            Quaternion targetRotation = Quaternion.Euler(new Vector3(Xaxis, Yaxis));
            transform.rotation = targetRotation;
            transform.position = player.position - transform.forward * distance + Vector3.up * 1.3f;
            // 카메라 위치 부드럽게 이동
            /*Vector3 targetPosition = player.position - transform.forward * distance + Vector3.up * 1.3f;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, currentSmoothMove);

            if (mouseInput > 0f) // 마우스가 움직일 때
            {
                currentSmoothMove = Mathf.Lerp(currentSmoothMove, minSmoothMove, Time.deltaTime / smoothChangeTime);
            }
            else // 마우스가 멈출 때
            {
                currentSmoothMove = Mathf.Lerp(currentSmoothMove, maxSmoothMove, Time.deltaTime / smoothChangeTime);
            }*/
        }
    }

    private void HandleCameraCollision()
    {
        Vector3 desiredPosition = player.position - transform.forward * distance; // 기본 카메라 위치 설정
        RaycastHit hit;

        // Raycast를 이용하여 Ground와 충돌 검사
        if (Physics.Raycast(player.position, -transform.forward, out hit, distance)) // 플레이어 위치에서 아래로 Raycast
        {
            if (hit.collider.CompareTag("Ground"))
            {
                // 충돌 시 distance 값을 2f로 점진적으로 줄여나감
                distance = Mathf.Lerp(distance, minDistance, Time.deltaTime * 5f); // 거리 감소
            }
        }
        else
        {
            distance = Mathf.Lerp(distance, 7f, Time.deltaTime * 5f); // 거리 복원
        }

        // 최종 카메라 위치 설정
        transform.position = player.position - transform.forward * distance; // 플레이어 기준으로 카메라 위치 설정
    }

    private void RotatePlayerTowardsLockedTarget()
    {
        if (lockedTarget != null)
        {
            Vector3 direction = lockedTarget.position - player.position;
            direction.y = 0; // Y축 방향 무시
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction); // 적 방향으로 회전
                player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * rotationSensitive); // 부드러운 회전
            }
        }
    }

    private void FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity; // 가장 가까운 적과의 거리 초기화
        Collider[] hitColliders = Physics.OverlapSphere(player.position, 30f); // 플레이어 주변의 적 탐색

        enemies.Clear(); // 리스트 초기화
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy")) // 적 태그 확인
            {
                enemies.Add(hitCollider.transform); // 적 추가
                float currentDistance = (player.position - hitCollider.transform.position).sqrMagnitude; // 현재 거리 계산
                if (currentDistance < closestDistance) // 가장 가까운 적 찾기
                {
                    closestDistance = currentDistance;
                    lockedTarget = hitCollider.transform; // Lock On할 적 설정
                }
            }
        }
    }

    private void ChangeLockedTarget(float mouseMovement)
    {
        int currentIndex = enemies.IndexOf(lockedTarget); // 현재 Lock On 타겟 인덱스
        if (currentIndex < 0) return; // 현재 Lock On 타겟이 없으면 반환

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
            currentMarker = Instantiate(lockOnMarker, lockedTarget.position + Vector3.up * 1f, Quaternion.identity); // 마커 생성
        }
        else
        {
            currentMarker.transform.position = lockedTarget.position + Vector3.up * 1f; // 마커 위치 업데이트
        }
    }

    private void DestroyCurrentMarker()
    {
        if (currentMarker != null)
        {
            Destroy(currentMarker);
            currentMarker = null; // 마커 초기화
        }
    }
}