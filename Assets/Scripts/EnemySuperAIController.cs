using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemySuperAIController : MonoBehaviour, Ienemy
{
    public Animator anim; //애니메이터
    UnityEngine.AI.NavMeshAgent nmAgent; //navmeshagent 추가

    public GameObject player_1;

    public Transform player; //플레이어 타겟
    public LayerMask playerLayer;
    public GameObject weaponAttackRange;

    private float HP = 0; //적 체력 선언 및 초기화
    private float MaxHP = 50;
    private float sensingRange = 30f;         //적 인지 거리
    private float checkRange = 10f;
    private float attackRange = 4f;           //적 공격 사거리
    private float smoothRotationSpeed = 15f;     //적 최적화 회전 속도
    public float Damage = 20f;
    float Ienemy.Damage => Damage;
    private float nowStun = 0;
    private float maxStun = 20;
    private float randomAttack = 0;
    private float comboCharge = 0;
    private bool isHeal = false;

    public bool isHit = false;                  //맞은 상태
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


    private bool moveRight;  // 좌우 이동 방향 제어를 위한 변수
    private bool directionInitialized = false;  // 방향이 초기화되었는지 여부를 확인하는 변수
    private float timeSinceLastCheck = 0f;
    private float checkInterval = 2f; // 2초마다 체크

    Vector3 initialPoint; //적 배치 위치 변수 선언

    private Slider hpSlider;                     // 몬스터 HP 슬라이더
    private GameObject hpSliderObject;          // 슬라이더 UI 오브젝트
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

        initialPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);   //적 배치 위치 초기화
        HP = MaxHP; //체력 초기화
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
        // 현재 animator 상태정보 얻기
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 이름이 Idle 이 아니면 Play
        if (curAnimStateInfo.IsName("idle") == false)
            anim.Play("idle", 0, 0);

        if (distanceToPlayer <= sensingRange) //탐지 범위 안에 플레이어가 들어오면
        {
            //hpSliderObject.SetActive(true);
            // StateMachine 을 소환 변경
            ChangeState(State.TAUNT);
            yield break;
        }
        yield return null;
    }  //완료.

    IEnumerator TAUNT()
    {
        hpSliderObject.SetActive(true);
        anim.CrossFade("taunt", 0.1f, 0, 0);

        float attackDuration = 4.5f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            if (distanceToPlayer <= attackRange && playerScript.currentState == SuperPlayerController.State.ATTACK)    //도발 중 가까워지고 공격 범위 안에 들어오고 공격을 시전하면
            {
                yield return new WaitForSecondsRealtime(0.35f);      //최소 반응시간 0.2초 후
                ChangeState(State.DIVE);        //구르기 시전
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if(distanceToPlayer <= checkRange) //경계 범위에서 플레이어가 공격하지 않고 도발이 끝나면 
        {
            ChangeState(State.CHECK); //경계 시작
            yield break;
        }
        else
        {
            ChangeState(State.CHASE); //그것도 아니라면 추격
        }
        yield return null;
    } //완료.

    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //애니매이션 전환 대기
        if (curAnimStateInfo.IsName("chase") == false)
        {
            anim.CrossFade("chase", 0.1f);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }

        //애니매이션 전환 체크
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

        //경계 이후 추격이 아니라면 공격.
        if (distanceToPlayer <= attackRange && isChecking == false)
        {
            if (comboCharge >= 5f)
            {
                isGuard = false;
                nmAgent.speed = 0;
                ChangeState(State.COMBO);

                yield break;  // 코루틴 종료
            }
            float randomValue = UnityEngine.Random.Range(0f, 2f);
            if (randomValue < 1f)
            {
                nmAgent.speed = 0;
                isGuard = false;
                ChangeState(State.ATTACK_1);
                yield break;  // 코루틴 종료
            }
            else if (randomValue < 2f)
            {
                nmAgent.speed = 0;
                isGuard = false;
                ChangeState(State.ATTACK_2);
                yield break;  // 코루틴 종료
            }
        }
        //경계 이후 추격이라면 다시 경계 상태로
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
    } //완료.

    IEnumerator ATTACK_1()
    {
        comboCharge++;

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("attack_1", 0.1f, 0, 0);

        float attackDuration = 2.242f;  // 공격 애니메이션의 지속 시간
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

    }  //완료.

    IEnumerator ATTACK_2()
    {
        comboCharge++;
        Damage = 40f;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("attack_2", 0.1f, 0, 0);

        float attackDuration = 3.4f;  // 공격 애니메이션의 지속 시간
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
    }  //완료.

    IEnumerator BACKSTEP()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("step_b", 0.1f, 0, 0);

        float attackDuration = 1.15f;  // 공격 애니메이션의 지속 시간
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
            ChangeState(State.BACKDIVE);    //거리를 벌려도 가까우면 구르기로 거리를 더 벌림.
            yield break;
        }

        ChangeState(State.HEAL);        //충분히 거리가 벌린거 같으면 회복.
        yield break;
    } //완료.

    IEnumerator BACKDIVE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("roll_b", 0.1f, 0, 0);

        float attackDuration = 1.32f;  // 공격 애니메이션의 지속 시간
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
        else if(distanceToPlayer < attackRange && isHeal)   //거리를 벌려도 플레이어가 가까우면
        {
            isHeal = false;     //힐이 필요한 상태 제거. 공격 이후 경계상태가 되면 다시 힐 준비.
            // 플레이어와의 남은 거리가 공격 지점보다 작거나 같으면

            if (comboCharge >= 5f)
            {
                ChangeState(State.COMBO);
                yield break;  // 코루틴 종료
            }
            float randomValue = UnityEngine.Random.Range(0f, 2f);

            if (randomValue < 1f)
            {
                ChangeState(State.ATTACK_1);
                yield break;  // 코루틴 종료
            }
            else if (randomValue < 2f)
            {
                ChangeState(State.ATTACK_2);
                yield break;  // 코루틴 종료
            }

        }
        //모든 조건이 아니면 경계상태
        ChangeState(State.CHECK);
        yield break;
    }  //완료.

    IEnumerator DIVE()
    {
        //좌우 랜덤 구르기
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

        float attackDuration = 0.81f;  // 공격 애니메이션의 지속 시간
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

        if (distanceToPlayer < attackRange && randomAttack > 0.3f)   //거리를 벌려도 플레이어가 가까우면 70% 확률로 반격.
        {
            if (comboCharge >= 5f)
            {
                ChangeState(State.COMBO);
                yield break;  // 코루틴 종료
            }
            float randomValue = UnityEngine.Random.Range(0f, 2f);

            if (randomValue < 1f)
            {
                ChangeState(State.ATTACK_1);
                yield break;  // 코루틴 종료
            }
            else if (randomValue < 2f)
            {
                ChangeState(State.ATTACK_2);
                yield break;  // 코루틴 종료
            }

        }

        ChangeState(State.CHECK);
        yield break;
    }   //완료

    IEnumerator HEAL()
    {
        havePosion -= 1;
        isHeal = false; // 힐 완료!
        anim.CrossFade("heal", 0.1f, 0, 0);

        float attackDuration = 1.6f;  // 회복 애니메이션의 지속 시간
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
        //회복 후 플레이어 와 거리가 가까우면 70% 확률로 공격 그 외엔 경계태새 
        if (distanceToPlayer <= attackRange && randomValue > 0.3f)
        {
            if (comboCharge >= 5f)
            {
                ChangeState(State.COMBO);
                yield break;  // 코루틴 종료
            }
            float randomAttack = UnityEngine.Random.Range(0f, 2f);

            if (randomAttack < 1f)
            {
                ChangeState(State.ATTACK_1);
                yield break;  // 코루틴 종료
            }
            else if (randomAttack <= 2f)
            {
                ChangeState(State.ATTACK_2);
                yield break;  // 코루틴 종료
            }
        }

        ChangeState(State.CHECK);
        yield break;

    }   //완료

    IEnumerator COMBO()
    {
        comboCharge = 0;

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("combo", 0.1f, 0, 0);

        float attackDuration = 5.3f;  // 공격 애니메이션의 지속 시간
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
    }   //완료.

    IEnumerator BACK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("walk") == false)
        {
            anim.CrossFade("walk", 0.1f);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }
        //애니매이션 전환 대기

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("walk"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        //애니매이션 전환 체크

        if (isHit)
        {
            isHit = false;
            ChangeState(State.CHECK);
            yield break;
        }

        yield return null;
    } //완료.

    IEnumerator STUN()
    {
        nmAgent.speed = 0;
        isStun = false;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("stun", 0.1f, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 3.33f)                                            //스턴 지속 시간
        {
            //사망판정
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;

        }

        isStuned = true;
        ChangeState(State.BACKDIVE); //스턴이 끝나면 뒤로 구르고 즉시 달려와서 공격 시전.
        yield break;
    }   //완료.

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
                yield break;            // 코루틴 종료
            }

            if (HP < 36f && havePosion > 0)
            {
                isHeal = true;
                ChangeState(State.BACKSTEP);  //힐을 하기 위해 거리를 벌림.
                yield break;
            }

            //플레이어가 공격을 시전하면
            if (distanceToPlayer <= attackRange && playerScript.currentState == SuperPlayerController.State.ATTACK && firstDiveCheck == false)
            {
                firstDiveCheck = true;
                float randomDive = UnityEngine.Random.Range(0f, 1f); //이번 공격 구르기 여부
                if (randomDive < 0.8f)  //50% 확률로
                {
                    yield return new WaitForSecondsRealtime(0.2f); //구르기 반응 기다림 시간;
                    ChangeState(State.DIVE); //구르기.
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

        //거리가 벌어지면 추격
        if (distanceToPlayer >= checkRange)
        {
            isChecking = true; //체크 했다는 정보 전달.
            ChangeState(State.CHASE);
            yield break;            // 코루틴 종료
        }

        // 플레이어와의 남은 거리가 공격 지점보다 작거나 같으면
        if (distanceToPlayer <= attackRange)
        {
            if (comboCharge >= 5f)
            {
                ChangeState(State.COMBO);
                yield break;  // 코루틴 종료
            }
            float randomValue = UnityEngine.Random.Range(0f, 2f);

            if (randomValue < 1f)
            {
                ChangeState(State.ATTACK_1);
                yield break;  // 코루틴 종료
            }
            else if (randomValue < 2f)
            {
                ChangeState(State.ATTACK_2);
                yield break;  // 코루틴 종료
            }
        }

        ChangeState(State.CHASE);
        yield break;
    } //완료.

    IEnumerator DIE()//완료.
    {
        if (isDead)
        {
            yield break;
        }
        isDead = true;

        // 애니메이터의 현재 애니메이션 상태 정보 가져오기
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 충돌 판정 제거 (Collider 비활성화)
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // 사망 애니메이션 재생
        anim.Play("dead");

        // 애니메이션의 normalizedTime이 1.0에 가까워질 때까지 대기
        while (true)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // 애니메이션의 normalizedTime이 1.0 이상일 때 애니메이션이 완료된 것으로 간주
            if (curAnimStateInfo.IsName("dead") && curAnimStateInfo.normalizedTime >= 1.0f)
            {
                break;
            }

            yield return null; // 한 프레임 대기
        }

        GameManager.Instance.UpdatePlayerSOUL(enemySoul);

        // 애니메이션이 끝난 후 오브젝트를 제거
        Destroy(gameObject);

        //작성자 이겸
        Destroy(hpSliderObject);
        //작성자 이겸
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
        
        UpdateHPBar(); // 작성자 이겸

        directionToPlayer = new Vector3(player.position.x - transform.position.x,
            0f, player.position.z - transform.position.z);    //플레이어와의 위치관계
        distanceToPlayer = directionToPlayer.magnitude;                                       //플레이어와의 위치관계 수치화

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
            // 공격 상태일 때 콜라이더를 활성화합니다.
            EnableAttackColliders(true);
        }
        else
        {
            // 공격 상태가 아닐 때 콜라이더를 비활성화합니다.
            EnableAttackColliders(false);
        }

        // 애니메이션에서 루트 모션을 사용하여 이동
        Vector3 movement = anim.deltaPosition; // 애니메이션의 변화량
        transform.position += movement; // 현재 위치 업데이트

    }

    void EnableAttackColliders(bool enable)
    {
        // attackRangeL과 attackRangeR의 콜라이더를 활성화 또는 비활성화
        if (weaponAttackRange != null)
        {
            weaponAttackRange.GetComponent<Collider>().enabled = enable;
        }

    }
    void ChangeState(State newState)
    {
        state = newState;
    }

    void ChasePlayer()                      //추격 기능
    {
        nmAgent.speed = 6f;
        nmAgent.SetDestination(player.position);
    }


    void LookPlayer(Vector3 direction)      //경계 기능
    {
        nmAgent.SetDestination(transform.position);
        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        if (flatDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothRotationSpeed * Time.deltaTime);
        }
    }

    void ReturnToBase()  //귀환 기능
    {
        nmAgent.speed = 2f;
        if (distanceToBase > 3)  // 거리가 0.1 이상일 경우에만 이동
        {
            nmAgent.SetDestination(initialPoint);
        }
        else
        {
            Debug.Log("집도착");
            ChangeState(State.IDLE); //idle 상태로 변경
        }
    }

    void CheckPlayer()
    {
        nmAgent.speed = 0f;
        // 방향이 초기화되지 않았으면 50% 확률로 좌우 이동 방향을 설정
        if (!directionInitialized)
        {
            moveRight = UnityEngine.Random.Range(0f, 1f) >= 0.5f;
            directionInitialized = true;  // 방향이 초기화되었음을 표시
        }

        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            timeSinceLastCheck = 0f;

            // 80% 확률로 좌우 이동 방향 변경
            if (UnityEngine.Random.Range(0f, 1f) < 0.8f)
            {
                moveRight = !moveRight;
            }
        }

    }


    //공격 비활성화 or 활성화 기능
    void TriggerAttack()
    {
        isAttack = true;
    }

    void EndAttack()
    {
        isAttack = false;
    }

    //데미지 판정
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
            Debug.Log("아프다!");
            isHit = true;

            isAlreadyHit = true; // 타격되었음을 표시
            StartCoroutine(ResetHit()); // 일정 시간 후 플래그 초기화

        }

        if (other.CompareTag("magic"))
        {
            HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
            Debug.Log("아프다!");
            isHit = true;

            isAlreadyHit = true; // 타격되었음을 표시
            StartCoroutine(ResetHit()); // 일정 시간 후 플래그 초기화

        }

    }


    IEnumerator ResetHit()
    {

        yield return new WaitForSeconds(0.3f); // 0.3초 후 초기화, 필요에 따라 조정 가능
        isAlreadyHit = false;
        isHit = false;
    }



    public void InitializeHPBar(GameObject hpSliderPrefab)
    {
        hpSliderObject = Instantiate(hpSliderPrefab, GameObject.Find("Canvas").transform); // 슬라이더 생성 및 부모를 Canvas로 설정
        hpSlider = hpSliderObject.GetComponent<Slider>();
        //hpSliderObject.SetActive(true);
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value = HP / MaxHP; // 슬라이더 값 업데이트
            PositionHPBarAboveMonster(); // HP 슬라이더 위치 업데이트
        }
    }

    private void PositionHPBarAboveMonster()
    {
        if (hpSliderObject != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 3f); // 머리 위로 위치 조정
            hpSliderObject.transform.position = screenPosition; // UI 슬라이더 위치 설정
        }
    }
}

