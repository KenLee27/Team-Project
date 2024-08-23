using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{

    public Animator anim; //애니메이터

    public Transform player;
    public LayerMask playerLayer; //하이

    private bool firstlooking = false; //캐릭터 최초 목격

    private float detectingRange = 20f;         //적 탐지 거리
    private float sensingRange = 13.5f;         //적 인지 거리
    public float attackRange = 2.5f;           //적 공격 사거리
    private float smoothRotationSpeed = 15f;     //적 최적화 회전 속도
    public float moveSpeed = 4.0f;             //적 이동속도
    private float returnSpeed = 2f;           //적 복귀속도

    private Quaternion targetRotation;
    private float currentSpeed;

    Vector3 initialPoint;

    void Awake()
    {
        playerLayer = LayerMask.GetMask("Player");
    }

    void Start()
    {
        initialPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);   //적 배치 위치
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = new Vector3(player.position.x - transform.position.x, 
            0f, player.position.z - transform.position.z);    //플레이어와의 위치관계
        float distanceToPlayer = directionToPlayer.magnitude;                                       //플레이어와의 위치관계 수치화

        Vector3 directionToBase = new Vector3(initialPoint.x - transform.position.x,
            0f, initialPoint.z - transform.position.z);
        float distanceToBase = directionToBase.magnitude;

        if (distanceToPlayer <= attackRange)        //적 공격 사거리 내에 들어오면 공격             
        {
            AttackPlayer(directionToPlayer);
        }
        else if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, sensingRange, playerLayer) && 
            distanceToPlayer > attackRange && distanceToPlayer < detectingRange && distanceToPlayer > attackRange)  //적 인지거리 내에 들어오면 추격                                    
        {
            if (hit.collider.CompareTag("Player"))
            {
                ChasePlayer(directionToPlayer);
            }
        }
        else if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectingRange, playerLayer))        //적 탐지거리 내에 들어오면 경계
        {
            if (hit.collider.CompareTag("Player"))
            {
                LookPlayer(directionToPlayer);
            }
        }
        else
        {
            ReturnToBase(initialPoint);
        }
    }

    void AttackPlayer(Vector3 direction)
    {
        Debug.Log("공격");
        anim.SetTrigger("attack"); //애니메이션 트리커 공격

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        if (flatDirection != Vector3.zero)
        {
            float distanceToPlayer = flatDirection.magnitude;
            if (distanceToPlayer > 0)
            {
                targetRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothRotationSpeed * Time.deltaTime);
            }
        }
    }

    void ChasePlayer(Vector3 direction)     //추격 기능
    {

        anim.SetTrigger("run"); //애니메이션 트리커 달리기
        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        if (flatDirection != Vector3.zero)
        {
            float distanceToPlayer = flatDirection.magnitude;
            if (distanceToPlayer > 2)
            {
                targetRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothRotationSpeed * Time.deltaTime);
            }
        }

        // 이동
        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime);
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        
    }

    void LookPlayer(Vector3 direction)      //경계 기능
    {
        anim.SetTrigger("look"); //애니메이션 트리커 경계
        Debug.Log("경계중!");

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        if (flatDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothRotationSpeed * Time.deltaTime);
        }
    }
    void ReturnToBase(Vector3 basePosition)
    {
        anim.SetTrigger("walk"); //애니메이션 트리커 걷기

        Vector3 directionToBase = basePosition - transform.position;
        directionToBase.y = 0;  // y축을 고려하지 않도록 설정

        float distanceToBase = directionToBase.magnitude;

        if (distanceToBase > 1.5)  // 거리가 0.1 이상일 경우에만 이동
        {
            if (distanceToBase > 2)
            {
                // 방향을 목표로 회전
                targetRotation = Quaternion.LookRotation(directionToBase);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (smoothRotationSpeed - 5.0f) * Time.deltaTime);
            }

            // 이동
            currentSpeed = Mathf.Lerp(currentSpeed, returnSpeed, Time.deltaTime);
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("집도착!");
            currentSpeed = 0;  // 목표 지점에 도달하면 멈춤
            anim.SetTrigger("home"); //애니메이션 트리커 집
        }
    }
}
