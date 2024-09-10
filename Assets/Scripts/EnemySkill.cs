using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkill : MonoBehaviour, Ienemy
{
    private Vector3 direction;
    private float speed;
    private float maxDistance;
    private Vector3 startPosition;

    public float Damage = 20f;
    float Ienemy.Damage => Damage;

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

    void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그에 닿았을 때만 실행
        if (other.CompareTag("Ground"))
        {
            // 스킬 오브젝트 파괴
            Destroy(gameObject);
        }

        if (other.CompareTag("Player"))
        {
            // 스킬 오브젝트 파괴
            Destroy(gameObject);
        }
    }
}
