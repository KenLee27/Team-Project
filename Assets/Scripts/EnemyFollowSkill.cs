using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowSkill : MonoBehaviour, Ienemy
{
    private Transform player;
    private float speed;
    private float lifeTime = 15f; // 15�� �Ŀ� ����

    public float Damage = 20f;
    float Ienemy.Damage => Damage;

    // ��ų �ʱ�ȭ �Լ�
    public void Initialize(Transform targetPlayer, float moveSpeed)
    {
        player = targetPlayer; // �÷��̾��� Transform ����
        speed = moveSpeed;

        // 10�� �Ŀ� �� ������Ʈ�� �ı��ϴ� �ڷ�ƾ ����
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // ��ų �̵� ó��
        if (player != null)
        {
            // �÷��̾� ���� ���
            Vector3 direction = (player.position - transform.position).normalized;

            // �ҵ��̸� �÷��̾� �������� �̵�
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            Debug.Log("�÷��̾ ����");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �÷��̾� �±׿� ����� �� ����
        if (other.CompareTag("Player"))
        {
            // Player ��ũ��Ʈ���� isInvinsivility ������ ������
            /*SuperPlayerController playerScript = other.GetComponent<SuperPlayerController>();

            if (playerScript != null && !playerScript.isinvincibility)
            {
                // �÷��̾ ���� ���°� �ƴ� ���� ��ų ������Ʈ �ı�
                Destroy(gameObject);
            }*/
            Destroy(gameObject);
        }
    }
}
