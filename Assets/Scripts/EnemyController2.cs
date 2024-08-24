using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyController2 : MonoBehaviour
{
    public Animator anim; //�ִϸ�����
    NavMeshAgent nmAgent; //navmeshagent �߰�

    public GameObject player_1;

    public Transform player; //�÷��̾� Ÿ��
    public LayerMask playerLayer;

    private bool firstlooking = true; //ĳ���� ���� ���

    private float HP = 0; //�� ü�� ���� �� �ʱ�ȭ
    private float detectingRange = 20f;         //�� Ž�� �Ÿ�
    private float sensingRange = 13.5f;         //�� ���� �Ÿ�
    public float attackRange = 2.5f;           //�� ���� ��Ÿ�
    private float smoothRotationSpeed = 15f;     //�� ����ȭ ȸ�� �ӵ�
    //public float moveSpeed = 4.0f;             //�� �̵��ӵ�
    //private float returnSpeed = 2f;           //�� ���ͼӵ�

    public bool isHit = false;

    Vector3 directionToPlayer;
    Vector3 directionToBase;

    float distanceToPlayer = 100f;
    float distanceToBase = 0f;

    private Quaternion targetRotation;
    private float currentSpeed;

    Vector3 initialPoint; //�� ��ġ ��ġ ���� ����

    enum State
    {
        IDLE,
        CHASE,
        ATTACK,
        BACK,
        LOOK,
        ROAR,
        HIT,
        DIE
    }

    State state;


    void Awake()
    {
        playerLayer = LayerMask.GetMask("Player");
    }

    void Start()
    {
        initialPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);   //�� ��ġ ��ġ �ʱ�ȭ
        HP = 10; //ü�� �ʱ�ȭ
        anim = GetComponent<Animator>();
        nmAgent = GetComponent<NavMeshAgent>();
        state = State.IDLE;
        StartCoroutine(StateMachine());
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            yield return StartCoroutine(state.ToString());
        }
        
    }

    IEnumerator IDLE()
    {
        // ���� animator �������� ���
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼� �̸��� Idle �� �ƴϸ� Play
        if (curAnimStateInfo.IsName("Creep|Idle1_Action") == false)
            anim.Play("Creep|Idle1_Action", 0, 0);

        if (distanceToPlayer <= detectingRange) //Ž�� ���� �ȿ� �÷��̾ ������
        {
            // StateMachine �� ���� ����
            ChangeState(State.LOOK);
        }
        yield return null;
    }

    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //�ִϸ��̼� ��ȯ ���
        if (curAnimStateInfo.IsName("Creep|Crouch_Action") == false)
        {
            anim.CrossFade("Creep|Crouch_Action", 0.1f);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }

        //�ִϸ��̼� ��ȯ üũ
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Creep|Crouch_Action"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        if (isHit)
        {
            ChangeState(State.HIT);
        }

        // �÷��̾���� ���� �Ÿ��� ���� �������� �۰ų� ������
        if (distanceToPlayer <= attackRange)
        {
            // StateMachine �� �������� ����
            ChangeState(State.ATTACK);
        }
        // ��ǥ���� �Ÿ��� �����Ÿ� ���� �־��� ���
        if (sensingRange < distanceToPlayer)
        {
            // StateMachine �� ���� ����
            ChangeState(State.LOOK);
        }

        if (distanceToBase >= 30.0f)
        {
            ChangeState(State.BACK);
        }

        yield return null;
    }

    IEnumerator ATTACK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Creep|Punch_Idle", 0.1f, 0, 0);

        float attackDuration = 2.0f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            // ���� ���� �������� �÷��̾���� �Ÿ��� �־�����
            if (distanceToPlayer > attackRange)
            {
                // StateMachine�� �������� ����
                ChangeState(State.CHASE);
                yield break;  // �ڷ�ƾ ����
            }

            if (isHit)
            {
                ChangeState(State.HIT);
                yield break;  // �ڷ�ƾ ����
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator BACK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("Creep|Walk1_Action") == false)
        {
            anim.CrossFade("Creep|Walk1_Action", 0.1f);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }
        //�ִϸ��̼� ��ȯ ���

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Creep|Walk1_Action"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        //�ִϸ��̼� ��ȯ üũ

        if(isHit)
        {
            ChangeState(State.HIT);
        }

        yield return null;
    }

    IEnumerator LOOK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        //���� ����� Ȱ��ȭ �Ǿ� ������ ��ȿ

        if (firstlooking)
        {
            ChangeState(State.ROAR);
        }


        if (curAnimStateInfo.IsName("Creep|Idle1_Action") == false)
        {
            anim.CrossFade("Creep|Idle1_Action", 0.1f);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }
        //�ִϸ��̼� ��ȯ ���

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Creep|Idle1_Action"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }


        // �Ÿ��� ���������
        if (sensingRange > distanceToPlayer)
        {
            // StateMachine�� �������� ����
            ChangeState(State.CHASE);
        }

        // �Ÿ��� �־�����
        if (detectingRange < distanceToPlayer)
        {
            // StateMachine�� BACK���� ����
            ChangeState(State.BACK);
        }
        yield return null;
    }

    IEnumerator ROAR()
    {

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (!curAnimStateInfo.IsName("Creep|Roar_Action"))
        {
            anim.Play("Creep|Roar_Action", 0, 0);
            firstlooking = false;
            Debug.Log("����");

            // �ִϸ��̼� ���°� ����� ������ ��ٸ���
            yield return null;

            // ���¸� �ٽ� ������
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            while (!curAnimStateInfo.IsName("Creep|Roar_Action"))
            {
                yield return null;
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            }
        }

        // �ִϸ��̼��� Roar���� Idle�� ����� ������ ���
        while (curAnimStateInfo.IsName("Creep|Roar_Action"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        if (isHit)
        {
            ChangeState(State.HIT);
            yield break;  // �ڷ�ƾ ����
        }

        if (curAnimStateInfo.IsName("Creep|Idle1_Action"))
        {
            ChangeState(State.LOOK);
            Debug.Log("��ȿ��");
        }

        yield return null;
    }

    IEnumerator HIT()
    {
        HP = HP - 3; //ĳ���� ������ ���߿� ��������
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (HP > 0)
        {

            if (!curAnimStateInfo.IsName("Creep|Hit_Action"))
            {
                anim.Play("Creep|Hit_Action", 0, 0);
                isHit = false;
                

                // �ִϸ��̼� ���°� ����� ������ ��ٸ���
                yield return null;

                // ���¸� �ٽ� ������
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                while (!curAnimStateInfo.IsName("Creep|Hit_Action"))
                {
                    yield return null;
                    curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                }
            }

            // �ִϸ��̼��� Walk1_Action ����� ������ ���
            while (curAnimStateInfo.IsName("Creep|Hit_Action"))
            {
                yield return null;
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            }

            if (curAnimStateInfo.IsName("Creep|Walk1_Action"))
            {
                ChangeState(State.ATTACK);
            }

            yield return null;
        }
        else
        {
            ChangeState(State.DIE);
            Debug.Log("�׾���....");
        }
        
    }

    IEnumerator DIE()
    {
        // �ִϸ������� ���� �ִϸ��̼� ���� ���� ��������
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �浹 ���� ���� (Collider ��Ȱ��ȭ)
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // ��� �ִϸ��̼� ���
        anim.Play("Creep|Death_Action");

        // �ִϸ��̼��� normalizedTime�� 1.0�� ������� ������ ���
        while (true)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // �ִϸ��̼��� normalizedTime�� 1.0 �̻��� �� �ִϸ��̼��� �Ϸ�� ������ ����
            if (curAnimStateInfo.IsName("Creep|Death_Action") && curAnimStateInfo.normalizedTime >= 1.0f)
            {
                break;
            }

            yield return null; // �� ������ ���
        }

        // �ִϸ��̼��� ���� �� ������Ʈ�� ����
        Destroy(gameObject);
    }






    void Update()
    {
        directionToPlayer = new Vector3(player.position.x - transform.position.x,
            0f, player.position.z - transform.position.z);    //�÷��̾���� ��ġ����
        distanceToPlayer = directionToPlayer.magnitude;                                       //�÷��̾���� ��ġ���� ��ġȭ

        directionToBase = new Vector3(initialPoint.x - transform.position.x,
            0f, initialPoint.z - transform.position.z);
        distanceToBase = directionToBase.magnitude;

        if (state == State.ROAR || state == State.LOOK || state == State.ATTACK)
        {
            LookPlayer(directionToPlayer);
        }

        else if (state == State.CHASE)
        {
            nmAgent.SetDestination(player.position);
        }

        else if (state == State.BACK)
        {
            ReturnToBase();
        }
        /*
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
        */
    }




    void ChangeState(State newState)
    {
        state = newState;
    }

    void ChasePlayer(Vector3 direction)     //�߰� ���
    {
        
    }

    void LookPlayer(Vector3 direction)      //��� ���
    {

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        if (flatDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothRotationSpeed * Time.deltaTime);
        }
    }

    void ReturnToBase()  //��ȯ ���
    {

        if (distanceToBase > 3)  // �Ÿ��� 0.1 �̻��� ��쿡�� �̵�
        {
            nmAgent.SetDestination(initialPoint);
        }
        else
        {
            Debug.Log("������");
            firstlooking = true; //���� ��� Ȱ��ȭ�ϰ�
            ChangeState(State.IDLE); //idle ���·� ����
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttacking)
        {
            Debug.Log("������!");
            isHit = true;

        }
    }

}
