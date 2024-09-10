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

    // ��ų �ʱ�ȭ �Լ�
    public void Initialize(Vector3 targetDirection, float moveSpeed, float range)
    {
        direction = targetDirection;
        speed = moveSpeed;
        maxDistance = range;
        startPosition = transform.position;
    }

    void Update()
    {
        // ��ų �̵� ó��
        transform.position += direction * speed * Time.deltaTime;

        // ���� �Ÿ� �̵� �� ��ų �ı�
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �÷��̾� �±׿� ����� ���� ����
        if (other.CompareTag("Ground"))
        {
            // ��ų ������Ʈ �ı�
            Destroy(gameObject);
        }

        if (other.CompareTag("Player"))
        {
            // ��ų ������Ʈ �ı�
            Destroy(gameObject);
        }
    }
}
