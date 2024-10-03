using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float maxDistance;
    private Vector3 startPosition;
    public GameObject explosionEffect; // 폭발 이펙트 프리팹

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
        transform.position = new Vector3
        (
            transform.position.x + direction.x * speed * Time.deltaTime,
            transform.position.y,  // Y값 유지
            transform.position.z + direction.z * speed * Time.deltaTime
        );
        // 일정 거리 이동 후 스킬 파괴
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 적 태그에 닿았을 때만 실행
        if (other.CompareTag("Enemy"))
        {
            // 폭발 이펙트 생성
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            // 스킬 오브젝트 파괴
            Destroy(gameObject);
        }
    }

}
