using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float maxDistance;
    private Vector3 startPosition;

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
}
