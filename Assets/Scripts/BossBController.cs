using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBController : MonoBehaviour, Ienemy
{

    public Animator anim; //�ִϸ�����
    UnityEngine.AI.NavMeshAgent nmAgent; //navmeshagent �߰�

    public GameObject player_1;

    public Transform player; //�÷��̾� Ÿ��
    public LayerMask playerLayer;
    public GameObject weaponAttackRange;         //���ݹ���


    private float HP = 0; //�� ü�� ���� �� �ʱ�ȭ
    private float MaxHP = 150;
    private float detectingRange = 29f;         //�� Ž�� �Ÿ�
    private float checkRange = 6f;             //��������Ÿ�
    private float attackRange = 7f;           //�� ���� ��Ÿ�
    private float smoothRotationSpeed = 15f;     //�� ����ȭ ȸ�� �ӵ�
    private float stunMax = 80f;
    private float stunGauge = 0f;
    private float alertSpeed = 1.6f;
    private float dashSpeed = 11f;
    private float chaseSpeed = 8f;
    public float Damage = 30f;

    private Slider hpSlider;                     // ���� HP �����̴�
    private GameObject hpSliderObject;          // �����̴� UI ������Ʈ

    float Ienemy.Damage => Damage;

    public bool isHit = false;                  //���� ����
    public bool isAttack = false;
    public bool isLittleStun = false;
    public bool firstStunCheck = true;
    private bool isAlreadyHit = false;          //�ǰ� ���� ����
    public bool is2Phase = false;
    public bool is3Phase = false;

    Vector3 directionToPlayer;
    Vector3 directionToBase;

    float distanceToPlayer = 100f;
    float distanceToBase = 0f;
    private float attackCount = 0f;
    private float attackMaxCount = 3f;


    private Quaternion targetRotation;
    private float currentSpeed;


    private bool moveRight;  // �¿� �̵� ���� ��� ���� ����
    private bool directionInitialized = false;  // ������ �ʱ�ȭ�Ǿ����� ���θ� Ȯ���ϴ� ����
    private float timeSinceLastCheck = 0f;
    private float checkInterval = 2f; // 2�ʸ��� üũ
    public float enemySoul = 150f;
    public bool isDead = false;

    Vector3 initialPoint; //�� ��ġ ��ġ ���� ����

    enum State
    {
        IDLE, //�⺻
        CHASE, //�߰�
        ATTACK_1, //��Ÿ1
        ATTACK_2, //��Ÿ2 
        ATTACK_3, //��Ÿ3
        ATTACK_4, //��Ÿ4
        ATTACK_5, //��Ÿ5
        COMBO_1, //3������ ���� ����
        COMBO_2, //�޺�2
        COMBO_3, //�޺�3
        JUMP_ATTACK, //2�� ���� ����
        BACK,  //��ȯ
        CHECK,  //��Ȳ����
        STUN,  //����
        DIE,  //���
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
        nmAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
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

    //���� �� ����
    IEnumerator CHECK()
    {
        smoothRotationSpeed = 15f;

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("move_walk_front", 0.2f, 0, 0);


        float checkTime = 1f;                         //���� ���� �ð�
        if(is2Phase)
        {
            checkTime = 0.1f;
        }
        float elapsedTime = 0f;
        while (elapsedTime < checkTime)
        {

            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP > 40 && HP < 80 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.JUMP_ATTACK);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            if (isHit && isLittleStun)                                                   //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��ǥ���� �Ÿ��� �����Ÿ� ���� �־��� ���
        if (attackRange < distanceToPlayer)
        {
            ChangeState(State.CHASE);                       //�߰�
            yield break;  // �ڷ�ƾ ����
        }

        //��Ÿ ��������
        if (attackCount >= attackMaxCount)
        {
            float randomValue = UnityEngine.Random.Range(0f, 6f);

            if (randomValue < 3)
            {
                ChangeState(State.COMBO_2);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 6)
            {
                ChangeState(State.COMBO_3);
                yield break;  // �ڷ�ƾ ����
            }
        }
        if (distanceToPlayer <= attackRange)
        {
            float randomValue = UnityEngine.Random.Range(0f, 6f);

            if (randomValue < 2)
            {
                ChangeState(State.ATTACK_1);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 4)
            {
                ChangeState(State.ATTACK_2);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 6)
            {
                ChangeState(State.ATTACK_3);
                yield break;  // �ڷ�ƾ ����
            }
        }
        else if(distanceToPlayer <= 10f)
        {
            float randomValue = UnityEngine.Random.Range(0f, 6f);

            if (randomValue < 3)
            {
                ChangeState(State.ATTACK_4);
                yield break;  // �ڷ�ƾ ����
            }
            else 
            {
                ChangeState(State.ATTACK_5);
                yield break;  // �ڷ�ƾ ����
            }
        }

        //50f �̻� �Ÿ��� ��������
        if (50f < distanceToPlayer)
        {
            // StateMachine�� BACK���� ����
            ChangeState(State.BACK);
            yield break;            // �ڷ�ƾ ����
        }

        ChangeState(State.CHECK);
        yield break;

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
        anim.Play("dead");

        // �ִϸ��̼��� normalizedTime�� 1.0�� ������� ������ ���
        while (true)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // �ִϸ��̼��� normalizedTime�� 1.0 �̻��� �� �ִϸ��̼��� �Ϸ�� ������ ����
            if (curAnimStateInfo.IsName("dead") && curAnimStateInfo.normalizedTime >= 1.0f)
            {
                break;
            }

            yield return null; // �� ������ ���
        }

        GameManager.Instance.UpdatePlayerSOUL(enemySoul);

        // �ִϸ��̼��� ���� �� ������Ʈ�� ����
        Destroy(gameObject);
        Destroy(hpSliderObject);
    }

    IEnumerator IDLE()
    {
        // ���� animator �������� ���
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼� �̸��� Idle �� �ƴϸ� Play
        if (curAnimStateInfo.IsName("idle") == false)
            anim.Play("idle", 0, 0);

        if (distanceToPlayer <= detectingRange) //Ž�� ���� �ȿ� �÷��̾ ������
        {
            hpSliderObject.SetActive(true);
            // StateMachine �� ��÷� ����
            ChangeState(State.CHECK);
            yield break;
        }

        yield return null;

        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }
    }

    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //�ִϸ��̼� ��ȯ ���
        if (curAnimStateInfo.IsName("move_run_front") == false)
        {
            anim.CrossFade("move_run_front", 0.1f, 0, 0);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }

        //�ִϸ��̼� ��ȯ üũ
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("move_run_front"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        //�������
        if (HP <= 0)
        {
            ChangeState(State.DIE);
            yield break;
        }

        if (HP > 40 && HP < 80 && !is2Phase)
        {
            isAttack = false;
            ChangeState(State.JUMP_ATTACK);
            yield break;
        }

        if (HP <= 40 && !is3Phase)
        {
            isAttack = false;
            ChangeState(State.COMBO_1);
            yield break;
        }

        if (isHit && isLittleStun)                                                   //����ġ
        {
            isAttack = false;
            ChangeState(State.STUN);
            yield break;  // �ڷ�ƾ ����
        }

        //������ ���� ����

        if (attackCount >= attackMaxCount)
        {
            float randomValue = UnityEngine.Random.Range(0f, 6f);

            if (randomValue < 3)
            {
                ChangeState(State.COMBO_2);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 6)
            {
                ChangeState(State.COMBO_3);
                yield break;  // �ڷ�ƾ ����
            }
        }

        if (distanceToPlayer <= attackRange)
        {
            float randomValue = UnityEngine.Random.Range(0f, 6f);

            if (randomValue < 2)
            {
                ChangeState(State.ATTACK_1);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 4)
            {
                ChangeState(State.ATTACK_2);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 6)
            {
                ChangeState(State.ATTACK_3);
                yield break;  // �ڷ�ƾ ����
            }
        }
        else if (distanceToPlayer <= 10f)
        {
            float randomValue = UnityEngine.Random.Range(0f, 6f);

            if (randomValue < 3)
            {
                ChangeState(State.ATTACK_4);
                yield break;  // �ڷ�ƾ ����
            }
            else
            {
                ChangeState(State.ATTACK_5);
                yield break;  // �ڷ�ƾ ����
            }
        }

        if (distanceToBase >= 50f) //�� �Ÿ� 50f �־����� ����
        {
            hpSliderObject.SetActive(false);
            ChangeState(State.BACK);
            yield break;
        }

        yield return null;
    }

    IEnumerator STUN()
    {
        nmAgent.speed = 0;
        isLittleStun = false;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("hit_body_back", 0.1f, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 1.1f)                                            //���� ���� �ð�
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP > 40 && HP < 80 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.JUMP_ATTACK);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;

        }
        ChangeState(State.CHECK);
    }

    IEnumerator BACK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);


        if (curAnimStateInfo.IsName("move_walk_front") == false)
        {
            anim.CrossFade("move_walk_front", 0.1f, 0, 0);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }
        //�ִϸ��̼� ��ȯ ���

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("move_walk_front"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        //�ִϸ��̼� ��ȯ üũ

        //�������
        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }

        yield return null;
    }

    //�������ϵ�

    IEnumerator ATTACK_1()
    {
        if(is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack01", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 5.3f; // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP > 40 && HP < 80 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.JUMP_ATTACK);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            if (isHit && isLittleStun)                                                   //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_2()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack02", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 4.4f; // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP > 40 && HP < 80 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.JUMP_ATTACK);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            if (isHit && isLittleStun)                                                   //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_3()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("combo02", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 5.4f;  // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP > 40 && HP < 80 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.JUMP_ATTACK);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            if (isHit && isLittleStun)                                                   //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_4()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("Combo03_2", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.4f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP > 40 && HP < 80 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.JUMP_ATTACK);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            if (isHit && isLittleStun)                                                   //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_5()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("Combo04_2", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.675f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP > 40 && HP < 80 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.JUMP_ATTACK);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            if (isHit && isLittleStun)                                                   //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator COMBO_1()
    {
        smoothRotationSpeed = 15f;     //�� ����ȭ ȸ�� �ӵ�
        if(HP <= 40)
        {
            is3Phase = true;
        }
        attackMaxCount = 2;

        anim.CrossFade("buff02", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 9f;  // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP > 40 && HP < 80 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.JUMP_ATTACK);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            if (isHit && isLittleStun)                                                   //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }   

    IEnumerator COMBO_2()
    {
        if (is2Phase)
        {
            attackCount = 0;
        }
        anim.CrossFade("Combo03", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 8.3f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP > 40 && HP < 80 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.JUMP_ATTACK);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            if (isHit && isLittleStun)                                                   //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator COMBO_3()
    {
        if (is2Phase)
        {
            attackCount = 0;
        }
        anim.CrossFade("Combo04", 0.04f, 0, 0);

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 7.8f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP > 40 && HP < 80 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.JUMP_ATTACK);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            if (isHit && isLittleStun)                                                   //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator JUMP_ATTACK()
    {
        smoothRotationSpeed = 15f;     //�� ����ȭ ȸ�� �ӵ�
        if (HP < 80 && HP >=40)
        {
            is2Phase = true;
        }

        anim.CrossFade("buff01", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 9.88f;  // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP <= 40 && !is3Phase)
            {
                isAttack = false;
                ChangeState(State.COMBO_1);
                yield break;
            }

            if (isHit && isLittleStun)                                                   //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }







    void Update()
    {
        StunCheck();
        UpdateHPBar();

        directionToPlayer = new Vector3(player.position.x - transform.position.x,
            0f, player.position.z - transform.position.z);    //�÷��̾���� ��ġ����
        distanceToPlayer = directionToPlayer.magnitude;                                       //�÷��̾���� ��ġ���� ��ġȭ

        directionToBase = new Vector3(initialPoint.x - transform.position.x,
            0f, initialPoint.z - transform.position.z);
        distanceToBase = directionToBase.magnitude;

        if (state != State.BACK && state != State.DIE && state != State.IDLE)
        {
            LookPlayer(directionToPlayer);
        }

        if (state == State.CHASE)
        {
            ChasePlayer();
        }

        if ( state == State.DIE || state == State.ATTACK_1 || state == State.ATTACK_2 || state == State.ATTACK_3 || state == State.ATTACK_4 || state == State.ATTACK_5
            || state == State.COMBO_1 || state == State.COMBO_2 || state == State.COMBO_3 || state == State.STUN || state == State.JUMP_ATTACK )
        {
            Stopnow();
        }

        if (state == State.BACK)
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

    public void TriggerAttack()
    {
        isAttack = true;
    }

    public void EndAttack()
    {
        isAttack = false;
    }

    public void CantRot()
    {
        smoothRotationSpeed = 0.1f;
    }

    public void CanRot()
    {
        smoothRotationSpeed = 3f;
    }

    public void FastRot()
    {
        smoothRotationSpeed = 15f;
    }

    public void InitializeHPBar(GameObject hpSliderPrefab)
    {
        hpSliderObject = Instantiate(hpSliderPrefab, GameObject.Find("Canvas").transform); // �����̴� ���� �� �θ� Canvas�� ����
        hpSlider = hpSliderObject.GetComponent<Slider>();
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value = HP / MaxHP; // �����̴� �� ������Ʈ
        }
    }

    void ChangeState(State newState)
    {
        state = newState;
    }

    void StunCheck()
    {
        if (stunGauge > stunMax)
        {
            isLittleStun = true;
            stunGauge = 0;
        }
    }

    void ChasePlayer()                      //�߰� ���
    {
        nmAgent.speed = chaseSpeed;
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
        nmAgent.speed = 4f;
        if (distanceToBase > 6f)  // �Ÿ��� 3 �̻��� ��쿡�� �̵�
        {
            nmAgent.SetDestination(initialPoint);
        }
        else
        {
            Debug.Log("������");
            HP = MaxHP;
            ChangeState(State.IDLE); //idle ���·� ����
        }
    }

    void CheckPlayer()
    {
        nmAgent.speed = alertSpeed;  //��� �»��϶��� ������
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
        Vector3 targetPosition = player.position - directionToPlayer * checkRange + moveDirection * 10f;

        // NavMeshAgent�� �������� ����
        nmAgent.SetDestination(targetPosition);
    }

    void Stopnow()
    {
        nmAgent.SetDestination(transform.position);
    }


    //���� ��Ȱ��ȭ or Ȱ��ȭ ���
    void EnableAttackColliders(bool enable)
    {
        // attackRangeL�� attackRangeR�� �ݶ��̴��� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
        if (weaponAttackRange != null)
        {
            weaponAttackRange.GetComponent<Collider>().enabled = enable;
        }

    }
    //������ ����
    void OnTriggerEnter(Collider other)
    {
        if (isAlreadyHit) { return; }
        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttackHit)
        {
            HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
            stunGauge += 3f;
            Debug.Log("������!");
            isHit = true;

            isAlreadyHit = true; // Ÿ�ݵǾ����� ǥ��
            StartCoroutine(ResetHit()); // ���� �ð� �� �÷��� �ʱ�ȭ

        }

        if (other.CompareTag("magic"))
        {
            HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
            stunGauge += 3f;
            Debug.Log("������!");
            isHit = true;

            isAlreadyHit = true; // Ÿ�ݵǾ����� ǥ��
            StartCoroutine(ResetHit()); // ���� �ð� �� �÷��� �ʱ�ȭ

        }

    }


    IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(0.05f); // 0.3�� �� �ʱ�ȭ, �ʿ信 ���� ���� ����
        isAlreadyHit = false;
    }
}
