using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossCController : MonoBehaviour, Ienemy
{
    public Animator anim; //애니메이터
    UnityEngine.AI.NavMeshAgent nmAgent; //navmeshagent 추가

    public GameObject player_1;

    public Transform player; //플레이어 타겟
    public LayerMask playerLayer;
    public GameObject weaponAttackRange;         //공격범위


    private float HP = 0; //적 체력 선언 및 초기화
    public float MaxHP = 200;
    private float detectingRange = 29f;         //적 탐지 거리
    private float checkRange = 6f;             //경계유지거리
    private float attackRange = 5f;           //적 공격 사거리
    private float smoothRotationSpeed = 15f;     //적 최적화 회전 속도
    private float stunMax = 80f;
    private float stunGauge = 0f;
    private float alertSpeed = 1.8f;
    private float dashSpeed = 11f;
    private float chaseSpeed = 6f;
    public float Damage = 30f;

    private Slider hpSlider;                     // 몬스터 HP 슬라이더
    private Text bossName;
    private GameObject hpSliderObject;          // 슬라이더 UI 오브젝트

    float Ienemy.Damage => Damage;

    public bool isHit = false;                  //맞은 상태
    public bool isAttack = false;
    private bool isAlreadyHit = false;          //피격 무적 판정
    public bool is2Phase = false;
    public bool is3Phase = false;

    Vector3 directionToPlayer;
    Vector3 directionToBase;

    float distanceToPlayer = 100f;
    float distanceToBase = 0f;
    private float attackCount = 0f;
    private float attackMaxCount = 4f;


    private Quaternion targetRotation;
    private float currentSpeed;


    private bool moveRight;  // 좌우 이동 방향 제어를 위한 변수
    private bool directionInitialized = false;  // 방향이 초기화되었는지 여부를 확인하는 변수
    private float timeSinceLastCheck = 0f;
    private float checkInterval = 2f; // 2초마다 체크
    public float enemySoul = 150f;
    public bool isDead = false;
    private SuperPlayerController playerScript;
    Vector3 initialPoint; //적 배치 위치 변수 선언

    public Transform FinalForcedTeleporter;
    public Transform hiddenMessage1;
    public Transform hiddenMessage2;
    public Transform hiddenMessage3;
    public Transform message3;
    public Transform hiddenDoor1;
    public Transform hiddenDoor2;
    public Transform hiddenBoss;
    public Transform removeBox;


    enum State
    {
        IDLE, //기본
        CHASE, //추격
        ATTACK_1, //평타1
        ATTACK_2, //평타2 
        ATTACK_3, //평타3
        ATTACK_4, //평타4
        ATTACK_5, //평타5
        ATTACK_6, //평타6
        ATTACK_7, //평타7
        COMBO_1, //콤보1
        COMBO_2, //콤보2
        COMBO_3, //콤보3
        DIVE,  //구르기
        STEP,  //사이드스탭
        BACK,  //귀환
        CHECK,  //상황견제
        DIE,  //사망
    }

    State state;

    public AudioSource audioSource;
    public AudioClip[] footSound;
    public AudioClip[] diveSound;
    public AudioClip[] dashSound;
    public AudioClip[] slashSound;
    public AudioClip[] airSound;
    public AudioClip[] crashSound;
    public AudioClip[] attackedSound;
    public AudioClip[] clearSound;

    void Awake()
    {
        if (PlayerPrefs.GetInt("Final", 0) == 1)
        {
            Destroy(gameObject);
            FinalForcedTeleporter.gameObject.SetActive(true);
            hiddenMessage1.gameObject.SetActive(true);
            hiddenMessage2.gameObject.SetActive(true);
            hiddenMessage3.gameObject.SetActive(true);
            message3.gameObject.SetActive(false);
            hiddenDoor1.gameObject.SetActive(false);
            hiddenDoor2.gameObject.SetActive(false);
            hiddenBoss.gameObject.SetActive(true);
            removeBox.gameObject.SetActive(false);
        }
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

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = 1.0f;
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 1.0f;
        audioSource.maxDistance = 10.0f;
    }


    IEnumerator StateMachine()
    {
        while (true)
        {
            yield return StartCoroutine(state.ToString());
        }

    }

    //공격 외 패턴
    IEnumerator CHECK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (moveRight)
        {
            anim.CrossFade("step_r", 0.1f, 0, 0);
        }
        else
        {
            anim.CrossFade("step_l", 0.1f, 0, 0);
        }
        float checkTime = 2f;
        if (!is2Phase)
        {
            checkTime = 2f;                         //패턴 쉬는 시간
        }
        else
        {
            checkTime = 0.5f;                         //패턴 쉬는 시간
        }

        float elapsedTime = 0f;

        bool firstDiveCheck = false;
        while (elapsedTime < checkTime)
        {

            //사망판정
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (distanceToPlayer <= attackRange && playerScript.currentState == SuperPlayerController.State.ATTACK && !firstDiveCheck)
            {
                firstDiveCheck = true;
                float randomDive = UnityEngine.Random.Range(0f, 10f); //이번 공격 구르기 여부
                if (randomDive < 4f)  //40% 확률, 구르기 빈도를 늘리고 싶다면 여기 확률을 조정.
                {
                    yield return new WaitForSecondsRealtime(0.2f); //구르기 반응 기다림 시간;
                    ChangeState(State.DIVE); //구르기.
                    yield break;
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 목표와의 거리가 인지거리 보다 멀어진 경우
        if (attackRange < distanceToPlayer)
        {
            ChangeState(State.CHASE);                       //추격
            yield break;  // 코루틴 종료
        }

        //평타 공격패턴
        if (attackCount >= attackMaxCount)
        {
            float randomValue = UnityEngine.Random.Range(0f, 6f);

            if (randomValue < 2)
            {
                ChangeState(State.COMBO_1);
                yield break;  // 코루틴 종료
            }
            else if (randomValue < 4)
            {
                ChangeState(State.COMBO_2);
                yield break;  // 코루틴 종료
            }
            else if (randomValue < 6)
            {
                ChangeState(State.COMBO_3);
                yield break;  // 코루틴 종료
            }
        }
        if (distanceToPlayer <= attackRange)
        {
            if(!is2Phase && !is3Phase)  //2페이즈가 아니고 3페이즈가 아니라면 평타모션 5개중 랜덤 수행
            {
                float randomValue = UnityEngine.Random.Range(0f, 5f);

                if (randomValue < 1)
                {
                    ChangeState(State.ATTACK_1);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 2)
                {
                    ChangeState(State.ATTACK_2);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 3)
                {
                    ChangeState(State.ATTACK_3);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.ATTACK_4);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 5)
                {
                    ChangeState(State.ATTACK_5);
                    yield break;  // 코루틴 종료
                }
            }
            else //2페이즈 혹은 3페이즈라면
            {
                float randomValue = UnityEngine.Random.Range(0f, 7f);

                if (randomValue < 1)
                {
                    ChangeState(State.ATTACK_1);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 2)
                {
                    ChangeState(State.ATTACK_2);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 3)
                {
                    ChangeState(State.ATTACK_3);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.ATTACK_4);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 5)
                {
                    ChangeState(State.ATTACK_5);
                    yield break;  // 코루틴 종료
                }

                else if (randomValue < 6)
                {
                    ChangeState(State.ATTACK_6);
                    yield break;  // 코루틴 종료
                }

                else if (randomValue < 7)
                {
                    ChangeState(State.ATTACK_7);
                    yield break;  // 코루틴 종료
                }
            }

        }

        //100f 이상 거리가 벌어지면
        if (100f < distanceToPlayer)
        {
            // StateMachine을 BACK으로 변경
            ChangeState(State.BACK);
            yield break;            // 코루틴 종료
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
        PlayerPrefs.SetInt("Final", 1);
        PlayerPrefs.Save();

        // 애니메이션이 끝난 후 오브젝트를 제거
        GameManager.Instance.DisplayDemonSlainedText();
        Destroy(gameObject);
        Destroy(hpSliderObject);
        GameManager.Instance.StopBackgroundMusic();
        GameManager.Instance.VolumeDown();
        GameManager.Instance.PlayBackgroundMusic();
        FinalForcedTeleporter.gameObject.SetActive(true);
        hiddenMessage1.gameObject.SetActive(true);
        hiddenMessage2.gameObject.SetActive(true);
        hiddenMessage3.gameObject.SetActive(true);
        message3.gameObject.SetActive(false);
        hiddenDoor1.gameObject.SetActive(false);
        hiddenDoor2.gameObject.SetActive(false);
        hiddenBoss.gameObject.SetActive(true);
        removeBox.gameObject.SetActive(false);
    }

    IEnumerator IDLE()
    {
        // 현재 animator 상태정보 얻기
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 이름이 Idle 이 아니면 Play
        if (curAnimStateInfo.IsName("idle") == false)
            anim.Play("idle", 0, 0);

        if (distanceToPlayer <= detectingRange) //탐지 범위 안에 플레이어가 들어오면
        {
            hpSliderObject.SetActive(true);
            GameManager.Instance.StopBackgroundMusic();
            GameManager.Instance.VolumeUp();
            GameManager.Instance.PlaySelectedBackgroundMusic(7);
            // StateMachine 을 대시로 변경
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

        //애니매이션 전환 대기
        if (curAnimStateInfo.IsName("run") == false)
        {
            anim.CrossFade("run", 0.1f, 0, 0);
            // SetDestination 을 위해 한 frame을 넘기기위한 코드
            yield return new WaitForSeconds(0.1f);
        }

        //애니매이션 전환 체크
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("run"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        //사망판정
        if (HP <= 0)
        {
            ChangeState(State.DIE);
            yield break;
        }
        //붙으면 공격 패턴

        if (attackCount >= attackMaxCount)
        {
            float randomValue = UnityEngine.Random.Range(0f, 6f);

            if (randomValue < 2)
            {
                ChangeState(State.COMBO_1);
                yield break;  // 코루틴 종료
            }
            else if (randomValue < 4)
            {
                ChangeState(State.COMBO_2);
                yield break;  // 코루틴 종료
            }
            else if (randomValue < 6)
            {
                ChangeState(State.COMBO_3);
                yield break;  // 코루틴 종료
            }
        }
        if (distanceToPlayer <= attackRange)
        {
            if (!is2Phase && !is3Phase)  //2페이즈가 아니고 3페이즈가 아니라면 평타모션 5개중 랜덤 수행
            {
                float randomValue = UnityEngine.Random.Range(0f, 5f);

                if (randomValue < 1)
                {
                    ChangeState(State.ATTACK_1);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 2)
                {
                    ChangeState(State.ATTACK_2);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 3)
                {
                    ChangeState(State.ATTACK_3);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.ATTACK_4);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 5)
                {
                    ChangeState(State.ATTACK_5);
                    yield break;  // 코루틴 종료
                }
            }
            else //2페이즈 혹은 3페이즈라면
            {
                float randomValue = UnityEngine.Random.Range(0f, 7f);

                if (randomValue < 1)
                {
                    ChangeState(State.ATTACK_1);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 2)
                {
                    ChangeState(State.ATTACK_2);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 3)
                {
                    ChangeState(State.ATTACK_3);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.ATTACK_4);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 5)
                {
                    ChangeState(State.ATTACK_5);
                    yield break;  // 코루틴 종료
                }

                else if (randomValue < 6)
                {
                    ChangeState(State.ATTACK_6);
                    yield break;  // 코루틴 종료
                }

                else if (randomValue < 7)
                {
                    ChangeState(State.ATTACK_7);
                    yield break;  // 코루틴 종료
                }
            }
        }

        if (distanceToBase >= 100f) //집 거리 100f 멀어지면 복귀
        {
            hpSliderObject.SetActive(false);
            GameManager.Instance.StopBackgroundMusic();
            GameManager.Instance.VolumeDown();
            GameManager.Instance.PlayBackgroundMusic();
            ChangeState(State.BACK);
            yield break;
        }

        yield return null;
    }

    IEnumerator DIVE()
    {
        //좌우 랜덤 구르기
        float randomDive = UnityEngine.Random.Range(0f, 1f);
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

        if (distanceToPlayer < attackRange)   //구르고 플레이어가 가까우면 반격.
        {
            if (attackCount >= attackMaxCount)
            {
                float randomValue = UnityEngine.Random.Range(0f, 6f);

                if (randomValue < 2)
                {
                    ChangeState(State.COMBO_1);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.COMBO_2);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 6)
                {
                    ChangeState(State.COMBO_3);
                    yield break;  // 코루틴 종료
                }
            }
            if (distanceToPlayer <= attackRange)
            {
                if (!is2Phase && !is3Phase)  //2페이즈가 아니고 3페이즈가 아니라면 평타모션 5개중 랜덤 수행
                {
                    float randomValue = UnityEngine.Random.Range(0f, 5f);

                    if (randomValue < 1)
                    {
                        ChangeState(State.ATTACK_1);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 2)
                    {
                        ChangeState(State.ATTACK_2);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 3)
                    {
                        ChangeState(State.ATTACK_3);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 4)
                    {
                        ChangeState(State.ATTACK_4);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 5)
                    {
                        ChangeState(State.ATTACK_5);
                        yield break;  // 코루틴 종료
                    }
                }
                else //2페이즈 혹은 3페이즈라면
                {
                    float randomValue = UnityEngine.Random.Range(0f, 7f);

                    if (randomValue < 1)
                    {
                        ChangeState(State.ATTACK_1);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 2)
                    {
                        ChangeState(State.ATTACK_2);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 3)
                    {
                        ChangeState(State.ATTACK_3);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 4)
                    {
                        ChangeState(State.ATTACK_4);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 5)
                    {
                        ChangeState(State.ATTACK_5);
                        yield break;  // 코루틴 종료
                    }

                    else if (randomValue < 6)
                    {
                        ChangeState(State.ATTACK_6);
                        yield break;  // 코루틴 종료
                    }

                    else if (randomValue < 7)
                    {
                        ChangeState(State.ATTACK_7);
                        yield break;  // 코루틴 종료
                    }
                }
            }
        }

        ChangeState(State.CHECK);
        yield break;
    }

    IEnumerator STEP()
    { 
        //좌우 랜덤 사이드
        float randomDive = UnityEngine.Random.Range(0f, 1f);
        if (randomDive < 0.5f)
        {
            anim.CrossFade("side_r", 0.1f, 0, 0);
        }
        else
        {
            anim.CrossFade("side_l", 0.1f, 0, 0);
        }

        float attackDuration = 0.5f;  // 공격 애니메이션의 지속 시간
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

        if (distanceToPlayer < attackRange)   // 플레이어가 가까우면 반격.
        {
            if (attackCount >= attackMaxCount)
            {
                float randomValue = UnityEngine.Random.Range(0f, 6f);

                if (randomValue < 2)
                {
                    ChangeState(State.COMBO_1);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.COMBO_2);
                    yield break;  // 코루틴 종료
                }
                else if (randomValue < 6)
                {
                    ChangeState(State.COMBO_3);
                    yield break;  // 코루틴 종료
                }
            }
            if (distanceToPlayer <= attackRange)
            {
                if (!is2Phase && !is3Phase)  //2페이즈가 아니고 3페이즈가 아니라면 평타모션 5개중 랜덤 수행
                {
                    float randomValue = UnityEngine.Random.Range(0f, 5f);

                    if (randomValue < 1)
                    {
                        ChangeState(State.ATTACK_1);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 2)
                    {
                        ChangeState(State.ATTACK_2);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 3)
                    {
                        ChangeState(State.ATTACK_3);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 4)
                    {
                        ChangeState(State.ATTACK_4);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 5)
                    {
                        ChangeState(State.ATTACK_5);
                        yield break;  // 코루틴 종료
                    }
                }
                else //2페이즈 혹은 3페이즈라면
                {
                    float randomValue = UnityEngine.Random.Range(0f, 7f);

                    if (randomValue < 1)
                    {
                        ChangeState(State.ATTACK_1);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 2)
                    {
                        ChangeState(State.ATTACK_2);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 3)
                    {
                        ChangeState(State.ATTACK_3);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 4)
                    {
                        ChangeState(State.ATTACK_4);
                        yield break;  // 코루틴 종료
                    }
                    else if (randomValue < 5)
                    {
                        ChangeState(State.ATTACK_5);
                        yield break;  // 코루틴 종료
                    }

                    else if (randomValue < 6)
                    {
                        ChangeState(State.ATTACK_6);
                        yield break;  // 코루틴 종료
                    }

                    else if (randomValue < 7)
                    {
                        ChangeState(State.ATTACK_7);
                        yield break;  // 코루틴 종료
                    }
                }
            }
        }

        ChangeState(State.CHECK);
        yield break;
    }

    //공격패턴들

    IEnumerator ATTACK_1()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_1", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.05f; // 공격 애니메이션의 지속 시간

        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
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

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //공격 후 30% 확률로 스탭 밟고 다시 공격.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if(is3Phase) //3페이즈라면 패턴 쉬는 시간 없이 바로 추격.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_2()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_2", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.05f; // 공격 애니메이션의 지속 시간

        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
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

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //공격 후 30% 확률로 스탭 밟고 다시 공격.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3페이즈라면 패턴 쉬는 시간 없이 바로 추격.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_3()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_3", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.2f;  // 공격 애니메이션의 지속 시간

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
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

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //공격 후 30% 확률로 스탭 밟고 다시 공격.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3페이즈라면 패턴 쉬는 시간 없이 바로 추격.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_4()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_4", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 2.6f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
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
        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //공격 후 30% 확률로 스탭 밟고 다시 공격.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3페이즈라면 패턴 쉬는 시간 없이 바로 추격.
        {
            ChangeState(State.CHASE);
            yield break;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_5()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_5", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 2.2f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
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
        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //공격 후 30% 확률로 스탭 밟고 다시 공격.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3페이즈라면 패턴 쉬는 시간 없이 바로 추격.
        {
            ChangeState(State.CHASE);
            yield break;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_6()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_6", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.4f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
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

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //공격 후 30% 확률로 스탭 밟고 다시 공격.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3페이즈라면 패턴 쉬는 시간 없이 바로 추격.
        {
            ChangeState(State.CHASE);
            yield break;
        }
        // StateMachine을 경계으로 변경

        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_7()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_7", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.9f;  // 공격 애니메이션의 지속 시간
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
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
        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //공격 후 30% 확률로 스탭 밟고 다시 공격.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3페이즈라면 패턴 쉬는 시간 없이 바로 추격.
        {
            ChangeState(State.CHASE);
            yield break;
        }
        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }


    IEnumerator COMBO_1()
    {
        if (is2Phase)
        {
            attackCount = 0;
        }

        anim.CrossFade("combo_1", 0.1f, 0, 0);
        float attackDuration = 6.3f;  // 공격 애니메이션의 지속 시간

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
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

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //공격 후 30% 확률로 스탭 밟고 다시 공격.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3페이즈라면 패턴 쉬는 시간 없이 바로 추격.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator COMBO_2()
    {
        if (is2Phase)
        {
            attackCount = 0;
        }

        anim.CrossFade("combo_2", 0.1f, 0, 0);
        float attackDuration = 4.1f;  // 공격 애니메이션의 지속 시간

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
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

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //공격 후 30% 확률로 스탭 밟고 다시 공격.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3페이즈라면 패턴 쉬는 시간 없이 바로 추격.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator COMBO_3()
    {
        if (is2Phase)
        {
            attackCount = 0;
        }

        anim.CrossFade("combo_3", 0.1f, 0, 0);
        float attackDuration = 4.23f;  // 공격 애니메이션의 지속 시간

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
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

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //공격 후 30% 확률로 스탭 밟고 다시 공격.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3페이즈라면 패턴 쉬는 시간 없이 바로 추격.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine을 경계으로 변경
        ChangeState(State.CHECK);
        yield return null;
    }





    void Update()
    {
        UpdateHPBar();

        directionToPlayer = new Vector3(player.position.x - transform.position.x,
            0f, player.position.z - transform.position.z);    //플레이어와의 위치관계
        distanceToPlayer = directionToPlayer.magnitude;                                       //플레이어와의 위치관계 수치화

        directionToBase = new Vector3(initialPoint.x - transform.position.x,
            0f, initialPoint.z - transform.position.z);
        distanceToBase = directionToBase.magnitude;

        if (HP < 150 && HP > 60)//페이즈 변화 체크, 페이즈 조절은 여기서
        {
            is2Phase = true;
        }
        else if (HP <= 60)
        {
            is3Phase = true;
            attackMaxCount = 2;
        }

        if (state != State.BACK && state != State.DIE && state != State.IDLE && state != State.DIVE )
        {
            LookPlayer(directionToPlayer);
        }

        if (state == State.CHASE)
        {
            ChasePlayer();
        }

        if (state == State.DIE || state == State.ATTACK_1 || state == State.ATTACK_2 || state == State.ATTACK_3 || state == State.ATTACK_4 || state == State.ATTACK_5 || state == State.ATTACK_6 || state == State.ATTACK_7
            || state == State.COMBO_1 || state == State.COMBO_2 || state == State.COMBO_3 || state ==  State.DIVE || state == State.STEP )
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
        hpSliderObject = Instantiate(hpSliderPrefab, GameObject.Find("Canvas").transform); // 슬라이더 생성 및 부모를 Canvas로 설정
        hpSlider = hpSliderObject.GetComponent<Slider>();
        bossName = hpSliderObject.GetComponent<Text>();
        UpdateHPBar();
        bossName.text = "검은달 기사 레온";
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

    void ChasePlayer()                      //추격 기능
    {
        nmAgent.speed = chaseSpeed;
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
        if (weaponAttackRange != null)
        {
            weaponAttackRange.GetComponent<Collider>().enabled = enable;
        }

    }
    //데미지 판정
    void OnTriggerEnter(Collider other)
    {
        if (isAlreadyHit) { return; }
        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttackHit)
        {
            HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
            Debug.Log("아프다!");
            isHit = true;
            PRSAttacked();

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
    }

    public void PRSFoot()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, footSound.Length);
        AudioClip selectedClip = footSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSDive()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, diveSound.Length);
        AudioClip selectedClip = diveSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSDash()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, dashSound.Length);
        AudioClip selectedClip = dashSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSSlash()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, slashSound.Length);
        AudioClip selectedClip = slashSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSAir()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, airSound.Length);
        AudioClip selectedClip = airSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSClash()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, crashSound.Length);
        AudioClip selectedClip = crashSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSAttacked()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, attackedSound.Length);
        AudioClip selectedClip = attackedSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }
}
