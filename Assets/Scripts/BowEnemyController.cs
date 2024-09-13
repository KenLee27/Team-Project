using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BowEnemyController : MonoBehaviour, Ienemy
{
    public Animator anim; //�ִϸ�����
    NavMeshAgent nmAgent; //navmeshagent �߰�

    public GameObject player_1;

    public Transform player; //�÷��̾� Ÿ��
    public LayerMask playerLayer;

    private float HP = 0; //�� ü�� ���� �� �ʱ�ȭ
    private float MaxHP = 10;
    private float detectingRange = 25f;         //�� Ž�� �Ÿ�
    private float sensingRange = 20f;         //�� ���� �Ÿ�
    private float checkRange = 13f;
    private float attackRange = 13.5f;           //�� ���� ��Ÿ�

    private float nearattackRange = 2f;           //�� ���� ��Ÿ�

    private float smoothRotationSpeed = 15f;     //�� ����ȭ ȸ�� �ӵ�
    //public float moveSpeed = 4.0f;             //�� �̵��ӵ�
    //private float returnSpeed = 2f;           //�� ���ͼӵ�
    public float Damage = 10f;
    float Ienemy.Damage => Damage;

    public bool isHit = false;                  //���� ����
    public bool isinvincibility = false;      //��������
    public bool isAttack = false;
    public bool isDead = false;

    public GameObject bowattackRange;           //���� Ȱ ���� ����
    public GameObject kickattackRange;           //���� ������ ���� ����

    Vector3 directionToPlayer;
    Vector3 directionToBase;

    float distanceToPlayer = 100f;
    float distanceToBase = 0f;

    private Quaternion targetRotation;
    private float currentSpeed;


    private bool moveRight;  // �¿� �̵� ���� ��� ���� ����
    private bool directionInitialized = false;  // ������ �ʱ�ȭ�Ǿ����� ���θ� Ȯ���ϴ� ����
    private float timeSinceLastCheck = 0f;
    private float checkInterval = 2f; // 2�ʸ��� üũ
    public float enemySoul = 50f;

    Vector3 initialPoint; //�� ��ġ ��ġ ���� ����

    private bool isAttacked = false;
    private Slider hpSlider;                     // ���� HP �����̴�
    private GameObject hpSliderObject;          // �����̴� UI ������Ʈ

    enum State
    {
        IDLE,
        CHASE,
        ATTACK_READY,
        ATTACK_CHARGE,
        ATTACK_SHOT,
        NEAR_ATTACK_READY,
        BACK,
        LOOK,
        CHECK,
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
        HP = MaxHP; //ü�� �ʱ�ȭ
        anim = GetComponent<Animator>();
        nmAgent = GetComponent<NavMeshAgent>();
        state = State.IDLE;
        StartCoroutine(StateMachine());

        bowattackRange.GetComponent<Collider>().enabled = false;
        kickattackRange.GetComponent<Collider>().enabled = false;

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
        if (curAnimStateInfo.IsName("Idle") == false)
            anim.Play("Idle", 0, 0);

        if (distanceToPlayer <= detectingRange && player_1.GetComponent<SuperPlayerController>().isStand) //Ž�� ���� �ȿ� �÷��̾ ������
        {
            // StateMachine �� ���� ����
            ChangeState(State.LOOK);
        }

        if (isHit)
        {
            ChangeState(State.DIE);
        }
        yield return null;
    }

    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //�ִϸ��̼� ��ȯ ���
        if (curAnimStateInfo.IsName("Walk") == false)
        {
            anim.CrossFade("Walk", 0.1f);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }

        //�ִϸ��̼� ��ȯ üũ
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Walk"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        if (isHit)
        {
            ChangeState(State.HIT);
            yield break;
        }

        // �÷��̾���� ���� �Ÿ��� ���� �������� �۰ų� ������
        if (distanceToPlayer <= attackRange)
        {
            // StateMachine �� �������� ����
            ChangeState(State.ATTACK_READY);
        }

        if (distanceToPlayer <= checkRange)
        {
            float randomValue = UnityEngine.Random.Range(0f, 100f); // 0���� 100 ������ ������ ��
            if (randomValue <= 0.1f)
            {
                ChangeState(State.CHECK); // check ���·� ��ȯ
                yield break;
            }
        }
        // ��ǥ���� �Ÿ��� �����Ÿ� ���� �־��� ���
        if (sensingRange < distanceToPlayer)
        {
            // StateMachine �� ���� ����
            ChangeState(State.LOOK);
        }

        if (distanceToBase >= 50.0f)
        {
            ChangeState(State.BACK);
        }

        yield return null;
    }

    IEnumerator ATTACK_READY()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        nmAgent.speed = 0f;
        anim.CrossFade("Ready", 0.1f, 0, 0);

        float attackDuration = 4f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            
                if (isHit)                                                      //������ ����
                {
                    isAttack = false;
                    ChangeState(State.HIT);
                    yield break;  // �ڷ�ƾ ����
                }
            
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // ���� ���� �������� �÷��̾���� �Ÿ��� �־�����
        if (distanceToPlayer > attackRange)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f); // 0���� 1 ������ ������ ��  
            if (randomValue <= 0.4f)
            {
                ChangeState(State.CHECK); // 40% Ȯ���� check ���·� ��ȯ
                yield break;
            }


            // StateMachine�� �������� ����
            ChangeState(State.CHASE);
            yield break;
        }

        ChangeState(State.CHECK); // ���� �� üũ ���� ����
        yield break;

    }
    IEnumerator NEAR_ATTACK_READY()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Bow_Attack", 0.1f, 0, 0);

        float attackDuration = 1.6f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("Bow_Attack"))
            {
                if (isHit)                                                      //������ ����
                {
                    isAttack = false;
                    ChangeState(State.HIT);
                    yield break;  // �ڷ�ƾ ����
                }
            }
            else
            {
                //������ HIT�� ����
                if (isHit)
                {
                    ChangeState(State.HIT);
                    yield break;  // �ڷ�ƾ ����
                }

            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // ���� ���� �������� �÷��̾���� �Ÿ��� �־�����
        if (distanceToPlayer > nearattackRange)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f); // 0���� 1 ������ ������ ��  
            if (randomValue <= 0.4f)
            {
                ChangeState(State.ATTACK_READY); // 40% Ȯ���� check ���·� ��ȯ
            }


            // StateMachine�� �������� ����
            ChangeState(State.CHECK);
        }

        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator BACK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("Walk") == false)
        {
            anim.CrossFade("Walk", 0.1f);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }
        //�ִϸ��̼� ��ȯ ���

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Walk"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        //�ִϸ��̼� ��ȯ üũ

        if (isHit)
        {
            ChangeState(State.HIT);
            yield break;
        }

        yield return null;
    }

    IEnumerator LOOK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (curAnimStateInfo.IsName("Idle") == false)
        {
            anim.CrossFade("Idle", 0.1f);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }
        //�ִϸ��̼� ��ȯ ���

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Idle"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        //������ ��� HIT ���·� ����
        if (isHit)
        {
            ChangeState(State.HIT);
            yield break;
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

    IEnumerator HIT()
    {
        AttackEnd();

        //�ۼ��� �̰�
        if (isAttacked != true)
        {
            isAttacked = true;
            hpSliderObject.SetActive(true);
        }
        //�ۼ��� �̰�



        nmAgent.speed = 0f;
        HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
        Debug.Log(HP + "���ҽ��ϴ�");

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (HP > 0)
        {

            if (!curAnimStateInfo.IsName("Hit"))
            {
                anim.Play("Hit", 0, 0);
                isHit = false;


                // �ִϸ��̼� ���°� ����� ������ ��ٸ���
                yield return null;

                // ���¸� �ٽ� ������
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                while (!curAnimStateInfo.IsName("Hit"))
                {
                    yield return null;
                    curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                }
            }

            // �ִϸ��̼��� Walk1_Action ����� ������ ���
            while (curAnimStateInfo.IsName("Hit"))
            {
                yield return null;
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            }
            isinvincibility = false;

            if (curAnimStateInfo.IsName("Idle"))
            {
                // �����̼� �¾����� üũ
                if (nearattackRange >= distanceToPlayer)
                {
                    float checkTime2 = UnityEngine.Random.Range(0f, 2f);

                    //60%Ȯ���� �ݰ�
                    if (checkTime2 > 1.2f)
                        ChangeState(State.NEAR_ATTACK_READY);
                    else
                        ChangeState(State.CHECK);
                    yield break;
                }

                else if (attackRange >= distanceToPlayer)
                {
                    //�ָ��� �¾����� ���Ÿ� ����
                    ChangeState(State.ATTACK_READY);
                    yield break;
                }


            }
            //�� �հŸ����� �¾����� �߰�
            ChangeState(State.CHASE);

            yield return null;
        }
        else
        {
            ChangeState(State.DIE);
            Debug.Log("�׾���....");
        }

    }

    IEnumerator CHECK()
    {
        Debug.Log("�ֽ���...");
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Walk", 0.1f, 0, 0);

        float checkTime = UnityEngine.Random.Range(2f, 4f);
        float elapsedTime = 0f;
        while (elapsedTime < checkTime)
        {

            //�Ÿ��� ��������
            if (detectingRange < distanceToPlayer)
            {
                // StateMachine�� BACK���� ����
                ChangeState(State.BACK);
                yield break;            // �ڷ�ƾ ����
            }

            //������
            if (isHit)
            {
                // StateMachine�� HIT���� ����
                ChangeState(State.HIT);
                yield break;            // �ڷ�ƾ ����
            }


            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �÷��̾���� ���� �Ÿ��� ���� �������� �۰ų� ������
        

        if (distanceToPlayer <= nearattackRange)
        {
            // StateMachine �� �������� ����
            ChangeState(State.NEAR_ATTACK_READY);
            yield break;            // �ڷ�ƾ ����
        }
        else if (distanceToPlayer <= attackRange)
        {
            // StateMachine �� �������� ����
            ChangeState(State.ATTACK_READY);
            yield break;            // �ڷ�ƾ ����
        }


        ChangeState(State.CHASE);

    }

    IEnumerator DIE()
    {
        if (isDead)
        {
            yield break;
        }
        isDead = true;

        // �ִϸ������� ���� �ִϸ��̼� ���� ���� ��������
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �浹 ���� ���� (Collider ��Ȱ��ȭ)
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // ��� �ִϸ��̼� ���
        anim.Play("Die");

        // �ִϸ��̼��� normalizedTime�� 1.0�� ������� ������ ���
        while (true)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // �ִϸ��̼��� normalizedTime�� 1.0 �̻��� �� �ִϸ��̼��� �Ϸ�� ������ ����
            if (curAnimStateInfo.IsName("Die") && curAnimStateInfo.normalizedTime >= 1.0f)
            {
                break;
            }

            yield return null; // �� ������ ���
        }

        GameManager.Instance.UpdatePlayerSOUL(enemySoul);

        // �ִϸ��̼��� ���� �� ������Ʈ�� ����
        Destroy(gameObject);

        //�ۼ��� �̰�
        Destroy(hpSliderObject);
        //�ۼ��� �̰�
    }






    void Update()
    {
        UpdateHPBar(); // �ۼ��� �̰�

        directionToPlayer = new Vector3(player.position.x - transform.position.x,
            0f, player.position.z - transform.position.z);    //�÷��̾���� ��ġ����
        distanceToPlayer = directionToPlayer.magnitude;                                       //�÷��̾���� ��ġ���� ��ġȭ

        directionToBase = new Vector3(initialPoint.x - transform.position.x,
            0f, initialPoint.z - transform.position.z);
        distanceToBase = directionToBase.magnitude;

        if (state == State.LOOK || state == State.ATTACK_READY || state == State.NEAR_ATTACK_READY || state == State.CHECK || state == State.HIT)
        {
            LookPlayer(directionToPlayer);
        }

        else if (state == State.CHASE)
        {
            ChasePlayer();
        }

        else if (state == State.BACK)
        {
            ReturnToBase();
        }

        if (state == State.CHECK)
        {
            CheckPlayer();
        }
        if(state == State.ATTACK_READY || state == State.NEAR_ATTACK_READY)
        {
            AttackPlayer();
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

    void ChasePlayer()                      //�߰� ���
    {
        nmAgent.speed = 2;
        nmAgent.SetDestination(player.position);
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
        nmAgent.speed = 2f;
        if (distanceToBase > 3)  // �Ÿ��� 0.1 �̻��� ��쿡�� �̵�
        {
            nmAgent.SetDestination(initialPoint);
        }
        else
        {
            Debug.Log("������");
            ChangeState(State.IDLE); //idle ���·� ����
        }
    }

    void CheckPlayer()
    {
        nmAgent.speed = 1.1f;
        // ������ �ʱ�ȭ���� �ʾ����� 50% Ȯ���� �¿� �̵� ������ ����
        if (!directionInitialized)
        {
            moveRight = UnityEngine.Random.Range(0f, 1f) >= 0.5f;
            directionInitialized = true;  // ������ �ʱ�ȭ�Ǿ����� ǥ��
        }

        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            timeSinceLastCheck = 0f;

            // 50% Ȯ���� �¿� �̵� ���� ����
            if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
            {
                moveRight = !moveRight;
            }
        }

        // �÷��̾���� ���� ���� ���
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // �÷��̾���� ���� �Ÿ�
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �¿�� �̵��� ������ ��� (���� Ȥ�� �������� 90�� ȸ��)
        Vector3 moveDirection;
        if (moveRight)
        {
            moveDirection = Quaternion.Euler(0, 90, 0) * directionToPlayer;  // �������� ȸ��
        }
        else
        {
            moveDirection = Quaternion.Euler(0, -90, 0) * directionToPlayer; // �������� ȸ��
        }

        // ��ǥ ��ġ ���
        Vector3 targetPosition = player.position - directionToPlayer * checkRange + moveDirection * 2f;

        // NavMeshAgent�� �������� ����
        nmAgent.SetDestination(targetPosition);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttackHit && !isinvincibility)
        {
            isinvincibility = true;
            Debug.Log("������!");
            isHit = true;
        }

        if (other.CompareTag("magic") && !isinvincibility)
        {
            isinvincibility = true;
            Debug.Log("������!");
            isHit = true;
        }
    }

    void AttackPlayer()
    {
        nmAgent.SetDestination(transform.position);
    }
    // �ۼ��� �̰�




    public void InitializeHPBar(GameObject hpSliderPrefab)
    {
        hpSliderObject = Instantiate(hpSliderPrefab, GameObject.Find("Canvas").transform); // �����̴� ���� �� �θ� Canvas�� ����
        hpSlider = hpSliderObject.GetComponent<Slider>();
        //hpSliderObject.SetActive(true);
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value = HP / MaxHP; // �����̴� �� ������Ʈ
            PositionHPBarAboveMonster(); // HP �����̴� ��ġ ������Ʈ
        }
    }

    private void PositionHPBarAboveMonster()
    {
        if (hpSliderObject != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 3f); // �Ӹ� ���� ��ġ ����
            hpSliderObject.transform.position = screenPosition; // UI �����̴� ��ġ ����
        }
    }

    void AttackPoint()
    {
        bowattackRange.GetComponent<Collider>().enabled = true;
        kickattackRange.GetComponent<Collider>().enabled = true;
    }
    void AttackEnd()
    {
        bowattackRange.GetComponent<Collider>().enabled = false;
        kickattackRange.GetComponent<Collider>().enabled = false;
    }

}

