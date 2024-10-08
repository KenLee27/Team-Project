using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemySuperAIController : MonoBehaviour, Ienemy
{
    public Animator anim; //�ִϸ�����
    UnityEngine.AI.NavMeshAgent nmAgent; //navmeshagent �߰�

    public GameObject player_1;

    public Transform player; //�÷��̾� Ÿ��
    public LayerMask playerLayer;
    public GameObject weaponAttackRange;

    private float HP = 0; //�� ü�� ���� �� �ʱ�ȭ
    private float MaxHP = 50;
    private float sensingRange = 30f;         //�� ���� �Ÿ�
    private float checkRange = 10f;
    private float attackRange = 4f;           //�� ���� ��Ÿ�
    private float smoothRotationSpeed = 15f;     //�� ����ȭ ȸ�� �ӵ�
    public float Damage = 20f;
    float Ienemy.Damage => Damage;
    private float nowStun = 0;
    private float maxStun = 20;
    private float randomAttack = 0;
    private float comboCharge = 0;
    private bool isHeal = false;

    public bool isHit = false;                  //���� ����
    public bool isAttack = false;
    public bool isDead = false;
    public bool isStun = false;
    public bool isGuard = false;
    public bool isAlreadyHit = false;
    public int havePosion = 3;

    private bool isChecking = false;
    private bool isStuned = false;

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

    private Slider hpSlider;                     // ���� HP �����̴�
    private GameObject hpSliderObject;          // �����̴� UI ������Ʈ
    public float enemySoul = 70f;
    private SuperPlayerController playerScript;

    enum State
    {
        IDLE,
        CHASE,
        ATTACK_1,
        ATTACK_2,
        COMBO,
        BACKDIVE,
        DIVE,
        BACKSTEP,
        BACK,
        CHECK,
        STUN,
        TAUNT,
        HEAL,
        DIE
    }

    State state;


    void Awake()
    {
        playerLayer = LayerMask.GetMask("Player");
    }

    void Start()
    {
        if (player_1 != null)
        {
            playerScript = player_1.GetComponent<SuperPlayerController>();
        }

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

    IEnumerator IDLE()
    {
        // ���� animator �������� ���
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼� �̸��� Idle �� �ƴϸ� Play
        if (curAnimStateInfo.IsName("idle") == false)
            anim.Play("idle", 0, 0);

        if (distanceToPlayer <= sensingRange) //Ž�� ���� �ȿ� �÷��̾ ������
        {
            //hpSliderObject.SetActive(true);
            // StateMachine �� ��ȯ ����
            ChangeState(State.TAUNT);
            yield break;
        }
        yield return null;
    }  //�Ϸ�.

    IEnumerator TAUNT()
    {
        hpSliderObject.SetActive(true);
        anim.CrossFade("taunt", 0.1f, 0, 0);

        float attackDuration = 4.5f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            if (distanceToPlayer <= attackRange && playerScript.currentState == SuperPlayerController.State.ATTACK)    //���� �� ��������� ���� ���� �ȿ� ������ ������ �����ϸ�
            {
                yield return new WaitForSecondsRealtime(0.35f);      //�ּ� �����ð� 0.2�� ��
                ChangeState(State.DIVE);        //������ ����
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if(distanceToPlayer <= checkRange) //��� �������� �÷��̾ �������� �ʰ� ������ ������ 
        {
            ChangeState(State.CHECK); //��� ����
            yield break;
        }
        else
        {
            ChangeState(State.CHASE); //�װ͵� �ƴ϶�� �߰�
        }
        yield return null;
    } //�Ϸ�.

    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //�ִϸ��̼� ��ȯ ���
        if (curAnimStateInfo.IsName("chase") == false)
        {
            anim.CrossFade("chase", 0.1f);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }

        //�ִϸ��̼� ��ȯ üũ
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("chase"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        if (HP <= 0)
        {
            ChangeState(State.DIE);
            yield break;
        }

        //��� ���� �߰��� �ƴ϶�� ����.
        if (distanceToPlayer <= attackRange && isChecking == false)
        {
            if (comboCharge >= 5f)
            {
                isGuard = false;
                nmAgent.speed = 0;
                ChangeState(State.COMBO);

                yield break;  // �ڷ�ƾ ����
            }
            float randomValue = UnityEngine.Random.Range(0f, 2f);
            if (randomValue < 1f)
            {
                nmAgent.speed = 0;
                isGuard = false;
                ChangeState(State.ATTACK_1);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 2f)
            {
                nmAgent.speed = 0;
                isGuard = false;
                ChangeState(State.ATTACK_2);
                yield break;  // �ڷ�ƾ ����
            }
        }
        //��� ���� �߰��̶�� �ٽ� ��� ���·�
        if(isChecking == true && distanceToPlayer <= attackRange)
        {
            ChangeState(State.ATTACK_1);
            yield break;
        }

        if (distanceToBase >= 100f)
        {
            ChangeState(State.BACK);
            yield break;
        }

        yield return null;
    } //�Ϸ�.

    IEnumerator ATTACK_1()
    {
        comboCharge++;

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("attack_1", 0.1f, 0, 0);

        float attackDuration = 2.242f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float randomValue = UnityEngine.Random.Range(0f, 1f);
        if (distanceToPlayer < 4f && randomValue > 0.2f)
        {
            ChangeState(State.DIVE);
            yield break;
        }

        ChangeState(State.CHECK);
        yield break;

    }  //�Ϸ�.

    IEnumerator ATTACK_2()
    {
        comboCharge++;
        Damage = 40f;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("attack_2", 0.1f, 0, 0);

        float attackDuration = 3.4f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Damage = 20f;
        ChangeState(State.CHECK);
        yield break;
    }  //�Ϸ�.

    IEnumerator BACKSTEP()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("step_b", 0.1f, 0, 0);

        float attackDuration = 1.15f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if(distanceToPlayer < attackRange)
        {
            ChangeState(State.BACKDIVE);    //�Ÿ��� ������ ������ ������� �Ÿ��� �� ����.
            yield break;
        }

        ChangeState(State.HEAL);        //����� �Ÿ��� ������ ������ ȸ��.
        yield break;
    } //�Ϸ�.

    IEnumerator BACKDIVE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("roll_b", 0.1f, 0, 0);

        float attackDuration = 1.32f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (distanceToPlayer > attackRange && isHeal)
        {
            ChangeState(State.HEAL);
            yield break;
        }
        if (isStuned)
        {
            isStuned = false;
            ChangeState(State.CHASE);
            yield break;
        }
        else if(distanceToPlayer < attackRange && isHeal)   //�Ÿ��� ������ �÷��̾ ������
        {
            isHeal = false;     //���� �ʿ��� ���� ����. ���� ���� �����°� �Ǹ� �ٽ� �� �غ�.
            // �÷��̾���� ���� �Ÿ��� ���� �������� �۰ų� ������

            if (comboCharge >= 5f)
            {
                ChangeState(State.COMBO);
                yield break;  // �ڷ�ƾ ����
            }
            float randomValue = UnityEngine.Random.Range(0f, 2f);

            if (randomValue < 1f)
            {
                ChangeState(State.ATTACK_1);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 2f)
            {
                ChangeState(State.ATTACK_2);
                yield break;  // �ڷ�ƾ ����
            }

        }
        //��� ������ �ƴϸ� ������
        ChangeState(State.CHECK);
        yield break;
    }  //�Ϸ�.

    IEnumerator DIVE()
    {
        //�¿� ���� ������
        float randomDive = UnityEngine.Random.Range(0f, 1f);
        float randomAttack = UnityEngine.Random.Range(0f, 1f);
        if (randomDive < 0.5f)
        {
            anim.CrossFade("roll_r", 0.1f, 0, 0);
        }
        else
        {
            anim.CrossFade("roll_l", 0.1f, 0, 0);
        }

        float attackDuration = 0.81f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (distanceToPlayer < attackRange && randomAttack > 0.3f)   //�Ÿ��� ������ �÷��̾ ������ 70% Ȯ���� �ݰ�.
        {
            if (comboCharge >= 5f)
            {
                ChangeState(State.COMBO);
                yield break;  // �ڷ�ƾ ����
            }
            float randomValue = UnityEngine.Random.Range(0f, 2f);

            if (randomValue < 1f)
            {
                ChangeState(State.ATTACK_1);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 2f)
            {
                ChangeState(State.ATTACK_2);
                yield break;  // �ڷ�ƾ ����
            }

        }

        ChangeState(State.CHECK);
        yield break;
    }   //�Ϸ�

    IEnumerator HEAL()
    {
        havePosion -= 1;
        isHeal = false; // �� �Ϸ�!
        anim.CrossFade("heal", 0.1f, 0, 0);

        float attackDuration = 1.6f;  // ȸ�� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        bool firstHeal = false;
        while (elapsedTime < attackDuration)
        {
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (elapsedTime > 0.8f && !firstHeal)
            {
                firstHeal = true;
                HP += 20f;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float randomValue = UnityEngine.Random.Range(0f, 1f);
        //ȸ�� �� �÷��̾� �� �Ÿ��� ������ 70% Ȯ���� ���� �� �ܿ� ����»� 
        if (distanceToPlayer <= attackRange && randomValue > 0.3f)
        {
            if (comboCharge >= 5f)
            {
                ChangeState(State.COMBO);
                yield break;  // �ڷ�ƾ ����
            }
            float randomAttack = UnityEngine.Random.Range(0f, 2f);

            if (randomAttack < 1f)
            {
                ChangeState(State.ATTACK_1);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomAttack <= 2f)
            {
                ChangeState(State.ATTACK_2);
                yield break;  // �ڷ�ƾ ����
            }
        }

        ChangeState(State.CHECK);
        yield break;

    }   //�Ϸ�

    IEnumerator COMBO()
    {
        comboCharge = 0;

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("combo", 0.1f, 0, 0);

        float attackDuration = 5.3f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ChangeState(State.CHECK);
        yield break;
    }   //�Ϸ�.

    IEnumerator BACK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("walk") == false)
        {
            anim.CrossFade("walk", 0.1f);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }
        //�ִϸ��̼� ��ȯ ���

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("walk"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        //�ִϸ��̼� ��ȯ üũ

        if (isHit)
        {
            isHit = false;
            ChangeState(State.CHECK);
            yield break;
        }

        yield return null;
    } //�Ϸ�.

    IEnumerator STUN()
    {
        nmAgent.speed = 0;
        isStun = false;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("stun", 0.1f, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 3.33f)                                            //���� ���� �ð�
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;

        }

        isStuned = true;
        ChangeState(State.BACKDIVE); //������ ������ �ڷ� ������ ��� �޷��ͼ� ���� ����.
        yield break;
    }   //�Ϸ�.

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
        bool firstDiveCheck = false;
        while (elapsedTime < checkTime)
        {
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isStun)
            {
                ChangeState(State.STUN);
                yield break;            // �ڷ�ƾ ����
            }

            if (HP < 36f && havePosion > 0)
            {
                isHeal = true;
                ChangeState(State.BACKSTEP);  //���� �ϱ� ���� �Ÿ��� ����.
                yield break;
            }

            //�÷��̾ ������ �����ϸ�
            if (distanceToPlayer <= attackRange && playerScript.currentState == SuperPlayerController.State.ATTACK && firstDiveCheck == false)
            {
                firstDiveCheck = true;
                float randomDive = UnityEngine.Random.Range(0f, 1f); //�̹� ���� ������ ����
                if (randomDive < 0.8f)  //50% Ȯ����
                {
                    yield return new WaitForSecondsRealtime(0.2f); //������ ���� ��ٸ� �ð�;
                    ChangeState(State.DIVE); //������.
                    yield break;
                }
            }
            else if(playerScript.currentState != SuperPlayerController.State.ATTACK)
            {
                firstDiveCheck = false;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //�Ÿ��� �������� �߰�
        if (distanceToPlayer >= checkRange)
        {
            isChecking = true; //üũ �ߴٴ� ���� ����.
            ChangeState(State.CHASE);
            yield break;            // �ڷ�ƾ ����
        }

        // �÷��̾���� ���� �Ÿ��� ���� �������� �۰ų� ������
        if (distanceToPlayer <= attackRange)
        {
            if (comboCharge >= 5f)
            {
                ChangeState(State.COMBO);
                yield break;  // �ڷ�ƾ ����
            }
            float randomValue = UnityEngine.Random.Range(0f, 2f);

            if (randomValue < 1f)
            {
                ChangeState(State.ATTACK_1);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 2f)
            {
                ChangeState(State.ATTACK_2);
                yield break;  // �ڷ�ƾ ����
            }
        }

        ChangeState(State.CHASE);
        yield break;
    } //�Ϸ�.

    IEnumerator DIE()//�Ϸ�.
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

        //�ۼ��� �̰�
        Destroy(hpSliderObject);
        //�ۼ��� �̰�
    }


    void TriggerDive()
    {
        isAlreadyHit = true;
    }

    void EndDive()
    {
        isAlreadyHit = false;
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

        if (state == State.TAUNT || state == State.STUN || state == State.ATTACK_1 || state == State.CHECK || state == State.ATTACK_2 || state == State.COMBO || state == State.BACKSTEP || state == State.BACKDIVE || state == State.DIVE)
        {
            LookPlayer(directionToPlayer);
        }

        if (state == State.CHASE)
        {
            ChasePlayer();
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

    void EnableAttackColliders(bool enable)
    {
        // attackRangeL�� attackRangeR�� �ݶ��̴��� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
        if (weaponAttackRange != null)
        {
            weaponAttackRange.GetComponent<Collider>().enabled = enable;
        }

    }
    void ChangeState(State newState)
    {
        state = newState;
    }

    void ChasePlayer()                      //�߰� ���
    {
        nmAgent.speed = 6f;
        nmAgent.SetDestination(player.position);
    }


    void LookPlayer(Vector3 direction)      //��� ���
    {
        nmAgent.SetDestination(transform.position);
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
    void TriggerAttack()
    {
        isAttack = true;
    }

    void EndAttack()
    {
        isAttack = false;
    }

    //������ ����
    void OnTriggerEnter(Collider other)
    {
        if (isAlreadyHit) { return; }

        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttackHit)
        {
            if(state == State.CHECK)
            {
                nowStun += 7f;
            }
            if (nowStun >= maxStun && state == State.CHECK)
            {
                isStun = true;
                nowStun = 0;
            }
            HP = HP - (player_1.GetComponent<SuperPlayerController>().PlayerDamage);
            Debug.Log("������!");
            isHit = true;

            isAlreadyHit = true; // Ÿ�ݵǾ����� ǥ��
            StartCoroutine(ResetHit()); // ���� �ð� �� �÷��� �ʱ�ȭ

        }

        if (other.CompareTag("magic"))
        {
            HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
            Debug.Log("������!");
            isHit = true;

            isAlreadyHit = true; // Ÿ�ݵǾ����� ǥ��
            StartCoroutine(ResetHit()); // ���� �ð� �� �÷��� �ʱ�ȭ

        }

    }


    IEnumerator ResetHit()
    {

        yield return new WaitForSeconds(0.3f); // 0.3�� �� �ʱ�ȭ, �ʿ信 ���� ���� ����
        isAlreadyHit = false;
        isHit = false;
    }



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
}

