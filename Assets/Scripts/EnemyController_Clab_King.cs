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
    public Animator anim; //애니메이터
    NavMeshAgent nmAgent; //navmeshagent 추가

    public GameObject player_1;

    public Transform player; //플레이어 타겟
    public LayerMask playerLayer;
    public GameObject attackRangeL;         //공격범위 왼팔
    public GameObject attackRangeR;         //공격범위 오른팔
    public GameObject attackRangeH;         //공격범위 머리


    private float HP = 0; //적 체력 선언 및 초기화
    private float MaxHP = 30;
    private float detectingRange = 30f;         //적 탐지 거리
    private float sensingRange = 20f;         //적 인지 거리
    private float checkRange = 7f;
    private float attackRange = 5f;           //적 공격 사거리
    private float smoothRotationSpeed = 15f;     //적 최적화 회전 속도
    private float stunMax = 50f;
    private float stunGauge = 0f;
    private float alertSpeed = 2f;
    private float dashSpeed = 12f;
    private float chaseSpeed = 6f;

    public bool isHit = false;                  //맞은 상태
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


    private bool moveRight;  // 좌우 이동 방향 제어를 위한 변수
    private bool directionInitialized = false;  // 방향이 초기화되었는지 여부를 확인하는 변수
    private float timeSinceLastCheck = 0f;
    private float checkInterval = 2f; // 2초마다 체크

    Vector3 initialPoint; //적 배치 위치 변수 선언

    enum State
    {
        IDLE,   //완료
        CHASE,  //완료
        ATTACK_READY_1, //완료
        ATTACK_READY_2, //완료
        ATTACK_1,  //완료
        ATTACK_2,  //완료
        ATTACK_3,  //완료
        LITTLE_ATTACK,  //완료
        DASH_ATTACK_READY,   //완료
        DASH_ATTACK,  //완료
        DASH,       //완료
        BACK,    //완료
        CHECK,    //완료
        STUN,
        BIG_STUN,
        DIE    //완료
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
        nmAgent = GetComponent<NavMeshAgent>();
        state = State.IDLE;
        StartCoroutine(StateMachine());
    }








    // 작성자 이겸


    private Slider hpSlider;                     // 몬스터 HP 슬라이더
    private GameObject hpSliderObject;          // 슬라이더 UI 오브젝트

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
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f); // 머리 위로 위치 조정
            hpSliderObject.transform.position = screenPosition; // UI 슬라이더 위치 설정
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
        // 현재 animator 상태정보 얻기
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 이름이 Idle 이 아니면 Play
        if (curAnimStateInfo.IsName("Sleep") == false)
            anim.Play("Sleep", 0, 0);

        if (distanceToPlayer <= detectingRange && player_1.GetComponent<SuperPlayerController>().isStand) //탐지 범위 안에 플레이어가 들어오면
        {
            // StateMachine 을 경계로 변경
            ChangeState(State.DASH);
        }

        if (isHit)  //기습 대경직
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

        //애니매이션 전환 대기
        if (curAnimStateInfo.IsName("DASH") == false)
        {
            anim.CrossFade("DASH", 0.1f);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }

        //애니매이션 전환 체크
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("DASH"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        if (isHit && isLittleStun)  //경직치
        {
            ChangeState(State.STUN);
            yield break;
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
            ChangeState(State.DASH_ATTACK);
        }

        yield return null;
    }

    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //애니매이션 전환 대기
        if (curAnimStateInfo.IsName("Walk") == false)
        {
            anim.CrossFade("Walk", 0.1f);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }

        //애니매이션 전환 체크
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Walk"))
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

        if (isHit&&isLittleStun)  //경직치
        {
            ChangeState(State.STUN);
            yield break;
        }

        

        // 플레이어와의 남은 거리가 공격 지점보다 작거나 같으면
        if (distanceToPlayer <= attackRange)
        {
            //공격 패턴 랜덤 실행
            float randomValue = UnityEngine.Random.Range(0f, 10f);
            // StateMachine 을 공격으로 변경
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


        if (distanceToBase >= 50f) //집 거리 50f 멀어지면 복귀
        {
            ChangeState(State.BACK);
        }

        yield return null;
    }

    IEnumerator ATTACK_READY_1()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Fight_Read_1", 0.1f, 0, 0);

        float checkTime = UnityEngine.Random.Range(0.5f, 2f);           //공격 준비 시간 랜덤
        float elapsedTime = 0f;
        while (elapsedTime < checkTime)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            //경직치
            if (isHit && isLittleStun)                                                      //스턴치
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

        float checkTime2 = UnityEngine.Random.Range(0f, 2f);
        //공격 기술 둘중 하나 사용
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
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            //경직치
            if (isHit && isLittleStun)                                                      //스턴치
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

        ChangeState(State.ATTACK_3);
        yield return null;
    }

    IEnumerator DASH_ATTACK_READY()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Fight_Ready_3", 0.1f, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 3f)                                            //대쉬 준비 시간
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            //경직치
            if (isHit && isLittleStun)                                                      //스턴치
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
        while (elapsedTime < 0.8f)                                            //스턴 지속 시간
        {
            //사망판정
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
        while (elapsedTime < 1.8f)                                            //스턴 지속 시간
        {
            //사망판정
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

        float attackDuration = curAnimStateInfo.length;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //스턴치
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
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("Attack_1"))
            {
                

                if (elapsedTime > 0.25f && elapsedTime < 0.417f)                   //공격 판정 시간
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
            // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_2()
    {
        anim.CrossFade("Attack_2", 0.05f, 0, 0);
        yield return null;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = curAnimStateInfo.length;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //스턴치
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

            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("Attack_2"))
            {
                
                if (elapsedTime > 0.4f && elapsedTime < 0.63f)                   //공격 판정 시간
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
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_3()
    {
        anim.CrossFade("Attack_3", 0.05f, 0, 0);
        yield return null;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = curAnimStateInfo.length;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //스턴치
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

            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("Attack_3"))
            {
                if (elapsedTime > 0.36f && elapsedTime < 0.667f)                   //공격 판정 시간
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
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator LITTLE_ATTACK()
    {
        anim.CrossFade("Attack_4", 0.05f, 0, 0);
        yield return null;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = curAnimStateInfo.length;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //스턴치
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

            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("Attack_4"))
            {

                if (elapsedTime > 0.5f && elapsedTime < 0.708f)                   //공격 판정 시간
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
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator DASH_ATTACK()
    {
        anim.CrossFade("Attack_5", 0.05f, 0, 0);
        yield return null;
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = curAnimStateInfo.length;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //스턴치
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

            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (curAnimStateInfo.IsName("Attack_5"))
            {
                if (elapsedTime > 0f && elapsedTime < 0.5f)                   //공격 판정 시간
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
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }



    IEnumerator BACK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("Walk") == false)
        {
            anim.CrossFade("Walk", 0.1f);
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
        if (isHit && isLittleStun)                                                      //스턴치
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
        Debug.Log("주시중...");

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Alert_Walk", 0.1f, 0, 0);

        float checkTime = UnityEngine.Random.Range(2f, 4f);                            //패턴 쉬는 시간
        float elapsedTime = 0f;
        while (elapsedTime < checkTime)
        {
            //사망판정
            if (HP < 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            if (isHit && isLittleStun)                                                      //스턴치
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
        if (sensingRange < distanceToPlayer)
        {
            float randomValue = UnityEngine.Random.Range(0f, 10f);
            //75% 확률로 추격
            if (randomValue <= 7.5f)
            {
                ChangeState(State.CHASE);
            }
            //25% 확률로 대쉬공격준비
            else
            {
                ChangeState(State.DASH_ATTACK_READY);
            }
        }

        // 플레이어와의 남은 거리가 공격 지점보다 작거나 같으면
        if (distanceToPlayer <= attackRange)
        {
            //공격 패턴 랜덤 실행
            float randomValue = UnityEngine.Random.Range(0f, 10f);
            // StateMachine 을 공격으로 변경
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

        //50f 이상 거리가 벌어지면
        if (50f < distanceToPlayer)
        {
            // StateMachine을 BACK으로 변경
            ChangeState(State.BACK);
            yield break;            // 코루틴 종료
        }


        ChangeState(State.CHASE);

    }

    IEnumerator DIE()
    {
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
            if (curAnimStateInfo.IsName("Creep|Death_Action") && curAnimStateInfo.normalizedTime >= 1.0f)
            {
                break;
            }

            yield return null; // 한 프레임 대기
        }

        // 애니메이션이 끝난 후 오브젝트를 제거
        Destroy(gameObject);

        //작성자 이겸
        Destroy(hpSliderObject);
        //작성자 이겸
    }






    void Update()
    {
        StunCheck();
        UpdateHPBar(); // 작성자 이겸

        directionToPlayer = new Vector3(player.position.x - transform.position.x,
            0f, player.position.z - transform.position.z);    //플레이어와의 위치관계
        distanceToPlayer = directionToPlayer.magnitude;                                       //플레이어와의 위치관계 수치화

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
            // 공격 상태일 때 콜라이더를 활성화합니다.
            EnableAttackColliders(true);
        }
        else
        {
            // 공격 상태가 아닐 때 콜라이더를 비활성화합니다.
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
        if (distanceToBase > 3)  // 거리가 3 이상일 경우에만 이동
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
        Vector3 targetPosition = player.position - directionToPlayer * checkRange + moveDirection * 2f;

        // NavMeshAgent의 목적지를 갱신
        nmAgent.SetDestination(targetPosition);
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

        if (attackRangeH != null)
        {
            attackRangeH.GetComponent<Collider>().enabled = enable;
        }
    }
    //데미지 판정
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttackHit)
        {
            HP -= 1;
            stunGauge += 3f;
            Debug.Log("아프다!");
            isHit = true;
        }
    }

}
