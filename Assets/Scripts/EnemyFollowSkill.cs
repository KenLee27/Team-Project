using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowSkill : MonoBehaviour, Ienemy
{
    private Transform player;
    private float speed;
    private float lifeTime = 15f; // 15초 후에 삭제

    public float Damage = 20f;
    float Ienemy.Damage => Damage;

    // 스킬 초기화 함수
    public void Initialize(Transform targetPlayer, float moveSpeed)
    {
        player = targetPlayer; // 플레이어의 Transform 설정
        speed = moveSpeed;

        // 10초 후에 이 오브젝트를 파괴하는 코루틴 시작
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 스킬 이동 처리
        if (player != null)
        {
            // 플레이어 방향 계산
            Vector3 direction = (player.position - transform.position).normalized;

            // 불덩이를 플레이어 방향으로 이동
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            Debug.Log("플레이어가 없다");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그에 닿았을 때 실행
        if (other.CompareTag("Player"))
        {
            // Player 스크립트에서 isInvinsivility 변수를 가져옴
            /*SuperPlayerController playerScript = other.GetComponent<SuperPlayerController>();

            if (playerScript != null && !playerScript.isinvincibility)
            {
                // 플레이어가 무적 상태가 아닐 때만 스킬 오브젝트 파괴
                Destroy(gameObject);
            }*/
            Destroy(gameObject);
        }
    }
}
