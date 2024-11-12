using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class EnemyMonsterController : MonoBehaviour, Ienemy
{
    public Animator anim; //�ִϸ�����
    NavMeshAgent nmAgent; //navmeshagent �߰�

    public GameObject player_1;
    public Transform player; //�÷��̾� Ÿ��

    public LayerMask playerLayer;
    public GameObject attackWeaponRange;

    private bool firstlooking = true; //ĳ���� ���� ���

    private float HP = 0; //�� ü�� ���� �� �ʱ�ȭ
    public float MaxHP = 10;
    private float detectingRange = 20f;         //�� Ž�� �Ÿ�
    private float sensingRange = 13.5f;         //�� ���� �Ÿ�
    private float checkRange = 7f;
    private float attackRange = 2f;           //�� ���� ��Ÿ�
    private float smoothRotationSpeed = 15f;     //�� ����ȭ ȸ�� �ӵ�
    //public float moveSpeed = 4.0f;             //�� �̵��ӵ�
    //private float returnSpeed = 2f;           //�� ���ͼӵ�
    public float Damage = 10f;
    float Ienemy.Damage => Damage;

    public bool isHit = false;                  //���� ����
    public bool isinvincibility = false;      //��������
    public bool isAttack = false;
    public bool isDead = false;

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

    Vector3 initialPoint; //�� ��ġ ��ġ ���� ����

    private bool isAttacked = false;
    private Slider hpSlider;                     // ���� HP �����̴�
    private GameObject hpSliderObject;          // �����̴� UI ������Ʈ


    public float enemySoul = 30f;

    enum State
    {
        IDLE,
        CHASE,
        ATTACK1,
        ATTACK2,
        BACK,
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
        if (curAnimStateInfo.IsName("idle") == false)
            anim.Play("Creep|Idle1_Action", 0, 0);

        if (distanceToPlayer <= detectingRange && player_1.GetComponent<SuperPlayerController>().isStand) //Ž�� ���� �ȿ� �÷��̾ ������
        {
            // StateMachine �� �߰����� ����
            ChangeState(State.CHASE);
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
        if (curAnimStateInfo.IsName("run") == false)
        {
            anim.CrossFade("run", 0.1f);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }

        //�ִϸ��̼� ��ȯ üũ
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("run"))
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
            // 50% Ȯ���� ATTACK1 �Ǵ� ATTACK2�� ����
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                ChangeState(State.ATTACK1);
            }
            else
            {
                ChangeState(State.ATTACK2);
            }

            yield break;  // �ڷ�ƾ ����
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
        if (distanceToBase >= 50.0f)
        {
            ChangeState(State.BACK);
        }

        yield return null;
    }

    IEnumerator ATTACK1()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("attack_1", 0.1f, 0, 0);

        float attackDuration = 1.6f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("attack_1"))
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
    }

    IEnumerator ATTACK2()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("attack_2", 0.1f, 0, 0);

        float attackDuration = 3.1f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("attack_2"))
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

        if (isHit)
        {
            ChangeState(State.HIT);
            yield break;
        }

        yield return null;
    }

    IEnumerator HIT()
    {
        //�ۼ��� �̰�
        if (isAttacked != true)
        {
            isAttacked = true;
            hpSliderObject.SetActive(true);
        }
        //�ۼ��� �̰�

        isAttack = false;

        nmAgent.speed = 0f;
        HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (HP > 0)
        {

            if (!curAnimStateInfo.IsName("hit"))
            {
                anim.Play("hit", 0, 0);
                isHit = false;


                // �ִϸ��̼� ���°� ����� ������ ��ٸ���
                yield return null;

                // ���¸� �ٽ� ������
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                while (!curAnimStateInfo.IsName("hit"))
                {
                    yield return null;
                    curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                }
            }

            // �ִϸ��̼��� Walk1_Action ����� ������ ���
            while (curAnimStateInfo.IsName("hit"))
            {
                yield return null;
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            }
            isinvincibility = false;

            if (curAnimStateInfo.IsName("idle"))
            {
                ChangeState(State.ATTACK2);
            }

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
        if (moveRight)
        {
            anim.CrossFade("check_r", 0.1f, 0, 0);
        }
        else
        {
            anim.CrossFade("check_l", 0.1f, 0, 0);
        }

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
        if (distanceToPlayer <= attackRange)
        {
            // 50% Ȯ���� ATTACK1 �Ǵ� ATTACK2�� ����
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                ChangeState(State.ATTACK1);
            }
            else
            {
                ChangeState(State.ATTACK2);
            }

            yield break;  // �ڷ�ƾ ����
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
        anim.Play("die");

        // �ִϸ��̼��� normalizedTime�� 1.0�� ������� ������ ���
        while (true)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // �ִϸ��̼��� normalizedTime�� 1.0 �̻��� �� �ִϸ��̼��� �Ϸ�� ������ ����
            if (curAnimStateInfo.IsName("die") && curAnimStateInfo.normalizedTime >= 1.0f)
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

        if (state == State.ATTACK1 || state == State.CHECK || state == State.HIT || state == State.ATTACK2)
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

        if (isAttack)
        {
            // ���� ������ �� �ݶ��̴��� Ȱ��ȭ�մϴ�.
            EnableAttackColliders(true);
        }
        else
        {
            // ���� ���°� �ƴ� �� �ݶ��̴��� ��Ȱ��ȭ�մϴ�.
            EnableAttackColliders(false);
        }


        // �ִϸ��̼ǿ��� ��Ʈ ����� ����Ͽ� �̵�
        Vector3 movement = anim.deltaPosition; // �ִϸ��̼��� ��ȭ��
        transform.position += movement; // ���� ��ġ ������Ʈ

    }


    void ChangeState(State newState)
    {
        state = newState;
    }

    void ChasePlayer()                      //�߰� ���
    {
        nmAgent.speed = 4;
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
            firstlooking = true; //���� ��� Ȱ��ȭ�ϰ�
            ChangeState(State.IDLE); //idle ���·� ����
        }
    }

    void CheckPlayer()
    {
        nmAgent.speed = 0f;
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

            // 80% Ȯ���� �¿� �̵� ���� ����
            if (UnityEngine.Random.Range(0f, 1f) < 0.8f)
            {
                moveRight = !moveRight;
            }
        }
    }


    //���� ��Ȱ��ȭ or Ȱ��ȭ ���
    void EnableAttackColliders(bool enable)
    {
        // attackRangeL�� attackRangeR�� �ݶ��̴��� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
        if (attackWeaponRange != null)
        {
            attackWeaponRange.GetComponent<Collider>().enabled = enable;
        }
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
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f); // �Ӹ� ���� ��ġ ����
            hpSliderObject.transform.position = screenPosition; // UI �����̴� ��ġ ����
        }
    }

    void TriggerAttack()
    {
        isAttack = true;
    }

    void EndAttack()
    {
        isAttack = false;
    }   



}
