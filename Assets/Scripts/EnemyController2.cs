using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyController2 : MonoBehaviour
{
    public Animator anim; //애니메이터
    NavMeshAgent nmAgent; //navmeshagent 추가

    public GameObject player_1;

    public Transform player; //플레이어 타겟
    public LayerMask playerLayer;

    private bool firstlooking = true; //캐릭터 최초 목격

    private float HP = 0; //적 체력 선언 및 초기화
    private float detectingRange = 20f;         //적 탐지 거리
    private float sensingRange = 13.5f;         //적 인지 거리
    public float attackRange = 2.5f;           //적 공격 사거리
    private float smoothRotationSpeed = 15f;     //적 최적화 회전 속도
    //public float moveSpeed = 4.0f;             //적 이동속도
    //private float returnSpeed = 2f;           //적 복귀속도

    public bool isHit = false;

    Vector3 directionToPlayer;
    Vector3 directionToBase;

    float distanceToPlayer = 100f;
    float distanceToBase = 0f;

    private Quaternion targetRotation;
    private float currentSpeed;

    Vector3 initialPoint; //적 배치 위치 변수 선언

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
        initialPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);   //적 배치 위치 초기화
        HP = 10; //체력 초기화
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
        // 현재 animator 상태정보 얻기
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 이름이 Idle 이 아니면 Play
        if (curAnimStateInfo.IsName("Creep|Idle1_Action") == false)
            anim.Play("Creep|Idle1_Action", 0, 0);

        if (distanceToPlayer <= detectingRange) //탐지 범위 안에 플레이어가 들어오면
        {
            // StateMachine 을 경계로 변경
            ChangeState(State.LOOK);
        }
        yield return null;
    }

    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //애니매이션 전환 대기
        if (curAnimStateInfo.IsName("Creep|Crouch_Action") == false)
        {
            anim.CrossFade("Creep|Crouch_Action", 0.1f);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }

        //애니매이션 전환 체크
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

        // 플레이어와의 남은 거리가 공격 지점보다 작거나 같으면
        if (distanceToPlayer <= attackRange)
        {
            // StateMachine 을 공격으로 변경
            ChangeState(State.ATTACK);
        }
        // 목표와의 거리가 인지거리 보다 멀어진 경우
        if (sensingRange < distanceToPlayer)
        {
            // StateMachine 을 경계로 변경
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

        float attackDuration = 2.0f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            // 공격 가능 범위보다 플레이어와의 거리가 멀어지면
            if (distanceToPlayer > attackRange)
            {
                // StateMachine을 추적으로 변경
                ChangeState(State.CHASE);
                yield break;  // 코루틴 종료
            }

            if (isHit)
            {
                ChangeState(State.HIT);
                yield break;  // 코루틴 종료
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
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }
        //애니매이션 전환 대기

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Creep|Walk1_Action"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        //애니매이션 전환 체크

        if(isHit)
        {
            ChangeState(State.HIT);
        }

        yield return null;
    }

    IEnumerator LOOK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        //최초 목격이 활성화 되어 있으면 포효

        if (firstlooking)
        {
            ChangeState(State.ROAR);
        }


        if (curAnimStateInfo.IsName("Creep|Idle1_Action") == false)
        {
            anim.CrossFade("Creep|Idle1_Action", 0.1f);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }
        //애니매이션 전환 대기

        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("Creep|Idle1_Action"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
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

    IEnumerator ROAR()
    {

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (!curAnimStateInfo.IsName("Creep|Roar_Action"))
        {
            anim.Play("Creep|Roar_Action", 0, 0);
            firstlooking = false;
            Debug.Log("어흥");

            // 애니메이션 상태가 변경될 때까지 기다리기
            yield return null;

            // 상태를 다시 가져옴
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            while (!curAnimStateInfo.IsName("Creep|Roar_Action"))
            {
                yield return null;
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            }
        }

        // 애니메이션이 Roar에서 Idle로 변경될 때까지 대기
        while (curAnimStateInfo.IsName("Creep|Roar_Action"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        if (isHit)
        {
            ChangeState(State.HIT);
            yield break;  // 코루틴 종료
        }

        if (curAnimStateInfo.IsName("Creep|Idle1_Action"))
        {
            ChangeState(State.LOOK);
            Debug.Log("포효끝");
        }

        yield return null;
    }

    IEnumerator HIT()
    {
        HP = HP - 3; //캐릭터 데미지 나중에 가져오기
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (HP > 0)
        {

            if (!curAnimStateInfo.IsName("Creep|Hit_Action"))
            {
                anim.Play("Creep|Hit_Action", 0, 0);
                isHit = false;
                

                // 애니메이션 상태가 변경될 때까지 기다리기
                yield return null;

                // 상태를 다시 가져옴
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                while (!curAnimStateInfo.IsName("Creep|Hit_Action"))
                {
                    yield return null;
                    curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                }
            }

            // 애니메이션이 Walk1_Action 변경될 때까지 대기
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
            Debug.Log("죽었다....");
        }
        
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
        anim.Play("Creep|Death_Action");

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
    }






    void Update()
    {
        directionToPlayer = new Vector3(player.position.x - transform.position.x,
            0f, player.position.z - transform.position.z);    //플레이어와의 위치관계
        distanceToPlayer = directionToPlayer.magnitude;                                       //플레이어와의 위치관계 수치화

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

    void ChasePlayer(Vector3 direction)     //추격 기능
    {
        
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

        if (distanceToBase > 3)  // 거리가 0.1 이상일 경우에만 이동
        {
            nmAgent.SetDestination(initialPoint);
        }
        else
        {
            Debug.Log("집도착");
            firstlooking = true; //최초 목격 활성화하고
            ChangeState(State.IDLE); //idle 상태로 변경
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttacking)
        {
            Debug.Log("아프다!");
            isHit = true;

        }
    }

}
