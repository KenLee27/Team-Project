using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public Animator anim; //�ִϸ�����
    NavMeshAgent nmAgent; //navmeshagent �߰�

    public GameObject player_1;

    public Transform player; //�÷��̾� Ÿ��
    public LayerMask playerLayer;
    public GameObject attackRangeL;         //���ݹ��� ����
    public GameObject attackRangeR;         //���ݹ��� ������
    public GameObject attackRangeH;         //���ݹ��� �Ӹ�


    private float HP = 0; //�� ü�� ���� �� �ʱ�ȭ
    private float MaxHP = 30;
    private float detectingRange = 30f;         //�� Ž�� �Ÿ�
    private float sensingRange = 20f;         //�� ���� �Ÿ�
    private float checkRange = 7f;
    private float attackRange = 5f;           //�� ���� ��Ÿ�
    private float smoothRotationSpeed = 15f;     //�� ����ȭ ȸ�� �ӵ�
    private float stunMax = 50f;
    private float stunGauge = 0f;
    private float alertSpeed = 2f;
    private float dashSpeed = 12f;
    private float chaseSpeed = 6f;

    public bool isHit = false;                  //���� ����
    public bool isAttack = false;
    public bool isLittleStun = false;
    public bool isBigStun = false;
    public bool firstStunCheck = true;

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

    enum State
    {
        IDLE,   //�Ϸ�
        CHASE,  //�Ϸ�
        ATTACK_READY_1, //�Ϸ�
        ATTACK_READY_2, //�Ϸ�
        ATTACK_1,  //�Ϸ�
        ATTACK_2,  //�Ϸ�
        ATTACK_3,  //�Ϸ�
        LITTLE_ATTACK,  //�Ϸ�
        DASH_ATTACK_READY,   //�Ϸ�
        DASH_ATTACK,  //�Ϸ�
        DASH,       //�Ϸ�
        BACK,    //�Ϸ�
        CHECK,    //�Ϸ�
        STUN,
        BIG_STUN,
        DIE    //�Ϸ�
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








    // �ۼ��� �̰�


    private Slider hpSlider;                     // ���� HP �����̴�
    private GameObject hpSliderObject;          // �����̴� UI ������Ʈ

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
        if (curAnimStateInfo.IsName("Sleep") == false)
            anim.Play("Sleep", 0, 0);

        if (distanceToPlayer <= detectingRange && player_1.GetComponent<SuperPlayerController>().isStand) //Ž�� ���� �ȿ� �÷��̾ ������
        {
            // StateMachine �� ���� ����
            ChangeState(State.DASH);
        }

        if (isHit)  //��� �����
        {
            ChangeState(State.BIG_STUN);
        }
        yield return null;

        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }
    }

    IEnumerator DASH()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //�ִϸ��̼� ��ȯ ���
        if (curAnimStateInfo.IsName("DASH") == false)
        {
            anim.CrossFade("DASH", 0.1f);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }

        //�ִϸ��̼� ��ȯ üũ
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("DASH"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        if (isHit && isLittleStun)  //����ġ
        {
            ChangeState(State.STUN);
            yield break;
        }

        //�������
        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }

        // �÷��̾���� ���� �Ÿ��� ���� �������� �۰ų� ������
        if (distanceToPlayer <= attackRange)
        {
            ChangeState(State.DASH_ATTACK);
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

        //�������
        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }

        if (isHit&&isLittleStun)  //����ġ
        {
            ChangeState(State.STUN);
            yield break;
        }

        

        // �÷��̾���� ���� �Ÿ��� ���� �������� �۰ų� ������
        if (distanceToPlayer <= attackRange)
        {
            //���� ���� ���� ����
            float randomValue = UnityEngine.Random.Range(0f, 10f);
            // StateMachine �� �������� ����
            if(randomValue <= 5f)
            {
                ChangeState(State.ATTACK_READY_1);
            }

            else if (randomValue > 5f && randomValue <= 7.5f)
            {
                ChangeState(State.ATTACK_READY_2);
            }

            else
            {
                ChangeState(State.LITTLE_ATTACK);
            }

        }


        if (distanceToBase >= 50f) //�� �Ÿ� 50f �־����� ����
        {
            ChangeState(State.BACK);
        }

        yield return null;
    }

    IEnumerator ATTACK_READY_1()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Fight_Read_1", 0.1f, 0, 0);

        float checkTime = UnityEngine.Random.Range(0.5f, 2f);           //���� �غ� �ð� ����
        float elapsedTime = 0f;
        while (elapsedTime < checkTime)
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            //����ġ
            if (isHit && isLittleStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            if (isHit && isBigStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float checkTime2 = UnityEngine.Random.Range(0f, 2f);
        //���� ��� ���� �ϳ� ���
        if (checkTime2 > 1f)
            ChangeState(State.ATTACK_1);
        else
            ChangeState(State.ATTACK_2);

        yield return null;
    }

    IEnumerator ATTACK_READY_2()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Alert_Walk", 0.1f, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            //����ġ
            if (isHit && isLittleStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            if (isHit && isBigStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ChangeState(State.ATTACK_3);
        yield return null;
    }

    IEnumerator DASH_ATTACK_READY()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Fight_Ready_3", 0.1f, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 3f)                                            //�뽬 �غ� �ð�
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            //����ġ
            if (isHit && isLittleStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            if (isHit && isBigStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
            
        }

        if (distanceToPlayer <= attackRange)
        {
            ChangeState(State.DASH_ATTACK);
        }

        else
        {
            ChangeState(State.DASH);
        }
        yield return null;

    }

    IEnumerator STUN()
    {
        isLittleStun = false;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Take_Damage_Little", 0.1f, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 0.8f)                                            //���� ���� �ð�
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;

        }
        ChangeState(State.CHECK);
    }

    IEnumerator BIG_STUN()
    {
        isBigStun = false;
        firstStunCheck = true;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Take_Damage_Large", 0.1f, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 1.8f)                                            //���� ���� �ð�
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;

        }
        ChangeState(State.CHECK);
    }

    IEnumerator ATTACK_1()
    {
        anim.CrossFade("Attack_1", 0.05f, 0, 0);
        yield return null;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = curAnimStateInfo.length;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            if (isHit && isBigStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // �ڷ�ƾ ����
            }
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("Attack_1"))
            {
                

                if (elapsedTime > 0.25f && elapsedTime < 0.417f)                   //���� ���� �ð�
                {
                    
                    isAttack = true;
                    
                }
                else
                {
                    isAttack = false;
                }
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
        anim.CrossFade("Attack_2", 0.05f, 0, 0);
        yield return null;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = curAnimStateInfo.length;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            if (isHit && isBigStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // �ڷ�ƾ ����
            }

            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("Attack_2"))
            {
                
                if (elapsedTime > 0.4f && elapsedTime < 0.63f)                   //���� ���� �ð�
                {

                    isAttack = true;

                }
                else
                {
                    isAttack = false;
                }
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
        anim.CrossFade("Attack_3", 0.05f, 0, 0);
        yield return null;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = curAnimStateInfo.length;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            if (isHit && isBigStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // �ڷ�ƾ ����
            }

            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("Attack_3"))
            {
                if (elapsedTime > 0.36f && elapsedTime < 0.667f)                   //���� ���� �ð�
                {

                    isAttack = true;

                }
                else
                {
                    isAttack = false;
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator LITTLE_ATTACK()
    {
        anim.CrossFade("Attack_4", 0.05f, 0, 0);
        yield return null;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = curAnimStateInfo.length;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            if (isHit && isBigStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // �ڷ�ƾ ����
            }

            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("Attack_4"))
            {

                if (elapsedTime > 0.5f && elapsedTime < 0.708f)                   //���� ���� �ð�
                {

                    isAttack = true;

                }
                else
                {
                    isAttack = false;
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator DASH_ATTACK()
    {
        anim.CrossFade("Attack_5", 0.05f, 0, 0);
        yield return null;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = curAnimStateInfo.length;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            if (isHit && isBigStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // �ڷ�ƾ ����
            }

            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (curAnimStateInfo.IsName("Attack_5"))
            {
                if (elapsedTime > 0f && elapsedTime < 0.5f)                   //���� ���� �ð�
                {

                    isAttack = true;

                }
                else
                {
                    isAttack = false;
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine�� ������� ����
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

        //�������
        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }
        if (isHit && isLittleStun)                                                      //����ġ
        {
            isAttack = false;
            ChangeState(State.STUN);
            yield break;  // �ڷ�ƾ ����
        }

        if (isHit && isBigStun)                                                      //����ġ
        {
            isAttack = false;
            ChangeState(State.BIG_STUN);
            yield break;  // �ڷ�ƾ ����
        }

        yield return null;
    }

    //������ ���� ����
    IEnumerator CHECK()
    {
        Debug.Log("�ֽ���...");

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Alert_Walk", 0.1f, 0, 0);

        float checkTime = UnityEngine.Random.Range(2f, 4f);                            //���� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < checkTime)
        {
            //�������
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // �ڷ�ƾ ����
            }

            if (isHit && isBigStun)                                                      //����ġ
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��ǥ���� �Ÿ��� �����Ÿ� ���� �־��� ���
        if (sensingRange < distanceToPlayer)
        {
            float randomValue = UnityEngine.Random.Range(0f, 10f);
            //75% Ȯ���� �߰�
            if (randomValue <= 7.5f)
            {
                ChangeState(State.CHASE);
            }
            //25% Ȯ���� �뽬�����غ�
            else
            {
                ChangeState(State.DASH_ATTACK_READY);
            }
        }

        // �÷��̾���� ���� �Ÿ��� ���� �������� �۰ų� ������
        if (distanceToPlayer <= attackRange)
        {
            //���� ���� ���� ����
            float randomValue = UnityEngine.Random.Range(0f, 10f);
            // StateMachine �� �������� ����
            if (randomValue <= 5f)
            {
                ChangeState(State.ATTACK_READY_1);
            }

            else if (randomValue > 5f && randomValue <= 7.5f)
            {
                ChangeState(State.ATTACK_READY_2);
            }

            else
            {
                ChangeState(State.LITTLE_ATTACK);
            }
        }

        //50f �̻� �Ÿ��� ��������
        if (50f < distanceToPlayer)
        {
            // StateMachine�� BACK���� ����
            ChangeState(State.BACK);
            yield break;            // �ڷ�ƾ ����
        }


        ChangeState(State.CHASE);

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
        anim.Play("Die");

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

        //�ۼ��� �̰�
        Destroy(hpSliderObject);
        //�ۼ��� �̰�
    }






    void Update()
    {
        StunCheck();
        UpdateHPBar(); // �ۼ��� �̰�

        directionToPlayer = new Vector3(player.position.x - transform.position.x,
            0f, player.position.z - transform.position.z);    //�÷��̾���� ��ġ����
        distanceToPlayer = directionToPlayer.magnitude;                                       //�÷��̾���� ��ġ���� ��ġȭ

        directionToBase = new Vector3(initialPoint.x - transform.position.x,
            0f, initialPoint.z - transform.position.z);
        distanceToBase = directionToBase.magnitude;

        if ( state == State.ATTACK_READY_1 || state == State.ATTACK_READY_2 || state == State.CHECK || state == State.STUN || state ==  State.BIG_STUN || state == State.DASH || state == State.ATTACK_1 || state == State.ATTACK_2
            || state == State.ATTACK_3 || state == State.LITTLE_ATTACK || state == State.DASH_ATTACK_READY || state == State.DASH_ATTACK ||state == State.DIE)
        {
            LookPlayer(directionToPlayer);
        }

        else if (state == State.CHASE)
        {
            ChasePlayer();
        }

        else if (state == State.DASH)
        {
            DashPlayer();
        }

        else if (state == State.BACK)
        {
            ReturnToBase();
        }

        else if(state == State.CHECK)
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

        
    }

    void ChangeState(State newState)
    {
        state = newState;
    }

    void StunCheck()
    {
        if (stunGauge > 27 && firstStunCheck)
        {
            isLittleStun = true;
            firstStunCheck = false;
        }

        if (stunGauge > stunMax)
        {
            isBigStun = true;
            stunGauge = 0;
        }
    }

    void ChasePlayer()                      //�߰� ���
    {
        nmAgent.speed = chaseSpeed;
        nmAgent.SetDestination(player.position);
    }

    void DashPlayer()                      //�߰� ���
    {
        nmAgent.speed = dashSpeed;
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
        if (distanceToBase > 3)  // �Ÿ��� 3 �̻��� ��쿡�� �̵�
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
        Vector3 targetPosition = player.position - directionToPlayer * checkRange + moveDirection * 2f;

        // NavMeshAgent�� �������� ����
        nmAgent.SetDestination(targetPosition);
    }


    //���� ��Ȱ��ȭ or Ȱ��ȭ ���
    void EnableAttackColliders(bool enable)
    {
        // attackRangeL�� attackRangeR�� �ݶ��̴��� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
        if (attackRangeL != null)
        {
            attackRangeL.GetComponent<Collider>().enabled = enable;
        }

        if (attackRangeR != null)
        {
            attackRangeR.GetComponent<Collider>().enabled = enable;
        }

        if (attackRangeH != null)
        {
            attackRangeH.GetComponent<Collider>().enabled = enable;
        }
    }
    //������ ����
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttackHit)
        {
            HP -= 1;
            stunGauge += 3f;
            Debug.Log("������!");
            isHit = true;
        }
    }

}
