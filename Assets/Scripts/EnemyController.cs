using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{

    public Animator anim; //�ִϸ�����

    public Transform player;
    public LayerMask playerLayer; //����

    private bool firstlooking = false; //ĳ���� ���� ���

    private float detectingRange = 20f;         //�� Ž�� �Ÿ�
    private float sensingRange = 13.5f;         //�� ���� �Ÿ�
    public float attackRange = 2.5f;           //�� ���� ��Ÿ�
    private float smoothRotationSpeed = 15f;     //�� ����ȭ ȸ�� �ӵ�
    public float moveSpeed = 4.0f;             //�� �̵��ӵ�
    private float returnSpeed = 2f;           //�� ���ͼӵ�

    private Quaternion targetRotation;
    private float currentSpeed;

    Vector3 initialPoint;

    void Awake()
    {
        playerLayer = LayerMask.GetMask("Player");
    }

    void Start()
    {
        initialPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);   //�� ��ġ ��ġ
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = new Vector3(player.position.x - transform.position.x, 
            0f, player.position.z - transform.position.z);    //�÷��̾���� ��ġ����
        float distanceToPlayer = directionToPlayer.magnitude;                                       //�÷��̾���� ��ġ���� ��ġȭ

        Vector3 directionToBase = new Vector3(initialPoint.x - transform.position.x,
            0f, initialPoint.z - transform.position.z);
        float distanceToBase = directionToBase.magnitude;

        if (distanceToPlayer <= attackRange)        //�� ���� ��Ÿ� ���� ������ ����             
        {
            AttackPlayer(directionToPlayer);
        }
        else if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, sensingRange, playerLayer) && 
            distanceToPlayer > attackRange && distanceToPlayer < detectingRange && distanceToPlayer > attackRange)  //�� �����Ÿ� ���� ������ �߰�                                    
        {
            if (hit.collider.CompareTag("Player"))
            {
                ChasePlayer(directionToPlayer);
            }
        }
        else if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectingRange, playerLayer))        //�� Ž���Ÿ� ���� ������ ���
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
        Debug.Log("����");
        anim.SetTrigger("attack"); //�ִϸ��̼� Ʈ��Ŀ ����

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

    void ChasePlayer(Vector3 direction)     //�߰� ���
    {

        anim.SetTrigger("run"); //�ִϸ��̼� Ʈ��Ŀ �޸���
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

        // �̵�
        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime);
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        
    }

    void LookPlayer(Vector3 direction)      //��� ���
    {
        anim.SetTrigger("look"); //�ִϸ��̼� Ʈ��Ŀ ���
        Debug.Log("�����!");

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        if (flatDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothRotationSpeed * Time.deltaTime);
        }
    }
    void ReturnToBase(Vector3 basePosition)
    {
        anim.SetTrigger("walk"); //�ִϸ��̼� Ʈ��Ŀ �ȱ�

        Vector3 directionToBase = basePosition - transform.position;
        directionToBase.y = 0;  // y���� ������� �ʵ��� ����

        float distanceToBase = directionToBase.magnitude;

        if (distanceToBase > 1.5)  // �Ÿ��� 0.1 �̻��� ��쿡�� �̵�
        {
            if (distanceToBase > 2)
            {
                // ������ ��ǥ�� ȸ��
                targetRotation = Quaternion.LookRotation(directionToBase);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (smoothRotationSpeed - 5.0f) * Time.deltaTime);
            }

            // �̵�
            currentSpeed = Mathf.Lerp(currentSpeed, returnSpeed, Time.deltaTime);
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("������!");
            currentSpeed = 0;  // ��ǥ ������ �����ϸ� ����
            anim.SetTrigger("home"); //�ִϸ��̼� Ʈ��Ŀ ��
        }
    }
}
