using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossAController : MonoBehaviour, Ienemy
{

    public Animator anim; //애니메이터
    UnityEngine.AI.NavMeshAgent nmAgent; //navmeshagent 추가

    public GameObject player_1;

    public Transform player; //플레이어 타겟
    public LayerMask playerLayer;
    public GameObject attackRangeL;         //공격범위 왼팔
    public GameObject attackRangeR;         //공격범위 오른팔


    private float HP = 0; //적 체력 선언 및 초기화
    private float MaxHP = 150;
    private float detectingRange = 29f;         //적 탐지 거리
    private float checkRange = 20f;             //경계유지거리
    private float attackRange = 20f;           //적 공격 사거리
    private float smoothRotationSpeed = 15f;     //적 최적화 회전 속도
    private float stunMax = 80f;
    private float stunGauge = 0f;
    private float alertSpeed = 1.6f;
    private float dashSpeed = 11f;
    private float chaseSpeed = 8f;
    public float Damage = 30f;

    private float maxMelee = 0;
    private float maxRanged = 0;
    private Slider hpSlider;                     // 몬스터 HP 슬라이더
    private GameObject hpSliderObject;          // 슬라이더 UI 오브젝트

    float Ienemy.Damage => Damage;


    private float Melee = 0f;                   // 근접 스킬 사용 횟수
    private float Ranged = 0f;                  // 원거리 스킬 사용 횟수


    public bool isHit = false;                  //맞은 상태
    public bool isAttack = false;
    public bool isLittleStun = false;
    public bool isBigStun = false;
    public bool firstStunCheck = true;
    private bool isAlreadyHit = false;          //피격 무적 판정
    private bool is2Phase = false;

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
    public float enemySoul = 150f;
    public bool isDead = false;

    Vector3 initialPoint; //적 배치 위치 변수 선언

    enum State
    {
        IDLE, //기본
        CHASE, //추격
        QUAKE, //근접
        CHARGE_QUAKE, //2페 개막 
        SCRATCH, //근접
        ATTACK_JUMP_MAGIC, //1페 개막 
        ATTACK_JUMP_QUAKE, //근접 강공격
        LITTLE_SKILL_ATTACK, //원거리
        BIG_SKILL_ATTACK,   // 원거리
        ATTACK, //원거리 2방 날리고 DASH 후 스크레치 근접 
        DASH,  //빨리 붙기
        BACK,  //귀환
        CHECK,  //상황견제 플레이어의 딜타임
        STUN,  //스턴
        BIG_STUN,  //강스턴
        DIE,  //사망
        REBORN,  //2페 시작
        COMBO,  //2페 원거리 패턴
        SSCRATCH,   //2페 근접 패턴
        BACKSTEP //원거리 전환
    }

    State state;


    void Awake()
    {
        playerLayer = LayerMask.GetMask("Player");
    }

    void Start()
    {
        initialPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);   //적 배치 위치 초기화
        HP = MaxHP; //체력 초기화
        anim = GetComponent<Animator>();
        nmAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        state = State.IDLE;
        StartCoroutine(StateMachine());
        maxMelee = 2;
        maxRanged = 2;
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
        if (curAnimStateInfo.IsName("Idle") == false)
            anim.Play("Idle", 0, 0);

        if (distanceToPlayer <= detectingRange && player_1.GetComponent<SuperPlayerController>().isStand) //탐지 범위 안에 플레이어가 들어오면
        {
            hpSliderObject.SetActive(true);
            // StateMachine 을 대시로 변경
            ChangeState(State.ATTACK_JUMP_MAGIC);
        }

        if (isHit)  //기습 대경직
        {
            hpSliderObject.SetActive(true);
            ChangeState(State.BIG_STUN);
        }
        yield return null;



        if (HP < 75 && !is2Phase)
        {
            isAttack = false;
            ChangeState(State.REBORN);
            yield break;
        }
        if (isHit && isLittleStun)                                                   //스턴치
        {
            isAttack = false;
            ChangeState(State.STUN);
            yield break;  // 코루틴 종료
        }

        if (isHit && isBigStun)                                                      //스턴치
        {
            isAttack = false;
            ChangeState(State.BIG_STUN);
            yield break;  // 코루틴 종료
        }

        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }
    }

    IEnumerator DASH()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //애니매이션 전환 대기
        if (curAnimStateInfo.IsName("Chase") == false)
        {
            anim.CrossFade("Chase", 0.1f, 0, 0);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }

        //애니매이션 전환 체크
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Chase"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }


        if (HP < 75 && !is2Phase)
        {
            isAttack = false;
            ChangeState(State.REBORN);
            yield break;
        }
        if (isHit && isLittleStun)                                                   //스턴치
        {
            isAttack = false;
            ChangeState(State.STUN);
            yield break;  // 코루틴 종료
        }

        if (isHit && isBigStun)                                                      //스턴치
        {
            isAttack = false;
            ChangeState(State.BIG_STUN);
            yield break;  // 코루틴 종료
        }
        //사망판정
        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }

        // 플레이어와의 남은 거리가 공격 지점보다 작거나 같으면
        if (distanceToPlayer <= attackRange)
        {
            //대쉬 이후 근접 모드

            ChangeState(State.SCRATCH);
        }

        yield return null;
    }

    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //애니매이션 전환 대기
        if (curAnimStateInfo.IsName("Chase") == false)
        {
            anim.CrossFade("Chase", 0.1f, 0, 0);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }

        //애니매이션 전환 체크
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Chase"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        //사망판정
        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }

        if (HP < 75 && !is2Phase)
        {
            isAttack = false;
            ChangeState(State.REBORN);
            yield break;
        }
        if (isHit && isLittleStun)                                                   //스턴치
        {
            isAttack = false;
            ChangeState(State.STUN);
            yield break;  // 코루틴 종료
        }

        if (isHit && isBigStun)                                                      //스턴치
        {
            isAttack = false;
            ChangeState(State.BIG_STUN);
            yield break;  // 코루틴 종료
        }



        // 플레이어와의 남은 거리가 공격 지점보다 작거나 같으면
        if (distanceToPlayer <= attackRange)
        {

            if (attackRange == 3f)  //공격 거리가 근접이면
            {
                if (Melee >= maxMelee) //그런데 근접 공격을 다했다면
                {
                    ChangeState(State.BACKSTEP);    //백스탭 후 원거리 전환
                    yield break;  // 코루틴 종료
                }

                float randomValue = UnityEngine.Random.Range(0f, 10f);

                if (is2Phase && randomValue > 7f && randomValue <= 9f)
                {
                    ChangeState(State.SSCRATCH); //근접 바닥 찍기 기술
                    yield break;  // 코루틴 종료
                }

                // 확률 패턴
                /*else if (randomValue <= 4.5f)
                {
                    ChangeState(State.SCRATCH); //근접 긁기 기술
                    yield break;  // 코루틴 종료
                }

                else if (randomValue > 4.5f && randomValue <= 9f)
                {
                    ChangeState(State.QUAKE); //근접 바닥 찍기 기술
                    yield break;  // 코루틴 종료
                }*/

                else
                {
                    ChangeState(State.ATTACK_JUMP_QUAKE); //10% 확률로 강력한 기술
                    yield break;  // 코루틴 종료
                }
            }
            else if (attackRange == 20f)
            {
                if (Ranged >= maxRanged)
                {
                    ChangeState(State.ATTACK);        //어택 후 대쉬로 붙고 근접 전환
                    yield break;  // 코루틴 종료
                }
                //공격 패턴 랜덤 실행
                float randomValue = UnityEngine.Random.Range(0f, 10f);

                if (is2Phase && randomValue > 7f && randomValue <= 9f)
                {
                    ChangeState(State.COMBO); //근접 바닥 찍기 기술
                    yield break;  // 코루틴 종료
                }

                // StateMachine 을 공격으로 변경
                else if (randomValue <= 5f)
                {
                    ChangeState(State.LITTLE_SKILL_ATTACK);    //원거리 견제
                    yield break;  // 코루틴 종료
                }

                else if (randomValue > 5f && randomValue <= 9f)
                {
                    ChangeState(State.BIG_SKILL_ATTACK);    //원거리 강 견제
                    yield break;  // 코루틴 종료
                }

                else
                {
                    ChangeState(State.ATTACK_JUMP_MAGIC); // 10% 확률로 강력한 기술
                    yield break;  // 코루틴 종료
                }
            }

        }


        if (distanceToBase >= 50f) //집 거리 50f 멀어지면 복귀
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

        anim.CrossFade("Stun_1", 0.1f, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 0.666f)                                            //스턴 지속 시간
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;

        }
        ChangeState(State.CHECK);
    }

    IEnumerator BIG_STUN()
    {
        nmAgent.speed = 0;
        isBigStun = false;
        firstStunCheck = true;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Stun_2", 0.1f, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 1.3f)                                            //스턴 지속 시간
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;

        }
        ChangeState(State.CHECK);
    }

    //공격패턴들

    IEnumerator QUAKE()
    {
        Melee += 1;
        anim.CrossFade("Quake_1", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.166f; // 공격 애니메이션의 지속 시간

        if (is2Phase)
        {
            attackDuration = 5.04f;
        }
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator CHARGE_QUAKE()
    {
        Melee += 1;
        anim.CrossFade("Charge_Quake", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 2.4f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator SCRATCH()
    {
        Melee += 1;
        anim.CrossFade("Scratch_1", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 1.399f;  // 공격 애니메이션의 지속 시간
        if (is2Phase)
        {
            attackDuration = 2f;
        }
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_JUMP_MAGIC()
    {
        Ranged += 1;
        anim.CrossFade("Jump_Magic", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 6f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_JUMP_QUAKE()
    {
        Melee += 1;
        anim.CrossFade("Jump_Attack", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.575f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator LITTLE_SKILL_ATTACK()
    {
        Ranged += 1;
        anim.CrossFade("Skill_1", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.5f;  // 공격 애니메이션의 지속 시간
        if(is2Phase)
        {
            attackDuration = 4.8f;
        }
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator BIG_SKILL_ATTACK()
    {
        Ranged += 1;
        anim.CrossFade("Charge_Skill", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 2.7f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }


    IEnumerator COMBO()
    {
        attackRange = 3f;
        checkRange = 3f;
        Ranged = 0;
        Melee = 0;

        anim.CrossFade("Combo_1", 0.04f, 0, 0);
        

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 4.1333f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }

            if(elapsedTime > 2.188f && elapsedTime < 3.094f)
            {
                nmAgent.speed = 50f;
                nmAgent.SetDestination(player.position);
            }
            else
            {
                nmAgent.SetDestination(transform.position);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    
    IEnumerator SSCRATCH()
    {
        anim.CrossFade("SScratch", 0.04f, 0, 0);
        

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 2.36f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK()
    {
        Ranged = 0;
        Melee = 0;
        attackRange = 3f;
        checkRange = 3f;
        maxMelee = UnityEngine.Random.Range(1f, 2f);

        anim.CrossFade("Attack_1", 0.04f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 1.666f;  // 공격 애니메이션의 지속 시간
        if (is2Phase)
        {
            attackDuration = 2.2f;
        }
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.DASH);
        yield return null;
    }

    //기믹 행동

    IEnumerator REBORN()    
    {
        anim.CrossFade("ReBorn", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        isAlreadyHit = true;
        is2Phase = true;
        anim.SetBool("Phase_2", is2Phase);      //2페이즈 애니메이션 시작!

        float attackDuration = 6f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            isAlreadyHit = true;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isAlreadyHit = false;
        //2페 개막 패턴
        Ranged = 0;
        Melee = 0;
        attackRange = 3f;
        checkRange = 3f;
        ChangeState(State.CHARGE_QUAKE);

        yield return null;
    }

    IEnumerator BACKSTEP()
    {
        Ranged = 0;
        Melee = 0;
        Debug.Log("적의 백스탭!");
        attackRange = 20f;
        checkRange = 20f;
        maxRanged = UnityEngine.Random.Range(1f, 3f);

        anim.CrossFade("Step_L", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 0.75f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }

            transform.Translate(Vector3.back * 7f * Time.deltaTime);            //뒤로 7f까지 이동

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        float randomValue = UnityEngine.Random.Range(0f, 10f);
        // StateMachine 을 공격으로 변경
        if (randomValue <= 5f)
        {
            ChangeState(State.LITTLE_SKILL_ATTACK);    //원거리 견제
            yield break;  // 코루틴 종료
        }

        else if (randomValue > 5f && randomValue <= 9f)
        {
            ChangeState(State.BIG_SKILL_ATTACK);    //원거리 강 견제
            yield break;  // 코루틴 종료
        }

        else
        {
            ChangeState(State.ATTACK_JUMP_MAGIC); // 10% 확률로 강력한 기술
            yield break;  // 코루틴 종료
        }
    }



    IEnumerator BACK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
        
        if (curAnimStateInfo.IsName("Walk") == false)
        {
            anim.CrossFade("Walk", 0.1f, 0, 0);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }
        //애니매이션 전환 대기

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Walk"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        //애니매이션 전환 체크

        //사망판정
        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }

        if (HP < 75 && !is2Phase)
        {
            isAttack = false;
            ChangeState(State.REBORN);
            yield break;
        }
        if (isHit && isLittleStun)                                                   //스턴치
        {
            isAttack = false;
            ChangeState(State.STUN);
            yield break;  // 코루틴 종료
        }

        if (isHit && isBigStun)                                                      //스턴치
        {
            isAttack = false;
            ChangeState(State.BIG_STUN);
            yield break;  // 코루틴 종료
        }

        yield return null;
    }

    //보스의 메인 상태
    IEnumerator CHECK()
    {
        Debug.Log("melee : "+Melee+"|"+"ranged" + Ranged);

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Walk", 0.1f, 0, 0);

        float checkTime = UnityEngine.Random.Range(1f, 2f);                            //패턴 쉬는 시간
        float elapsedTime = 0f;
        while (elapsedTime < checkTime)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (HP < 75 && !is2Phase)
            {
                isAttack = false;
                ChangeState(State.REBORN);
                yield break;
            }
            if (isHit && isLittleStun)                                                   //스턴치
            {
                isAttack = false;
                ChangeState(State.STUN);
                yield break;  // 코루틴 종료
            }

            if (isHit && isBigStun)                                                      //스턴치
            {
                isAttack = false;
                ChangeState(State.BIG_STUN);
                yield break;  // 코루틴 종료
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 목표와의 거리가 인지거리 보다 멀어진 경우
        if (attackRange < distanceToPlayer)
        {
            ChangeState(State.CHASE);                       //추격
        }

        // 플레이어와의 남은 거리가 공격 지점보다 작거나 같으면
        
        if (distanceToPlayer <= attackRange)
        {
            if (attackRange == 3f)  //공격 거리가 근접이면
            {
                if (Melee >= maxMelee) //그런데 근접 공격을 다했다면
                {
                    ChangeState(State.BACKSTEP);    //백스탭 후 원거리 전환
                    yield break;  // 코루틴 종료
                }

                float randomValue = UnityEngine.Random.Range(0f, 10f);
                // 확률 패턴

                if (is2Phase && randomValue > 7f && randomValue <= 9f)
                {
                    ChangeState(State.SSCRATCH); //근접 바닥 찍기 기술
                    yield break;  // 코루틴 종료
                }

                /*else if (randomValue <= 4.5f)
                {
                    ChangeState(State.SCRATCH); //근접 긁기 기술
                    yield break;  // 코루틴 종료
                }

                else if (randomValue > 4.5f && randomValue <= 8f)
                {
                    ChangeState(State.QUAKE); //근접 바닥 찍기 기술
                    yield break;  // 코루틴 종료
                }*/

                else
                {
                    ChangeState(State.ATTACK_JUMP_QUAKE); //10% 확률로 강력한 기술
                    yield break;  // 코루틴 종료
                }
            }
            else if(attackRange == 20f)
            {
                if(Ranged >= maxRanged)
                {
                    ChangeState(State.ATTACK);        //어택 후 대쉬로 붙고 근접 전환
                    yield break;  // 코루틴 종료
                }
                //공격 패턴 랜덤 실행
                float randomValue = UnityEngine.Random.Range(0f, 10f);
                // StateMachine 을 공격으로 변경


                if (is2Phase && randomValue > 7f && randomValue <= 9f)
                {
                    ChangeState(State.COMBO); //근접 바닥 찍기 기술
                    yield break;  // 코루틴 종료
                }

                else if (randomValue <= 5f)
                {
                    ChangeState(State.LITTLE_SKILL_ATTACK);    //원거리 견제
                    yield break;  // 코루틴 종료
                }

                else if (randomValue > 5f && randomValue <= 9f)
                {
                    ChangeState(State.BIG_SKILL_ATTACK);    //원거리 강 견제
                    yield break;  // 코루틴 종료
                }

                else
                {
                    ChangeState(State.ATTACK_JUMP_MAGIC); // 10% 확률로 강력한 기술
                    yield break;  // 코루틴 종료
                }
            }
        }

        //50f 이상 거리가 벌어지면
        if (50f < distanceToPlayer)
        {
            // StateMachine을 BACK으로 변경
            ChangeState(State.BACK);
            yield break;            // 코루틴 종료
        }
    }

    IEnumerator DIE()
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
        anim.Play("Die");

        // 애니메이션의 normalizedTime이 1.0에 가까워질 때까지 대기
        while (true)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // 애니메이션의 normalizedTime이 1.0 이상일 때 애니메이션이 완료된 것으로 간주
            if (curAnimStateInfo.IsName("Die") && curAnimStateInfo.normalizedTime >= 1.0f)
            {
                break;
            }

            yield return null; // 한 프레임 대기
        }

        GameManager.Instance.UpdatePlayerSOUL(enemySoul);

        // 애니메이션이 끝난 후 오브젝트를 제거
        Destroy(gameObject);
        Destroy(hpSliderObject);
    }






    void Update()
    {
        StunCheck();
        UpdateHPBar();

        directionToPlayer = new Vector3(player.position.x - transform.position.x,
            0f, player.position.z - transform.position.z);    //플레이어와의 위치관계
        distanceToPlayer = directionToPlayer.magnitude;                                       //플레이어와의 위치관계 수치화

        directionToBase = new Vector3(initialPoint.x - transform.position.x,
            0f, initialPoint.z - transform.position.z);
        distanceToBase = directionToBase.magnitude;

        if (state != State.BACK && state != State.DIE && state != State.IDLE && state != State.REBORN)
        {
            LookPlayer(directionToPlayer);
        }

        if (state == State.CHASE)
        {
            ChasePlayer();
        }

        if(state == State.REBORN || state == State.DIE || state == State.QUAKE || state == State.CHARGE_QUAKE || state == State.SCRATCH || state == State.ATTACK_JUMP_MAGIC || state == State.ATTACK_JUMP_QUAKE 
            || state == State.LITTLE_SKILL_ATTACK || state == State.BIG_SKILL_ATTACK || state == State.ATTACK || state == State.STUN || state == State.BIG_STUN || state == State.SSCRATCH)
        {
            Stopnow();
        }

        if (state == State.DASH)
        {
            DashPlayer();
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


    }


    public void InitializeHPBar(GameObject hpSliderPrefab)
    {
        hpSliderObject = Instantiate(hpSliderPrefab, GameObject.Find("Canvas").transform); // 슬라이더 생성 및 부모를 Canvas로 설정
        hpSlider = hpSliderObject.GetComponent<Slider>();
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value = HP / MaxHP; // 슬라이더 값 업데이트
        }
    }

    void ChangeState(State newState)
    {
        state = newState;
    }

    void StunCheck()
    {
        if (stunGauge > 40 && firstStunCheck)
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

    void ChasePlayer()                      //추격 기능
    {
        nmAgent.speed = chaseSpeed;
        nmAgent.SetDestination(player.position);
    }

    void DashPlayer()                      //추격 기능
    {
        nmAgent.speed = dashSpeed;
        nmAgent.SetDestination(player.position);
    }


    void LookPlayer(Vector3 direction)      //경계 기능
    {

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        if (flatDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothRotationSpeed * Time.deltaTime);
        }
    }

    void ReturnToBase()  //귀환 기능
    {
        nmAgent.speed = 4f;
        if (distanceToBase > 6f)  // 거리가 3 이상일 경우에만 이동
        {
            nmAgent.SetDestination(initialPoint);
        }
        else
        {
            Debug.Log("집도착");
            HP = MaxHP;
            ChangeState(State.IDLE); //idle 상태로 변경
        }
    }

    void CheckPlayer()
    {
        nmAgent.speed = alertSpeed;  //경계 태새일때의 움직임
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

            // 50% 확률로 좌우 이동 방향 변경
            if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
            {
                moveRight = !moveRight;
            }
        }

        // 플레이어와의 방향 벡터 계산
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // 플레이어와의 현재 거리
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 좌우로 이동할 방향을 계산 (좌측 혹은 우측으로 90도 회전)
        Vector3 moveDirection;
        if (moveRight)
        {
            moveDirection = Quaternion.Euler(0, 90, 0) * directionToPlayer;  // 우측으로 회전
        }
        else
        {
            moveDirection = Quaternion.Euler(0, -90, 0) * directionToPlayer; // 좌측으로 회전
        }

        // 목표 위치 계산
        Vector3 targetPosition = player.position - directionToPlayer * checkRange + moveDirection * 10f;

        // NavMeshAgent의 목적지를 갱신
        nmAgent.SetDestination(targetPosition);
    }

    void Stopnow()
    {
        nmAgent.SetDestination(transform.position);
    }

    //공격 비활성화 or 활성화 기능
    void EnableAttackColliders(bool enable)
    {
        // attackRangeL과 attackRangeR의 콜라이더를 활성화 또는 비활성화
        if (attackRangeL != null)
        {
            attackRangeL.GetComponent<Collider>().enabled = enable;
        }

        if (attackRangeR != null)
        {
            attackRangeR.GetComponent<Collider>().enabled = enable;
        }

    }
    //데미지 판정
    void OnTriggerEnter(Collider other)
    {
        if (isAlreadyHit) { return; }
        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttackHit)
        {
            HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
            stunGauge += 3f;
            Debug.Log("아프다!");
            isHit = true;

            isAlreadyHit = true; // 타격되었음을 표시
            StartCoroutine(ResetHit()); // 일정 시간 후 플래그 초기화

        }

        if (other.CompareTag("magic"))
        {
            HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
            stunGauge += 3f;
            Debug.Log("아프다!");
            isHit = true;

            isAlreadyHit = true; // 타격되었음을 표시
            StartCoroutine(ResetHit()); // 일정 시간 후 플래그 초기화

        }

    }


    IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(0.05f); // 0.3초 후 초기화, 필요에 따라 조정 가능
        isAlreadyHit = false;
    }

}
