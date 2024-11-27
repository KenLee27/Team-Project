using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quakeSkill : MonoBehaviour, Ienemy
{
    private Vector3 direction;
    private float speed;
    private float maxDistance;
    private Vector3 startPosition;

    public float Damage = 40f;
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

    }

    void Start()
    {
        Invoke("DestroyObject", 0.1f);
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }

}
