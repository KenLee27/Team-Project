using System.Collections;
using UnityEngine;

public class SuperPlayerController : MonoBehaviour
{
    private float smoothMoveTime = 0.15f;
    private float speedVelocity;
    private float currentSpeed;
    private float targetSpeed;
    private float rotationVelocity;
    private float smoothRotationTime = 0.15f;
    private Vector3 lastMovement;                  //최근 이동 방향
    private float timeSinceLastDive;

    public string currentWeapon;                  //무기를 받음
    public string currentWeaponName;              //무기의 이름을 받음
    public string currentWeaponSkill;             //무기의 스킬을 받음
    public void SetCurrentWeaponType(string weaponType)
    {
        currentWeapon = weaponType;

        // 언더바로 구분하여 무기 이름과 무기 스킬 저장
        string[] parts = currentWeapon.Split('_'); // 언더바로 문자열 분리

        if (parts.Length == 2) // 두 부분이 있는지 확인
        {
            currentWeaponName = parts[0];        // 첫 번째 부분은 무기 이름
            currentWeaponSkill = parts[1];       // 두 번째 부분은 무기 스킬
        }
        else
        {
            // 예외 처리: 유효하지 않은 형식
            currentWeaponName = string.Empty;
            currentWeaponSkill = string.Empty;
            Debug.Log("Invalid weapon type format.");
        }
    }

    public float moveSpeed = 4f;                   // 이동 속도
    public float jumpForce = 5f;                   // 점프 힘
    public float resetPhaseDelay = 1.2f;             // 공격 리셋 시간
    public float DiveDelay = 0.8f;                 // 다이브 쿨타임
    public float PlayerHP = 0f;
    public float PlayerAtk = 3f;    
    public float PlayerMaxHP = 100f;
    public float PlayerStamina = 0f;
    public float PlayerMaxStamina = 100f;
    public float StaminaRegenTime = 0.1f;          //스테미나 회복을 위해 스테미나 소모를 멈추고 기다려야 하는 시간
    public float StaminaRegenSpeed = 20f;          //스테미나 초당 회복 수치
    public float PlayerMana = 0f; // 현재 마나
    public float PlayerMaxMana = 100f; // 최대 마나
    public float playerSoul = 0f;

    public float totalHPRecovery = 40f; // 총 회복할 체력
    private float recoveryPerSecond;
    public float totalManaRecovery = 40f; // 총 회복할 체력
    private float recoveryManaPerSecond;
    public float nowPosion = 0;
    public float maxPosion = 2f;
    public float nowMpPosion = 0;
    public float maxMpPosion = 2f;

    public InPutBuffer inputBuffer;

    private Vector2 velocity = Vector2.zero;

    public float PlayerDamage = 3f;                //플레이어 데미지

    public SuperCameraController cameraController; // SuperCameraController 참조
    public Animator animator;                      // 애니메이터 참조
    private Rigidbody rb;

    public bool isGround = true;
    public bool isMoving = false;
    public bool isAttacking = false;
    public bool isDive = false;
    public bool isStand = true;
    public bool isAttacked = false;
    public bool isinvincibility = false;
    public bool isDie = false;
    private bool isRunning = false;  // 플레이어가 현재 달리는 중인지 체크하는 변수
    public bool isAttackHit = false;
    public bool FirstStaminaCheck = false;
    private bool CanSave = false;

    private bool firstDropDie = true;


    private Coroutine resetPhaseCoroutine;
    private Coroutine resetS_PhaseCoroutine;

    private float keyHoldTime = 0.0f;  // LeftControl 키가 눌린 시간을 측정할 변수
    public float crouchHoldThreshold = 1.0f;  // 달리기와 웅크리기의 기준 시간 (1초)
    public Transform myHand;


    private enum State
    {
        IDLE,
        MOVE,
        JUMP,
        ATTACK,
        DIVE,
        HIT,
        DIE,
        DRINK,
        SAVE
    }

    private State currentState = State.IDLE;
    private Transform cameraTransform; // 카메라 Transform 변수 추가

    private int attackPhase = -1;
    private int s_attackPhase = 0;
    KeyCode bufferedInput;
    public bool canAttack = true; // 공격 가능 여부
    public bool canDive = true;
    public bool canCrouched = true;
    public bool CanDrink = true;

    private float attackDelay = 1f; // 각 공격 사이의 딜레이
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    public void SetTransparency(float alpha)
    {
        foreach (var renderer in skinnedMeshRenderers)
        {
            foreach (var material in renderer.materials // 각 Skinned Mesh의 모든 Material을 순회
            )
            {
                // URP/Lit인지 확인
                if (alpha <= 0f)
                {
                    // Surface Type을 Transparent로 변경
                    material.SetFloat("_Surface", 1.0f);
                }
                else
                {
                    // Surface Type을 Opaque로 변경
                    material.SetFloat("_Surface", 0f);
                }

                // 알파 값 변경
                Color color = material.color;
                color.a = alpha; // 원하는 알파 값으로 설정
                material.color = color;

                // 블렌딩 모드 설정
                if (alpha < 1f) // 투명한 경우
                {
                    material.SetFloat("_Blend", 0.0f); // 0 = Alpha
                    material.SetInt("_ZWrite", 0); // 투명 재질에 대해 ZWrite 비활성화
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent; // 투명 큐 설정
                }
                else // 불투명한 경우
                {
                    material.SetFloat("_Blend", 1.0f); // 블렌딩을 불투명으로 설정
                    material.SetInt("_ZWrite", 1); // 불투명 재질에 대해 ZWrite 활성화
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.EnableKeyword("_SURFACE_TYPE_OPAQUE");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry; // 불투명 큐 설정 (2000)
                }
            }
        }
    }


    void Start()
    {
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        transform.position = GameManager.Instance.LoadPosition();        //게임매니저에 저장된 스폰 위치로 부활
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isinvincibility = false;
        firstDropDie = true;

        PlayerMana = PlayerMaxMana;
        PlayerHP = PlayerMaxHP;
        PlayerStamina = PlayerMaxStamina;
        nowPosion = maxPosion;
        nowMpPosion = maxMpPosion;

        GameManager.Instance.UpdatePlayerHP(PlayerHP);
        GameManager.Instance.UpdatePlayerST(PlayerStamina);
        GameManager.Instance.UpdatePlayerMana(PlayerMana);
        
        isAttackHit = false;
        timeSinceLastDive = StaminaRegenTime;
        recoveryPerSecond = totalHPRecovery / 1.7f;
        recoveryManaPerSecond = totalManaRecovery / 1.7f;

        StandUpTime();
    }

    private void HandleEmptyDrink()
    {
        animator.Play("No_Item_Drink", 0, 0);
        CanDrink = false;
        canDive = false;
        canCrouched = false;
        canAttack = false;
        StartCoroutine(EmptyDrinkDelay());
    }

    private IEnumerator EmptyDrinkDelay()
    {
        float startTime = Time.time;
        while (Time.time < startTime + 3.6f)                                //애니메이션 시간
        {
            if (currentState == State.HIT)
            {
                yield break;
            }
            yield return null;
        }
        CanDrink = true;
        canDive = true;
        canCrouched = true;
        canAttack = true;
        currentState = State.IDLE;

        yield return null;
    }

    private void HandleHPDrink()
    {
        animator.Play("Item_Drink", 0, 0);
        CanDrink = false;
        canDive = false;
        canCrouched = false;
        canAttack = false;
        StartCoroutine(HPDrinkDelay());
    }

    private IEnumerator HPDrinkDelay()
    {
        float startTime = Time.time;
        while (Time.time < startTime + 1.7f)                                //애니메이션 시간
        {
            if (currentState == State.HIT)
            {
                yield break;
            }

            PlayerHP += recoveryPerSecond * Time.deltaTime; // 매 프레임마다 체력을 증가시킴
            PlayerHP = Mathf.Clamp(PlayerHP, 0, PlayerMaxHP); // 체력을 최대값으로 클램프

            GameManager.Instance.UpdatePlayerHP(PlayerHP);
            yield return null;
        }
        CanDrink = true;
        canDive = true;
        canCrouched = true;
        canAttack = true;
        currentState = State.IDLE;

        yield return null;
    }

    private void HandleMPDrink()
    {
        animator.Play("Item_Drink", 0, 0);
        CanDrink = false;
        canDive = false;
        canCrouched = false;
        canAttack = false;
        StartCoroutine(MPDrinkDelay());
    }

    private IEnumerator MPDrinkDelay()
    {
        float startTime = Time.time;
        while (Time.time < startTime + 1.7f)                                //애니메이션 시간
        {
            if (currentState == State.HIT)
            {
                yield break;
            }

            PlayerMana += recoveryManaPerSecond * Time.deltaTime; // 매 프레임마다 체력을 증가시킴
            PlayerMana = Mathf.Clamp(PlayerMana, 0, PlayerMaxMana); // 체력을 최대값으로 클램프

            GameManager.Instance.UpdatePlayerMana(PlayerMana);
            yield return null;
        }
        CanDrink = true;
        canDive = true;
        canCrouched = true;
        canAttack = true;
        currentState = State.IDLE;

        yield return null;
    }

    void Update()
    {
        if(!isGround)
        {
            timeSinceLastDive = 0;
        }
        HandleBuffer();

        GameManager.Instance.UpdatePlayerST(PlayerStamina);

        if (Input.GetKeyDown(KeyCode.P))                                //P키 입력시 게임진도 초기화 테스트 전용 스크립트
        {
            PlayerPrefs.DeleteAll(); // 모든 PlayerPrefs 데이터 삭제
            PlayerPrefs.Save(); // 즉시 저장
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && CanDrink && (currentState == State.IDLE || currentState == State.MOVE) && isStand)                                //F키 입력시 게임매니저 SavePosition을 실행
        {
            if (nowPosion <= 0)
            {
                currentState = State.DRINK;

                HandleEmptyDrink();
            }
            else
            {
                nowPosion -= 1;

                currentState = State.DRINK;

                HandleHPDrink();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && CanDrink && (currentState == State.IDLE || currentState == State.MOVE) && isStand)                                //F키 입력시 게임매니저 SavePosition을 실행
        {
            if (nowMpPosion <= 0)
            {
                currentState = State.DRINK;

                HandleEmptyDrink();
            }
            else
            {
                nowMpPosion -= 1;

                currentState = State.DRINK;

                HandleMPDrink();
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && CanSave && (currentState == State.IDLE || currentState == State.MOVE))                                //F키 입력시 게임매니저 SavePosition을 실행
        {
            Vector3 directionToSavePoint = (collider.transform.position - transform.position).normalized; // 세이브 포인트 방향
            directionToSavePoint.y = 0; // Y축 회전 유지
            Quaternion targetRotation = Quaternion.LookRotation(directionToSavePoint);
            transform.rotation = targetRotation; // 플레이어 회전

            GameManager.Instance.SavePosition(transform.position);
            Debug.Log("save!");

            string savePointName = collider.name; // 세이브 포인트 이름 가져오기
            UIManager.Instance.ActivateSaveButton(savePointName);


            HandleSave();
            // 버튼 활성화 상태 저장
            if(PlayerPrefs.GetInt(collider.name, 0) == 0)
            {
                PlayerPrefs.SetInt(savePointName, 1);  // 1은 버튼이 활성화되었음을 의미
                PlayerPrefs.Save();
            }
        }

        //무기 스위칭 확인
        if (currentWeaponName == "Falchion" || currentWeaponName == "Longsword")
        {
            animator.SetFloat("Blend", 0f);
        }
        else if (currentWeaponName == "Axe")
        {
            animator.SetFloat("Blend", 0.5f);
        }
        else if (currentWeaponName == "Dagger")
        {
            animator.SetFloat ("Blend", 1f);
        }

        //플레이어 스테미나 컨트롤러
        if ( PlayerStamina < 15f )
        {
            canDive = false;
            FirstStaminaCheck = true;
        }
        else if( PlayerStamina >=15 && FirstStaminaCheck)
        {
            canDive = true;
            FirstStaminaCheck = false;
        }

        //회피 시전 후 1초가 지나야 스테미나가 회복 가능.
        if (timeSinceLastDive >= StaminaRegenTime)
        {
            RecoverStamina();
        }
        else
        {
            timeSinceLastDive += Time.deltaTime;
        }

        //사망 컨트롤러
        if (PlayerHP <= 0)
        {
            HandleDead();
        }

        //서있을 때와 앉아 있을 때의 속도 컨트롤러
        if(isStand && !isRunning)
        {
            moveSpeed = 4f;
        }
        else if(isStand && isRunning)
        {
            moveSpeed = 5.6f;
        }
        else if(!isStand)
        {
            moveSpeed = 2.5f;
        }

        //락온 상태일때 적 바라보기 기능
        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {
            RotateTowardsEnemy();
        }

        HandleState();

        //점프 컨트롤러
        if (Input.GetButtonDown("Jump") && isGround && !isAttacking && !isDive && currentState != State.HIT && currentState != State.DRINK)
        {
            currentState = State.JUMP;
        }

        //버튼 클릭 & 공격 컨트롤러
        if ((currentState == State.DIVE || currentState == State.ATTACK || currentState == State.IDLE || currentState == State.MOVE)&& isGround )
        {
            if (canDive && canAttack && inputBuffer.GetBufferedInput(out bufferedInput) && PlayerStamina >= 5f)
            {
                if (bufferedInput == KeyCode.Mouse0)
                {
                    HandleAttack();
                    Debug.Log(currentWeaponName + " 발동");
                }
                else if (bufferedInput == KeyCode.Mouse1 && PlayerMana >= 10)
                {
                    HandleSpecialAttack();
                    Debug.Log(currentWeaponName + " 발동");
                }

                else if (cameraController.IsLockedOn && cameraController.LockedTarget != null && PlayerStamina >= 15f)
                {
                    if (Input.GetAxisRaw("Horizontal") < 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_left();
                            Debug.Log("좌로 구른다!");
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") > 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_Right();
                            Debug.Log("우로 구른다!");
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") > 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_Forward();
                            Debug.Log("앞로 구른다!");
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") < 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_Back();
                            Debug.Log("뒤로 구른다!");
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleStep_Back();
                            Debug.Log("백스탭!");
                        }
                    }
                }
                else if(PlayerStamina >= 15f)
                {
                    if (Input.GetAxisRaw("Horizontal") != 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_Forward();
                            Debug.Log("구른다!");
                        }
                    }
                    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") != 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_Forward();
                            Debug.Log("구른다!");
                        }
                    }
                    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleStep_Back();
                            Debug.Log("백스탭!");
                        }
                    }
                }
            }
        }

        //앉기
        if (!isMoving || cameraController.IsLockedOn)
        {
            keyHoldTime = 0.0f;

        }

        // LeftControl 키가 눌렸을 때 시간 증가
        if (Input.GetKey(KeyCode.LeftControl))
        {
            keyHoldTime += Time.deltaTime;  // 눌린 시간 증가
        }

        // LeftControl 키를 뗐을 때 조건 처리
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            // 달리기를 중지하고 isRunning 변수를 false로 설정
            if (isRunning)
            {
                animator.SetBool("isRunning", false);
                isRunning = false;
            }
            else if (keyHoldTime < crouchHoldThreshold && canCrouched && isGround && !isAttacking && !isDive)
            {
                // 1초 미만으로 눌렸을 때 웅크리기 동작
                HandleCrouched();
            }

            // 키를 뗀 후 시간 초기화
            keyHoldTime = 0.0f;
        }

        // 1초 이상 누르고 있고, isMoving이 true이며 아직 달리고 있지 않을 때
        if (keyHoldTime >= crouchHoldThreshold && isMoving && !isRunning && isGround)
        {
            // 달리기 동작 시작
            animator.SetBool("isRunning", true);
            isRunning = true;
        }

        // 달리는 중일 때, 키를 계속 누르고 있는지 확인하여 유지
        if (isRunning && !Input.GetKey(KeyCode.LeftControl))
        {
            // 키를 놓았을 경우 달리기 멈추기
            animator.SetBool("isRunning", false);
            isRunning = false;
        }

        if (isRunning && cameraController.IsLockedOn)
        {
            // 키를 놓았을 경우 달리기 멈추기
            animator.SetBool("isRunning", false);
            isRunning = false;
        }

        if (isRunning)
        {
            timeSinceLastDive = 0;
            PlayerStamina -= 5f * Time.deltaTime;
            if (PlayerStamina <= 0)
            {
                PlayerStamina = 0f;
                animator.SetBool("isRunning", false);
                isRunning = false;
            }
        }

        /*
        if (isRunning &&!IsCurrentAnimation("Run"))
        {
            animator.SetBool("isRunning", false);  // 애니메이터의 isRunning을 false로 설정
            isRunning = false;  // 달리기 상태도 false로 초기화
        }*/
        HideWeapon();
    }

    bool IsCurrentAnimation(string animationName)
    {
        // 애니메이터의 현재 애니메이션 상태 가져오기
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);  // 레이어 0 사용
        return currentState.IsName(animationName);
    }

    private void HandleBuffer()
    {
        if(Input.GetMouseButtonDown(0))
        {
            inputBuffer.ReplaceBufferedInput(KeyCode.Mouse0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            inputBuffer.ReplaceBufferedInput(KeyCode.Mouse1);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            inputBuffer.ReplaceBufferedInput(KeyCode.LeftShift);
        }
    }

    private void HandleSave()
    {
        isinvincibility = true;
        canAttack = false;
        canDive = false;
        canCrouched = false;
        CanSave = false;
        //GameManager.Instance.UpdatePlayerHP(PlayerHP);
        if(PlayerPrefs.GetInt(collider.name, 0) == 0)
        {
            animator.Play("Save", 0, 0);

            currentState = State.SAVE;
            StartCoroutine(SaveDelay());
        }
        else
        {
            animator.Play("rest", 0, 0);

            nowPosion = maxPosion;
            nowMpPosion = maxMpPosion;
            PlayerHP = PlayerMaxHP;
            PlayerMana = PlayerMaxMana;
            PlayerStamina = PlayerMaxStamina;

            GameManager.Instance.UpdatePlayerHP(PlayerHP);
            GameManager.Instance.UpdatePlayerMana(PlayerMana);

            currentState = State.SAVE;
            StartCoroutine(RestDelay());
        }
    }

    private IEnumerator SaveDelay()
    {
        yield return new WaitForSeconds(4f);
        isinvincibility = false;
        canAttack = true;
        canDive = true;
        canCrouched = true;
        CanSave = true;
        currentState = State.IDLE;
    }

    private IEnumerator RestDelay()
    {
        yield return new WaitForSeconds(2f);

        UIManager.Instance.OpenTeleport();
        UIManager.Instance.LoadSaveButtonState();

        while (true)
        {
            if (UIManager.Instance.currentState == UIManager.UIState.Game)        //이 부분은 차후 수정가능.
            {
                animator.Play("rest_end", 0, 0);
                StartCoroutine(StandUpTime());
                break;
            }

            yield return null;
        }
        yield return null;
    }

    private IEnumerator StandUpTime()
    {
        currentState = State.SAVE;
        yield return new WaitForSeconds(2.1f);
        isinvincibility = false;
        canAttack = true;
        canDive = true;
        canCrouched = true;
        CanSave = true;
        currentState = State.IDLE;
    }


    void RecoverStamina()
    {
        PlayerStamina += StaminaRegenSpeed * Time.deltaTime;
        if (PlayerStamina > PlayerMaxStamina)
        {
            PlayerStamina = PlayerMaxStamina;
        }
    }


    private void HandleDead()
    {
        isinvincibility = true;
        canAttack = false;
        canDive = false;

        //GameManager.Instance.UpdatePlayerHP(PlayerHP);
        animator.SetBool("isDead", true);


        currentState = State.DIE;
        isDie = true;
    }

    private void HandleCrouched()
    {
        if(isStand == true)
        {
            Debug.Log("앉아야지");
            isStand = false;
        }
        else if(isStand == false)
        {
            Debug.Log("일어나야지");
            isStand = true;
        }

        animator.SetBool("isCrouching",!isStand);

        StartCoroutine(EnableNextCrouchedAfterDelay());
    }

    private IEnumerator EnableNextCrouchedAfterDelay()
    {
        canCrouched = false;
        Debug.Log("앉기 쿨타임");
        yield return new WaitForSeconds(0.2f);
        Debug.Log("앉기 쿨타임 끝!");
        canCrouched = true;
    }

    private void HandleStep_Back()
    {
        canDive = false;
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("BackStep", 0.1f, 0, 0);
        StartCoroutine(BackStepDirection());
        StartCoroutine(EnableNextBackStepAfterDelay());
    }
        

    private IEnumerator EnableNextBackStepAfterDelay()
    {

        Debug.Log("백스탭 쿨타임");
        yield return new WaitForSeconds(0.7f);
        Debug.Log("백스탭 쿨타임 끝!");
        canDive = true;
        currentState = State.IDLE;
    }

    private IEnumerator BackStepDirection()
    {
        PlayerStamina -= 15f;
        isStand = true;
        float startTime = Time.time;
        while (Time.time < startTime + 0.7f)                                //애니메이션 시간
        {
            transform.Translate(Vector3.back * 3f * Time.deltaTime);

            //벡스탭 실행 중에 스테미나 회복 대기 시간 0초 고정
            timeSinceLastDive = 0f;
            yield return null;
        }
        Debug.Log("애니메이션 끝!");
        animator.SetBool("isCrouching", !isStand);
        isDive = false;
        currentState = State.IDLE;
    }

    private void HandleDive_left()
    {
        canDive = false;
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("LeftDive",0.1f,0,0);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private void HandleDive_Right()
    {
        canDive = false;
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("RightDive",0.1f,0,0);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private void HandleDive_Forward()
    {
        canDive = false;
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("ForwardDive", 0.1f,0,0);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private void HandleDive_Back()
    {
        canDive = false;
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("BackDive", 0.1f,0,0);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private IEnumerator EnableNextDiveAfterDelay()
    {

        Debug.Log("다이브 쿨타임");
        yield return new WaitForSeconds(DiveDelay);
        Debug.Log("다이브 쿨타임 끝!");
        canDive = true;
    }


    public void TriggerDive()
    {
        isinvincibility = true;
    }
    public void EndDive()
    {
        isinvincibility = false;
    }


    private IEnumerator DiveDirection()
    {
        PlayerStamina -= 15f;
        isStand = true;
        canAttack = false;

        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = Time.time;
        while (Time.time < startTime + 0.7f)        //다이브 시간
        {
            //다이브 애니메이션이 실행 중일 때 스테미나 회복 대기 시간 0초 고정
            timeSinceLastDive = 0f;
            yield return null;
        }
        animator.SetBool("isCrouching", !isStand);
        currentState = State.IDLE;
        isDive = false;
        isMoving = true;
        canAttack = true;
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case State.IDLE:
                HandleMove();
                CanDrink = true;
                break;
            case State.MOVE:
                HandleMove();
                CanDrink = true;
                break;
            case State.JUMP:
                HandleJump();

                HandleMove();
                break;
            case State.ATTACK:
                currentSpeed = 0;
                isMoving = false;
                break;
            case State.DIVE:
                currentSpeed = 0;
                isMoving = false;
                break;
            case State.SAVE:
                currentSpeed = 0;
                isMoving = false;
                break;
            case State.HIT:
                currentSpeed = 0;
                isMoving = false;
                break;
            case State.DRINK:
                currentSpeed = 0;
                isMoving = false;
                break;
        }
    }

    private void HideWeapon()
    {
        Transform nowWeapon = myHand.transform.Find(currentWeapon + "_Instance");
        if(nowWeapon == null)
        {
            return;
        }
        bool FirstCheck = true;
        if (currentState == State.DRINK)
        {
            nowWeapon.gameObject.SetActive(false);
            FirstCheck = true;
        }
        else if(FirstCheck == true)
        {
            FirstCheck = false;
            nowWeapon.gameObject.SetActive(true);
        }
    }

    private void HandleMove()
    {
        //움직이지 못하는 상태 구현
        if (currentState == State.SAVE || currentState == State.DIVE || currentState == State.ATTACK || currentState == State.HIT || currentState == State.DIE)
        {
            return;
        }

        //움직임 구현
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        isMoving = inputDir != Vector2.zero;

        animator.SetBool("isMoving", isMoving);

        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {
            animator.SetBool("isLockOn", true);

            Vector3 toTarget = (cameraController.LockedTarget.position - transform.position).normalized;
            float targetRotation = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
            float newRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, smoothRotationTime);
            transform.eulerAngles = Vector3.up * newRotation;

            Vector3 moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;
            targetSpeed = moveSpeed * moveDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
            transform.Translate(moveDir.normalized * currentSpeed * Time.deltaTime, Space.World);

            Vector3 localMoveDir = transform.InverseTransformDirection(moveDir);
            float smoothX = Mathf.SmoothDamp(animator.GetFloat("Xaxis"), localMoveDir.x, ref velocity.x, 0.1f);
            float smoothY = Mathf.SmoothDamp(animator.GetFloat("Yaxis"), localMoveDir.z, ref velocity.y, 0.1f);

            animator.SetFloat("Xaxis", smoothX);
            animator.SetFloat("Yaxis", smoothY);


        }
        else
        {
            animator.SetBool("isLockOn", false);
            if (isMoving)
            {
                float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float newRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, smoothRotationTime);
                transform.eulerAngles = Vector3.up * newRotation;
            }

            targetSpeed = moveSpeed * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
            transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

            animator.SetBool("isLockOn", false);
        }



        if (!isMoving && currentState == State.MOVE)
        {
            currentState = State.IDLE;
        }
        else if (isMoving && currentState == State.IDLE)
        {
            currentState = State.MOVE;
        }
    }

    private void HandleJump()
    {
        if (!isStand)
        {
            isStand = true;
            animator.SetBool("isCrouching", !isStand);
            currentState = State.IDLE;

            return;
        }

        if (isGround && PlayerStamina > 10f && currentState != State.DRINK)
        {
            PlayerStamina -= 10f;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGround = false;
            animator.SetBool("isJumping", !isGround);
            animator.CrossFade("Jump", 0.1f, 0, 0);
        }

    }


    private void HandleAttack()
    {
        PlayerStamina -= 5f;
        currentState = State.ATTACK;
        isAttacking = true;
        canAttack = false; // 공격 가능 플래그를 false로 설정

        attackPhase++; // 공격 단계 증가

        switch (attackPhase)
        {
            case 0:
                if(currentWeaponName == "Axe")
                {
                    animator.CrossFade(currentWeaponName + "Attack_0", 0.1f);
                    PlayerDamage = (PlayerAtk / 3) * 5;
                    break;
                }
                else if(currentWeaponName == "Longsword")
                {
                    animator.CrossFade(currentWeaponName + "Attack_0", 0.1f);
                    PlayerDamage = (PlayerAtk / 3) * 4;
                    break;
                }
                else
                {
                    attackPhase++;
                }
                goto case 1;
            case 1:
                animator.CrossFade(currentWeaponName+"Attack_1", 0.1f);
                if (currentWeaponName == "Dagger")
                {
                    PlayerDamage = (PlayerAtk / 3) * 2;
                }
                else if (currentWeaponName == "Axe")
                {
                    PlayerDamage = (PlayerAtk / 3) * 5;
                }
                else if (currentWeaponName == "Falchion")
                {
                    PlayerDamage = PlayerAtk;
                }
                else if (currentWeaponName == "Longsword")
                {
                    PlayerDamage = (PlayerAtk / 3) * 4;
                }

                break;
            case 2:
                animator.CrossFade(currentWeaponName + "Attack_2", 0.1f);
                if (currentWeaponName == "Dagger")
                {
                    PlayerDamage = PlayerAtk;
                }
                else if (currentWeaponName == "Axe")
                {
                    PlayerDamage = PlayerAtk * 2;
                }
                else if (currentWeaponName == "Falchion")
                {
                    PlayerDamage = (PlayerAtk / 3) * 4;
                }
                else if (currentWeaponName == "Longsword")
                {
                    PlayerDamage = (PlayerAtk / 3) * 6;
                }

                break;
            case 3:
                animator.CrossFade(currentWeaponName + "Attack_3", 0.1f);
                if (currentWeaponName == "Dagger")
                {
                    PlayerDamage = (PlayerAtk / 3) * 5;
                }
                else if (currentWeaponName == "Axe")
                {
                    PlayerDamage = PlayerAtk * 3;
                }
                else if (currentWeaponName == "Falchion")
                {
                    PlayerDamage = PlayerAtk * 2;
                }
                else if (currentWeaponName == "Longsword")
                {
                    PlayerDamage = (PlayerAtk / 3) * 6;
                }

                break;
            default:
                return;
        }
        StartCoroutine(EnableNextAttackAfterDelay()); // 공격 가능 대기

        if (resetPhaseCoroutine != null)
        {
            StopCoroutine(resetPhaseCoroutine);
        }

        resetPhaseCoroutine = StartCoroutine(ResetAttackPhaseAfterDelay());
    }
    private void HandleSpecialAttack()
    {
        PlayerStamina -= 5f;
        currentState = State.ATTACK;
        isAttacking = true;
        canAttack = false; // 공격 가능 플래그를 false로 설정

        isStand = true;
        animator.SetBool("isCrouching", !isStand); //공격 후 서있는 자세


        s_attackPhase++; // 공격 단계 증가

        switch (s_attackPhase)
        {
            case 1:
                animator.CrossFade(currentWeaponSkill + "SpecialAttack_1", 0.1f,0,0);
                
                if (currentWeaponSkill == "Red")
                {
                    PlayerDamage = PlayerAtk * 2;
                    PlayerMana -= 10; // 1타 마나 소모
                }
                else if (currentWeaponSkill == "Exe")
                {
                    PlayerDamage = PlayerAtk * 4;
                    PlayerMana -= 10; // 1타 마나 소모
                }
                else if (currentWeaponSkill == "Cross")
                {
                    PlayerDamage = (PlayerAtk / 3) * 8;
                    PlayerMana -= 25; // 1타 마나 소모
                }
                else if (currentWeaponSkill == "Rot")
                {
                    PlayerDamage = (PlayerAtk / 3) * 3;
                    PlayerMana -= 30; // 1타 마나 소모
                }
                else if (currentWeaponSkill == "Elite")
                {
                    PlayerDamage = (PlayerAtk / 3) * 15;
                    PlayerMana -= 20; // 1타 마나 소모
                }
                else if (currentWeaponSkill == "Blood")
                {
                    PlayerDamage = (PlayerAtk / 3) * 12;
                    PlayerMana -= 12; // 1타 마나 소모
                }
                GameManager.Instance.UpdatePlayerMana(PlayerMana); // 마나 UI 업데이트
                break;
            case 2:
                animator.CrossFade(currentWeaponSkill + "SpecialAttack_2", 0.1f,0,0);
                
                if (currentWeaponSkill == "Red")
                {
                    PlayerDamage = PlayerAtk * 2;
                    PlayerMana -= 10; // 2타 마나 소모
                    Debug.Log(PlayerDamage + " 데미지!");
                }
                else if (currentWeaponSkill == "Exe")
                {
                    PlayerDamage = PlayerAtk * 4;
                    PlayerMana -= 15; // 2타 마나 소모
                    Debug.Log(PlayerDamage + " 데미지!");
                }
                else if (currentWeaponSkill == "Cross")
                {
                    PlayerDamage = (PlayerAtk / 3) * 11;
                    PlayerMana -= 15; // 2타 마나 소모
                    Debug.Log(PlayerDamage + " 데미지!");
                }
                else if (currentWeaponSkill == "Elite")
                {
                    PlayerDamage = (PlayerAtk / 3) * 15;
                    PlayerMana -= 20; // 1타 마나 소모
                }
                GameManager.Instance.UpdatePlayerMana(PlayerMana); // 마나 UI 업데이트
                break;
            default:
                return;
        }
        StartCoroutine(EnableNextS_AttackAfterDelay()); // 공격 가능 대기

        if (resetS_PhaseCoroutine != null)
        {
            StopCoroutine(resetS_PhaseCoroutine);
        }

        resetS_PhaseCoroutine = StartCoroutine(ResetS_AttackPhaseAfterDelay());
    }

    private IEnumerator EnableNextAttackAfterDelay()
    {
        //무기 별 공격 단수 별 딜레이 조정
        if(currentWeaponName == "Falchion")
        {
            //공격 단수 별 딜레이 조정
            if (attackPhase == 3)
            {
                attackDelay = 1.8f;
            }
            else
            {
                attackDelay = 0.9f;
            }
        }
        else if (currentWeaponName == "Axe")
        {
            if (attackPhase == 3)
            {
                attackDelay = 1.8f;
            }
            else
            {
                attackDelay = 1.3f;
            }
        }
        else if (currentWeaponName == "Dagger")
        {
            //공격 단수 별 딜레이 조정
            if (attackPhase == 3)
            {
                attackDelay = 1f;
            }
            else if (attackPhase == 2)
            {
                attackDelay = 0.4f;
            }
            else if (attackPhase == 1)
            {
                attackDelay = 0.6f;
            }
        }
        else if (currentWeaponName == "Longsword")
        {
            if (attackPhase == 3)
            {
                attackDelay = 1.8f;
            }
            else
            {
                attackDelay = 1f;
            }
        }

        float startTime = Time.time;
        while (Time.time < startTime + attackDelay)     //공격(콤보 딜레이)
        {
            if(currentState ==State.HIT)
            {
                yield break;                            //맞을 경우 코루틴 종료
            }
            yield return null;
        }


        isStand = true;
        animator.SetBool("isCrouching", !isStand); //공격 후 서있는 자세


        Debug.Log("Can Attack!");
        canAttack = true; // 다시 공격 가능해짐
        isAttacking = false;

        if (attackPhase >= 3) // 공격이 완료된 경우
        {
            attackPhase = -1; // 공격 단계 초기화
        }
    }
    private IEnumerator EnableNextS_AttackAfterDelay()
    {
        //공격 단수 별 딜레이 조정

        if (currentWeaponSkill == "Cross")
        {
            if (s_attackPhase == 2)
            {
                attackDelay = 1.8f;
            }
            else
            {
                attackDelay = 1.2f;
            }
        }
        else if (currentWeaponSkill == "Exe")
        {
            attackDelay = 2.7f;
        }
        else if (currentWeaponSkill == "Red")
        {
            attackDelay = 1.3f;
        }
        else if (currentWeaponSkill == "Rot")
        {
            attackDelay = 3.1f;
        }
        else if (currentWeaponSkill == "Elite")
        {
            attackDelay = 1.9f;
        }
        else if (currentWeaponSkill == "Blood")
        {
            attackDelay = 2.4f;
        }
        float startTime = Time.time;
        while (Time.time < startTime + attackDelay)     //공격(콤보 딜레이)
        {
            if (currentWeaponSkill == "Rot")
            {
                transform.Translate(Vector3.forward * 2f * Time.deltaTime);
            }

            bool firstcheck = false;
            if (currentWeaponSkill == "Elite" && s_attackPhase == 2 && Time.time < startTime + 0.333f)
            {
                Transform Weapon_ = myHand.transform.Find(currentWeapon + "_Instance");

                SetTransparency(0f);
                transform.Translate(Vector3.forward * 9f * Time.deltaTime);
                Weapon_.gameObject.SetActive(false);
                isinvincibility = true;
            }
            else if(!firstcheck && s_attackPhase == 2)
            {
                firstcheck = true;
                Transform Weapon_ = myHand.transform.Find(currentWeapon + "_Instance");
                SetTransparency(1f);
                Weapon_.gameObject.SetActive(true);
                isinvincibility = false;
            }

            if (currentState == State.HIT)
            {
                yield break;                            //맞을 경우 코루틴 종료
            }
            yield return null;
        }


        Debug.Log("you can attack!");
        isStand = true;
        animator.SetBool("isCrouching", !isStand);
        isAttacking = false;
        canAttack = true; // 다시 공격 가능해짐

        if (s_attackPhase >= 2) // 공격이 완료된 경우
        {
            s_attackPhase = 0; // 공격 단계 초기화
        }
    }

    private IEnumerator ResetAttackPhaseAfterDelay()
    {
        //무기 별 공격 초기화 시간 조정
        if (currentWeaponName == "Falchion")
        {
            resetPhaseDelay = 1.2f;
        }
        else if (currentWeaponName == "Axe")
        {
            resetPhaseDelay = 2f;
        }
        else if (currentWeaponName == "Dagger")
        {
            resetPhaseDelay = 0.9f;
        }
        else if (currentWeaponName == "Longsword")
        {
            resetPhaseDelay = 1.4f;
        }
        float startTime = Time.time;
        while (Time.time < startTime + resetPhaseDelay)     //공격(콤보 딜레이)
        {
            timeSinceLastDive = 0f;

            if (currentState == State.HIT)
            {
                yield break;                            //맞을 경우 코루틴 종료
            }
            yield return null;
        }


        if (attackPhase > -1) // 공격 단계가 0이 아니면
        {
            attackPhase = -1; // 공격 단계 초기화
            Debug.Log("공격초기화!");
        }
    }
    private IEnumerator ResetS_AttackPhaseAfterDelay()
    {

        if (currentWeaponSkill == "Cross")
        {
            resetPhaseDelay = 2.2f;
        }
        else if (currentWeaponSkill == "Exe")
        {
            resetPhaseDelay = 2.2f;
        }
        else if (currentWeaponSkill == "Red")
        {
            resetPhaseDelay = 1.7f;
        }
        else if (currentWeaponSkill == "Rot")
        {
            resetPhaseDelay = 1.7f;
        }
        else if (currentWeaponSkill == "Elite")
        {
            resetPhaseDelay = 2.4f;
        }
        else if (currentWeaponSkill == "Blood")
        {
            resetPhaseDelay = 1f;
        }
        float startTime = Time.time;
        while (Time.time < startTime + resetPhaseDelay)     //공격(콤보 딜레이)
        {
            timeSinceLastDive = 0f;

            if (currentState == State.HIT)
            {
                yield break;                            //맞을 경우 코루틴 종료
            }
            yield return null;
        }


        if (s_attackPhase > 0) // 공격 단계가 0이 아니면
        {
            s_attackPhase = 0; // 공격 단계 초기화
            Debug.Log("공격초기화!");
        }
    }


    public void TriggerAttack()
    {
        isAttackHit = true;
    }
    public void EndAttack()
    {
        isAttackHit = false;
    }

    public void CanMove()
    {
        currentState = State.IDLE;
    }
    public void StartCanMove()
    {
        isinvincibility = false;
        currentState = State.IDLE;
    }
    public void StartCantMove()
    {
        isinvincibility = true;
        currentState = State.SAVE;
    }


    private void RotateTowardsEnemy()
    {
        Vector3 direction = cameraController.LockedTarget.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            animator.SetBool("isJumping", false);
            if(currentState == State.JUMP)
            {
                currentState = State.IDLE;
            }
        }

        if (firstDropDie&&collision.gameObject.CompareTag("DeathZone"))
        {
            PlayerHP -= PlayerMaxHP;
            GameManager.Instance.UpdatePlayerHP(PlayerHP);
            firstDropDie = false;
        }

    }
    private Collider collider;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack") && !isinvincibility)
        {
            isinvincibility = true;                    //피격 무적 활성화

            isAttacking = false;
            currentState = State.HIT;
            Debug.Log("맞았다!");
            isAttacked = true;
            HandleHit(other);

        }

        if (other.CompareTag("SavePoint")) // 플레이어와의 충돌 여부 확인
        {
            collider = other;
            CanSave = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        collider = other;

        if (other.CompareTag("SavePoint")) // 플레이어가 콜라이더를 나갔을 때
        {
            CanSave = false;
        }
    }


    private void HandleHit(Collider other_hit)
    {
        currentState = State.HIT;

        Ienemy enemy = other_hit.GetComponentInParent<Ienemy>();
        float GetDamage = enemy.Damage;                     //데미지 계산

        PlayerHP = PlayerHP - GetDamage;
        GameManager.Instance.UpdatePlayerHP(PlayerHP);      //데미지 UI표시

        if (GetDamage < 29)
        {
            isAttackHit = false;                                //공격판정 취소

            animator.CrossFade("Hit", 0.1f, 0, 0);
            attackPhase = -1;                                   //공격페이즈 초기화
            s_attackPhase = 0;

            StartCoroutine(AttackedMotionDelay(0.8f, 0.7f));
        }

        else if (other_hit.gameObject.name == "SquakeSkill(Clone)")
        {
            isAttackHit = false;                                //공격판정 취소

            animator.CrossFade("Hit_Up", 0.1f, 0, 0);
            attackPhase = -1;                                   //공격페이즈 초기화
            s_attackPhase = 0;

            StartCoroutine(AttackedMotionDelay(3.5f, 2f));
        }

        else
        {
            Vector3 hitDirection = other_hit.transform.position - transform.position;

            if (hitDirection.z > 0) // 앞에서 맞은 경우
            {
                isAttackHit = false;                                //공격판정 취소

                animator.CrossFade("Hit_F", 0.1f, 0, 0);
                attackPhase = -1;                                   //공격페이즈 초기화
                s_attackPhase = 0;

                StartCoroutine(AttackedMotionDelay(3f, 1.4f));
            }
            else
            {
                isAttackHit = false;                                //공격판정 취소

                animator.CrossFade("Hit_B", 0.1f, 0, 0);
                attackPhase = -1;                                   //공격페이즈 초기화
                s_attackPhase = 0;

                StartCoroutine(AttackedMotionDelay(3f, 1.4f));
            }
        }

    }

    private IEnumerator AttackedMotionDelay(float stunTime, float stunTime_)
    {
        isAttacked = false;
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = Time.time;
        bool firstcheck = false;

        animator.SetBool("isAttacked", isAttacked);
        while (Time.time < startTime + stunTime)
        {
            currentSpeed = 0f;

            if (isAttacked || currentState == State.DIVE || currentState == State.ATTACK || currentState == State.JUMP)
            {
                yield break;
            }
            isAttackHit = false;
            //피격 못움직임
            if (Time.time <= startTime + stunTime_)
            {
                isinvincibility = true;
                currentState = State.HIT;
                isStand = true;
                canAttack = false;
                canDive = false;
                canCrouched = false;
            }
            else
            {
                if (!firstcheck)
                {
                    firstcheck = true;

                    isinvincibility = false;
                    currentState = State.IDLE;

                    canAttack = true;
                    canDive = true;
                }
            }

            yield return null;
        }


        isGround = true;
        canCrouched = true;


        animator.SetBool("isCrouching", !isStand);


    }
}