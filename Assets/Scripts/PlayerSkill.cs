using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float maxDistance;
    private Vector3 startPosition;

    // 스킬 초기화 함수
    public void Initialize(Vector3 targetDirection, float moveSpeed, float range)
    {
        direction = targetDirection;
        speed = moveSpeed;
        maxDistance = range;
        startPosition = transform.position;
    }

    void Update()
    {
        // 스킬 이동 처리
        transform.position += direction * speed * Time.deltaTime;

        // 일정 거리 이동 후 스킬 파괴
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
