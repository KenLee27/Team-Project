using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float maxDistance;
    private Vector3 startPosition;
    public GameObject explosionEffect; // ���� ����Ʈ ������

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
        transform.position = new Vector3
        (
            transform.position.x + direction.x * speed * Time.deltaTime,
            transform.position.y,  // Y�� ����
            transform.position.z + direction.z * speed * Time.deltaTime
        );
        // ���� �Ÿ� �̵� �� ��ų �ı�
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �� �±׿� ����� ���� ����
        if (other.CompareTag("Enemy"))
        {
            // ���� ����Ʈ ����
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            // ��ų ������Ʈ �ı�
            Destroy(gameObject);
        }
    }

}
