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
    public Animator anim; //애니메이터
    NavMeshAgent nmAgent; //navmeshagent 추가

    public GameObject player_1;

    public Transform player; //플레이어 타겟
    public LayerMask playerLayer;

    private float HP = 0; //적 체력 선언 및 초기화
    private float MaxHP = 10;
    private float detectingRange = 25f;         //적 탐지 거리
    private float sensingRange = 20f;         //적 인지 거리
    private float checkRange = 13f;
    private float attackRange = 13.5f;           //적 공격 사거리

    private float nearattackRange = 2f;           //적 공격 사거리

    private float smoothRotationSpeed = 15f;     //적 최적화 회전 속도
    //public float moveSpeed = 4.0f;             //적 이동속도
    //private float returnSpeed = 2f;           //적 복귀속도
    public float Damage = 10f;
    float Ienemy.Damage => Damage;

    public bool isHit = false;                  //맞은 상태
    public bool isinvincibility = false;      //무적상태
    public bool isAttack = false;
    public bool isDead = false;

    public GameObject bowattackRange;           //근접 활 공격 판정
    public GameObject kickattackRange;           //근접 발차기 공격 판정

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
    public float enemySoul = 50f;

    Vector3 initialPoint; //적 배치 위치 변수 선언

    private bool isAttacked = false;
    private Slider hpSlider;                     // 몬스터 HP 슬라이더
    private GameObject hpSliderObject;          // 슬라이더 UI 오브젝트

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
        initialPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);   //적 배치 위치 초기화
        HP = MaxHP; //체력 초기화
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
        // 현재 animator 상태정보 얻기
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 이름이 Idle 이 아니면 Play
        if (curAnimStateInfo.IsName("Idle") == false)
            anim.Play("Idle", 0, 0);

        if (distanceToPlayer <= detectingRange && player_1.GetComponent<SuperPlayerController>().isStand) //탐지 범위 안에 플레이어가 들어오면
        {
            // StateMachine 을 경계로 변경
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

        if (isHit)
        {
            ChangeState(State.HIT);
            yield break;
        }

        // 플레이어와의 남은 거리가 공격 지점보다 작거나 같으면
        if (distanceToPlayer <= attackRange)
        {
            // StateMachine 을 공격으로 변경
            ChangeState(State.ATTACK_READY);
        }

        if (distanceToPlayer <= checkRange)
        {
            float randomValue = UnityEngine.Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값
            if (randomValue <= 0.1f)
            {
                ChangeState(State.CHECK); // check 상태로 전환
                yield break;
            }
        }
        // 목표와의 거리가 인지거리 보다 멀어진 경우
        if (sensingRange < distanceToPlayer)
        {
            // StateMachine 을 경계로 변경
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

        float attackDuration = 4f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            
                if (isHit)                                                      //데미지 판정
                {
                    isAttack = false;
                    ChangeState(State.HIT);
                    yield break;  // 코루틴 종료
                }
            
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 공격 가능 범위보다 플레이어와의 거리가 멀어지면
        if (distanceToPlayer > attackRange)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f); // 0에서 1 사이의 무작위 값  
            if (randomValue <= 0.4f)
            {
                ChangeState(State.CHECK); // 40% 확률로 check 상태로 전환
                yield break;
            }


            // StateMachine을 추적으로 변경
            ChangeState(State.CHASE);
            yield break;
        }

        ChangeState(State.CHECK); // 공격 후 체크 상태 돌입
        yield break;

    }
    IEnumerator NEAR_ATTACK_READY()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Bow_Attack", 0.1f, 0, 0);

        float attackDuration = 1.6f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (curAnimStateInfo.IsName("Bow_Attack"))
            {
                if (isHit)                                                      //데미지 판정
                {
                    isAttack = false;
                    ChangeState(State.HIT);
                    yield break;  // 코루틴 종료
                }
            }
            else
            {
                //맞으면 HIT로 변경
                if (isHit)
                {
                    ChangeState(State.HIT);
                    yield break;  // 코루틴 종료
                }

            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 공격 가능 범위보다 플레이어와의 거리가 멀어지면
        if (distanceToPlayer > nearattackRange)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f); // 0에서 1 사이의 무작위 값  
            if (randomValue <= 0.4f)
            {
                ChangeState(State.ATTACK_READY); // 40% 확률로 check 상태로 전환
            }


            // StateMachine을 추적으로 변경
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
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }
        //애니매이션 전환 대기

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Idle"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        //맞으면 즉시 HIT 상태로 변경
        if (isHit)
        {
            ChangeState(State.HIT);
            yield break;
        }

        // 거리가 가까워지면
        if (sensingRange > distanceToPlayer)
        {
            // StateMachine을 추적으로 변경
            ChangeState(State.CHASE);
        }

        // 거리가 멀어지면
        if (detectingRange < distanceToPlayer)
        {
            // StateMachine을 BACK으로 변경
            ChangeState(State.BACK);
        }
        yield return null;
    }

    IEnumerator HIT()
    {
        AttackEnd();

        //작성자 이겸
        if (isAttacked != true)
        {
            isAttacked = true;
            hpSliderObject.SetActive(true);
        }
        //작성자 이겸



        nmAgent.speed = 0f;
        HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
        Debug.Log(HP + "남았습니다");

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (HP > 0)
        {

            if (!curAnimStateInfo.IsName("Hit"))
            {
                anim.Play("Hit", 0, 0);
                isHit = false;


                // 애니메이션 상태가 변경될 때까지 기다리기
                yield return null;

                // 상태를 다시 가져옴
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                while (!curAnimStateInfo.IsName("Hit"))
                {
                    yield return null;
                    curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                }
            }

            // 애니메이션이 Walk1_Action 변경될 때까지 대기
            while (curAnimStateInfo.IsName("Hit"))
            {
                yield return null;
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            }
            isinvincibility = false;

            if (curAnimStateInfo.IsName("Idle"))
            {
                // 가까이서 맞았으면 체크
                if (nearattackRange >= distanceToPlayer)
                {
                    float checkTime2 = UnityEngine.Random.Range(0f, 2f);

                    //60%확률로 반격
                    if (checkTime2 > 1.2f)
                        ChangeState(State.NEAR_ATTACK_READY);
                    else
                        ChangeState(State.CHECK);
                    yield break;
                }

                else if (attackRange >= distanceToPlayer)
                {
                    //멀리서 맞았으면 원거리 공격
                    ChangeState(State.ATTACK_READY);
                    yield break;
                }


            }
            //더 먼거리에서 맞았으면 추격
            ChangeState(State.CHASE);

            yield return null;
        }
        else
        {
            ChangeState(State.DIE);
            Debug.Log("죽었다....");
        }

    }

    IEnumerator CHECK()
    {
        Debug.Log("주시중...");
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.CrossFade("Walk", 0.1f, 0, 0);

        float checkTime = UnityEngine.Random.Range(2f, 4f);
        float elapsedTime = 0f;
        while (elapsedTime < checkTime)
        {

            //거리가 벌어지면
            if (detectingRange < distanceToPlayer)
            {
                // StateMachine을 BACK으로 변경
                ChangeState(State.BACK);
                yield break;            // 코루틴 종료
            }

            //맞으면
            if (isHit)
            {
                // StateMachine을 HIT으로 변경
                ChangeState(State.HIT);
                yield break;            // 코루틴 종료
            }


            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 플레이어와의 남은 거리가 공격 지점보다 작거나 같으면
        

        if (distanceToPlayer <= nearattackRange)
        {
            // StateMachine 을 공격으로 변경
            ChangeState(State.NEAR_ATTACK_READY);
            yield break;            // 코루틴 종료
        }
        else if (distanceToPlayer <= attackRange)
        {
            // StateMachine 을 공격으로 변경
            ChangeState(State.ATTACK_READY);
            yield break;            // 코루틴 종료
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

        //작성자 이겸
        Destroy(hpSliderObject);
        //작성자 이겸
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
                if (distanceToPlayer <= attackRange)        //적 공격 사거리 내에 들어오면 공격             
                {
                    AttackPlayer(directionToPlayer);
                }
                else if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, sensingRange, playerLayer) &&
                    distanceToPlayer > attackRange && distanceToPlayer < detectingRange && distanceToPlayer > attackRange)  //적 인지거리 내에 들어오면 추격                                    
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        ChasePlayer(directionToPlayer);
                    }
                }
                else if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectingRange, playerLayer))        //적 탐지거리 내에 들어오면 경계
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

    void ChasePlayer()                      //추격 기능
    {
        nmAgent.speed = 2;
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
        nmAgent.speed = 1.1f;
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


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttackHit && !isinvincibility)
        {
            isinvincibility = true;
            Debug.Log("아프다!");
            isHit = true;
        }

        if (other.CompareTag("magic") && !isinvincibility)
        {
            isinvincibility = true;
            Debug.Log("아프다!");
            isHit = true;
        }
    }

    void AttackPlayer()
    {
        nmAgent.SetDestination(transform.position);
    }
    // 작성자 이겸




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

